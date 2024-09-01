namespace OpenWindowsPatchMan.Agent.Core.Models;

public class WindowsUpdateInfo
{
    public string UpdateId { get; set; } = string.Empty;
    public DateTimeOffset UpdateCheckTime { get; set; } = DateTimeOffset.Now;
    public DateTimeOffset FirstSeenTime { get; set; }
    public string Title { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public List<string>? KBArticleIDs { get; set; }
    public List<string> Categories { get; set; }
    public double DownloadSizeMB { get; set; }
    public List<string>? MoreInfoUrls { get; set; }
    public int RevisionNumber { get; set; }
    public DateTime DatePublished { get; set; }
    public string? DeploymentAction { get; set; } = string.Empty;
    public bool IsBeta { get; set; } = false;
    public bool IsDownloaded { get; set; } = false;
    public bool IsHidden { get; set; } = false;
    public bool IsInstalled { get; set; } = false;
    public bool IsMandatory { get; set; } = false;
    public string? InstallationRebootBehavior { get; set; } = string.Empty;
    public bool IsUninstallable { get; set; } = false;
    public string? ReleaseNotes { get; set; } = string.Empty;
    public List<string>? UninstallationSteps { get; set; }
    public string? UninstallationNotes { get; set; } = string.Empty;
    public List<string>? SupersededUpdateIDs { get; set; }
    public List<string>? SecurityBulletinIDs { get; set; }
    public string? UninstallationRebootBehavior { get; set; } = string.Empty;
    public string Type { get; set; } = string.Empty;
    public string? SupportUrl { get; set; } = string.Empty;
    public List<string>? BundledUpdates { get; set; }

    // Navigation property
    public ICollection<UpdateInstallation> UpdateInstallations { get; set; }

    public WindowsUpdateInfo()
    {
        KBArticleIDs = new List<string>();
        Categories = new List<string>();
        MoreInfoUrls = new List<string>();
        SupersededUpdateIDs = new List<string>();
        SecurityBulletinIDs = new List<string>();
        BundledUpdates = new List<string>();
        UninstallationSteps = new List<string>();
    }

    public override string ToString()
    {
        return $"Update Check Time: {UpdateCheckTime:O}\n" +
               $"Title: {Title}\n" +
               $"Description: {Description}\n" +
               $"KB Article IDs: {string.Join(", ", KBArticleIDs)}\n" +
               $"Categories: {string.Join(", ", Categories)}\n" +
               $"Download Size: {DownloadSizeMB} MB\n" +
               $"More Info: {string.Join(", ", MoreInfoUrls)}\n" +
               $"Deployment Action: {DeploymentAction}\n" +
               $"Is Beta: {IsBeta}\n" +
               $"Is Downloaded: {IsDownloaded}\n" +
               $"Is Hidden: {IsHidden}\n" +
               $"Is Installed: {IsInstalled}\n" +
               $"Is Mandatory: {IsMandatory}\n" +
               $"Installation Behavior: {InstallationRebootBehavior}\n" +
               $"Is Uninstallable: {IsUninstallable}\n" +
               $"Release Notes: {ReleaseNotes}\n" +
               $"Uninstallation Steps: {string.Join(", ", UninstallationSteps)}\n" +
               $"Uninstallation Notes: {UninstallationNotes}\n" +
               $"Superseded Update IDs: {string.Join(", ", SupersededUpdateIDs)}\n" +
               $"Security Bulletin IDs: {string.Join(", ", SecurityBulletinIDs)}\n" +
               $"Uninstallation Reboot Behavior: {UninstallationRebootBehavior}\n" +
               $"Type: {Type}\n" +
               $"Support URL: {SupportUrl}\n" +
               $"Bundled Updates: {string.Join(", ", BundledUpdates)}";
        //$"Support URL: {InstallationResultCode}\n" +
        //$"Support URL: {RebootRequired}\n";

    }
}
