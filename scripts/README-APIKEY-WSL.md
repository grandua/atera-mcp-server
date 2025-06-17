# Setting the Atera API Key for Local Builds (WSL/Linux/macOS)

The `build.sh` script uses the `Atera__ApiKey` for integration tests. Here's how to configure it for local development:

**Recommended Method: Environment Variable**

Set the `Atera__ApiKey` environment variable in your terminal session before running the build script:
```bash
export Atera__ApiKey="your_actual_api_key_here"
./scripts/build.sh
```
This is the most flexible method and aligns with CI/CD practices.

**Alternative: Local `.atera_apikey` File (for convenience)**

If the `Atera__ApiKey` environment variable is not set, the script will automatically look for a file named `.atera_apikey` in the project root (`c:/Work/Projects/Fiverr/AteraMcpServer/.atera_apikey`).

1.  Create a file named `.atera_apikey` in the project root.
2.  Paste your Atera API key into this file. Make sure there are no extra spaces or newlines.
3.  **Ensure this file is listed in your `.gitignore` file to prevent committing your API key.**

The script will load the key from this file if the environment variable is not found.

**Priority:**
1.  `Atera__ApiKey` environment variable (if set).
2.  Content of `./.atera_apikey` file (if environment variable is not set and file exists).
3.  If neither is found, a warning will be displayed, and integration tests requiring the API key will likely fail.

**CI/CD Environments:**
For CI/CD pipelines (e.g., GitHub Actions), always inject the `Atera__ApiKey` as a secure environment variable provided by the CI/CD platform.

**Security Note:**
Storing API keys in plaintext files, even locally, has security implications. Ensure `.atera_apikey` is in `.gitignore`. For production or shared environments, always prefer environment variables injected securely.
