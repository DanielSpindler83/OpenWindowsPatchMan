using OpenWindowsPatchMan.Agent.Core.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OpenWindowsPatchMan.Agent.Service;

public interface IPatchManUpdateChecker
{
    List<WindowsUpdateInfo> CheckForUpdates();
}

public interface IPatchManUpdateFilter
{
    List<WindowsUpdateInfo> FilterUpdates(List<WindowsUpdateInfo> updates);
}

public interface IPatchManUpdateInstaller
{
    void InstallUpdates(List<WindowsUpdateInfo> updates);
}

public interface IPatchManDatabaseService
{
    void InitializeDatabase();
    void SaveUpdateInfo(List<WindowsUpdateInfo> updatesInfo);
}
