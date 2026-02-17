// File: Services/DeskSweepService.cs

using System.Diagnostics;

namespace HearthOwlCS.Services;

public sealed class DeskSweepService
{
    private readonly Utilities _utils;
    private readonly ProcessService _process;

    public DeskSweepService(Utilities utils, ProcessService process)
    {
        _utils = utils ?? throw new ArgumentNullException(nameof(utils));
        _process = process ?? throw new ArgumentNullException(nameof(process));
    }

    public SweepResult SweepDesktopToMyDocuments(object optionsObj)
    {
        // keep options strongly typed without exposing Program's nested record publicly
        // (simple approach: reflect the two bools we need)
        var openExplorer = GetBool(optionsObj, "OpenExplorer", defaultValue: true);
        var dryRun = GetBool(optionsObj, "DryRun", defaultValue: false);

        var now = DateTime.Now;

        // My Documents\YYYY\Month\DD
        var myDocs = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments);

        var yearFolder = Path.Combine(myDocs, now.Year.ToString());
        var monthFolder = Path.Combine(yearFolder, _utils.GetMonthName(now.Month));
        var dayFolder = Path.Combine(monthFolder, _utils.GetTwoDigitNumber(now.Day));

        EnsureDirectory(dayFolder, dryRun);

        var desktop = Environment.GetFolderPath(Environment.SpecialFolder.DesktopDirectory);
        if (!Directory.Exists(desktop))
        {
            return new SweepResult(dayFolder, 0, 0);
        }

        int dirsMoved = 0;
        int filesMoved = 0;

        // 1) Move directories first
        foreach (var srcDir in Directory.GetDirectories(desktop))
        {
            try
            {
                var destDir = Path.Combine(dayFolder, Path.GetFileName(srcDir));

                // If destination exists, we’ll make a unique folder name
                destDir = EnsureUniquePath(destDir, isDirectory: true);

                if (!dryRun)
                {
                    Directory.Move(srcDir, destDir);
                }

                dirsMoved++;
            }
            catch (Exception ex)
            {
                _utils.CatchEx(ex);
            }
        }

        // 2) Move files
        foreach (var srcFile in Directory.GetFiles(desktop))
        {
            try
            {
                var destFile = Path.Combine(dayFolder, Path.GetFileName(srcFile));

                // If destination exists, make unique filename
                destFile = EnsureUniquePath(destFile, isDirectory: false);

                if (!dryRun)
                {
                    File.Move(srcFile, destFile);
                }

                filesMoved++;
            }
            catch (Exception ex)
            {
                _utils.CatchEx(ex);
            }
        }

        // 3) Open destination (matches old clsProcess.Start(targetPathInMyDocuments))
        if (openExplorer && !dryRun)
        {
            try
            {
                _process.OpenFolder(dayFolder);
            }
            catch (Exception ex)
            {
                _utils.CatchEx(ex);
            }
        }

        return new SweepResult(dayFolder, dirsMoved, filesMoved);
    }

    private static void EnsureDirectory(string path, bool dryRun)
    {
        if (Directory.Exists(path)) return;
        if (!dryRun) Directory.CreateDirectory(path);
    }

    private static string EnsureUniquePath(string path, bool isDirectory)
    {
        if (isDirectory)
        {
            if (!Directory.Exists(path)) return path;

            var basePath = path;
            for (var i = 2; i < 10_000; i++)
            {
                var candidate = $"{basePath} ({i})";
                if (!Directory.Exists(candidate)) return candidate;
            }
        }
        else
        {
            if (!File.Exists(path)) return path;

            var dir = Path.GetDirectoryName(path) ?? "";
            var file = Path.GetFileNameWithoutExtension(path);
            var ext = Path.GetExtension(path);

            for (var i = 2; i < 10_000; i++)
            {
                var candidate = Path.Combine(dir, $"{file} ({i}){ext}");
                if (!File.Exists(candidate)) return candidate;
            }
        }

        // Extremely unlikely
        throw new IOException($"Could not generate a unique destination path for: {path}");
    }

    private static bool GetBool(object obj, string propName, bool defaultValue)
    {
        try
        {
            var p = obj.GetType().GetProperty(propName);
            if (p?.PropertyType == typeof(bool))
            {
                return (bool)(p.GetValue(obj) ?? defaultValue);
            }
        }
        catch { /* ignore */ }

        return defaultValue;
    }
}

public sealed record SweepResult(string DestinationPath, int DirectoriesMoved, int FilesMoved);
