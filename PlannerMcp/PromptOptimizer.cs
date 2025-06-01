using Microsoft.SemanticKernel;
using System.Threading.Tasks;

namespace PlannerMcp
{
    public class PromptOptimizer
    {
        public static KernelFunction CreateOptimizedPromptFunction(Kernel kernel)
        {
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

            return kernel.CreateFunctionFromPrompt(promptTemplate);
        }

        public static async Task RunExample(Kernel kernel)
        {
            var optimizePromptFunc = CreateOptimizedPromptFunction(kernel);

            var arguments = new KernelArguments
            {
                ["input"] = @"First, analyze the code for bugs. 
Then check if there are any security issues. 
Finally suggest improvements for performance."
            };

            var result = await kernel.InvokeAsync(
                optimizePromptFunc,
                new() { ["input"] = arguments["input"] }
            );

            Console.WriteLine("Original Prompt: " + arguments["input"]);
            Console.WriteLine("Optimized Prompt: " + result.ToString());
        }
    }
}
