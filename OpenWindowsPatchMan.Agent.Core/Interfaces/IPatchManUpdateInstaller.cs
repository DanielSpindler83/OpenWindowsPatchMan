using OpenWindowsPatchMan.Agent.Core.Models;

namespace OpenWindowsPatchMan.Agent.Core;

public interface IPatchManUpdateInstaller
{
    void InstallUpdates(List<WindowsUpdateInfo> updates);
}
