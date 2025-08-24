namespace ShareXLauncher
{
    internal class ArchiveHandler
    {
        public void Extract(string FileName)
        {
            FileInfo file = new FileInfo($"{Path.GetTempPath()}/{FileName}");
            string ExtractDirectoryLocation = new FileInfo("ShareX.exe").DirectoryName;
            if (file.Exists)
            {
                Console.WriteLine($"Extracting to: {ExtractDirectoryLocation}");
                System.IO.Compression.ZipFile.ExtractToDirectory($"{Path.GetTempPath()}/{FileName}", $"{ExtractDirectoryLocation}", true);
                Console.WriteLine($"Done!");
                return;
            }

            Console.WriteLine($"ShareX zip file is missing! Did it download?\nPlease check {Path.GetTempPath()} for {FileName}\nPress any key to exit.");
            Console.ReadLine();
            Environment.Exit(0);
        }
    }
}
