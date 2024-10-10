## Local Development Lifecycle

1. Write TDD Tests for verifying implementation
2. Implement code changes
3. Local Testing

3a. Run tests & gates:
  * Blackduck
  * unit tests
  * functional tests
  * integration tests
  * SonarQube

3b. Verify via mocks that the implementation performs as intended

4. Optionally Remote Testing { DEV or Sandbox }
  * Deploy a version of the software as a snapshot to development
  * Perform Azure integration e2e testing in DEV environment.
5. Create a PR
6. PR is reviewed and approved
7. PR is merged to main or master
