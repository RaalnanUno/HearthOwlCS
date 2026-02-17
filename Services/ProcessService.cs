// File: Services/ProcessService.cs

using System.Diagnostics;

namespace HearthOwlCS.Services;

public sealed class ProcessService
{
    public void OpenFolder(string folderPath)
    {
        if (string.IsNullOrWhiteSpace(folderPath))
            throw new ArgumentException("Folder path is empty.", nameof(folderPath));

        // Explorer open (best practice)
        var psi = new ProcessStartInfo
        {
            FileName = "explorer.exe",
            Arguments = $"\"{folderPath}\"",
            UseShellExecute = true
        };

        Process.Start(psi);
    }
}
