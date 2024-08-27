

namespace OpenWindowsPatchMan.Agent.Core.Interfaces;

public interface IPatchManConfigureWindowsUpdate
{
    /// <summary>
    /// Locks the Windows Update GUI and disables certain update settings.
    /// </summary>
    void LockWindowsUpdateGUI();

}
