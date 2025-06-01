## Implementation Plan for Prompt Optimization

### Overview
The goal is to optimize/rewrite user prompts into comprehensive single tasks using Semantic Kernel and MCP SDK.

### Install Semantic Kernel NuGet package
dotnet add package Microsoft.SemanticKernel --version 1.0.1

### Implementation Steps

1. Create a semantic function using SK:
```csharp
const string promptTemplate = @"
Rewrite the user request into a single comprehensive task that:
1. Combines all steps into one clear instruction
2. Preserves all original requirements
3. Is specific and actionable (include verbs like ""create"", ""implement"", ""refactor"")
4. Removes redundancy
5> Follows KISS

Example transformation:
Input: ""First fix the bugs, then add tests, finally optimize performance""
Output: ""Implement comprehensive solution fixing bugs, adding test coverage, and optimizing performance""

Original request: {{$input}}

Rewritten task: ";

var promptConfig = new PromptTemplateConfig
{
    Input = new PromptTemplateConfig.InputConfig
    {
        Parameters = new List<PromptTemplateConfig.InputParameter>
        {
            new() { Name = "input", Description = "The original user request to optimize" }
        }
    }
};

var kernel = Kernel.Builder.Build();
var promptOptimizer = kernel.CreateSemanticFunction(
    promptTemplate,
    config: promptConfig,
    functionName: "OptimizePrompt"
);
```

2. Usage example:
```csharp
// Input: Complex multi-step user request
string userPrompt = @"First, analyze the code for bugs. 
Then check if there are any security issues. 
Finally suggest improvements for performance.";

// Get optimized single task
var result = await kernel.RunAsync(userPrompt, promptOptimizer);
string optimizedPrompt = result.GetValue<string>();

// Output example:
// "Perform a comprehensive code review analyzing bugs, security vulnerabilities, 
// and performance optimization opportunities in the provided code."
```

### Key Components:

1. Prompt Template
- Uses SK's templating system
- Takes original request as input
- Provides clear guidelines for rewriting
- Maintains semantic meaning while optimizing structure

2. Semantic Function
- Single LLM call
- Uses default model settings
- Returns optimized prompt as string

3. Integration
- Can be used as standalone function
- Easily integrated into larger SK plugins
- Compatible with MCP protocol for deployment

This implementation follows KISS principles with:
- Single responsibility
- One LLM call
- Clear input/output
- Minimal dependencies
- Easy maintenance## Relevant Documentation
