using System;
using System.IO;
using System.Media;
using System.Runtime.Versioning;

namespace CyberSecurityChatbot
{
    internal static class AudioPlayer
    {
        private static SoundPlayer? _player;

        [SupportedOSPlatform("windows")]
        public static void PlayGreeting()
        {
            var path = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Greeting.wav");
            if (!File.Exists(path))
            {
                Console.ForegroundColor = ConsoleColor.Yellow;
                Console.WriteLine("Audio greeting is missing. Add Greeting.wav to the application folder to enable playback.");
                Console.ResetColor();
                return;
            }

            try
            {
                if (_player == null)
                {
                    _player = new SoundPlayer(path);
                }
                else if (!string.Equals(_player.SoundLocation, path, StringComparison.OrdinalIgnoreCase))
                {
                    _player.SoundLocation = path;
                }

                _player.Play();
            }
            catch (Exception ex)
            {
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine($"Unable to play greeting audio: {ex.Message}");
                Console.ResetColor();
            }
        }
    }
}
