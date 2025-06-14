## Milestone 1: Python Reference Code Analysis

This document outlines the findings from analyzing the Python reference project (`C:\Work\Projects\Fiverr\Python-reference-atera-mcp-server\`) for the Atera API, specifically focusing on the `GetAgentList` functionality.

### GetAgentList API Analysis

#### Endpoint Details
*   **Base URL:** `https://app.atera.com` (This is a common base URL for Atera; verify if the Python code uses a configurable base URL or hardcodes this).
*   **Endpoint Path:** `/api/v3/agents`
*   **HTTP Method:** `GET`
*   **Authentication:**
    *   Requires an API Key, which is can be found in C:\Work\Projects\Fiverr\Python-reference-atera-mcp-server\test_data\config.json
    *   The API Key is sent in the `X-API-KEY` HTTP header.

#### Request Parameters
*   **Query Parameters (Optional):**
    *   `page` (integer): For pagination, specifies the page number to retrieve. Defaults to `1` if not provided.
    *   `itemsInPage` (integer): For pagination, specifies the number of items to retrieve per page. Defaults to `50` or `100` (verify exact default from Python code or Atera documentation). A common maximum is 100.

#### Response Structure
The API returns a JSON object containing a list of agents and pagination details.

*   **Overall Response Object:**
    ```json
    {
      "items": [
        // Array of Agent objects (see structure below)
      ],
      "page": "integer", // Current page number
      "itemsInPage": "integer", // Items requested per page
      "totalItems": "integer", // Total number of agents available across all pages
      "totalPages": "integer" // Total number of pages
    }
    ```

*   **Agent Object Structure (within the `items` array):**
    ```json
    {
      "AgentID": "integer", // e.g., 12345
      "CustomerID": "integer", // e.g., 6789
      "CustomerName": "string", // e.g., "Client Corp"
      "MachineName": "string", // e.g., " бухгалтер-ПК" (Note: can contain non-ASCII characters)
      "DeviceType": "string", // e.g., "Workstation", "Server"
      "DomainName": "string", // e.g., "WORKGROUP" or "client.local"
      "Online": "boolean", // true if online, false if offline
      "LastRebootTime": "datetime", // ISO 8601 format, e.g., "2023-10-26T10:30:00Z" (can be null)
      "LastLoginUser": "string", // e.g., "jsmith" (can be null)
      "LastSeenTime": "datetime", // ISO 8601 format, e.g., "2023-10-27T14:15:20Z"
      "OSType": "string", // e.g., "Windows"
      "OSVersion": "string", // e.g., "Windows 10 Pro" (can also include build numbers)
      "OSDistribution": "string", // More specific OS info, e.g., "Windows 10 Pro, 64-bit, version 22H2, build 19045.3570"
      "IPAddresses": "string", // Comma-separated list of IP addresses, e.g., "192.168.1.100,10.0.0.5"
      "ReportedExternalIPAddress": "string", // e.g., "80.178.54.12" (can be null)
      "MonitoredAgent": "boolean", // Indicates if the agent is actively monitored
      // Other potential fields observed in typical Atera responses (verify from Python code):
      // "AgentVersion": "string",
      // "MachineId": "string", // Potentially a more stable machine identifier than name
      // "SiteName": "string", // If Atera supports sites/locations under customers
      // "AlertsCount": "integer"
    }
    ```

#### Key Fields for Domain Model (`Agent.cs`)
Based on the above, the essential fields to consider for our C# `Agent` domain entity are:

*   `AgentID` (int or long)
*   `CustomerID` (int or long)
*   `CustomerName` (string)
*   `MachineName` (string)
*   `DeviceType` (string, consider an enum if values are fixed)
*   `DomainName` (string, nullable)
*   `IsOnline` (bool, mapped from `Online`)
*   `LastRebootTime` (DateTimeOffset?, nullable)
*   `LastLoginUser` (string, nullable)
*   `LastSeenTime` (DateTimeOffset?)
*   `OperatingSystemType` (string, mapped from `OSType`)
*   `OperatingSystemVersion` (string, mapped from `OSVersion` or `OSDistribution`)
*   `IpAddresses` (string or `List<string>`)
*   `ExternalIpAddress` (string, nullable, mapped from `ReportedExternalIPAddress`)
*   `IsMonitored` (bool, mapped from `MonitoredAgent`)

#### Notes & Considerations:
*   **Error Handling:** The Python code should be checked for how it handles API errors (e.g., 401 Unauthorized, 403 Forbidden, 404 Not Found, 5xx Server Errors).
*   **Rate Limiting:** Investigate if the Python code implements any rate limit handling. Atera APIs typically have rate limits.
*   **Date/Time Parsing:** Ensure robust parsing for `datetime` fields, considering potential nulls and timezone information (ISO 8601 usually implies UTC, which `DateTimeOffset` handles well).
*   **Configuration:** How is the API Key and Base URL configured in the Python project? (e.g., environment variables, config file). This will inform our C# configuration strategy.
*   **Specific Python Libraries:** Note any specific Python libraries used for HTTP requests (e.g., `requests`) and JSON parsing, as this might give clues about specific behaviors or default settings.

This analysis will serve as the foundation for designing the `Agent` entity in the Domain layer and implementing the `AteraApiService` in the Data Access/Infrastructure layer of the C# project.