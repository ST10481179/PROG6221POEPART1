# CyberSecurityChatbot

A simple WPF/.NET chatbot that provides cybersecurity tips about passwords, phishing, malware, VPNs, privacy, and safe browsing.

## Run

Build and run from the project folder:

```powershell
# build
dotnet build "CyberSecurityChatbot.csproj" -c Debug

# run GUI (default)
dotnet run --project "CyberSecurityChatbot.csproj" -c Debug

# run console mode
dotnet run --project "CyberSecurityChatbot.csproj" -c Debug -- -console
```

## Features

- Neat WPF GUI with helpful prompt design and clean layout
- Keyboard shortcut support (Enter to send)
- Personalised responses using the user's name and favourite topic
- Keyword recognition for cybersecurity topics like password, scam, phishing, privacy, VPN, and malware
- Random topic responses to keep answers varied and engaging
- Follow-up handling for requests such as "give me another tip" or "tell me more"
- Sentiment-aware responses for positive and negative user tone
- Memory recall for interests and past details shared by the user
- EF Core + SQLite persistence for remembering user data between runs
- Automatic database migrations on startup
- Optional audio greeting playback when `Greeting.wav` is present

## YouTube Demo

YouTube: <PLACEHOLDER_FOR_YOUTUBE_LINK>



