namespace OpenWindowsPatchMan.ConsoleAgent;

public class WindowsUpdateInfo
{
    public string Title { get; set; }
    public string Description { get; set; }
    public List<string> KBArticleIDs { get; set; }
    public List<string> Categories { get; set; }
    public double DownloadSizeMB { get; set; }
    public List<string> MoreInfoUrls { get; set; }

    public WindowsUpdateInfo()
    {
        KBArticleIDs = new List<string>();
        Categories = new List<string>();
        MoreInfoUrls = new List<string>();
    }

    public override string ToString()
    {
        return $"Title: {Title}\n" +
               $"Description: {Description}\n" +
               $"KB Article IDs: {string.Join(", ", KBArticleIDs)}\n" +
               $"Categories: {string.Join(", ", Categories)}\n" +
               $"Download Size: {DownloadSizeMB} MB\n" +
               $"More Info: {string.Join(", ", MoreInfoUrls)}";
    }
}
