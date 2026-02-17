This is my project.
I want to convert it to a C# project.
Project name will be HearthOwlCS.
Right now, it looks at app.config for a boolean "AutoRun" variable.
We will always have AutoRun be true.
Also, the old version had a form, but we don't really need a form for the cS version. It should simply run and close.

## frmDeskSweep.vb

```vb
'This is required since Microsoft, in all their glory, left this out of the .NET framework
'You must go to "Project->Add Reference->COM Tab" and locate "Windows Script Host Object Model"
'Of course this project already has it added as a reference.

Imports IWshRuntimeLibrary
Imports System.IO
Public Class frmDeskSweep

#Region "Region"

#Region "Declaration"

#End Region

#Region "Function"

#End Region

#Region "Sub"

#End Region

#End Region

#Region "Declaration"
    Dim WikiSiteRoot As String
    Dim TargetDirectoryPath As String
    Dim DeskTopDirectoryPath As String
    Dim SourceFileName As String
    Dim DestFileName As String
    Dim FilePath As String
    Dim FilePaths() As String
    Dim SourceDirectory As String
    Dim SourceDirectories() As String
    Dim ShortCutFileName As String
    Dim WikiEditRoot As String
    Dim UserName As String
    Dim MovedFilePath As String '   The path of the file after it has been moved.
    Dim AutoRun As Boolean
    Dim CreationTime As Date
    Dim LastAccessTime As Date

    Dim File As System.IO.File
    Dim Path As Path
    Dim clsProcess As clsProcess
    Dim clsUtilities As clsUtilities
#End Region

#Region "Function"
    Private Function getAppSetting(ByVal AppSettingName) As String
        Return System.Configuration.ConfigurationSettings.AppSettings(AppSettingName)
    End Function
#End Region

#Region "Sub"

    Private Sub CreateShortcut(ByVal IconLocation As String, ByVal TargetPath As String, ByVal Arguments As String, ByVal ShortCutFileName As String, ByVal ShortCutSavePath As String)
        'Call functions from the Windows Script Host
        Dim objShell As IWshRuntimeLibrary.WshShell
        Dim objShortcut As IWshRuntimeLibrary.IWshShortcut
        'Initialize the Object WshShell
        objShell = New WshShell
        'This line will determine where you want the shortcut to be created
        objShortcut = objShell.CreateShortcut(ShortCutSavePath + ShortCutFileName)

        'This line sets the icon for the shortcut.
        If IconLocation = "" Then
        Else
            objShortcut.IconLocation = IconLocation
        End If

        'This will determine what file or path the shortcut is pointing too
        objShortcut.TargetPath = TargetPath

        'Arguments are used to add additional commands (Switches) to your shortcut.  If you don't know what arguments are you don't need this command here
        If Arguments = "" Then
        Else
            objShortcut.Arguments = Arguments
        End If

        'Create and save the shortcut to the path chosen.
        objShortcut.Save()
    End Sub

    Private Sub frmDeskSweep_Load(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles MyBase.Load
        Me.Text = "Loading..."
        Me.Enabled = False
        If clsProcess Is Nothing Then clsProcess = New clsProcess
        WikiSiteRoot = getAppSetting("WikiSiteRoot") + Application.ProductName
        WikiEditRoot = getAppSetting("WikiEditRoot") + Application.ProductName
        UserName = getAppSetting("UserName")
        AutoRun = getAppSetting("AutoRun")

        If AutoRun = True Then
            '            doSweep()
            SaveToMyDocuments()
            Close()
        Else
            '            WebBrowser1.Url = New Uri(WikiSiteRoot)
        End If


        Me.Text = Application.ProductName + " " + Application.ProductVersion
        Me.Enabled = True
    End Sub

    Private Sub SaveToMyDocuments()
        If clsUtilities Is Nothing Then clsUtilities = New clsUtilities
        If clsProcess Is Nothing Then clsProcess = New clsProcess

        ' Get the "My Documents" directory path
        Dim myDocumentsPath As String = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments)

        ' Set up the target directory path within "My Documents"
        Dim yearFolder As String = Path.Combine(myDocumentsPath, Year(Now).ToString())
        Dim monthFolder As String = Path.Combine(yearFolder, clsUtilities.getMonthName(Month(Now)))
        Dim targetPathInMyDocuments As String = Path.Combine(monthFolder, clsUtilities.getTwoDigitNumber(Now.Day))


        ' Create the target directory if it doesn't exist
        If Not Directory.Exists(targetPathInMyDocuments) Then
            Directory.CreateDirectory(targetPathInMyDocuments)
        End If

        ' Get the desktop directory path
        DeskTopDirectoryPath = Environment.GetFolderPath(Environment.SpecialFolder.Desktop)

        ' Check if the desktop directory exists
        If Directory.Exists(DeskTopDirectoryPath) Then
            ' Process directories
            SourceDirectories = Directory.GetDirectories(DeskTopDirectoryPath)
            For Each Me.SourceDirectory In SourceDirectories
                Try
                    Dim destDirName As String = Path.Combine(targetPathInMyDocuments, Path.GetFileName(SourceDirectory))
                    Directory.Move(SourceDirectory, destDirName)
                Catch ex As Exception
                    clsUtilities.CatchEx(ex)
                End Try
            Next

            ' Process files
            FilePaths = Directory.GetFiles(DeskTopDirectoryPath)
            For Each Me.FilePath In FilePaths
                Try
                    Dim destFileName As String = Path.Combine(targetPathInMyDocuments, Path.GetFileName(FilePath))
                    System.IO.File.Move(FilePath, destFileName)
                Catch ex As Exception
                    clsUtilities.CatchEx(ex)
                End Try
            Next
        End If

        ' Optional: Start the target path in explorer or perform other actions
        clsProcess.Start(targetPathInMyDocuments)
    End Sub

    Private Sub doSweep()
        If clsUtilities Is Nothing Then clsUtilities = New clsUtilities
        ShortCutFileName = clsUtilities.getMonthName(Month(Today)) + clsUtilities.getTwoDigitNumber(Today.Day) + ".lnk"

        If System.IO.File.Exists(ShortCutFileName) Then
            clsProcess.Start(clsUtilities.getTwoDigitNumber(Today.Day) + ".lnk")
        Else
            If UserName Is Nothing Then
                MsgBox("UserName Is Nothing")
            Else
                If UserName = "" Then
                    MsgBox("UserName = ''")
                Else

                    If clsUtilities Is Nothing Then clsUtilities = New clsUtilities
                    If clsProcess Is Nothing Then clsProcess = New clsProcess

                    DeskTopDirectoryPath = Environment.GetFolderPath(Environment.SpecialFolder.Desktop) '
                    'DeskTopDirectoryPath = "c:\Users\" + UserName + "\Desktop\"

                    TargetDirectoryPath = Path.GetPathRoot(Application.StartupPath)
                    TargetDirectoryPath = TargetDirectoryPath + Year(Now).ToString + "\"
                    TargetDirectoryPath = TargetDirectoryPath + clsUtilities.getMonthName(Month(Now)) + "\"
                    TargetDirectoryPath = TargetDirectoryPath + clsUtilities.getTwoDigitNumber(Now.Day) + "\"

                    If Directory.Exists(TargetDirectoryPath) = False Then Directory.CreateDirectory(TargetDirectoryPath)

                    If Directory.Exists(DeskTopDirectoryPath) = False Then
                    Else
                        If Directory.Exists(TargetDirectoryPath) = False Then
                        Else
                            SourceDirectories = Directory.GetDirectories(DeskTopDirectoryPath)
                            For Each Me.SourceDirectory In SourceDirectories
                                Try
                                    Dim SourceDirName As String
                                    Dim DestDirName As String

                                    SourceDirName = SourceDirectory
                                    DestDirName = TargetDirectoryPath + Path.GetFileName(SourceDirectory)

                                    Directory.Move(SourceDirName, DestDirName)

                                Catch ex As Exception
                                    clsUtilities.CatchEx(ex)
                                End Try
                            Next

                            FilePaths = Directory.GetFiles(DeskTopDirectoryPath)
                            For Each Me.FilePath In FilePaths
                                Try
                                    CreationTime = IO.File.GetCreationTime(FilePath)
                                    LastAccessTime = IO.File.GetLastAccessTime(FilePath)

                                    SourceFileName = Path.GetFileName(FilePath)
                                    DestFileName = TargetDirectoryPath + SourceFileName
                                    Try
                                        System.IO.File.Move(FilePath, DestFileName)
                                    Catch ex As Exception
                                        clsUtilities.CatchEx(ex)
                                    End Try
                                Catch ex As Exception

                                End Try
                            Next
                        End If
                    End If


                    '                    CreateShortcut("", TargetDirectoryPath, "", ShortCutFileName, DeskTopDirectoryPath)
                    clsProcess.Start(TargetDirectoryPath)
                End If
            End If
        End If

    End Sub

    Private Sub doSweep_1605()
        If clsUtilities Is Nothing Then clsUtilities = New clsUtilities
        ShortCutFileName = clsUtilities.getMonthName(Month(Today)) + clsUtilities.getTwoDigitNumber(Today.Day) + ".lnk"

        If System.IO.File.Exists(ShortCutFileName) Then
            clsProcess.Start(clsUtilities.getTwoDigitNumber(Today.Day) + ".lnk")
        Else
            If UserName Is Nothing Then
                MsgBox("UserName Is Nothing")
            Else
                If UserName = "" Then
                    MsgBox("UserName = ''")
                Else

                    If clsUtilities Is Nothing Then clsUtilities = New clsUtilities
                    If clsProcess Is Nothing Then clsProcess = New clsProcess
                    DeskTopDirectoryPath = "C:\Documents and Settings\" + UserName + "\Desktop\"
                    TargetDirectoryPath = Path.GetPathRoot(Application.StartupPath)
                    TargetDirectoryPath = TargetDirectoryPath + Year(Now).ToString + "\"
                    TargetDirectoryPath = TargetDirectoryPath + clsUtilities.getMonthName(Month(Now)) + "\"
                    TargetDirectoryPath = TargetDirectoryPath + clsUtilities.getTwoDigitNumber(Now.Day) + "\"

                    If Directory.Exists(TargetDirectoryPath) = False Then Directory.CreateDirectory(TargetDirectoryPath)

                    If Directory.Exists(DeskTopDirectoryPath) = False Then
                    Else
                        If Directory.Exists(TargetDirectoryPath) = False Then
                        Else
                            SourceDirectories = Directory.GetDirectories(DeskTopDirectoryPath)
                            For Each Me.SourceDirectory In SourceDirectories
                                Try
                                    Dim SourceDirName As String
                                    Dim DestDirName As String

                                    SourceDirName = SourceDirectory
                                    DestDirName = TargetDirectoryPath + Path.GetFileName(SourceDirectory)

                                    Directory.Move(SourceDirName, DestDirName)

                                Catch ex As Exception
                                    clsUtilities.CatchEx(ex)
                                End Try
                            Next

                            FilePaths = Directory.GetFiles(DeskTopDirectoryPath)
                            For Each Me.FilePath In FilePaths
                                FilePath = MoveFile_1605(FilePath)

                            Next
                        End If
                    End If


                    CreateShortcut("", TargetDirectoryPath, "", ShortCutFileName, DeskTopDirectoryPath)
                    clsProcess.Start(TargetDirectoryPath)
                End If
            End If
        End If

    End Sub

    Private Sub EditSitePageToolStripMenuItem_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles EditSitePageToolStripMenuItem.Click
        If clsProcess Is Nothing Then clsProcess = New clsProcess
        clsProcess.Start(WikiEditRoot)
    End Sub

    Private Sub SweepToolStripMenuItem_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles SweepToolStripMenuItem.Click
        '        doSweep()
        SaveToMyDocuments()
    End Sub

    Private Sub BrowseWorkingDirectoryToolStripMenuItem_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles BrowseWorkingDirectoryToolStripMenuItem.Click
        If clsProcess Is Nothing Then clsProcess = New clsProcess
        clsProcess.Start(Application.StartupPath)
    End Sub

#End Region

    Private Sub SToolStripMenuItem_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles SToolStripMenuItem.Click
        doSweep_1605()
    End Sub

    Private Function MoveFile_1605(ByVal FilePath As String) As String
        Dim ShortCutPath As String

        DestFileName = "DestFileName"
        CreationTime = IO.File.GetCreationTime(FilePath)
        LastAccessTime = IO.File.GetLastAccessTime(FilePath)

        TargetDirectoryPath = Path.GetPathRoot(Application.StartupPath)
        TargetDirectoryPath = TargetDirectoryPath + Year(CreationTime).ToString + "\"
        TargetDirectoryPath = TargetDirectoryPath + clsUtilities.getMonthName(Month(CreationTime)) + "\"
        TargetDirectoryPath = TargetDirectoryPath + clsUtilities.getTwoDigitNumber(CreationTime.Day) + "\"

        If Directory.Exists(TargetDirectoryPath) = False Then
            Directory.CreateDirectory(TargetDirectoryPath)
        End If

        Try
            SourceFileName = Path.GetFileName(FilePath)
            DestFileName = TargetDirectoryPath + SourceFileName
            System.IO.File.Move(FilePath, DestFileName)
        Catch ex As Exception
            If clsUtilities Is Nothing Then clsUtilities = New clsUtilities
            clsUtilities.CatchEx(ex)
        End Try

        If LastAccessTime = CreationTime Then
            '   No need for shortcuts if the creation and last access are the same.
        Else
            ShortCutPath = MakeShortCut_1605(DestFileName)

        End If

        Return ShortCutPath
    End Function

    Private Function MakeShortCut_1605(ByVal FilePath As String) As String
        Dim TargetPath As String
        Dim ShortCutSavePath As String

        DestFileName = "DestFileName"
        CreationTime = IO.File.GetCreationTime(FilePath)
        LastAccessTime = IO.File.GetLastAccessTime(FilePath)

        TargetDirectoryPath = Path.GetPathRoot(Application.StartupPath)
        TargetDirectoryPath = TargetDirectoryPath + Year(LastAccessTime).ToString + "\"
        TargetDirectoryPath = TargetDirectoryPath + clsUtilities.getMonthName(Month(LastAccessTime)) + "\"
        TargetDirectoryPath = TargetDirectoryPath + clsUtilities.getTwoDigitNumber(LastAccessTime.Day) + "\"
        ShortCutSavePath = TargetDirectoryPath

        If Directory.Exists(ShortCutSavePath) = False Then
            Directory.CreateDirectory(ShortCutSavePath)
        End If

        TargetPath = FilePath

        Try
            CreateShortcut("", TargetPath, "", Path.GetFileNameWithoutExtension(TargetPath), ShortCutSavePath)
        Catch ex As Exception
            If clsUtilities Is Nothing Then clsUtilities = New clsUtilities
            clsUtilities.CatchEx(ex)
        End Try


        DestFileName = ShortCutSavePath + ShortCutFileName

        Return DestFileName
    End Function
End Class
```