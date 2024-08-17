using OpenWindowsPatchMan.Agent.Core.Models;

namespace OpenWindowsPatchMan.Agent.Core;

public interface IPatchManUpdateFilter
{
    List<WindowsUpdateInfo> FilterUpdates(List<WindowsUpdateInfo> updates);
}
