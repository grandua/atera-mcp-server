# MCP Server Rebuild - Acceptance Criteria & Technical Overview
 
## 1. Project Overview
The goal of this MCP server is to interface with the Atera RMM helpdesk system, allowing AI-driven ticket handling, support agent assistance, and system integrations. The existing implementation includes Python, FastAPI, PostgreSQL, and Docker. The server is currently able to sync ticket and alert data into a local PostgreSQL database (read operations), and write operations are performed directly through Atera’s API.
 
## 2. System Architecture
The MCP system is hosted locally and built using Docker with two containers: one for the PostgreSQL database, and one for the MCP server. Reads are performed against the synced local Postgres database (updated every 5 minutes), and writes are pushed to Atera via API.
 
## 3. Key Functional Requirements
The system must support:
- Querying tickets from the local DB
- Creating new tickets via a wizard or direct entry with all relevant required fields, including the company field which cannot be amended later
- Assigning tickets to a specific engineer
- Adding internal/public notes with a confirmation prompt
- Sorting and filtering tickets by engineer
- Displaying comments in a user-friendly format
- Closing tickets with time tracking prompts and classification (e.g., on-site/off-site)
- Querying the database for specific conditions, such as finding all machines that don't match Windows 11 criteria or have a certain type of hard disk
 
## 4. Technical Constraints
The rebuild must:
- Be modular and easy to extend with new MCP tools (e.g., Meraki integration)
- Use Docker for deployment
- Be hosted locally on an internal server with an RTX 4080
- Avoid tightly coupling components (LLM, DB, API) in a monolithic design
 
## 5. Example Use Cases
1. "Create a new ticket for [Client] describing [Issue] using the wizard, ensuring all required fields are completed."
2. "Add an internal note to ticket #1234: 'Awaiting vendor response.'"
3. "Assign ticket #5678 to [Engineer]."
4. "List all open tickets assigned to [Engineer]."
5. "Close ticket #4321 and log 1 hour as off-site time."
6. "Show ticket #4321 and display all public comments."
7. "Find all machines that don't match Windows 11 criteria."
 
## 6. Future Expansion Plans
David intends to add additional MCP tools in the future to handle tasks like checking Meraki WAN status, automating repetitive ticket flows, and building an AI-powered virtual helpdesk coordinator to prompt engineers and streamline internal operations.
 
## P.S.
Am I approaching this in the correct way by having the MCP tool access the PostgreSQL database via the FastAPI for reads? Or would it be better to have a dedicated PostgreSQL MCP tool for handling the read operations, rather than relying on the FastAPI? This current setup was put together to prove the concept, but I'm open to the best approach and trust your expertise in the rewrite. I value your advice and will follow your guidance on the optimal architecture.