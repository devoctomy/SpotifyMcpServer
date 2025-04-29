using ModelContextProtocol.Server;
using System.Text.Json;
using System.Text;
using SpotifyAPI.Web;
using System.ComponentModel;
using Microsoft.Extensions.Logging;
using static System.Runtime.InteropServices.JavaScript.JSType;
using ModelContextProtocol;

namespace SpotifyMcpServer
{
    [McpServerToolType]
    public class SpotifyServer
    {
        private readonly ILogger<SpotifyServer> _logger;
        private string? _accessToken;

        public SpotifyServer(ILogger<SpotifyServer> logger)
        {
            _logger = logger;
        }

        [McpServerTool, Description("Get playlists belonging to a specific user")]
        public async Task<string> GetUsersPlaylists(string userId)
        {
            _logger.LogInformation($"GetUsersPlaylists: {userId}");
            _logger.LogInformation($"Getting access token");
            var accessToken = await GetAccessToken();
            _logger.LogInformation($"Got access token");
            var api = new SpotifyClient(accessToken!, "Bearer");
            var allPlayLists = new List<FullPlaylist>();
            var playlists = await api.Playlists.GetUsers(userId);
            if (playlists == null ||
                playlists.Items == null ||
                playlists.Items.Count == 0)
            {
                var error = $"No playlists found for user {userId}.";
                _logger.LogInformation(error);
                throw new McpException(error);
            }
            allPlayLists.AddRange(playlists.Items);
            while (allPlayLists.Count < playlists.Total)
            {
                playlists = await api.NextPage(playlists);
                if (playlists == null ||
                    playlists.Items == null ||
                    playlists.Items.Count == 0)
                {
                    break;
                }

                allPlayLists.AddRange(playlists.Items);
            }

            var json = JsonSerializer.Serialize(allPlayLists);
            _logger.LogDebug(json);
            return json;
        }

        private async Task<string?> GetAccessToken()
        {
            if(_accessToken != null)
            {
                return _accessToken;
            }

            try
            {
                var clientId = Environment.GetEnvironmentVariable("SPOTIFY_API_CLIENTID");
                var clientSecret = Environment.GetEnvironmentVariable("SPOTIFY_API_SECRET");

                _logger.LogInformation($"Attempting authentication for client {clientId}");
                using var client = new HttpClient();
                var content = new StringContent($"grant_type=client_credentials&client_id={clientId}&client_secret={clientSecret}", Encoding.UTF8, "application/x-www-form-urlencoded");
                var response = await client.PostAsync("https://accounts.spotify.com/api/token", content);

                if (response.IsSuccessStatusCode)
                {
                    _logger.LogInformation("Successfully authenticating, extracting access token from response.");
                }
                else
                {
                    throw new McpException($"Authentication failed, status code {response.StatusCode}.");
                }

                var json = await response.Content.ReadAsStringAsync();
                var doc = JsonDocument.Parse(json);
                _accessToken = doc.RootElement.GetProperty("access_token").GetString();
                return _accessToken;
            }
            catch(Exception ex)
            {
                throw new McpException($"Authentication failed. {ex.Message}", ex);
            }
        }
    }
}
