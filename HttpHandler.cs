using System.Net;

namespace ShareXLauncher
{
    class HttpHandler
    {
        private readonly Uri _url = new Uri("https://api.github.com/repos/ShareX/ShareX/releases/latest");
        private HttpClient _httpClient = new HttpClient();

        public async Task<string> Get()
        {
            _httpClient.DefaultRequestHeaders.Add("Accept", "application/vnd.github+json");
            _httpClient.DefaultRequestHeaders.Add("User-Agent", "request");

            HttpResponseMessage? Response = await _httpClient.GetAsync(_url);
            if (Response == null)
                return $"{this.GetType().Name}: Unable to get json.";

            if (Response.StatusCode == HttpStatusCode.OK)
            {
                return await Response.Content.ReadAsStringAsync();
            }
            else
            {
                switch (Response.StatusCode)
                {
                    case HttpStatusCode.NotFound:
                        Console.WriteLine($"{this.GetType().Name}.Get(): {Response.StatusCode}");
                        break;
                    case HttpStatusCode.Unauthorized:
                        Console.WriteLine($"{this.GetType().Name}.Get(): {Response.StatusCode}");
                        break;
                    case HttpStatusCode.Forbidden:
                        Console.WriteLine($"{this.GetType().Name}.Get(): {Response.StatusCode}");
                        break;
                    case HttpStatusCode.NoContent:
                        Console.WriteLine($"{this.GetType().Name}.Get(): {Response.StatusCode}");
                        break;
                    default:
                        Console.WriteLine($"{this.GetType().Name}.Get(): {Response.StatusCode}");
                        break;
                }
                return string.Empty;
            }
        }

        public async Task Download(Uri uri, string FileLocation)
        {
            _httpClient = new HttpClient();
            var Response = _httpClient.GetAsync(uri).Result;
            Stream DownloadStream = Response.Content.ReadAsStreamAsync().Result;
            if (DownloadStream == null)
            {
                Console.WriteLine($"AAAAAAAAAAAAA");
                return;
            }

            Console.WriteLine($"Downloading {FileLocation}");
            using (FileStream fs = new FileStream($"{Path.GetTempPath()}/{FileLocation}", FileMode.Create))
            {
                DownloadStream.CopyTo(fs);
            }
            Console.WriteLine($"Done!");
        }
    }
}
