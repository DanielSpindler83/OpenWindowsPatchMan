using OpenWindowsPatchMan.Agent.Core;
using OpenWindowsPatchMan.Agent.Core.Models;

namespace OpenWindowsPatchMan.Agent.Core.Services;

public class PatchManUpdateFilter : IPatchManUpdateFilter
{
    public List<WindowsUpdateInfo> FilterUpdates(List<WindowsUpdateInfo> updates)
    {
        // Example filtering logic
        return updates.Take(1).ToList(); // for testing purposes, only return the first update
    }
}
