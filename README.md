# League Manager

League Manager is a C# console application designed to manage football (soccer) leagues, teams, fixtures, and match results. It provides functionality to add leagues, teams, schedule fixtures, process match results, display league standings, and predict match outcomes based on historical data and team performance metrics.

---

## Features

1. **League Management**:
   - Create and manage multiple leagues.
   - Add teams to a league.
   - Schedule fixtures between teams.
   - Process match results and update league standings.

2. **League Table**:
   - Display the league table sorted by points, goal difference, and goals scored.
   - Automatically update standings after processing match results.

3. **Match Prediction**:
   - Predict the outcome of upcoming fixtures using advanced algorithms.
   - Factors include recent form, head-to-head records, team strength, and league position.
   - Adjust predictions for fixture congestion and fatigue.

4. **Data Persistence**:
   - Save and load league data to/from a JSON file.
   - Retain league, team, and fixture information between sessions.

5. **Fixture Management**:
   - Delete past fixtures that have not been played.
   - View upcoming fixtures and their predictions.

---

## How It Works

### Core Classes

1. **Team**:
   - Represents a football team.
   - Tracks matches played, goals scored, goals conceded, and points.

2. **League**:
   - Represents a football league.
   - Manages a list of teams and fixtures.
   - Provides methods to add teams, schedule fixtures, process match results, and display the league table.

3. **Fixture**:
   - Represents a match between two teams.
   - Tracks match date, goals scored, and whether the match has been played.

4. **Prediction**:
   - Predicts the outcome of matches using weighted factors:
     - Recent form (last 10 matches).
     - Head-to-head records.
     - Team strength (goals scored/conceded, points efficiency).
     - League position difference.
     - Home advantage.
   - Adjusts predictions for fixture congestion and fatigue.

5. **ManageLeagues**:
   - Manages multiple leagues.
   - Provides functionality to save and load league data to/from a JSON file.

---

## Usage

### Menu Options

1. **Add League**:
   - Create a new league by providing a name.

2. **Add Team to a League**:
   - Add a team to an existing league.

3. **Schedule Fixture**:
   - Schedule a match between two teams in a league.
   - Specify the match date or use the current date.

4. **Process Match Result**:
   - Enter the result of a match (goals scored by each team).
   - Update league standings automatically.

5. **Display League Table**:
   - View the current standings of a league.

6. **Predict All Fixtures**:
   - Predict the outcomes of all upcoming fixtures in a league.

7. **Predict Specific Match**:
   - Predict the outcome of a specific match between two teams.

8. **Delete Past Fixtures**:
   - Remove past fixtures that have not been played.

9. **Save Data**:
   - Save all league, team, and fixture data to a JSON file.

10. **Exit**:
    - Exit the application.

---

## Prediction Algorithm

The prediction algorithm uses the following factors to determine the likely outcome of a match:

1. **Recent Form**:
   - Weighted points from the last 10 matches (recent matches have higher weight).

2. **Head-to-Head Records**:
   - Historical performance between the two teams.

3. **Team Strength**:
   - Goals scored/conceded per game compared to the league average.
   - Points efficiency (points per game ratio to the maximum possible).

4. **League Position Difference**:
   - Advantage for teams higher in the league table.

5. **Home Advantage**:
   - Teams playing at home have a slight advantage.

6. **Fixture Congestion**:
   - Adjusts predictions for teams with multiple matches in a short period (fatigue impact).

---

## Data Persistence

- League, team, and fixture data are saved to a JSON file (`league_data.json`).
- Data is automatically loaded when the application starts.
- Use the **Save Data** option to manually save changes.

---

## Example Workflow

1. Add a league (e.g., "Premier League").
2. Add teams to the league (e.g., "Team A", "Team B").
3. Schedule fixtures between teams.
4. Process match results to update standings.
5. View the league table.
6. Predict upcoming fixtures.
7. Save data before exiting.

---

## Requirements

- .NET SDK (compatible with .NET 6 or later).
- A text editor or IDE (e.g., Visual Studio, Visual Studio Code).

---

## How to Run

1. Clone the repository or copy the code into a `.cs` file.
2. Open a terminal and navigate to the project directory.
3. Run the following command to compile and execute the program:
   ```bash
   dotnet run
