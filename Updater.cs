using System.Diagnostics;
using System.Text.Json.Nodes;

namespace ShareXLauncher
{
    internal class Updater
    {
        private readonly HttpHandler _HttpHandler = new HttpHandler();
        private GitRelease gitRelease = new GitRelease();
        private bool IsPortable = false;
        private string LocalVersion = string.Empty;
        private bool AutoLaunch = false;

        public async Task Run()
        {
            string JsonString = _HttpHandler.Get().Result;
            if(File.Exists("portable"))
                IsPortable = true;

            if (!string.IsNullOrEmpty(JsonString))
            {
                JsonHandler JHandler = new JsonHandler();
                JsonNode Json = JHandler.GetJsonNode(JsonString).Result;
                LocalVersion = $"v"+JHandler.GetJsonNodeFromFile("ShareX/ApplicationConfig.json").Result["ApplicationVersion"].ToString();

                gitRelease = new GitRelease()
                {
                    Version = Json["tag_name"]?.ToString() ?? string.Empty,
                    PreRelease = (bool)Json["prerelease"]
                };

                if(IsPortable)
                {
                    if ((bool)(Json["assets"][0]?.ToString().Contains("Portable", StringComparison.CurrentCultureIgnoreCase)))
                    {
                        gitRelease.DownloadUrl = new Uri(Json["assets"][0]["browser_download_url"]?.ToString());
                    }
                    else
                    {
                        gitRelease.DownloadUrl = new Uri(Json["assets"][1]["browser_download_url"]?.ToString());
                    }
                }
                else
                {
                    if (!(bool)(Json["assets"][0]?.ToString().Contains("Portable", StringComparison.CurrentCultureIgnoreCase)))
                    {
                        gitRelease.DownloadUrl = new Uri(Json["assets"][0]["browser_download_url"]?.ToString());
                    }
                    else
                    {
                        gitRelease.DownloadUrl = new Uri(Json["assets"][1]["browser_download_url"]?.ToString());
                    }
                }
            }
            else
            {
                Console.WriteLine($"{this.GetType().Name}.Run(): JsonString was null.\nPress any key to exit.");
                Console.ReadKey();
                Environment.Exit(0);
            }

            if (HasUpdate())
            {
                ArchiveHandler archiveHandler = new ArchiveHandler();
                await _HttpHandler.Download(gitRelease.DownloadUrl, gitRelease.DownloadUrl.Segments[6]);

                if (IsPortable)
                {
                    using (StreamReader reader = new("Portable"))
                    {
                        bool.TryParse(reader.ReadLineAsync().Result, out AutoLaunch);
                    }

                    archiveHandler.Extract(gitRelease.DownloadUrl.Segments[6]);

                    using (StreamWriter writer = new("Portable"))
                    {
                        writer.WriteLine(AutoLaunch.ToString());
                    }
                }
                else
                {
                    ProcessStartInfo processStartInfo = new ProcessStartInfo() { FileName = $"{Path.GetTempPath()}/{gitRelease.DownloadUrl.Segments[6]}" };
                    Console.WriteLine($"Launching {processStartInfo.FileName}");
                    Process.Start(processStartInfo);
                }
                
                Console.WriteLine("Update done!");
            }

            if (AutoLaunch)
            {
                ProcessStartInfo processStartInfo = new ProcessStartInfo() { FileName = "ShareX.exe", };
                Process.Start(processStartInfo);
            }
        }

        bool HasUpdate()
        {
            if (!File.Exists("ShareX.exe"))
            {
                Console.WriteLine("ShareX does not exist!\n"
                    + "Make sure you've placed this file in the same folder as the ShareX executable!\n"
                    + "Press any key to close.");
                Console.ReadKey();
                Environment.Exit(0);
            }

            Console.WriteLine($"LocalVersion: {LocalVersion}\nRemoteVersion: {gitRelease.Version}");

            if (LocalVersion != gitRelease.Version)
            {
                Console.WriteLine("Update Detected, updating");
                return true;
            }

            Console.WriteLine("Versions match, launching.");
            AutoLaunch = true;
            return false;
        }
    }
}
