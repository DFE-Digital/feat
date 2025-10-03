# Release Process

This document outlines the process for deploying the application to the **Test** and **Production** environments.

---

## Automated Deployments to Test

When a pull request is merged into the `main` branch, the following actions are automatically triggered:

1.  **New Tag Creation**: The workflow finds the most recent tag (e.g., `v1.2.3`), increments it (to `v1.2.4`), and applies it to the merge commit.
2.  **Deployment to Test**: The newly tagged version is automatically deployed to the **Test** environment.
3.  **Contentful Migrations**: Any new Contentful migrations are also deployed to the **Test** environment.

This process ensures that the **Test** environment always reflects the latest version of the `main` branch.

---

## Deployments to environments

Deployments to the **Production** environment are **manual**. The primary method is by creating a new release in GitHub.

### 1. Create a GitHub Release (Primary Method)

This is the standard way to deploy a version that has been fully validated in the Test environment.

1.  **Choose a Version**: Identify the tag you want to release to production (e.g., `v1.2.4`).
2.  **Create a New Release**:
    * Navigate to the **Releases** page in the GitHub repository.
    * Click **Draft a new release**.
    * Select the tag you want to deploy from the **Choose a tag** dropdown.
    * Add a title and description for the release notes.
    * Click **Publish release**.
3.  **Automatic Deployment**: Publishing the release automatically triggers the deployment workflow to the **Production** environment.

### 2. Manual Workflow Dispatch - if you just want to deploy latest version of Main (Alternative Method)

You can also trigger a deployment manually without creating a formal release. This is useful for hotfixes or special circumstances.

1.  Navigate to the **Actions** tab in the GitHub repository.
2.  In the left sidebar, find and click on the **Deploy - Environment** workflow.
3.  Click the **Run workflow** dropdown button, which appears on the right side of the page.
4.  Select the branch you want to run the Workflow from **or** select the specific **tag version** (e.g., `v1.2.5`), the **environment you want to deploy** and if you want to **Clear Cache**. 
5.  Click the green **Run workflow** button to start the deployment.