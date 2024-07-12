using System.Text;
using System.Text.Json.Nodes;

namespace ShareXLauncher
{
    internal class JsonHandler
    {

        public async Task<JsonNode> GetJsonNode(string JsonString)
        {
            byte[] byteArray = Encoding.UTF8.GetBytes(JsonString);

            JsonNode? jsonNode = await JsonNode.ParseAsync(new MemoryStream(byteArray));
            if (jsonNode != null)
            {
                return jsonNode;
            }

            Console.WriteLine($"{this.GetType().Name}: JsonNode was null.\nPress any key to exit.");
            Console.ReadKey();
            Environment.Exit(0);
            return null;
        }

        public async Task<JsonNode> GetJsonNodeFromFile(string FileLocation)
        {
            using (StreamReader reader = new(FileLocation))
            {
                string json = await reader.ReadToEndAsync();
                return await GetJsonNode(json);
            }
        }
    }
}