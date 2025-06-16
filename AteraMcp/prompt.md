# CI/CD & Local-Runner Setup Task

## Mission
Create a portable, script-centric CI/CD pipeline for this stdio MCP server,
testable both locally (via scripts`and with act`) and in GitHub Actions runners on every push.

---

## High-Level Plan (checked off sequentially)

- [ ] 1. Install **act** on Windows 10 (see “Act installation” below)  
- [ ] 2. Verify **act** works: `act --list` shows default workflows  
- [ ] 3. Scaffold `.github/workflows/build.yml` that delegates almost everything to scripts  
- [ ] 4. Add cross-platform scripts under `/scripts`  
  - `build.ps1` / `build.sh` — dotnet restore + build + test  
  - `docker-build.ps1` / `docker-build.sh` — build Docker image using **Dockerfile**  
  - `deploy-local.ps1` / `deploy-local.sh` — run image locally for smoke tests  
- [ ] 5. Run those scripts first directly, and second with **act** until they pass  
- [ ] 6. Push to `main`; GitHub Actions must go green  
- [ ] 7. Document one-liner developer usage in `ReadMe.md`

---

## Acceptance Criteria

1. `act -W .github/workflows/build.yml` completes with exit 0 on Windows 10  
2. GitHub Actions shows ✔ Build & ✔ Deploy for pushes and PRs  
3. All pipeline logic (build, test, docker, deploy) lives in `/scripts/*`  
4. Secrets (`ATERA_API_KEY`) consumed identically in **act** and in GitHub  
5. Artifacts: published Docker image `atera-mcp:local` and test report `.trx`

---

## Act installation (Windows 10)

```bash
# Download latest release (x64) – adjust version if newer
Invoke-WebRequest -Uri https://github.com/nektos/act/releases/latest/download/act_Windows_x86_64.zip -OutFile act.zip
Expand-Archive -Path act.zip -DestinationPath "$env:ProgramFiles\act"
setx PATH "%PATH%;%ProgramFiles%\act"
```

Alternative: `winget install act` or use WSL-2 + Docker-Desktop.

Test:

```bash
act --version
```

---

## Secrets while running **act**

Create `.actrc` (git-ignored):

```
-P ubuntu-latest=catthehacker/ubuntu:act-latest
-s ATERA_API_KEY=xxxxxxxxxxxxxxxx
```

On GitHub, add the same key under **Settings → Secrets → Actions → New repository secret**.

---

## References

* GitHub Actions docs – https://docs.github.com/actions  
* Local CI article – https://dev.to/vishnusatheesh/how-to-set-up-a-cicd-pipeline-with-github-actions-for-automated-deployments-j39


## Information about Act and ways to run and test GitHub Actions locally
Yes, you can test GitHub Actions CI/CD pipelines locally. This practice allows for more efficient development and debugging without needing to commit and push every change to a remote repository[2][5][9].

### Tools and Methods for Local Testing

Several tools and methods are available to run and validate your GitHub Actions workflows on your local machine.

**`act` Command-Line Tool**
The most popular tool for this purpose is `act`, an open-source project that runs your GitHub Actions workflows locally[1][9].

*   **How it Works**: `act` reads your workflow files from the `.github/workflows/` directory and uses Docker to simulate the GitHub Actions runtime environment[1][9]. It executes the jobs defined in your YAML files, allowing you to see the output and troubleshoot any issues[5].
*   **Installation**: You can install `act` using package managers like Homebrew for macOS/Linux or npm for Node.js[9].
*   **Usage**: After installation, navigate to the root directory of your repository in the terminal and simply run the `act` command. `act` will detect and execute the workflows[5][9].

**Benefits of using `act`:**
*   **Faster Iteration**: Quickly validate changes to your workflows without the commit-push-wait cycle[2][5].
*   **Cost Savings**: Reduce the number of workflow runs on GitHub's hosted runners, which can save costs[5].
*   **Offline Development**: Test and develop workflows even without an internet connection[5].

**GitHub Actions Toolkit**
GitHub provides the `@actions/core` package, a set of Node.js libraries that can be used to create a custom test script[5].

*   **How it Works**: You can write a Node.js script that imports modules from the toolkit to simulate workflow execution. This allows you to set inputs, execute steps, and validate outputs programmatically[5].
*   **Use Case**: This method is suitable for testing the logic of individual actions or more complex workflow scenarios that require custom test harnesses[5].

**Example `test.js` script using the toolkit:**
```javascript
const core = require('@actions/core');
const exec = require('@actions/exec');

async function run() {
  try {
    // Simulate setting an input variable
    core.getInput('myInputVariable');

    // Execute a workflow step
    await exec.exec('echo', ['Hello, from a local test!']);

    // Simulate setting an output variable
    core.setOutput('myOutputVariable', 'Test successful');
  } catch (error) {
    core.setFailed(error.message);
  }
}

run();
```
To run this test, you would execute `node test.js` in your terminal[5].

**BrowserStack Integration**
For workflows involving web application testing, you can integrate with services like BrowserStack. This allows you to run your local tests on BrowserStack's cloud infrastructure, providing access to a wide range of browsers and devices while triggering the workflow from your local machine[5].

### Alternative Strategy

An alternative approach is to minimize the logic within the GitHub Actions YAML file itself. Instead, place complex build, test, or deployment logic into scripts (e.g., shell, Python, or PowerShell scripts). These scripts can be developed and debugged locally using standard tools. The YAML file then becomes a simple orchestrator that just calls these scripts. This approach reduces the need to debug the workflow syntax and helps avoid vendor lock-in[2].

Confidence Level: 100%

[1] https://github.com/nektos/act
[2] https://www.reddit.com/r/opensource/comments/13jc81f/github_actions_can_be_tested_locally_with_act/
[3] https://stackoverflow.com/questions/59241249/how-can-i-run-github-actions-workflows-locally
[4] https://docs.github.com/en/actions/use-cases-and-examples/building-and-testing
[5] https://www.browserstack.com/guide/test-github-actions-locally
[6] https://www.testmo.com/guides/github-actions-test-automation/
[7] https://dev.to/vishnusatheesh/how-to-set-up-a-cicd-pipeline-with-github-actions-for-automated-deployments-j39
[8] https://docs.github.com/en/actions/about-github-actions/about-continuous-integration-with-github-actions
[9] https://www.infralovers.com/blog/2024-08-14-github-actions-locally/
[10] https://www.youtube.com/watch?v=YORvmxQBPeM
[11] https://docs.github.com/articles/getting-started-with-github-actions
[12] https://techaiinsights.in/local-ci-cd-testing-github-actions-act/