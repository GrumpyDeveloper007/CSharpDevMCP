# Lessons 

This doc describes lessons learned from past changes.
Each heading represents a folder within the repo.

## CSharpDevMCP

- An example lesson to keep in mind when reviewing future changes.

Language guidelines

The following sections describe practices that the .NET docs team follows to prepare code examples and samples. In general, follow these practices:

    Utilize modern language features and C# versions whenever possible.
    Avoid outdated language constructs.
    Only catch exceptions that can be properly handled; avoid catching general exceptions. For example, sample code shouldn't catch the System.Exception type without an exception filter.
    Use specific exception types to provide meaningful error messages.
    Use LINQ queries and methods for collection manipulation to improve code readability.
    Use asynchronous programming with async and await for I/O-bound operations.
    Be cautious of deadlocks and use Task.ConfigureAwait when appropriate.
    Use the language keywords for data types instead of the runtime types. For example, use string instead of System.String, or int instead of System.Int32. This recommendation includes using the types nint and nuint.
    Use int rather than unsigned types. The use of int is common throughout C#, and it's easier to interact with other libraries when you use int. Exceptions are for documentation specific to unsigned data types.
    Use var only when a reader can infer the type from the expression. Readers view our samples on the docs platform. They don't have hover or tool tips that display the type of variables.
    Write code with clarity and simplicity in mind.
    Avoid overly complex and convoluted code logic.
