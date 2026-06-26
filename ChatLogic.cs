using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;

namespace CyberSecurityChatbot
{
    internal enum Sentiment
    {
        Negative,
        Neutral,
        Positive
    }

    internal static class ChatLogic
    {
        public delegate string ResponseModifier(string response, Sentiment sentiment);

        public static ResponseModifier? Modifier = null;

        private static readonly Random Random = new();

        private static readonly Dictionary<string, List<string>> TopicResponses = new()
        {
            ["password"] = new()
            {
                "Use a long, unique password for each account, enable multi-factor authentication, and store passwords in a password manager.",
                "Avoid reusing passwords. A strong password is at least 12 characters and uses letters, numbers, and symbols.",
                "Use a password manager to generate and store one strong unique password per account."
            },
            ["phishing"] = new()
            {
                "Phishing attacks use fake emails and links. Always verify the sender, hover over links, and never share your password.",
                "Be cautious of unexpected emails demanding action. Scammers often disguise themselves as trusted organisations.",
                "Check the sender address and look for poor spelling or urgent language. If it seems odd, don't click links.",
                "Legitimate organisations won't ask for passwords by email. When in doubt, contact them directly instead of responding."
            },
            ["vpn"] = new()
            {
                "A VPN helps protect your data on public Wi-Fi by encrypting your connection. Use one from a trusted provider.",
                "Public Wi-Fi is risky. A VPN encrypts your traffic so others cannot read it.",
                "Choose a reputable VPN service and avoid free providers that may sell your browsing data."
            },
            ["malware"] = new()
            {
                "Malware is harmful software. Keep your system updated, install antivirus software, and avoid downloading unknown files.",
                "Only download apps and files from trusted sites, and scan new downloads before opening them.",
                "If something seems suspicious, don't install it. Keeping your software patched is one of the best protections."
            },
            ["privacy"] = new()
            {
                "Privacy means controlling what you share. Review your account settings and limit what personal data you make public.",
                "Think before you post. Personal information can be used by attackers, so keep sensitive details private.",
                "Use strong privacy settings on social networks and only share personal information with people you trust."
            },
            ["safebrowsing"] = new()
            {
                "Safe browsing means avoiding suspicious links, checking the URL, and keeping your browser extensions up to date.",
                "Use HTTPS sites and avoid downloading files from unknown pages or pop-up ads.",
                "Enable browser security warnings and keep your browser patched to reduce exposure to malicious sites."
            },
            ["2fa"] = new()
            {
                "Two-factor authentication adds a second layer of protection beyond your password. Always enable it when available.",
                "Use an authenticator app or security key for 2FA instead of SMS when possible for stronger protection.",
                "Two-factor authentication helps prevent account takeover even if your password is leaked."
            }
        };

        private static readonly Dictionary<string, string[]> TopicKeywords = new()
        {
            ["password"] = new[] { "password", "passphrase", "credential", "password tips", "password safety tips", "password tip" },
            ["phishing"] = new[] { "phishing", "scam", "spoof", "fraud", "phishing tips", "scam tips" },
            ["vpn"] = new[] { "vpn" },
            ["malware"] = new[] { "malware", "virus", "ransomware", "spyware" },
            ["privacy"] = new[] { "privacy", "private", "personal data", "privacy tips" },
            ["safebrowsing"] = new[] { "safe browsing", "safe browsing tips", "browsing tips", "safe-browsing" },
            ["2fa"] = new[] { "two-factor", "2fa", "multi-factor", "mfa" }
        };

        private static readonly string[] PositiveKeywords = { "good", "great", "happy", "fine", "well", "awesome", "curious" };
        private static readonly string[] NegativeKeywords = { "worried", "upset", "angry", "terrible", "bad", "scared", "afraid", "frustrat", "frustrated", "nervous", "anxious", "confused" };
        private static readonly string[] InterestMarkers = { "i am interested in", "i'm interested in", "i am curious about", "i'm curious about", "i care about", "i like", "i love", "i want to learn about", "i want to know about", "i'm into", "i am into", "i'm passionate about", "i am passionate about", "i'm keen on", "i am keen on", "my interest is", "my interests are", "my favourite topic is", "my favorite topic is" };
        private static readonly string[] FollowUpMarkers = { "tell me more", "another tip", "more about", "explain more", "tell me another", "more", "what else", "give me another tip", "give me another" };

        public static string GetResponse(User user, string text)
        {
            var normalized = text.ToLowerInvariant();
            var sentiment = DetectSentiment(normalized);
            var topic = DetermineTopic(normalized);
            TryCaptureInterest(user, text, normalized, topic);
            TryStoreMemory(user, text);

            string response;
            var usedTopicResponse = false;

            if (normalized.Contains("how are you"))
            {
                response = "I am fully operational and ready to keep you safe.";
            }
            else if (normalized.Contains("purpose") || normalized.Contains("what do you do") || normalized.Contains("what is your purpose"))
            {
                response = "My purpose is to help you learn cybersecurity best practices and stay protected online.";
            }
            else if (normalized.Contains("what do you remember") || normalized.Contains("remember what") || normalized.Contains("what can you remember"))
            {
                response = user.GetMemorySummary();
            }
            else if (IsFollowUp(normalized) && !string.IsNullOrWhiteSpace(user.LastTopic))
            {
                response = BuildTopicResponse(user, user.LastTopic, true, sentiment);
                usedTopicResponse = true;
            }
            else if (!string.IsNullOrWhiteSpace(topic))
            {
                response = BuildTopicResponse(user, topic, false, sentiment);
                usedTopicResponse = true;
            }
            else if (normalized.Contains("help"))
            {
                response = "Ask me anything about cybersecurity: password safety, phishing, malware, VPNs, privacy, or account protection. You can also say 'tell me more' to continue a topic.";
            }
            else
            {
                response = "I'm not sure I understand. Can you rephrase or ask about passwords, phishing, malware, VPNs, privacy, or account safety?";
            }

            response = ApplyPersonalization(user, response, topic);
            response = ApplyNamePersonalization(user, response);
            if (Modifier != null)
            {
                try
                {
                    response = Modifier(response, sentiment);
                }
                catch
                {
                }
            }

            if (!usedTopicResponse)
            {
                if (sentiment == Sentiment.Negative && !normalized.Contains("how are you"))
                {
                    response = "It's completely understandable to feel that way. " + response;
                }
                else if (sentiment == Sentiment.Positive && !normalized.Contains("how are you"))
                {
                    response = "I'm glad you're feeling positive. " + response;
                }
            }

            return response;
        }

        public static string DetectTopic(string text)
        {
            return DetermineTopic(text.ToLowerInvariant());
        }

        public static bool IsFollowUpMessage(string text)
        {
            return IsFollowUp(text.ToLowerInvariant());
        }

        private static string DetermineTopic(string normalized)
        {
            foreach (var pair in TopicKeywords)
            {
                if (pair.Value.Any(keyword => normalized.Contains(keyword)))
                {
                    return pair.Key;
                }
            }

            return string.Empty;
        }

        private static bool IsFollowUp(string normalized)
        {
            return FollowUpMarkers.Any(marker => normalized.Contains(marker));
        }

        private static void TryCaptureInterest(User user, string originalText, string normalized, string explicitTopic)
        {
            if (!InterestMarkers.Any(marker => normalized.Contains(marker)))
            {
                return;
            }

            var interestTopic = ExtractInterestTopic(originalText, normalized, explicitTopic);
            if (!string.IsNullOrWhiteSpace(interestTopic))
            {
                user.RememberInterest(interestTopic);
            }
        }

        private static string ExtractInterestTopic(string originalText, string normalized, string explicitTopic)
        {
            if (!string.IsNullOrWhiteSpace(explicitTopic))
            {
                return explicitTopic;
            }

            foreach (var marker in InterestMarkers)
            {
                var markerIndex = normalized.IndexOf(marker, StringComparison.Ordinal);
                if (markerIndex < 0)
                {
                    continue;
                }

                var start = markerIndex + marker.Length;
                if (start >= originalText.Length)
                {
                    continue;
                }

                var segment = originalText.Substring(start).Trim();
                segment = segment.Trim().TrimEnd('.', ',', '!', '?', ':', ';');
                var topic = NormalizeInterestTopic(segment);
                if (!string.IsNullOrWhiteSpace(topic))
                {
                    return topic;
                }
            }

            return string.Empty;
        }

        private static string NormalizeInterestTopic(string topic)
        {
            if (string.IsNullOrWhiteSpace(topic))
            {
                return string.Empty;
            }

            var cleaned = topic.Trim();
            cleaned = cleaned.TrimStart('"', '\'', '(');
            cleaned = cleaned.TrimEnd('"', '\'', ')', '.', ',', '!', '?', ':', ';');
            cleaned = Regex.Replace(cleaned, @"\s+", " ");
            cleaned = cleaned.Replace(" and ", " ").Replace(" or ", " ").Replace(" / ", " ");
            cleaned = cleaned.Replace(" about ", " ").Replace(" the ", " ").Replace(" a ", " ").Replace(" an ", " ");

            var normalized = cleaned.Trim().ToLowerInvariant();
            if (string.IsNullOrWhiteSpace(normalized))
            {
                return string.Empty;
            }

            var canonicalTopic = DetermineTopic(normalized);
            return string.IsNullOrWhiteSpace(canonicalTopic) ? normalized : canonicalTopic;
        }

        private static void TryStoreMemory(User user, string originalText)
        {
            var normalized = originalText.ToLowerInvariant();

            string ExtractAfter(string marker)
            {
                var idx = normalized.IndexOf(marker, System.StringComparison.Ordinal);
                if (idx >= 0)
                {
                    var start = idx + marker.Length;
                    if (start < originalText.Length)
                    {
                        var segment = originalText.Substring(start).Trim();
                        // Trim trailing punctuation
                        segment = segment.Trim().TrimEnd('.', ',', '!','?');
                        return segment;
                    }
                }

                return string.Empty;
            }

            if (normalized.Contains("i work at"))
            {
                var company = ExtractAfter("i work at");
                if (!string.IsNullOrWhiteSpace(company)) user.Remember($"Works at {company}");
                return;
            }

            if (normalized.Contains("my company is"))
            {
                var company = ExtractAfter("my company is");
                if (!string.IsNullOrWhiteSpace(company)) user.Remember($"Works at {company}");
                return;
            }

            if (normalized.Contains("i am a") || normalized.Contains("i'm a"))
            {
                var marker = normalized.Contains("i am a") ? "i am a" : "i'm a";
                var role = ExtractAfter(marker);
                if (!string.IsNullOrWhiteSpace(role)) user.Remember($"Role: {role}");
                return;
            }
        }

        private static string BuildTopicResponse(User user, string topic, bool followUp, Sentiment sentiment)
        {
            if (!TopicResponses.TryGetValue(topic, out var responses) || responses.Count == 0)
            {
                return "I can help with cybersecurity tips, especially on passwords, phishing, malware, VPNs, and privacy.";
            }

            user.LastTopic = topic;
            var index = 0;
            if (user.TopicResponseIndexes.TryGetValue(topic, out var lastIndex))
            {
                index = lastIndex;
            }

            var response = responses[index];
            user.TopicResponseIndexes[topic] = (index + 1) % responses.Count;

            if (followUp)
            {
                response = "Here is another tip: " + response;
            }

            if (sentiment == Sentiment.Negative)
            {
                response = "I understand your concern. " + response;
            }
            else if (sentiment == Sentiment.Positive)
            {
                response = "That's great that you're exploring this topic. " + response;
            }

            return response;
        }

        private static string ApplyPersonalization(User user, string response, string topic)
        {
            if (!string.IsNullOrWhiteSpace(topic) && user.HasInterests && user.Interests.Contains(topic))
            {
                return $"As someone interested in {topic}, {LowercaseFirst(response)}";
            }

            if (string.IsNullOrWhiteSpace(topic) && user.HasInterests)
            {
                var interests = string.Join(", ", user.Interests);
                return $"Since you've told me you're interested in {interests}, {LowercaseFirst(response)}";
            }

            return response;
        }

        private static string LowercaseFirst(string text)
        {
            if (string.IsNullOrEmpty(text) || char.IsLower(text[0]))
            {
                return text;
            }

            return char.ToLowerInvariant(text[0]) + text.Substring(1);
        }

        private static string ApplyNamePersonalization(User user, string response)
        {
            if (string.IsNullOrWhiteSpace(user.Name) || string.IsNullOrWhiteSpace(response))
            {
                return response;
            }

            return $"Hi {user.Name}, {response}";
        }

        public static Sentiment DetectSentiment(string text)
        {
            var positiveCount = PositiveKeywords.Count(w => text.Contains(w));
            var negativeCount = NegativeKeywords.Count(w => text.Contains(w));

            if (positiveCount > negativeCount && positiveCount > 0) return Sentiment.Positive;
            if (negativeCount > positiveCount && negativeCount > 0) return Sentiment.Negative;
            return Sentiment.Neutral;
        }
    }
}
