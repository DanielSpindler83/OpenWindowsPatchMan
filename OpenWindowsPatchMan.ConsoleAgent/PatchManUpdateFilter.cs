using OpenWindowsPatchMan.Agent.Core.Models;

namespace OpenWindowsPatchMan.Agent.Service;

public class PatchManUpdateFilter : IPatchManUpdateFilter
{
    public List<WindowsUpdateInfo> FilterUpdates(List<WindowsUpdateInfo> updates)
    {
        // Example filtering logic
        return updates;
    }
}