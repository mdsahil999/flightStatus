# AI Prompts Used

* **Prompt**: Initial prompt for understanding the problem statement and generating the implementation plan.

  * *Note*: I have shared the document with Antigravity so it can analyze the requirements.
* **Prompt**: "Use these mockups as a reference for the UI design only. You don't need to follow them exactly or include all the fields. Please base the implementation on the initial backend specification."

  * *Note*: I first created the design in Stitch, then shared it with Antigravity using the MCP server to generate the UI.(https://stitch.withgoogle.com/projects/11645737150955882549)
* **Prompt**: "Please follow these requirements to start creating the application:
Backend: Use .NET 8 and follow Clean Architecture principles with a clean, scalable, and maintainable project structure.
Frontend: Use Angular 19.
Use Reactive Forms for all forms.
Use Signals wherever appropriate for state management and component state.
Use Angular's latest built-in control flow syntax (@if, @for, @switch) instead of the legacy structural directives (\*ngIf, \*ngFor, etc.) whenever possible.
Follow Angular best practices, keeping the code modular, reusable, and maintainable.
Ensure the application is production-ready, with clean code, proper error handling, validation, and a responsive UI."
* **Prompt**: "Please restructure the backend to follow the Repository Design Pattern."
* **Prompt**: "Please add or update the backend unit tests for **FlightStatusService**, **MergeService**, and **StatusNormalizer**.
I will share a table containing the test scenarios. Please ensure that all of those scenarios are covered by the unit tests. If any existing tests need to be updated to accommodate the new scenarios, please make the necessary changes while maintaining good test coverage and following unit testing best practices.
| Flight Number | AeroTrack Status | AeroTrack Last Updated | QuickFlight Status | QuickFlight Last Updated | Expected Provider | Expected Unified Status |
| ------------- | ---------------- | ---------------------- | ------------------ | ------------------------ | ----------------- | ----------------------- |
|   AI101       | Departed         | 10:30                  | RunningBehind      | 10:15                    |   AeroTrack       |   OnTime                |
|   AI102       | DepartedLate     | 10:10                  | OnSchedule         | 10:45                    |   QuickFlight     |   OnTime                |
|   AI103       | Cancelled        | 09:30                  | NotOperating       | 09:20                    |   AeroTrack       |   Cancelled             |
|   AI104       | Diverted         | 11:00                  | LandedElsewhere    | 11:15                    |   QuickFlight     |   Diverted              |
|   AI105       | Scheduled        | 12:00                  | *No Response*        | -                        |   AeroTrack       |   OnTime                |
|   AI106       | *No Response*      | -                      | RunningBehind      | 12:00                    |   QuickFlight     |   Delayed               |
|   AI107       | *No Response*      | -                      | *No Response*        | -                        |   None            |   Unknown               |

**Prompt**: "Please use \*Moq\* to mock any dependencies or services in the unit tests."

