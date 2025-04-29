## Spotify MCP Server

A simple STDIO MCP server for Spotify.

After not having any luck with existing tools that required a return url, I decided to write one myself.

### Requirements

- .NET 8.0 SDK

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