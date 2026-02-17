// File: Program.cs
// Project: HearthOwlCS
//
// Behavior:
//  - Sweep Desktop -> My Documents\YYYY\Month\DD\
//  - Move directories then files
//  - Open destination folder (optional; matches old behavior)
//  - Exit

using HearthOwlCS.Services;

internal static class Program
{
    private static int Main(string[] args)
    {
        // Optional switches:
        //  --no-open   -> don't open Explorer at end
        //  --dry-run   -> log what would move, but don't move
        var options = AppOptions.Parse(args);

        var utilities = new Utilities();
        var process = new ProcessService();
        var sweeper = new DeskSweepService(utilities, process);

        try
        {
            var result = sweeper.SweepDesktopToMyDocuments(options);

            // Simple console output for troubleshooting / Task Scheduler logs
            Console.WriteLine($"Moved dirs: {result.DirectoriesMoved}, files: {result.FilesMoved}");
            Console.WriteLine($"Destination: {result.DestinationPath}");

            return 0;
        }
        catch (Exception ex)
        {
            // Keep it simple: write to stderr so scheduler logs capture it
            Console.Error.WriteLine(ex.ToString());
            return 1;
        }
    }

    private sealed record AppOptions(bool OpenExplorer, bool DryRun)
    {
        public static AppOptions Parse(string[] args)
        {
            var openExplorer = true;
            var dryRun = false;

            foreach (var a in args.Select(a => a.Trim().ToLowerInvariant()))
            {
                if (a is "--no-open" or "/no-open") openExplorer = false;
                if (a is "--dry-run" or "/dry-run") dryRun = true;
            }

            return new AppOptions(openExplorer, dryRun);
        }
    }
}
