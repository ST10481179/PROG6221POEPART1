# CyberSecurityChatbot

<<<<<<< HEAD
A C# console chatbot that provides cybersecurity guidance with a voice greeting, ASCII banner, personalized user interaction, and enhanced console styling.


# CyberSecurityChatbot

A simple WPF/.NET console chatbot that provides cybersecurity tips (passwords, phishing, malware, VPNs, privacy).

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

## Persistence

The app uses EF Core with SQLite (`chatbot.db`) to persist a single user profile (name, memory, interests).

## YouTube Demo

Add your YouTube link here:

YouTube: <PLACEHOLDER_FOR_YOUTUBE_LINK>

## Suggestions / Polishing

- Add DI and repository layer for cleaner persistence.
- Add migrations (`dotnet ef migrations add InitialCreate`) for schema management.
- Improve UI in `MainWindow.xaml` and add settings page.
- Add unit tests for `ChatLogic`.

I applied some immediate polish: integrated EF Core (SQLite) for persistence and autosave on exit.

If you want, I can:
- Create GitHub Releases and push tags (requires authenticated git credentials on this machine).
- Add EF migrations and a README section with `dotnet ef` commands.
---

Generated/updated by assistant to prepare for release.
dotnet build "CyberSecurityChatbot.csproj" -c Debug

# run GUI (default)
dotnet run --project "CyberSecurityChatbot.csproj" -c Debug

# run console mode
dotnet run --project "CyberSecurityChatbot.csproj" -c Debug -- -console
```

## Persistence

The app uses EF Core with SQLite (`chatbot.db`) to persist a single user profile (name, memory, interests).

## YouTube Demo

Add your YouTube link here:

YouTube: <PLACEHOLDER_FOR_YOUTUBE_LINK>

## Suggestions / Polishing

- Add DI and repository layer for cleaner persistence.
- Add migrations (`dotnet ef migrations add InitialCreate`) for schema management.
- Improve UI in `MainWindow.xaml` and add settings page.
- Add unit tests for `ChatLogic`.

I applied some immediate polish: integrated EF Core (SQLite) for persistence and autosave on exit.

If you want, I can:
- Create GitHub Releases and push tags (requires authenticated git credentials on this machine).
- Add EF migrations and a README section with `dotnet ef` commands.


---

Generated/updated by assistant to prepare for release.
>>>>>>> 431d709 (Add EF Core persistence, entities, README and persistence wiring)
