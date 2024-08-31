using System;
using System.Diagnostics;
using System.ServiceProcess;
using Microsoft.Win32;
using OpenWindowsPatchMan.Agent.Core.Interfaces;

namespace OpenWindowsPatchMan.Agent.Core.Services;


public class PatchManConfigureWindowsUpdate : IPatchManConfigureWindowsUpdate
{
    /// <summary>
    /// Locks the Windows Update GUI and disables certain update settings.
    /// </summary>
    /// 

    ///////////////// We need to ensure this runs as admin or it doesnt work....
    public void LockWindowsUpdateGUI()
    {
        try
        {
            // Define registry paths
            string rootKey = "HKLM";
            string windowsUpdateSubKey = @"SOFTWARE\Policies\Microsoft\Windows\WindowsUpdate\AU";
            string disableWindowsUpdateAccessSubKey = @"SOFTWARE\Policies\Microsoft\Windows\WindowsUpdate";

            // Backup registry keys before modification
            BackupRegistryKey(rootKey, windowsUpdateSubKey);
            BackupRegistryKey(rootKey, disableWindowsUpdateAccessSubKey);

            // Modify Windows Update settings
            using (RegistryKey key = RegistryKey.OpenBaseKey(RegistryHive.LocalMachine, RegistryView.Registry64)
                .CreateSubKey(windowsUpdateSubKey))
            {
                key.SetValue("NoAutoUpdate", 1, RegistryValueKind.DWord); // Disable automatic updates
                key.SetValue("NoAUOptions", 1, RegistryValueKind.DWord);  // Disable access to update settings
            }

            // Disable access to Windows Update GUI
            using (RegistryKey key = RegistryKey.OpenBaseKey(RegistryHive.LocalMachine, RegistryView.Registry64)
                .CreateSubKey(disableWindowsUpdateAccessSubKey))
            {
                key.SetValue("SetDisableUXWUAccess", 1, RegistryValueKind.DWord); // Disable access to Windows Update features
            }

            Console.WriteLine("Windows Update GUI settings have been successfully locked.");

            // Restart Windows Update service to apply changes - i dont think this is needed but maybe cant hurt?
            RestartWindowsUpdateService();
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error locking Windows Update GUI: {ex.Message}");
            // Additional logging or error handling can be added here
        }
    }

    /// <summary>
    /// Restarts the Windows Update service to apply registry changes.
    /// </summary>
    public void RestartWindowsUpdateService()
    {
        try
        {
            using (ServiceController service = new ServiceController("wuauserv"))
            {
                if (service.Status == ServiceControllerStatus.Running)
                {
                    service.Stop();
                    service.WaitForStatus(ServiceControllerStatus.Stopped, TimeSpan.FromSeconds(15));
                    Console.WriteLine("Windows Update service stopped successfully.");
                }

                service.Start();
                service.WaitForStatus(ServiceControllerStatus.Running, TimeSpan.FromSeconds(15));
                Console.WriteLine("Windows Update service started successfully.");
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error restarting Windows Update service: {ex.Message}");
            // Additional logging or error handling can be added here
        }
    }

    /// <summary>
    /// Backs up the specified registry key and saves it to a .reg file with a timestamp.
    /// </summary>
    /// <param name="rootKey">The root registry hive (e.g., HKLM, HKCU).</param>
    /// <param name="subKeyPath">The subkey path to backup.</param>
    /// <param name="registryView">The registry view (Default, Registry32, Registry64).</param>
    public void BackupRegistryKey(string rootKey, string subKeyPath, RegistryView registryView = RegistryView.Default)
    {
        try
        {
            // Validate root key
            if (!IsValidRootKey(rootKey))
            {
                throw new ArgumentException($"Invalid root key specified: {rootKey}");
            }

            // Prepare full key path
            string fullKeyPath = $"{rootKey}\\{subKeyPath}";

            // Prepare backup file path
            string dateTime = DateTime.Now.ToString("yyyyMMdd_HHmmss");
            string sanitizedKeyPath = fullKeyPath.Replace('\\', '_').Replace('/', '_');
            string fileName = $"{sanitizedKeyPath}_backup_{dateTime}.reg";
            string backupDirectory = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "RegistryBackups");
            Directory.CreateDirectory(backupDirectory); // Ensure directory exists
            string backupFilePath = Path.Combine(backupDirectory, fileName);

            // Prepare registry view argument
            string registryViewArgument = registryView switch
            {
                RegistryView.Registry32 => "/reg:32",
                RegistryView.Registry64 => "/reg:64",
                _ => string.Empty
            };

            // Prepare the command to export the registry key
            string arguments = $"EXPORT \"{fullKeyPath}\" \"{backupFilePath}\" /y {registryViewArgument}".Trim();

            // Execute the command
            using (Process process = new Process())
            {
                process.StartInfo.FileName = "reg.exe";
                process.StartInfo.Arguments = arguments;
                process.StartInfo.RedirectStandardOutput = true;
                process.StartInfo.RedirectStandardError = true;
                process.StartInfo.UseShellExecute = false;
                process.StartInfo.CreateNoWindow = true;

                process.Start();
                string output = process.StandardOutput.ReadToEnd();
                string error = process.StandardError.ReadToEnd();
                process.WaitForExit();

                if (process.ExitCode == 0)
                {
                    Console.WriteLine($"Successfully backed up registry key '{fullKeyPath}' to '{backupFilePath}'.");
                }
                else
                {
                    Console.WriteLine($"Failed to back up registry key '{fullKeyPath}'. Exit Code: {process.ExitCode}");
                    Console.WriteLine($"Error Message: {error}");
                }
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error backing up registry key '{rootKey}\\{subKeyPath}': {ex.Message}");
            // Additional logging or error handling can be added here
        }
    }

    /// <summary>
    /// Validates if the provided root key is valid.
    /// </summary>
    /// <param name="rootKey">The root key to validate.</param>
    /// <returns>True if valid; otherwise, false.</returns>
    private bool IsValidRootKey(string rootKey)
    {
        string[] validRootKeys = { "HKLM", "HKCU", "HKCR", "HKU", "HKCC" };
        return Array.Exists(validRootKeys, key => key.Equals(rootKey, StringComparison.OrdinalIgnoreCase));
    }
}