# CyberSecurityChatbot

A simple WPF/.NET chatbot that provides cybersecurity tips about passwords, phishing, malware, VPNs, privacy, and account security.

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

- WPF GUI plus console mode support
- Personalized responses with user name
- Cybersecurity advice for common topics
- EF Core + SQLite persistence for user memory and interests
- Automatic database migrations on startup

## EF Core Migrations

This project is configured to use EF Core migrations.

Install the EF CLI if needed:

```powershell
dotnet tool install --global dotnet-ef
```

Create the initial migration:

```powershell
dotnet ef migrations add InitialCreate --project CyberSecurityChatbot.csproj
```

Apply the migration and create the database:

```powershell
dotnet ef database update --project CyberSecurityChatbot.csproj
```

The app will also apply pending migrations automatically when it starts.

## Persistence

The SQLite database file `chatbot.db` stores one user profile and its memory entries and interests.

## YouTube Demo

Add your YouTube link here:

YouTube: <PLACEHOLDER_FOR_YOUTUBE_LINK>

## Suggestions / Polishing

- Add unit tests for `ChatLogic` and persistence.
- Improve the WPF UI in `MainWindow.xaml`.
- Add a settings screen or saved conversation history.
- Add a release notes section and GitHub release automation.

---

Generated/updated by assistant to prepare this project for release.
