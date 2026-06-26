 # Student Number: ST10481179
# CyberSecurityChatbot — Part 3 Submission

A WPF GUI application that extends the Part 2 cybersecurity chatbot with a Task Assistant, Quiz game, NLP-style intent recognition, and Activity Log.

## Project Description

This project is the final Part 3 submission for PROG6221. It builds on the existing WPF chatbot and adds:

- Task Assistant with CRUD task management and reminders
- Cybersecurity quiz game with 10+ questions, immediate feedback, and final score
- NLP-style intent detection for varied user phrases
- Activity log with recent action history and show more support

## Features

- GUI-based WPF application using XAML controls and styles
- Task Assistant panel with title, description, reminder, complete, and delete actions
- SQLite database persistence in `database.db` for tasks and activity logs
- Chat-driven task and reminder creation via natural language phrases
- Cybersecurity quiz with one question at a time and scoring
- Activity log display for recent actions and full history
- Keyword matching, follow-up handling, and sentiment-aware chatbot responses
- Optional audio greeting playback using `Greeting.wav`

## Database

The application stores tasks and activity log entries in `database.db` in the project folder. The database is created automatically when the app runs.

## Notes

- This solution extends the existing Part 2 WPF project; no new project was added.
- The GUI contains tabs for Status, Tasks, Quiz, and Activity Log.
- The chat interface still supports the original Part 1 and Part 2 cybersecurity response flow.
## Releases:
## v1.6.0
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
- Optional audio greeting playback when Greeting.wav is present
- Please download all the files and put them in a folder then you can run the .exe.
## v1.7.0
- Include Greeting.wav in output and fix audio playback packaging
- Fix Part 2 build: add EF package references and resolve WPF startup/JSON converter issues
- Fix startup audio playback and improve phishing/topic response handling
## v1.8.0
- Task Assistant with CRUD task management and reminders
- Cybersecurity quiz game with 10+ questions, immediate feedback, and final score
- NLP-style intent detection for varied user phrases
- Activity log with recent action history and show more support
##  GitHub Actions:

<img width="1407" height="857" alt="green " src="https://github.com/user-attachments/assets/10d31bef-0b61-4a5b-8866-4bfcea7e8ba2" />

## YouTube Link

YouTube: 


