using OpenWindowsPatchMan.Agent.Core.Models;

namespace OpenWindowsPatchMan.Agent.Core;

public interface IPatchManUpdateChecker
{
    List<WindowsUpdateInfo> CheckForUpdates();
}
