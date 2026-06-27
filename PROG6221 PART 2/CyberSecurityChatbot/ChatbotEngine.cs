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
        private string _lastSentiment = string.Empty;

        private readonly Dictionary<string, List<string>> _responses = new Dictionary<string, List<string>>
        {
            ["password"] = new List<string>
            {
                "Make sure to use strong, unique passwords for each account. Avoid using personal details in your passwords.",
                "A strong password should include uppercase letters, lowercase letters, numbers, and symbols.",
                "Never share your password with anyone, and avoid saving passwords on public computers.",
                "Use a password manager if you struggle to remember many strong passwords."
            },
            ["scam"] = new List<string>
            {
                "Scammers often create urgency. Slow down, check the sender, and verify the message before clicking anything.",
                "Do not send money or personal information to someone online unless you have verified them properly.",
                "Online scammers can pretend to be banks, delivery companies, or even friends. Always check carefully.",
                "If something sounds too good to be true, it is usually safer to verify it first."
            },
            ["privacy"] = new List<string>
            {
                "Review your privacy settings on social media and limit who can see your personal information.",
                "Do not overshare information such as your address, school, phone number, or daily location online.",
                "As someone interested in privacy, you should regularly check app permissions on your phone and computer.",
                "Only give personal information to trusted websites and organisations."
            },
            ["phishing"] = new List<string>
            {
                "Be cautious of emails asking for personal information. Scammers often disguise themselves as trusted organisations.",
                "Check the sender's email address carefully before clicking links or downloading attachments.",
                "Phishing messages often use urgent wording like 'verify now' or 'your account will be closed'.",
                "Never enter your login details after clicking a suspicious email link. Open the real website manually instead."
            },
            ["browsing"] = new List<string>
            {
                "For safe browsing, visit trusted websites, check for HTTPS, and avoid unknown downloads.",
                "Keep your browser updated because updates often fix security weaknesses.",
                "Avoid clicking pop-up adverts that claim your device is infected.",
                "Do not allow random websites to send notifications or download files automatically."
            },
            ["link"] = new List<string>
            {
                "Do not click suspicious links. Hover over the link first and check if the website address looks correct.",
                "Shortened links can hide the real website, so be careful before opening them.",
                "Only open links from trusted sources, especially when the message asks for login details.",
                "If a link asks you to sign in urgently, go to the official website yourself instead of using the link."
            },
            ["malware"] = new List<string>
            {
                "Malware is harmful software that can damage your device or steal your information.",
                "Avoid downloading files from unknown websites because they may contain malware.",
                "Use antivirus protection and keep your operating system updated.",
                "Do not open unknown attachments, especially files ending in .exe, .bat, .scr, or suspicious zip files."
            }
        };

        private readonly Dictionary<string, string[]> _keywordMap = new Dictionary<string, string[]>
        {
            ["password"] = new[] { "password", "passcode", "login", "credential" },
            ["scam"] = new[] { "scam", "fraud", "fake", "con", "scammer", "online scam" },
            ["privacy"] = new[] { "privacy", "private", "personal information", "personal data", "data" },
            ["phishing"] = new[] { "phishing", "email", "attachment", "suspicious email" },
            ["browsing"] = new[] { "safe browsing", "browser", "browsing", "website", "https", "online" },
            ["link"] = new[] { "link", "url", "suspicious link" },
            ["malware"] = new[] { "malware", "virus", "download", "infected" }
        };

        private readonly string[] _worriedWords = { "worried", "scared", "afraid", "overwhelmed", "unsure", "nervous", "panic", "anxious" };
        private readonly string[] _frustratedWords = { "frustrated", "confused", "annoyed", "angry", "don't understand", "do not understand" };
        private readonly string[] _curiousWords = { "curious", "interested", "want to know", "tell me", "explain", "learn" };

        public string GetResponse(string input, string userName)
        {
            string text = input.ToLower().Trim();

            if (string.IsNullOrWhiteSpace(text))
                return "I'm not sure I understand. Can you try rephrasing?";

            if (text.Contains("how are you"))
                return $"I'm doing great, {userName}. I'm ready to help you stay safe online.";

            if (text.Contains("purpose") || text.Contains("what do you do"))
                return "My purpose is to teach cyber-security awareness and help users avoid online risks such as scams, phishing, weak passwords, and unsafe websites.";

            if (text.Contains("what can i ask") || text == "help" || text.Contains("topics"))
                return "You can ask me about password safety, scams, privacy, phishing tips, safe browsing, suspicious links, malware, and online safety. You can also say 'tell me more' for another tip.";

            if (text.Contains("what do you remember") || text.Contains("remember about me"))
                return GetMemoryResponse(userName);

            string? detectedTopic = DetectTopic(text);
            string sentiment = DetectSentiment(text);
            if (!string.IsNullOrWhiteSpace(sentiment))
                _lastSentiment = sentiment;

            if (IsMemoryStatement(text, detectedTopic))
            {
                _favouriteTopic = detectedTopic!;
                _lastTopic = detectedTopic!;
                return $"Great! I'll remember that you're interested in {detectedTopic}. It's a crucial part of staying safe online. " + GetRandomResponse(detectedTopic!);
            }

            if (IsFollowUp(text))
            {
                if (!string.IsNullOrWhiteSpace(_lastTopic))
                    return BuildSentimentPrefix(_lastSentiment) + "Here is another tip about " + _lastTopic + ": " + GetRandomResponse(_lastTopic);

                if (!string.IsNullOrWhiteSpace(_favouriteTopic))
                    return BuildSentimentPrefix(_lastSentiment) + $"Since you are interested in {_favouriteTopic}, here is a useful tip: " + GetRandomResponse(_favouriteTopic);

                return "Please mention the topic you want more information about, such as password, scam, privacy, phishing, safe browsing, links, or malware.";
            }

            if (detectedTopic != null)
            {
                _lastTopic = detectedTopic;
                string response = BuildSentimentPrefix(sentiment) + GetRandomResponse(detectedTopic);

                if (!string.IsNullOrWhiteSpace(_favouriteTopic) && detectedTopic == _favouriteTopic)
                    response += $" I also remember that {detectedTopic} is one of your interests, {userName}.";

                return response;
            }

            if (!string.IsNullOrWhiteSpace(sentiment))
                return BuildSentimentPrefix(sentiment) + "Ask me about passwords, scams, privacy, phishing, safe browsing, links, or malware and I will guide you.";

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

        private string DetectSentiment(string text)
        {
            if (ContainsAny(text, _worriedWords))
                return "worried";
            if (ContainsAny(text, _frustratedWords))
                return "frustrated";
            if (ContainsAny(text, _curiousWords))
                return "curious";
            return string.Empty;
        }

        private string BuildSentimentPrefix(string sentiment)
        {
            return sentiment switch
            {
                "worried" => "It's completely understandable to feel that way. Scammers can be very convincing, but I can help you stay safe. ",
                "frustrated" => "I understand that this can feel frustrating. Let's take it step by step. ",
                "curious" => "That's a good topic to learn about. ",
                _ => string.Empty
            };
        }

        private string GetMemoryResponse(string userName)
        {
            if (string.IsNullOrWhiteSpace(_favouriteTopic))
                return $"I remember your name is {userName}. You have not told me your favourite cybersecurity topic yet. Try saying: I'm interested in privacy.";

            return $"I remember your name is {userName}, and you are interested in {_favouriteTopic}. As someone interested in {_favouriteTopic}, you might want to review your security settings and keep learning about this topic.";
        }

        private bool IsMemoryStatement(string text, string? detectedTopic)
        {
            return detectedTopic != null &&
                   (text.Contains("i'm interested in") ||
                    text.Contains("i am interested in") ||
                    text.Contains("my favourite topic is") ||
                    text.Contains("my favorite topic is") ||
                    text.Contains("i like") ||
                    text.Contains("remember that"));
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
