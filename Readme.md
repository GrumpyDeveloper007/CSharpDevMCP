Create sample .mcp.json file in the root of your solution with the following content:

```json
{
  "inputs": [],
  "servers": {
    "test": {
      "type": "stdio",
      "command": "<Path to exe>\\CSharpDevMCP.exe",
      "args": [
        "blah",
      ],
      "env": {}
    }
  }
}
```

Run copilot with the following query : call get_pending_changes and produce a code review of the changes
