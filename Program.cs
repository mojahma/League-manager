using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text.Json;
using System.IO;

namespace LeagueManager
{
    class Program
    {
        // Class representing a Team
        class Team
        {
            public string TeamName { get; set; }
            public int MatchesPlayed { get; set; } = 0;
            public int GoalsScored { get; set; } = 0;
            public int GoalsConceded { get; set; } = 0;
            public int Points { get; set; } = 0;

            public Team(string teamName)
            {
                TeamName = teamName;
            }

            // Updates the team statistics based on match results
            public void UpdateStats(int goalsScored, int goalsConceded)
            {
                MatchesPlayed++;
                GoalsScored += goalsScored;
                GoalsConceded += goalsConceded;

                if (goalsScored > goalsConceded)
                    Points += 3;  // Win
                else if (goalsScored == goalsConceded)
                    Points += 1;  // Draw
            }

            // Calculate goal difference
            public int GoalDifference => GoalsScored - GoalsConceded;
        }

        // Class representing a League
        class League
        {
            public string LeagueName { get; set; }
            public List<Team> TeamsList { get; set; } = new List<Team>();
            public List<Fixture> Fixtures { get; set; } = new List<Fixture>();

            public League(string leagueName)
            {
                LeagueName = leagueName;
            }

            // Add a team to the league
            public void AddTeam(Team team)
            {
                TeamsList.Add(team);
                Console.WriteLine($"Team {team.TeamName} added to {LeagueName}.");
            }

            // Schedule a fixture (match) between two teams
            public void ScheduleFixture(string homeTeam, string awayTeam, DateTime fixtureDate)
            {
                if (TeamsList.All(t => t.TeamName != homeTeam) || TeamsList.All(t => t.TeamName != awayTeam))
                {
                    Console.WriteLine("One or both teams do not exist in the league.");
                    return;
                }

                Fixtures.Add(new Fixture(homeTeam, awayTeam, fixtureDate));
                Console.WriteLine($"Fixture scheduled: {homeTeam} vs {awayTeam} on {fixtureDate.ToShortDateString()}");
            }

            // Process match result and update standings
            public void ProcessMatchResult(string homeTeam, string awayTeam, int homeGoals, int awayGoals, DateTime matchDate)
            {
                Team? home = TeamsList.FirstOrDefault(t => t.TeamName == homeTeam);
                Team? away = TeamsList.FirstOrDefault(t => t.TeamName == awayTeam);

                if (home == null || away == null)
                {
                    Console.WriteLine("One or both teams do not exist.");
                    return;
                }

                // Find the fixture if it exists
                var fixture = Fixtures.FirstOrDefault(f =>
                    f.HomeTeam == homeTeam &&
                    f.AwayTeam == awayTeam &&
                    !f.IsPlayed &&
                    f.FixtureDate.Date == matchDate.Date);

                if (fixture != null)
                {
                    fixture.HomeGoals = homeGoals;
                    fixture.AwayGoals = awayGoals;
                    fixture.IsPlayed = true;
                }
                else
                {
                    // Create a new fixture with the result
                    var newFixture = new Fixture(homeTeam, awayTeam, matchDate)
                    {
                        HomeGoals = homeGoals,
                        AwayGoals = awayGoals,
                        IsPlayed = true
                    };
                    Fixtures.Add(newFixture);
                }

                // Update team stats
                home.UpdateStats(homeGoals, awayGoals);
                away.UpdateStats(awayGoals, homeGoals);

                Console.WriteLine($"Match Result: {homeTeam} {homeGoals} - {awayGoals} {awayTeam}");
            }

            // Display the league table
            public void DisplayLeagueTable()
            {
                if (TeamsList.Count == 0)
                {
                    Console.WriteLine($"No teams available in {LeagueName}.");
                    return;
                }

                var sortedTeams = TeamsList.OrderByDescending(t => t.Points)
                                            .ThenByDescending(t => t.GoalDifference)
                                            .ThenByDescending(t => t.GoalsScored)
                                            .ToList();

                Console.WriteLine($"\n===== {LeagueName} Standings =====");
                Console.WriteLine($"{"Pos",-5} {"Team",-15} {"MP",-5} {"GS",-5} {"GC",-5} {"GD",-5} {"Points",-5}");
                Console.WriteLine(new string('-', 55));

                int position = 1;
                foreach (var team in sortedTeams)
                {
                    Console.WriteLine($"{position,-5} {team.TeamName,-15} {team.MatchesPlayed,-5} {team.GoalsScored,-5} {team.GoalsConceded,-5} {team.GoalDifference,-5} {team.Points,-5}");
                    position++;
                }
                Console.WriteLine();
            }

            // Predict all upcoming fixtures
            public void PredictAllFixtures()
            {
                if (Fixtures.Count == 0)
                {
                    Console.WriteLine("No fixtures available.");
                    return;
                }

                var upcomingFixtures = Fixtures.Where(f => !f.IsPlayed && f.FixtureDate > DateTime.Now).ToList();

                if (upcomingFixtures.Count == 0)
                {
                    Console.WriteLine("No upcoming fixtures.");
                    return;
                }

                Console.WriteLine("\n===== Upcoming Fixtures Predictions =====");
                foreach (var fixture in upcomingFixtures)
                {
                    Prediction.PredictMatch(this, fixture.HomeTeam, fixture.AwayTeam);
                }
            }

            // Predict a specific match
            public void PredictMatch(string homeTeam, string awayTeam)
            {
                if (TeamsList.All(t => t.TeamName != homeTeam) || TeamsList.All(t => t.TeamName != awayTeam))
                {
                    Console.WriteLine("One or both teams do not exist in the league.");
                    return;
                }

                Prediction.PredictMatch(this, homeTeam, awayTeam);
            }

            // Delete past fixtures
            public void DeletePastFixtures()
            {
                int deletedCount = Fixtures.RemoveAll(f => f.FixtureDate < DateTime.Now && !f.IsPlayed);
                Console.WriteLine($"Deleted {deletedCount} past fixtures.");
            }
        }

        // Class representing a fixture (match)
        class Fixture
        {
            public string HomeTeam { get; set; }
            public string AwayTeam { get; set; }
            public DateTime FixtureDate { get; set; }
            public bool IsPlayed { get; set; } = false;
            public int? HomeGoals { get; set; } = null;
            public int? AwayGoals { get; set; } = null;

            public Fixture(string homeTeam, string awayTeam, DateTime fixtureDate)
            {
                HomeTeam = homeTeam;
                AwayTeam = awayTeam;
                FixtureDate = fixtureDate;
            }
        }

        // Enhanced Prediction of upcoming matches
        class Prediction
        {
            private const int MATCH_HISTORY_COUNT = 10;
            private const double HOME_ADVANTAGE = 1.3;
            private const double H2H_WEIGHT = 0.25;
            private const double RECENT_FORM_WEIGHT = 0.35;
            private const double TEAM_STRENGTH_WEIGHT = 0.25;
            private const double POSITION_DIFFERENCE_WEIGHT = 0.15;

            // Predict match outcome with enhanced factors
            public static void PredictMatch(League league, string homeTeam, string awayTeam)
            {
                Team? home = league.TeamsList.FirstOrDefault(t => t.TeamName == homeTeam);
                Team? away = league.TeamsList.FirstOrDefault(t => t.TeamName == awayTeam);

                if (home == null || away == null)
                {
                    Console.WriteLine("One or both teams do not exist in the league.");
                    return;
                }

                // Get last 10 matches for each team
                var homeMatches = GetLastMatches(league, homeTeam, MATCH_HISTORY_COUNT);
                var awayMatches = GetLastMatches(league, awayTeam, MATCH_HISTORY_COUNT);

                // Get head-to-head matches
                var h2hMatches = GetHeadToHeadMatches(league, homeTeam, awayTeam);

                // Calculate team form (points from last 10 games)
                double homeForm = CalculateForm(homeMatches, homeTeam);
                double awayForm = CalculateForm(awayMatches, awayTeam);

                // Calculate home/away performance
                double homeTeamHomeAvgGoals = CalculateHomeGoalAverage(homeMatches, homeTeam);
                double awayTeamAwayAvgGoals = CalculateAwayGoalAverage(awayMatches, awayTeam);

                // Calculate team strength metrics
                double homeStrength = CalculateTeamStrength(league, home);
                double awayStrength = CalculateTeamStrength(league, away);

                // Calculate league position difference
                double positionFactor = CalculatePositionFactor(league, homeTeam, awayTeam);

                // Calculate H2H advantage
                double h2hFactor = CalculateH2HFactor(h2hMatches, homeTeam, awayTeam);

                // Apply weighting to different factors
                double homeTeamScore = (homeForm * RECENT_FORM_WEIGHT) +
                                       (homeStrength * TEAM_STRENGTH_WEIGHT) +
                                       (h2hFactor * H2H_WEIGHT) +
                                       (positionFactor * POSITION_DIFFERENCE_WEIGHT);

                double awayTeamScore = (awayForm * RECENT_FORM_WEIGHT) +
                                       (awayStrength * TEAM_STRENGTH_WEIGHT) +
                                       ((2.0 - h2hFactor) * H2H_WEIGHT) +
                                       ((2.0 - positionFactor) * POSITION_DIFFERENCE_WEIGHT);

                // Predict scores
                double predictedHomeGoals = homeTeamHomeAvgGoals * homeTeamScore * HOME_ADVANTAGE;
                double predictedAwayGoals = awayTeamAwayAvgGoals * awayTeamScore;

                // Add randomness factor for realism
                Random random = new Random();
                predictedHomeGoals += (random.NextDouble() - 0.5) * 0.5;
                predictedAwayGoals += (random.NextDouble() - 0.5) * 0.5;

                // Round to realistic values
                int homeGoalsPrediction = Math.Max(0, (int)Math.Round(predictedHomeGoals));
                int awayGoalsPrediction = Math.Max(0, (int)Math.Round(predictedAwayGoals));

                // Account for fatigue and fixture congestion
                homeGoalsPrediction = AdjustForFixtureCongestion(league, homeTeam, homeGoalsPrediction);
                awayGoalsPrediction = AdjustForFixtureCongestion(league, awayTeam, awayGoalsPrediction);

                // Output prediction
                Console.WriteLine("\n===== Match Prediction =====");
                Console.WriteLine($"{homeTeam} vs {awayTeam}");
                Console.WriteLine($"Predicted Score: {homeTeam} {homeGoalsPrediction} - {awayGoalsPrediction} {awayTeam}");

                // Determine outcome
                if (homeGoalsPrediction > awayGoalsPrediction)
                    Console.WriteLine($"Prediction: {homeTeam} Win");
                else if (homeGoalsPrediction < awayGoalsPrediction)
                    Console.WriteLine($"Prediction: {awayTeam} Win");
                else
                    Console.WriteLine("Prediction: Draw");

                // Output prediction factors
                Console.WriteLine("\nPrediction Factors:");
                Console.WriteLine($"Recent Form: {homeTeam} {homeForm:F2} vs {awayTeam} {awayForm:F2}");
                Console.WriteLine($"Team Strength: {homeTeam} {homeStrength:F2} vs {awayTeam} {awayStrength:F2}");
                Console.WriteLine($"Head-to-Head Advantage: {(h2hFactor > 1.0 ? homeTeam : awayTeam)}");
                Console.WriteLine($"League Position Advantage: {(positionFactor > 1.0 ? homeTeam : awayTeam)}");

                // Display confidence level based on available match history
                int matchHistoryCount = Math.Min(homeMatches.Count, awayMatches.Count);
                int h2hCount = h2hMatches.Count;
                
                string confidenceLevel = (matchHistoryCount >= 8 && h2hCount >= 3) ? "High" :
                                       (matchHistoryCount >= 5 && h2hCount >= 1) ? "Medium" : "Low";
                                       
                Console.WriteLine($"\nConfidence Level: {confidenceLevel}");
                Console.WriteLine($"Based on {matchHistoryCount} recent matches and {h2hCount} head-to-head encounters");
                Console.WriteLine();
            }

            // Get last N matches for a team
            private static List<Fixture> GetLastMatches(League league, string teamName, int count)
            {
                return league.Fixtures
                    .Where(f => f.IsPlayed && (f.HomeTeam == teamName || f.AwayTeam == teamName))
                    .OrderByDescending(f => f.FixtureDate)
                    .Take(count)
                    .ToList();
            }

            // Get head-to-head matches between two teams
            private static List<Fixture> GetHeadToHeadMatches(League league, string teamA, string teamB)
            {
                return league.Fixtures
                    .Where(f => f.IsPlayed && 
                          ((f.HomeTeam == teamA && f.AwayTeam == teamB) || 
                           (f.HomeTeam == teamB && f.AwayTeam == teamA)))
                    .OrderByDescending(f => f.FixtureDate)
                    .ToList();
            }

            // Calculate team form (points from recent matches)
            private static double CalculateForm(List<Fixture> matches, string teamName)
            {
                if (matches.Count == 0) return 1.0; // Default form if no matches

                // Weight more recent matches higher
                double totalWeightedPoints = 0;
                double weightSum = 0;

                for (int i = 0; i < matches.Count; i++)
                {
                    var match = matches[i];
                    // Weight decreases with match age (more recent matches have higher weight)
                    double weight = 1.0 - (i * 0.05);
                    weightSum += weight;

                    bool isHome = match.HomeTeam == teamName;
                    int? teamGoals = isHome ? match.HomeGoals : match.AwayGoals;
                    int? opponentGoals = isHome ? match.AwayGoals : match.HomeGoals;

                    if (teamGoals > opponentGoals)
                        totalWeightedPoints += 3 * weight;
                    else if (teamGoals == opponentGoals)
                        totalWeightedPoints += 1 * weight;
                }

                // Calculate weighted form (normalize to scale between 0.5 and 1.5)
                double maxPossiblePoints = weightSum * 3.0;
                return 0.5 + (totalWeightedPoints / maxPossiblePoints);
            }

            // Calculate head-to-head advantage factor (>1 means home team advantage, <1 means away team advantage)
            private static double CalculateH2HFactor(List<Fixture> h2hMatches, string homeTeam, string awayTeam)
            {
                if (h2hMatches.Count == 0) return 1.0; // Neutral if no H2H history

                int homeTeamWins = 0;
                int awayTeamWins = 0;
                int draws = 0;

                foreach (var match in h2hMatches)
                {
                    bool isHomeTeamHome = match.HomeTeam == homeTeam;
                    int? homeTeamGoals = isHomeTeamHome ? match.HomeGoals : match.AwayGoals;
                    int? awayTeamGoals = isHomeTeamHome ? match.AwayGoals : match.HomeGoals;

                    if (homeTeamGoals > awayTeamGoals)
                        homeTeamWins++;
                    else if (homeTeamGoals < awayTeamGoals)
                        awayTeamWins++;
                    else
                        draws++;
                }

                // Calculate weighted H2H factor (recent matches count more)
                double totalMatches = h2hMatches.Count;
                double homeWinRatio = homeTeamWins / totalMatches;
                double awayWinRatio = awayTeamWins / totalMatches;

                // Scale from 0.7 to 1.3 (1.0 is neutral)
                return 1.0 + ((homeWinRatio - awayWinRatio) * 0.3);
            }

            // Calculate average goals scored at home
            private static double CalculateHomeGoalAverage(List<Fixture> matches, string teamName)
            {
                var homeMatches = matches.Where(m => m.HomeTeam == teamName && m.HomeGoals.HasValue).ToList();

                if (homeMatches.Count == 0) return 1.2; // Default if no home matches

                double totalGoals = homeMatches.Sum(m => m.HomeGoals ?? 0);
                return totalGoals / homeMatches.Count;
            }

            // Calculate average goals scored away
            private static double CalculateAwayGoalAverage(List<Fixture> matches, string teamName)
            {
                var awayMatches = matches.Where(m => m.AwayTeam == teamName && m.AwayGoals.HasValue).ToList();

                if (awayMatches.Count == 0) return 0.8; // Default if no away matches (away teams typically score less)

                double totalGoals = awayMatches.Sum(m => m.AwayGoals ?? 0);
                return totalGoals / awayMatches.Count;
            }

            // Calculate position difference factor (advantage for higher position teams)
            private static double CalculatePositionFactor(League league, string homeTeam, string awayTeam)
            {
                var sortedTeams = league.TeamsList
                    .OrderByDescending(t => t.Points)
                    .ThenByDescending(t => t.GoalDifference)
                    .ThenByDescending(t => t.GoalsScored)
                    .ToList();

                int homePosition = sortedTeams.FindIndex(t => t.TeamName == homeTeam) + 1;
                int awayPosition = sortedTeams.FindIndex(t => t.TeamName == awayTeam) + 1;
                
                // Calculate position difference (positive if home team is higher ranked)
                int positionDifference = awayPosition - homePosition;
                
                // Scale from 0.7 to 1.3 (1.0 is neutral - same position)
                double maxPositionDiff = league.TeamsList.Count / 2.0;
                return 1.0 + (Math.Min(positionDifference, maxPositionDiff) / maxPositionDiff) * 0.3;
            }

            // Calculate team strength based on goals scored/conceded and overall performance
            private static double CalculateTeamStrength(League league, Team team)
            {
                if (team.MatchesPlayed == 0) return 1.0; // Default if no matches played

                // Calculate attack strength (goals scored per game ratio to league average)
                double leagueAvgGoalsScored = league.TeamsList.Where(t => t.MatchesPlayed > 0)
                                              .Average(t => (double)t.GoalsScored / t.MatchesPlayed);
                double teamGoalsPerGame = (double)team.GoalsScored / team.MatchesPlayed;
                double attackStrength = leagueAvgGoalsScored > 0 ? teamGoalsPerGame / leagueAvgGoalsScored : 1.0;

                // Calculate defense strength (inverse of goals conceded ratio)
                double leagueAvgGoalsConceded = league.TeamsList.Where(t => t.MatchesPlayed > 0)
                                                .Average(t => (double)t.GoalsConceded / t.MatchesPlayed);
                double teamGoalsConcededPerGame = (double)team.GoalsConceded / team.MatchesPlayed;
                double defenseStrength = leagueAvgGoalsConceded > 0 ? 
                                       leagueAvgGoalsConceded / teamGoalsConcededPerGame : 1.0;

                // Calculate points efficiency (points per game ratio to max possible)
                double pointsEfficiency = (double)team.Points / (team.MatchesPlayed * 3.0);

                // Combine factors with different weights
                return (attackStrength * 0.4) + (defenseStrength * 0.4) + (pointsEfficiency * 0.2);
            }

            // Adjust for fixture congestion (fatigue impact)
            private static int AdjustForFixtureCongestion(League league, string teamName, int predictedGoals)
            {
                // Check if team has had multiple games in short period (last 10 days)
                var recentFixtures = league.Fixtures
                    .Where(f => f.IsPlayed && (f.HomeTeam == teamName || f.AwayTeam == teamName))
                    .Where(f => (DateTime.Now - f.FixtureDate).TotalDays <= 10)
                    .ToList();

                if (recentFixtures.Count >= 3)
                {
                    // Reduce predicted goals due to fatigue (max 1 goal reduction)
                    return Math.Max(0, predictedGoals - 1);
                }

                return predictedGoals;
            }
        }

        // Class to manage leagues
        class ManageLeagues
        {
            public List<League> LeagueList { get; set; } = new List<League>();

            // Add a new league
            public void AddLeague()
            {
                Console.Write("Enter League Name: ");
                string leagueName = Console.ReadLine()!;
                LeagueList.Add(new League(leagueName));
                Console.WriteLine($"League '{leagueName}' added successfully.");
            }

            // Find a league by name
            public League? GetLeague(string leagueName)
            {
                return LeagueList.FirstOrDefault(league => league.LeagueName.Equals(leagueName, StringComparison.OrdinalIgnoreCase));
            }

            // Save league data to a file
            public void SaveData(string filePath)
            {
                var options = new JsonSerializerOptions { WriteIndented = true };
                string json = JsonSerializer.Serialize(LeagueList, options);
                File.WriteAllText(filePath, json);
                Console.WriteLine("Data saved successfully.");
            }

            // Load league data from a file
            public void LoadData(string filePath)
            {
                if (File.Exists(filePath))
                {
                    string json = File.ReadAllText(filePath);
                    LeagueList = JsonSerializer.Deserialize<List<League>>(json)!;
                    Console.WriteLine("Data loaded successfully.");
                }
                else
                {
                    Console.WriteLine("No saved data found.");
                }
            }
        }

        // Main method
        static void Main(string[] args)
        {
            ManageLeagues manageLeagues = new ManageLeagues();
            string filePath = "league_data.json";

            // Load saved data
            manageLeagues.LoadData(filePath);

            while (true)
            {
                Console.WriteLine("\nMenu:");
                Console.WriteLine("1. Add League");
                Console.WriteLine("2. Add Team to a League");
                Console.WriteLine("3. Schedule Fixture");
                Console.WriteLine("4. Process Match Result");
                Console.WriteLine("5. Display League Table");
                Console.WriteLine("6. Predict All Fixtures");
                Console.WriteLine("7. Predict Specific Match");
                Console.WriteLine("8. Delete Past Fixtures");
                Console.WriteLine("9. Save Data");
                Console.WriteLine("10. Exit");
                Console.Write("Choose an option: ");

                if (!int.TryParse(Console.ReadLine(), out int choice))
                {
                    Console.WriteLine("Invalid input. Please enter a number.");
                    continue;
                }

                switch (choice)
                {
                    case 1:
                        manageLeagues.AddLeague();
                        break;
                    case 2:
                        Console.Write("Enter League Name: ");
                        string leagueName = Console.ReadLine()!;
                        League? selectedLeague = manageLeagues.GetLeague(leagueName);

                        if (selectedLeague == null)
                        {
                            Console.WriteLine("League not found.");
                            continue;
                        }

                        Console.Write("Enter Team Name: ");
                        string teamName = Console.ReadLine()!;
                        selectedLeague.AddTeam(new Team(teamName));
                        break;
                    case 3:
                        Console.Write("Enter League Name: ");
                        leagueName = Console.ReadLine()!;
                        selectedLeague = manageLeagues.GetLeague(leagueName);

                        if (selectedLeague == null)
                        {
                            Console.WriteLine("League not found.");
                            continue;
                        }

                        Console.Write("Enter Home Team: ");
                        string homeTeam = Console.ReadLine()!;
                        Console.Write("Enter Away Team: ");
                        string awayTeam = Console.ReadLine()!;
                        Console.Write("Enter Match Date (DD/MM/YYYY) or press Enter for today: ");
                        string dateInput = Console.ReadLine()!;

                        DateTime matchDate;
                        if (string.IsNullOrWhiteSpace(dateInput))
                        {
                            matchDate = DateTime.Now;
                        }
                        else if (!DateTime.TryParseExact(dateInput, "dd/MM/yyyy", CultureInfo.InvariantCulture, DateTimeStyles.None, out matchDate))
                        {
                            Console.WriteLine("Invalid date format. Using today's date.");
                            matchDate = DateTime.Now;
                        }

                        selectedLeague.ScheduleFixture(homeTeam, awayTeam, matchDate);
                        break;
                    case 4:
                        Console.Write("Enter League Name: ");
                        leagueName = Console.ReadLine()!;
                        selectedLeague = manageLeagues.GetLeague(leagueName);

                        if (selectedLeague == null)
                        {
                            Console.WriteLine("League not found.");
                            continue;
                        }

                        Console.Write("Enter Home Team: ");
                        homeTeam = Console.ReadLine()!;
                        Console.Write("Enter Away Team: ");
                        awayTeam = Console.ReadLine()!;
                        Console.Write("Enter Home Team Goals: ");
                        if (!int.TryParse(Console.ReadLine(), out int homeGoals))
                        {
                            Console.WriteLine("Invalid input for goals. Please enter a number.");
                            continue;
                        }

                        Console.Write("Enter Away Team Goals: ");
                        if (!int.TryParse(Console.ReadLine(), out int awayGoals))
                        {
                            Console.WriteLine("Invalid input for goals. Please enter a number.");
                            continue;
                        }

                        Console.Write("Enter Match Date (DD/MM/YYYY) or press Enter for today: ");
                        dateInput = Console.ReadLine()!;

                        if (string.IsNullOrWhiteSpace(dateInput))
                        {
                            matchDate = DateTime.Now;
                        }
                        else if (!DateTime.TryParseExact(dateInput, "dd/MM/yyyy", CultureInfo.InvariantCulture, DateTimeStyles.None, out matchDate))
                        {
                            Console.WriteLine("Invalid date format. Using today's date.");
                            matchDate = DateTime.Now;
                        }

                        selectedLeague.ProcessMatchResult(homeTeam, awayTeam, homeGoals, awayGoals, matchDate);
                        break;
                    case 5:
                        Console.Write("Enter League Name: ");
                        leagueName = Console.ReadLine()!;
                        selectedLeague = manageLeagues.GetLeague(leagueName);
                        selectedLeague?.DisplayLeagueTable();
                        break;
                    case 6:
                        Console.Write("Enter League Name: ");
                        leagueName = Console.ReadLine()!;
                        selectedLeague = manageLeagues.GetLeague(leagueName);
                        selectedLeague?.PredictAllFixtures();
                        break;
                    case 7:
                        Console.Write("Enter League Name: ");
                        leagueName = Console.ReadLine()!;
                        selectedLeague = manageLeagues.GetLeague(leagueName);

                        if (selectedLeague == null)
                        {
                            Console.WriteLine("League not found.");
                            continue;
                        }

                        Console.Write("Enter Home Team: ");
                        homeTeam = Console.ReadLine()!;
                        Console.Write("Enter Away Team: ");
                        awayTeam = Console.ReadLine()!;
                        
                        selectedLeague.PredictMatch(homeTeam, awayTeam);
                        break;
                    case 8:
                        Console.Write("Enter League Name: ");
                        leagueName = Console.ReadLine()!;
                        selectedLeague = manageLeagues.GetLeague(leagueName);
                        selectedLeague?.DeletePastFixtures();
                        break;
                    case 9:
                        manageLeagues.SaveData(filePath);
                        break;
                    case 10:
                        Console.WriteLine("Exiting program.");
                        return;
                    default:
                        Console.WriteLine("Invalid option. Please try again.");
                        break;
                }
            }
        }
    }
}