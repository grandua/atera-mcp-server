# Software planner and designer AI agent

## Goal
I am about to start building software planner and designer AI agent as MCP server. 
This planner should follow KISS, Rich Domain Model, OOP, DDD, TDD, SOLID principles, clean architecure with 3 layers (Presentation, Domain and Data Access where Domain access does not depend on Data Access and Presentation layer because of DIP) 
Initially it will request all related existing code context as one of its MCP tools parameters, as it is intended to be run inside Windsurf, Cody and/or Cline and those IDEs and IDE extensions are good at collecting relevant code context and analyzing code. 
Later on I might create my own MCP for code search and analysis and use that one.

## Clarifications
Context scoping is handled by the Agentic IDE.
Validation steps imply potential loops, driven by another specialized AI model.
A "full navigation map" includes all calls and their parameters.
The planner will halt and request user input if critical information is missing.

## Thechnology portfolio
C#, Semantic Kernel, .NET MCP SDK
### Optional but likely parts of technology portfolio
Taskmaster MCP server, Perplexity Search MCP server

This Planenr MCP Server will have the next steps:

- Steps to be done by Cody or Windsurf (out of scope) for now:
    - Re-write user prompt as multi-hop queries for RAG
    - Advanced agentic RAG, including re-ranker
    - Compress context: Summarize conversation history and relevant context

- Based on user prompt and code context provided as tool parameters, re-write/optimize user prompt as one comprehensive task
- Try to generate first draft of acceptance criteria, technology portfolio, full navigation map, CRC cards, plan steps and test cases
- Identify gaps, unknowns and assumptions in optimized user prompt and between optimized user prompt and compressed context
- Try to fill in gaps and assumptions from the original user prompt and full code context
- Generate a prompt for follow up multi-hop queries and ask AI agent to execute them and call back this MCP with the results again
- Remove queries for which we executed advanced RAG before already
- Generate questions to user for remaining gaps. Suggest a list of spikes/PoCs to clarify remaining questions if that makes sense.
- Iteratively generate and validate acceptance criteria and test cases (ensuring KISS, completeness, and alignment with the optimized prompt and context). Loop until satisfactory or critical gaps require user input.
- Generate initial technology portfolio (this may be refined later).
- Iteratively generate and validate initial plan steps (ensuring alignment with acceptance criteria). Loop until satisfactory or critical gaps require user input.
- Iteratively generate and validate a full navigation map (all calls and parameters, ensuring KISS, completeness, and alignment with prompt, context, and acceptance criteria). Loop until satisfactory or critical gaps require user input.
- Iteratively generate and validate CRC cards (ensuring KISS, Rich Domain Model adherence, completeness, and alignment with prompt, context, acceptance criteria, and navigation map). Loop until satisfactory or critical gaps require user input.
- Based on insights from artifact generation (acceptance criteria, navigation map, CRC cards), revise and refine the overall plan steps.
- Validate revised plan steps for internal consistency, completeness, and alignment with all generated artifacts and user requirements.
- Consolidate all identified gaps, assumptions, and areas requiring clarification from previous steps.
- If critical gaps or unknowns persist:
    - Formulate precise questions for the user.
    - Suggest targeted PoCs/Spikes to resolve ambiguities.
    - Halt further detailed planning and present these to the user, awaiting clarification.
- Once all critical clarifications are received (if any were needed):
    - Finalize and optimize the plan, CRC cards, and navigation map, ensuring all components are consistent and fully aligned with the user prompt, context, and agreed-upon clarifications.
    - Validate that the final optimized plan aligns with previous iterations and has not inadvertently lost key requirements.
- Add comprehensive explanations and present the final technology portfolio, plan, navigation map, and CRC cards to the user.