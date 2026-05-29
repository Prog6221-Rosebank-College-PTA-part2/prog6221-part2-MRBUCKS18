using System;
using System.Drawing;
using System.Windows.Forms;

namespace CyberSecurityChatbot
{
    public class MainForm : Form
    {
        private readonly ChatbotEngine _chatbot = new ChatbotEngine();
        private bool _chatStarted = false;
        private string _userName = "Friend";

        private Label logoLabel = new Label();
        private TextBox nameTextBox = new TextBox();
        private Button startButton = new Button();
        private RichTextBox chatBox = new RichTextBox();
        private TextBox questionTextBox = new TextBox();
        private Button sendButton = new Button();
        private Button clearButton = new Button();
        private Button exitButton = new Button();
        private Button helpButton = new Button();
        private Button passwordButton = new Button();
        private Button phishingButton = new Button();
        private Button scamButton = new Button();

        private const string AsciiLogo = @"  ____  __   __ ____  _____ ____      ____  _____ ____ _   _ ____  ___ _______   __
 / ___| \ \ / /| __ )| ____|  _ \    / ___|| ____/ ___| | | |  _ \|_ _|_   _\ \ / /
| |      \ V / |  _ \|  _| | |_) |   \___ \|  _|| |   | | | | |_) || |  | |  \ V / 
| |___    | |  | |_) | |___|  _ <     ___) | |__| |___| |_| |  _ < | |  | |   | |  
 \____|   |_|  |____/|_____|_| \_\   |____/|_____\____|\___/|_| \_\___| |_|   |_|  ";

        public MainForm()
        {
            Text = "Cyber Security Chatbot";
            StartPosition = FormStartPosition.CenterScreen;
            MinimumSize = new Size(1000, 720);
            BackColor = Color.FromArgb(18, 24, 38);
            ForeColor = Color.White;
            Font = new Font("Segoe UI", 10F);

            BuildInterface();

            Shown += MainForm_Shown;
        }

        private void MainForm_Shown(object? sender, EventArgs e)
        {
            AudioPlayer.PlayGreeting();
        }

        private void BuildInterface()
        {
            var mainPanel = new TableLayoutPanel
            {
                Dock = DockStyle.Fill,
                ColumnCount = 1,
                RowCount = 6,
                Padding = new Padding(18),
                BackColor = Color.FromArgb(18, 24, 38)
            };
            mainPanel.RowStyles.Add(new RowStyle(SizeType.Absolute, 115));
            mainPanel.RowStyles.Add(new RowStyle(SizeType.Absolute, 55));
            mainPanel.RowStyles.Add(new RowStyle(SizeType.Percent, 100));
            mainPanel.RowStyles.Add(new RowStyle(SizeType.Absolute, 55));
            mainPanel.RowStyles.Add(new RowStyle(SizeType.Absolute, 52));
            mainPanel.RowStyles.Add(new RowStyle(SizeType.Absolute, 45));
            Controls.Add(mainPanel);

            logoLabel = new Label
            {
                Text = AsciiLogo,
                Dock = DockStyle.Fill,
                Font = new Font("Consolas", 9.5F, FontStyle.Bold),
                ForeColor = Color.FromArgb(78, 219, 255),
                BackColor = Color.FromArgb(9, 14, 26),
                TextAlign = ContentAlignment.MiddleCenter,
                AutoSize = false,
                BorderStyle = BorderStyle.FixedSingle
            };
            mainPanel.Controls.Add(logoLabel, 0, 0);

            var namePanel = new FlowLayoutPanel
            {
                Dock = DockStyle.Fill,
                FlowDirection = FlowDirection.LeftToRight,
                BackColor = Color.FromArgb(18, 24, 38),
                Padding = new Padding(0, 10, 0, 0)
            };
            mainPanel.Controls.Add(namePanel, 0, 1);

            namePanel.Controls.Add(new Label
            {
                Text = "Enter your name:",
                ForeColor = Color.White,
                Width = 125,
                Height = 30,
                TextAlign = ContentAlignment.MiddleLeft
            });

            nameTextBox = new TextBox
            {
                Width = 250,
                Height = 30,
                Font = new Font("Segoe UI", 10F)
            };
            nameTextBox.KeyDown += (s, e) => { if (e.KeyCode == Keys.Enter) StartChat(); };
            namePanel.Controls.Add(nameTextBox);

            startButton = CreateButton("Start Chat", 120);
            startButton.Click += (s, e) => StartChat();
            namePanel.Controls.Add(startButton);

            chatBox = new RichTextBox
            {
                Dock = DockStyle.Fill,
                ReadOnly = true,
                BackColor = Color.FromArgb(245, 248, 252),
                ForeColor = Color.FromArgb(25, 25, 25),
                Font = new Font("Segoe UI", 10.5F),
                BorderStyle = BorderStyle.FixedSingle,
                HideSelection = false
            };
            mainPanel.Controls.Add(chatBox, 0, 2);
            AddBotMessage("Welcome! The voice greeting will play automatically when the program launches. Enter your name and click Start Chat.");

            var questionPanel = new TableLayoutPanel
            {
                Dock = DockStyle.Fill,
                ColumnCount = 2,
                BackColor = Color.FromArgb(18, 24, 38),
                Padding = new Padding(0, 10, 0, 0)
            };
            questionPanel.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 100));
            questionPanel.ColumnStyles.Add(new ColumnStyle(SizeType.Absolute, 120));
            mainPanel.Controls.Add(questionPanel, 0, 3);

            questionTextBox = new TextBox
            {
                Dock = DockStyle.Fill,
                Enabled = false,
                Font = new Font("Segoe UI", 11F)
            };
            questionTextBox.KeyDown += (s, e) => { if (e.KeyCode == Keys.Enter) SendQuestion(); };
            questionPanel.Controls.Add(questionTextBox, 0, 0);

            sendButton = CreateButton("Send", 100);
            sendButton.Dock = DockStyle.Fill;
            sendButton.Enabled = false;
            sendButton.Click += (s, e) => SendQuestion();
            questionPanel.Controls.Add(sendButton, 1, 0);

            var examplesPanel = new FlowLayoutPanel
            {
                Dock = DockStyle.Fill,
                FlowDirection = FlowDirection.LeftToRight,
                BackColor = Color.FromArgb(18, 24, 38),
                Padding = new Padding(0, 8, 0, 0)
            };
            mainPanel.Controls.Add(examplesPanel, 0, 4);

            helpButton = CreateButton("Help", 120);
            helpButton.Click += (s, e) => FillExample("What can I ask you about?");
            examplesPanel.Controls.Add(helpButton);

            passwordButton = CreateButton("Password Tip", 140);
            passwordButton.Click += (s, e) => FillExample("Tell me about password safety");
            examplesPanel.Controls.Add(passwordButton);

            phishingButton = CreateButton("Phishing Tip", 140);
            phishingButton.Click += (s, e) => FillExample("Give me a phishing tip");
            examplesPanel.Controls.Add(phishingButton);

            scamButton = CreateButton("Scam Help", 130);
            scamButton.Click += (s, e) => FillExample("I am worried about online scams");
            examplesPanel.Controls.Add(scamButton);

            clearButton = CreateButton("Clear Chat", 130);
            clearButton.Click += (s, e) => ClearChat();
            examplesPanel.Controls.Add(clearButton);

            exitButton = CreateButton("Exit", 100);
            exitButton.Click += (s, e) => Close();
            examplesPanel.Controls.Add(exitButton);

            var footer = new Label
            {
                Text = "Features: keyword recognition, random responses, conversation flow, memory and recall, sentiment detection, error handling, and organised OOP code.",
                Dock = DockStyle.Fill,
                ForeColor = Color.LightGray,
                TextAlign = ContentAlignment.MiddleCenter
            };
            mainPanel.Controls.Add(footer, 0, 5);
        }

        private Button CreateButton(string text, int width)
        {
            return new Button
            {
                Text = text,
                Width = width,
                Height = 34,
                BackColor = Color.FromArgb(37, 99, 235),
                ForeColor = Color.White,
                FlatStyle = FlatStyle.Flat,
                Font = new Font("Segoe UI", 9.5F, FontStyle.Bold),
                Margin = new Padding(5)
            };
        }

        private void StartChat()
        {
            string name = nameTextBox.Text.Trim();
            if (string.IsNullOrWhiteSpace(name))
            {
                MessageBox.Show("Name cannot be empty. Please enter your name.", "Validation Error", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                nameTextBox.Focus();
                return;
            }

            _userName = FormatName(name);
            _chatStarted = true;
            questionTextBox.Enabled = true;
            sendButton.Enabled = true;
            questionTextBox.Focus();

            chatBox.Clear();
            AddBotMessage($"Hello {_userName}! Welcome to the Cyber Security Chatbot.");
            AddBotMessage("You can ask about passwords, scams, privacy, phishing, safe browsing, suspicious links, malware, and online safety.");
        }

        private void SendQuestion()
        {
            if (!_chatStarted)
            {
                MessageBox.Show("Please enter your name and click Start Chat first.", "Start Required", MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }

            string question = questionTextBox.Text.Trim();
            if (string.IsNullOrWhiteSpace(question))
            {
                MessageBox.Show("Please enter a valid question.", "Validation Error", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                questionTextBox.Focus();
                return;
            }

            AddUserMessage(question);

            string lower = question.ToLower();
            if (lower == "exit" || lower == "quit")
            {
                AddBotMessage($"Goodbye {_userName}! Stay safe online.");
                questionTextBox.Clear();
                questionTextBox.Enabled = false;
                sendButton.Enabled = false;
                return;
            }

            AddBotMessage(_chatbot.GetResponse(question, _userName));
            questionTextBox.Clear();
            questionTextBox.Focus();
        }

        private void AddUserMessage(string message)
        {
            chatBox.SelectionColor = Color.FromArgb(37, 99, 235);
            chatBox.SelectionFont = new Font(chatBox.Font, FontStyle.Bold);
            chatBox.AppendText($"\nYou: ");
            chatBox.SelectionColor = Color.Black;
            chatBox.SelectionFont = chatBox.Font;
            chatBox.AppendText(message + "\n");
            chatBox.ScrollToCaret();
        }

        private void AddBotMessage(string message)
        {
            chatBox.SelectionColor = Color.FromArgb(20, 120, 80);
            chatBox.SelectionFont = new Font(chatBox.Font, FontStyle.Bold);
            chatBox.AppendText("Bot: ");
            chatBox.SelectionColor = Color.Black;
            chatBox.SelectionFont = chatBox.Font;
            chatBox.AppendText(message + "\n");
            chatBox.ScrollToCaret();
        }

        private void FillExample(string text)
        {
            questionTextBox.Text = text;
            questionTextBox.Focus();
        }

        private void ClearChat()
        {
            chatBox.Clear();
            AddBotMessage(_chatStarted ? $"Chat cleared. How can I help you, {_userName}?" : "Welcome! Enter your name and click Start Chat.");
        }

        private static string FormatName(string name)
        {
            name = name.Trim();
            return name.Length <= 1 ? name.ToUpper() : char.ToUpper(name[0]) + name.Substring(1).ToLower();
        }
    }
}
