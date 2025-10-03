# Development Process

These are the preliminary development processes for developers and QAs. These processes are subject to continuous changes so it's advice to frequently review it for changes.

## Developer Development Process

1. Pick up ticket (3 amigos; optional)
2. Define branch.
   ```
   Format:
   
   e.g.
   
   feat/FEAT-XXX/{title}
   bug/FEAT-XXX/{title}
   ```
3. Run tests to make sure you know everything is working before you start.
4. Implement the necessary changes
5. Implement playwright tests and fix broken tests
6. Commit changes
    1. Message format: 
      ```
      FEAT-XXX {message}
      
      {details}
      ```
7. Create pull request 
8. When pull request has been approved and merged, create release tag and push to master
9. Deploy the generated release to the test environment
9. Move the ticket to the Jira `Test` column and inform test lead
10. The Test Lead will then run manual tests to see if the acceptance criteria was met. 
11. Once the Test Lead has signed off, deploy the tagged release to `staging` environment

## QA Test Suite Development Process

1. Define or Pick up ticket in Jira Test column
    1. Subtask to fix related tests (Ideally the responsibility of the Devs).
    2. Define general test fix, load testing, etc. ticket(s).
2. Move Jira ticket the 'In Progress'.
3. Define branch. Format:
   1. task/FEAT-XXX/{title} (for test fixes, modifications, etc.)
   2. test/FEAT-XXX/{title} (for running complex tests, e.g. performance, load, security, etc. testing)
4. Implement the necessary changes
   1. Add test modifications
5. Commit changes
   1. Message format: "FEAT-XXX {message}"
6. Create pull request
7. Move Jira ticket the 'Ready for Review'.
8. Pull request approved and merged, by at least a Dev, QA or Technical Architect
9. Mark ticket as 'Done'.site. Pass or fail.