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

## Requirements

- Visual Studio 2022 or Visual Studio Code
- .NET 10 SDK
- Installed NuGet packages:
  - `Microsoft.EntityFrameworkCore`
  - `Microsoft.EntityFrameworkCore.Sqlite`
  - `Microsoft.EntityFrameworkCore.Proxies`
  - `Microsoft.EntityFrameworkCore.Tools`

## Setup and Run

1. Open `CyberSecurityChatbot.sln` in Visual Studio 2022 or your editor.
2. Restore NuGet packages.
3. Ensure `Greeting.wav` is present in the project root to enable the audio greeting.
4. Build the solution.
5. Run the project from `CyberSecurityChatbot.csproj`.

```powershell
dotnet build "CyberSecurityChatbot.sln" -c Debug
dotnet run --project "CyberSecurityChatbot.csproj" -c Debug
```

## Database

The application stores tasks and activity log entries in `database.db` in the project folder. The database is created automatically when the app runs.

## Notes

- This solution extends the existing Part 2 WPF project; no new project was added.
- The GUI contains tabs for Status, Tasks, Quiz, and Activity Log.
- The chat interface still supports the original Part 1 and Part 2 cybersecurity response flow.

## YouTube Link

YouTube: 


