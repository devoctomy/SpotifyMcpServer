## Spotify MCP Server

A simple STDIO MCP server for Spotify.

After not having any luck with existing tools that required a return url, I decided to write one myself.

### Requirements

- .NET 8.0 SDK

Following environment variables

```
SPOTIFY_API_CLIENTID
SPOTIFY_API_SECRET
```

You can get these by creating an application in Spotify developer portal. You can enter anything for the
return url as it will not be used, as long as it accepts what you enter.

### Configure in Cursor

```
{
  "mcpServers": {
    "spotify": {
      "command": "<full path>\\SpotifyMcpServer.exe",
      "args": []
    }
  }
}
```

### Current Tools

1. GetUserPlaylists - Gets playlists for a specific user by user id.
