using WUApiLib;

namespace OpenWindowsPatchMan.ConsoleAgent;

class Program
{
    static void Main(string[] args)
    {
        try
        {
            // Create UpdateSession object
            UpdateSession updateSession = new UpdateSession();

            // Create UpdateSearcher object
            IUpdateSearcher updateSearcher = updateSession.CreateUpdateSearcher();

            // Search for updates
            ISearchResult searchResult = updateSearcher.Search("IsInstalled=0");

            Console.WriteLine($"Found {searchResult.Updates.Count} updates:");

            List<WindowsUpdateInfo> updatesInfo = new List<WindowsUpdateInfo>();

            // Iterate through updates
            foreach (IUpdate update in searchResult.Updates)
            {
                double downloadSizeMB = Math.Round((double)update.MaxDownloadSize / 1024.0 / 1024.0, 2);

                WindowsUpdateInfo updateInfo = new WindowsUpdateInfo
                {
                    Title = update.Title,
                    Description = update.Description,
                    DownloadSizeMB = downloadSizeMB
                };

                // Convert COM objects to strings
                foreach (string kbId in update.KBArticleIDs)
                {
                    updateInfo.KBArticleIDs.Add(kbId);
                }

                foreach (ICategory category in update.Categories)
                {
                    updateInfo.Categories.Add(category.Name);
                }

                foreach (string url in update.MoreInfoUrls)
                {
                    updateInfo.MoreInfoUrls.Add(url);
                }

                updatesInfo.Add(updateInfo);
            }

            // Print the updates information
            foreach (var updateInfo in updatesInfo)
            {
                Console.WriteLine(updateInfo);
                Console.WriteLine();
            }

            // Optionally, install updates
            Console.Write("Do you want to install these updates? (y/n): ");
            var input = Console.ReadLine();
            if (input?.ToLower() == "y")
            {
                InstallUpdates(searchResult.Updates);
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine($"An error occurred: {ex.Message}");
        }
    }

    private static void InstallUpdates(UpdateCollection updates)
    {
        UpdateSession updateSession = new UpdateSession();
        IUpdateInstaller updateInstaller = updateSession.CreateUpdateInstaller();
        updateInstaller.Updates = updates;

        Console.WriteLine("Starting installation...");
        IInstallationResult installationResult = updateInstaller.Install();

        Console.WriteLine($"Installation result: {installationResult.ResultCode}");
        Console.WriteLine($"Reboot required: {installationResult.RebootRequired}");
    }
}