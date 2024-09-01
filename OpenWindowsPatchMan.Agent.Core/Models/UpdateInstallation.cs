namespace OpenWindowsPatchMan.Agent.Core.Models;

public class UpdateInstallation
{
    public int UpdateInstallationId { get; set; }
    public string UpdateId { get; set; }
    public string InstallationStatus { get; set; }
    public DateTime InstallationTime { get; set; }
    public string InstallationResultCode { get; set; } = string.Empty; 
    public bool RebootRequired { get; set; } 

    // Navigation property
    public WindowsUpdateInfo WindowsUpdateInfo { get; set; }
}