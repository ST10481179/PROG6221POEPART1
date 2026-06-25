# PROG6221 POE Part 3 — CyberSecurity Chatbot

This repository contains the Part 3 version of the CyberSecurity Chatbot WPF application. The app delivers cybersecurity advice through a desktop GUI, remembers user data with SQLite, and supports interactive follow-up prompts.

## Requirements

- .NET 10 SDK
- Visual Studio 2022 or Visual Studio Code
- Windows with WPF support

## Run

From the repository root:

```powershell
# build the solution
dotnet build "CyberSecurityChatbot.sln" -c Debug

# run the app using the solution project
dotnet run --project "CyberSecurityChatbot.csproj" -c Debug
```

## Project layout

- `CyberSecurityChatbot.sln` — main solution
- `CyberSecurityChatbot.csproj` — WPF application project
- `App.xaml`, `MainWindow.xaml` — WPF startup and UI
- `Chatbot.cs`, `ChatLogic.cs` — chatbot behavior and response logic
- `Data/`, `Entities/`, `Migrations/` — EF Core persistence structure
- `Greeting.wav` — optional startup greeting audio

## Features

- WPF desktop UI for chatbot interaction
- Interactive keyword and follow-up handling
- Personalization with user name and favorite topic
- EF Core + SQLite data persistence
- Automatic database migration on startup
- Optional greeting audio playback

## Clean repository rules

- `bin/` and `obj/` build outputs are excluded from git
- SQLite database files are ignored
- Temporary database journal files (`*.db-shm`, `*.db-wal`) are ignored
- The repository keeps only source, project, and solution files for Part 3



