using OpenWindowsPatchMan.Agent.Core.Models;

namespace OpenWindowsPatchMan.Agent.Core;

public interface IPatchManUpdateService
{
    List<WindowsUpdateInfo> CheckForUpdates();
    List<WindowsUpdateInfo> FilterUpdates(List<WindowsUpdateInfo> updates);
    void InstallFilteredUpdates();
    void RetrieveUpdateHistory();
}
