
namespace OpenWindowsPatchMan.ConsoleAgent;

public class PatchManUpdateFilter : IPatchManUpdateFilter
{
    public List<WindowsUpdateInfo> FilterUpdates(List<WindowsUpdateInfo> updates)
    {
        // Example filtering logic
        return updates;
    }
}