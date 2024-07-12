namespace ShareXLauncher
{
    public class GitRelease
    {
        public string Version { get; set; } = string.Empty;
        public Uri? DownloadUrl { get; set; }
        public bool PreRelease { get; set; } = false;
    }
}
