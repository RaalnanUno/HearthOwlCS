// File: Services/Utilities.cs

namespace HearthOwlCS.Services;

public sealed class Utilities
{
    public string GetTwoDigitNumber(int n) => n.ToString("00");

    public string GetMonthName(int month)
    {
        // Equivalent vibe to VB's month name usage; invariant for folder naming
        // (If you want localized month names, use CultureInfo.CurrentCulture instead.)
        return month switch
        {
            1 => "January",
            2 => "February",
            3 => "March",
            4 => "April",
            5 => "May",
            6 => "June",
            7 => "July",
            8 => "August",
            9 => "September",
            10 => "October",
            11 => "November",
            12 => "December",
            _ => throw new ArgumentOutOfRangeException(nameof(month), month, "Month must be 1-12.")
        };
    }

    public void CatchEx(Exception ex)
    {
        // Minimal replacement for your VB clsUtilities.CatchEx(ex)
        // Later we can upgrade this to:
        //  - write to log file in %LocalAppData%\HearthOwlCS\logs\
        //  - Windows Event Log
        Console.Error.WriteLine(ex.Message);
    }
}
