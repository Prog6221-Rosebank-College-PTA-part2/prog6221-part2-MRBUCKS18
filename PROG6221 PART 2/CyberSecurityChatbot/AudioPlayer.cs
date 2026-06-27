using System;
using System.IO;
using System.Media;
using System.Windows.Forms;

namespace CyberSecurityChatbot
{
    public static class AudioPlayer
    {
        private static SoundPlayer? _soundPlayer;

        public static void PlayGreeting()
        {
            try
            {
                string audioPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Assets", "CyberSecurityGreeting.wav");

                if (!File.Exists(audioPath))
                {
                    MessageBox.Show("Voice greeting audio file was not found in the Assets folder.",
                        "Audio Missing", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }

                _soundPlayer = new SoundPlayer(audioPath);
                _soundPlayer.LoadAsync();
                _soundPlayer.Play();
            }
            catch (Exception ex)
            {
                MessageBox.Show("Unable to play the voice greeting audio. " + ex.Message,
                    "Audio Error", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
        }
    }
}
