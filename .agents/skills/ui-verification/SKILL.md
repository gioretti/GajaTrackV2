---
name: ui-verification
description: UI interaction and validation expert. Use when: (1) Verifying Blazor WebApp changes, (2) Testing user flows, (3) Checking accessibility or responsive design after front-end changes.
---

# UI Verification Skill

When you modify the Presentation Layer (specifically the Blazor WebApp), you cannot rely on unit tests alone. You must visually and interactively verify the output.

## Operational Workflow

1. **Launch the Environment**
   - Ensure the API and WebApp are running (usually `dotnet run` or hitting F5 in the editor context).
   - If they are not running, start them using the appropriate terminal command.

2. **Browser Subagent Activation**
   - You MUST use the `browser_subagent` tool to navigate to the local application URL (e.g., `https://localhost:5001`).

3. **Verification Checklist**
   - **Render Check:** Did the page load without throwing Blazor fatal errors (yellow bottom bar)?
   - **Console Check:** Open the DevTools via the subagent. Are there any JavaScript or WASM exception logs in the console?
   - **Behavior Check:** Interact with the specific feature you just built. (e.g., If you added a "Save" button, click it. Did the UI state update? Did the API call succeed?)
   - **Responsive Check:** (If applicable) Ask the subagent to resize the window to emulate a mobile device and verify the layout didn't break.

4. **Iterative Fixing**
   - If the verification fails, you must drop back into Execution Mode, fix the code, write/fix the test, and then trigger this skill again. You cannot proceed to Self-Review if the UI is broken.
