using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OpenWindowsPatchMan.ConsoleAgent;

public class PatchManUpdateFilter : IPatchManUpdateFilter
{
    public List<WindowsUpdateInfo> FilterUpdates(List<WindowsUpdateInfo> updates)
    {
        // Example filtering logic
        return updates;
    }
}