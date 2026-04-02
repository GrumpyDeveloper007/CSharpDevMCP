# Initial setup

Create local.settings.json

{
  "Values": {
    "PathToSolution": "<Path to your test project>"
    "ApplicationName": "Calculator"
  }
}

Create sample .mcp.json file in the root of your solution with the following content:

```json
{
  "inputs": [],
  "servers": {
    "test": {
      "type": "stdio",
      "command": "<Path to exe>\\CSharpDevMCP.exe",
      "args": [],
      "env": {}
    }
  }
}
```

# Test MCP

Try the following query : call the 'run_test' function
It should say something like : Output of run_test: It works!

If you have a .mcp.json file open in visual studio, you can stop and restart this tool (this is useful for testing changes to the tool).

# Sample query

## Code review

Run copilot with the following query : call get_pending_changes and produce a code review of the changes, use get_past_lessons to load past lessons learned.

or to review changes on your branch : call Get_branch_changes and produce a code review of the changes

## UI Interaction

E.g. run get_snapshot and click on the number 7 button
