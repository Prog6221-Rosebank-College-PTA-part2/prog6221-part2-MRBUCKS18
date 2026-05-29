using System;
using System.Collections.Generic;
using System.Linq;

namespace CyberSecurityChatbot
{
    public class ChatbotEngine
    {
        private readonly Random _random = new Random();
        private string _lastTopic = string.Empty;
        private string _favouriteTopic = string.Empty;

        private readonly Dictionary<string, List<string>> _responses = new Dictionary<string, List<string>>
        {
            ["password"] = new List<string>
            {
                "Make sure to use strong, unique passwords for each account. Avoid using personal details in your passwords.",
                "A good password should include uppercase letters, lowercase letters, numbers, and symbols.",
                "Never share your password with anyone, and avoid saving passwords on public computers."
            },
            ["scam"] = new List<string>
            {
                "Scams often create urgency. Always slow down, check the sender, and verify the message before clicking anything.",
                "Do not send money or personal information to someone you only know online unless you have verified them properly.",
                "Online scammers can pretend to be banks, delivery companies, or even friends. Always check carefully."
            },
            ["privacy"] = new List<string>
            {
                "Review your privacy settings on social media and limit who can see your personal information.",
                "Do not overshare information such as your address, school, phone number, or daily location online.",
                "As someone interested in privacy, you should regularly check app permissions on your phone and computer."
            },
            ["phishing"] = new List<string>
            {
                "Be cautious of emails asking for personal information. Scammers often disguise themselves as trusted organisations.",
                "Check the sender's email address carefully before clicking links or downloading attachments.",
                "Phishing messages often use urgent wording like 'verify now' or 'your account will be closed'."
            },
            ["browsing"] = new List<string>
            {
                "For safe browsing, visit trusted websites, check for HTTPS, and avoid unknown downloads.",
                "Keep your browser updated because updates often fix security weaknesses.",
                "Avoid clicking pop-up adverts that claim your device is infected."
            },
            ["link"] = new List<string>
            {
                "Do not click suspicious links. Hover over the link first and check if the website address looks correct.",
                "Shortened links can hide the real website, so be careful before opening them.",
                "Only open links from trusted sources, especially when the message asks for login details."
            },
            ["malware"] = new List<string>
            {
                "Malware is harmful software that can damage your device or steal your information.",
                "Avoid downloading files from unknown websites because they may contain malware.",
                "Use antivirus protection and keep your operating system updated."
            }
        };

        private readonly Dictionary<string, string[]> _keywordMap = new Dictionary<string, string[]>
        {
            ["password"] = new[] { "password", "passcode", "login" },
            ["scam"] = new[] { "scam", "fraud", "fake", "con", "scammer" },
            ["privacy"] = new[] { "privacy", "private", "personal information", "data" },
            ["phishing"] = new[] { "phishing", "email", "attachment" },
            ["browsing"] = new[] { "safe browsing", "browser", "browsing", "website", "https" },
            ["link"] = new[] { "link", "url", "suspicious link" },
            ["malware"] = new[] { "malware", "virus", "download", "infected" }
        };

        private readonly string[] _worriedWords = { "worried", "scared", "afraid", "overwhelmed", "unsure", "frustrated", "confused" };
        private readonly string[] _curiousWords = { "curious", "interested", "want to know", "tell me" };

        public string GetResponse(string input, string userName)
        {
            string text = input.ToLower().Trim();

            if (string.IsNullOrWhiteSpace(text))
                return "I'm not sure I understand. Can you try rephrasing?";

            if (text.Contains("how are you"))
                return $"I'm doing great, {userName}. I'm ready to help you stay safe online.";

            if (text.Contains("purpose") || text.Contains("what do you do"))
                return "My purpose is to teach cyber-security awareness and help users avoid online risks.";

            if (text.Contains("what can i ask") || text == "help")
                return "You can ask me about password safety, scams, privacy, phishing tips, safe browsing, suspicious links, malware, and online safety.";

            string? detectedTopic = DetectTopic(text);

            if (text.Contains("interested in") && detectedTopic != null)
            {
                _favouriteTopic = detectedTopic;
                _lastTopic = detectedTopic;
                return $"Great! I'll remember that you're interested in {detectedTopic}. It's an important part of staying safe online. " + GetRandomResponse(detectedTopic);
            }

            if (IsFollowUp(text))
            {
                if (!string.IsNullOrWhiteSpace(_lastTopic))
                    return "Sure, here is another tip: " + GetRandomResponse(_lastTopic);

                if (!string.IsNullOrWhiteSpace(_favouriteTopic))
                    return $"Since you are interested in {_favouriteTopic}, here is a useful tip: " + GetRandomResponse(_favouriteTopic);

                return "Please mention the topic you want more information about, such as password, scam, privacy, or phishing.";
            }

            if (detectedTopic != null)
            {
                _lastTopic = detectedTopic;
                string response = GetRandomResponse(detectedTopic);

                if (ContainsAny(text, _worriedWords))
                    response = "It's completely understandable to feel that way. Let me help you step by step. " + response;
                else if (ContainsAny(text, _curiousWords))
                    response = "That's a good topic to learn about. " + response;

                if (!string.IsNullOrWhiteSpace(_favouriteTopic) && detectedTopic == _favouriteTopic)
                    response += $" I remember that {detectedTopic} is one of your interests.";

                return response;
            }

            if (ContainsAny(text, _worriedWords))
                return "I understand. Cybersecurity can feel confusing, but you can stay safer by taking small steps. Ask me about passwords, scams, privacy, or phishing.";

            return "I'm not sure I understand. Can you try rephrasing? You can ask about password, scam, privacy, phishing, safe browsing, links, or malware.";
        }

        private string? DetectTopic(string text)
        {
            foreach (var topic in _keywordMap)
            {
                if (topic.Value.Any(keyword => text.Contains(keyword)))
                    return topic.Key;
            }
            return null;
        }

        private string GetRandomResponse(string topic)
        {
            List<string> options = _responses[topic];
            return options[_random.Next(options.Count)];
        }

        private bool IsFollowUp(string text)
        {
            return text.Contains("another tip") || text.Contains("explain more") || text.Contains("tell me more") || text == "more" || text.Contains("more details");
        }

        private bool ContainsAny(string text, string[] words)
        {
            return words.Any(text.Contains);
        }
    }
}
