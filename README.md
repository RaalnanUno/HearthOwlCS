# 🦉 HearthOwlCS

HearthOwlCS is a lightweight Windows utility that automatically sweeps the Desktop and organizes files into a date-based structure inside **My Documents**.

It runs silently, performs its operation, optionally opens the destination folder, and exits.

No UI. No config file. No nonsense.

---

## 📦 What It Does

When executed, HearthOwlCS:

1. Reads the current date.
2. Creates a folder structure:

My Documents\YYYY\Month\DD\


Example:

My Documents\2026\February\17\


3. Moves:
   - All folders from Desktop
   - All files from Desktop
4. Opens the destination folder in Windows Explorer (unless disabled)
5. Exits

---

## 🏗 Architecture

Target Framework:
- `net8.0-windows`

Structure:

