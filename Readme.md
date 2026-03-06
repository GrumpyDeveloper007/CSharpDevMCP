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
