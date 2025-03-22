# League Manager
League Manager is a C# console application designed to manage football (soccer) leagues, teams, fixtures, and match results. It provides functionality to add leagues, teams, schedule fixtures, process match results, display league standings, and predict match outcomes based on historical data and team performance metrics.

###Features
League Management:

####Create and manage multiple leagues.

Add teams to a league.

#### Schedule fixtures between teams.

Process match results and update league standings.

League Table:

Display the league table sorted by points, goal difference, and goals scored.

Automatically update standings after processing match results.

Match Prediction:

Predict the outcome of upcoming fixtures using advanced algorithms.

Factors include recent form, head-to-head records, team strength, and league position.

Adjust predictions for fixture congestion and fatigue.

Data Persistence:

Save and load league data to/from a JSON file.

Retain league, team, and fixture information between sessions.

Fixture Management:

Delete past fixtures that have not been played.

View upcoming fixtures and their predictions.

How It Works
Core Classes
Team:

Represents a football team.

Tracks matches played, goals scored, goals conceded, and points.

League:

Represents a football league.

Manages a list of teams and fixtures.

Provides methods to add teams, schedule fixtures, process match results, and display the league table.

Fixture:

Represents a match between two teams.

Tracks match date, goals scored, and whether the match has been played.

Prediction:

Predicts the outcome of matches using weighted factors:

Recent form (last 10 matches).

Head-to-head records.

Team strength (goals scored/conceded, points efficiency).

League position difference.

Home advantage.

Adjusts predictions for fixture congestion and fatigue.

ManageLeagues:

Manages multiple leagues.

Provides functionality to save and load league data to/from a JSON file.

Usage
Menu Options
Add League:

Create a new league by providing a name.

Add Team to a League:

Add a team to an existing league.

Schedule Fixture:

Schedule a match between two teams in a league.

Specify the match date or use the current date.

Process Match Result:

Enter the result of a match (goals scored by each team).

Update league standings automatically.

Display League Table:

View the current standings of a league.

Predict All Fixtures:

Predict the outcomes of all upcoming fixtures in a league.

Predict Specific Match:

Predict the outcome of a specific match between two teams.

Delete Past Fixtures:

Remove past fixtures that have not been played.

Save Data:

Save all league, team, and fixture data to a JSON file.

Exit:

Exit the application.

Prediction Algorithm
The prediction algorithm uses the following factors to determine the likely outcome of a match:

Recent Form:

Weighted points from the last 10 matches (recent matches have higher weight).

Head-to-Head Records:

Historical performance between the two teams.

Team Strength:

Goals scored/conceded per game compared to the league average.

Points efficiency (points per game ratio to the maximum possible).

League Position Difference:

Advantage for teams higher in the league table.

Home Advantage:

Teams playing at home have a slight advantage.

Fixture Congestion:

Adjusts predictions for teams with multiple matches in a short period (fatigue impact).

Data Persistence
League, team, and fixture data are saved to a JSON file (league_data.json).

Data is automatically loaded when the application starts.

Use the Save Data option to manually save changes.

Example Workflow
Add a league (e.g., "Premier League").

Add teams to the league (e.g., "Team A", "Team B").

Schedule fixtures between teams.

Process match results to update standings.

View the league table.

Predict upcoming fixtures.

Save data before exiting.

Requirements
.NET SDK (compatible with .NET 6 or later).

A text editor or IDE (e.g., Visual Studio, Visual Studio Code).

How to Run
Clone the repository or copy the code into a .cs file.

Open a terminal and navigate to the project directory.

Run the following command to compile and execute the program:

bash
Copy
dotnet run
Future Enhancements
User Authentication:

Add user roles (e.g., admin, viewer) with different permissions.

Advanced Statistics:

Include more detailed team and player statistics.

Graphical Interface:

Develop a GUI for better user interaction.

Integration with APIs:

Fetch real-time data from football APIs for live updates.

Multi-League Support:

Enable cross-league comparisons and predictions.

License
This project is open-source and available under the MIT License. Feel free to modify and distribute it as needed.

Author
[Your Name]
[Your Email]
[GitHub Profile]

Enjoy managing your leagues and predicting match outcomes with League Manager! ⚽# League-manager
