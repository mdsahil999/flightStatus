# Flight Status Application

## Setup Steps
1. Ensure you have .NET 8 SDK and Node.js (v18+) installed.
2. Clone this repository.

## How to Run
### Backend (.NET 8 Minimal API)
1. Navigate to `FlightStatus.Api/`
2. Run `dotnet run`

### Frontend (Angular)
1. Navigate to `flight-status-ui/`
2. Run `npm install` (if not already installed)
3. Run `npm start`
4. Access the UI at `http://localhost:4200`

### Running Tests
1. Navigate to `FlightStatus.Tests/`
2. Run `dotnet test`

---

## Testing the Merge Logic & Normalization

The system queries two mock downstream providers (**AeroTrack** and **QuickFlight**). They each have their own distinct vocabulary for statuses. The backend API automatically normalizes these varying terminologies into a single, unified `FlightStatusEnum`.

### Vocabulary Normalization Map

| AeroTrack    | QuickFlight     | Unified   |
| ------------ | --------------- | --------- |
| Scheduled    | OnSchedule      | OnTime    |
| Boarding     | OnSchedule      | OnTime    |
| Departed     | OnSchedule      | OnTime    |
| DepartedLate | RunningBehind   | Delayed   |
| Cancelled    | NotOperating    | Cancelled |
| Diverted     | LandedElsewhere | Diverted  |

### Test Scenarios (Flight Data)

To test the system, open the Angular UI (`http://localhost:4200`), enter one of the following **Flight Numbers**, pick any date, and click "Search Flight". 

The backend has been explicitly seeded with deterministic mock data for flights **AI101** through **AI107** to demonstrate all merge rules (Latest timestamp wins, Single provider fallback, and Unknown failures).

| Flight Number | AeroTrack Status | AeroTrack Last Updated | QuickFlight Status | QuickFlight Last Updated | Expected Provider | Expected Unified Status |
| ------------- | ---------------- | ---------------------- | ------------------ | ------------------------ | ----------------- | ----------------------- |
|   AI101       | Departed         | 10:30                  | RunningBehind      | 10:15                    |   AeroTrack       |   OnTime                |
|   AI102       | DepartedLate     | 10:10                  | OnSchedule         | 10:45                    |   QuickFlight     |   OnTime                |
|   AI103       | Cancelled        | 09:30                  | NotOperating       | 09:20                    |   AeroTrack       |   Cancelled             |
|   AI104       | Diverted         | 11:00                  | LandedElsewhere    | 11:15                    |   QuickFlight     |   Diverted              |
|   AI105       | Scheduled        | 12:00                  | *No Response*      | -                        |   AeroTrack       |   OnTime                |
|   AI106       | *No Response*    | -                      | RunningBehind      | 12:00                    |   QuickFlight     |   Delayed               |
|   AI107       | *No Response*    | -                      | *No Response*      | -                        |   None            |   Unknown               |

>   Note on Delay Reason: If a flight is normalized to `Delayed` but the winning provider (e.g., QuickFlight on AI106) does not provide a delay reason, the API will gracefully fall back to returning `"Reason unavailable"`.

---

## Architectural Decisions
- **Minimal API**: Refactored from traditional Controllers to modern .NET 8 Minimal API for leaner, faster request pipelines.
- **Repository Pattern**: Used `IFlightStatusRepository` to cleanly aggregate data from multiple decoupled providers (`IFlightStatusProvider`), keeping the Minimal API routes clean.
- **Normalization Layer**: Implemented a standalone `StatusNormalizer` to enforce strict status mappings and time calculations.
- **Dependency Injection**: Utilized traditional constructor injection for strict dependency rules across the backend.

## Copilot Usage Summary
GitHub Copilot was heavily utilized throughout the development process:
- **Code Generation**: Used Copilot to generate the boilerplate for the Minimal API setup, the Angular Reactive Forms, and the initial xUnit test scaffolding.
