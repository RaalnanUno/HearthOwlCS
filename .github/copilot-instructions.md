# HearthOwlCS – AI Instructions

You are assisting with the HearthOwlCS project.

## Project Purpose
HearthOwlCS is a Windows utility that:
- Moves all Desktop files and folders
- Into: My Documents\YYYY\Month\DD\
- Then optionally opens the destination folder
- Runs silently and exits

This is a C# .NET 8 console application targeting Windows.

---

## Coding Standards

- Target: net8.0-windows
- No UI (no WinForms, no WPF)
- Keep logic in Services/ folder
- Keep Program.cs thin
- Prefer small, testable classes
- Avoid unnecessary external packages
- Avoid COM unless absolutely required
- Keep everything Windows-native

---

## Behavior Rules

- Always assume AutoRun = true
- Never reintroduce app.config unless explicitly requested
- Prefer Environment.SpecialFolder over hard-coded paths
- Handle file collisions safely
- Do not crash on individual file failures
- Log errors to Console.Error

---

## Architecture Style

Follow this pattern:

Program.cs
  -> Services
      -> Utilities
      -> ProcessService
      -> Domain Service (DeskSweepService)

Keep code readable and senior-level clean.
Avoid cleverness.
Prefer explicit over magic.

---

## Design Philosophy

"Nail it, then scale it."

Keep v1 minimal.
Add switches only if clearly useful.
