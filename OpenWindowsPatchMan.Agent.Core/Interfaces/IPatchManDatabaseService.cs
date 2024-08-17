using OpenWindowsPatchMan.Agent.Core.Models;

namespace OpenWindowsPatchMan.Agent.Core;

public interface IPatchManDatabaseService
{
    void InitializeDatabase();
    void SaveUpdateInfo(List<WindowsUpdateInfo> updatesInfo);
}
