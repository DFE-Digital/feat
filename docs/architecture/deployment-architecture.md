# Deployment Architecture

The build and release pipelines are all controlled via GitHub actions which act as our CI/CD process.

The actions are categorised into different types:
- **Validate**: Perform some sort of validation, such as running tests and are typically pass/fail
- **Generate**: Generate some sort of artifact, such as reporting
- **Deploy**: Perform a deployment to a target environment

## Build Pipeline

On creation of a Pull Request or push to main, multiple validation actions run, depending on what areas of the repo
have been modified. Any failures should be addressed before merging. Failures on main should be fixed as priority.

## Release Pipeline

Releases are currently run manually.

These releases should come from `main` to ensure the artifacts deployed are the latest tested versoins.

The ability to release from an alternate branch has been enabled to allow testing of fixes/patches in lower environments if needed.