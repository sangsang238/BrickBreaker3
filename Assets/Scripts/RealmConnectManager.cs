//using UnityEngine;
//using Realms;
//using System.Collections.Generic;
//using Realms.Sync;
//using System.Linq;
//using Realms.Exceptions;

//public class RealmConnectManager : MonoBehaviour
//{
//    private const string appID = "trochoixepgach-mpddg"; // Replace this with your Realm app ID
//    private Realm _realm;


//    private void Start()
//    {
//        // Initialize the Realm app with the provided app ID
//        var app = App.Create(appID);
//        var user = app.CurrentUser;

//        if (user == null)
//        {
//            Debug.Log("User is not logged in. Cannot initialize Realm.");
//            return;
//        }

//        // Set up the Realm configuration with MongoDB Realm cluster connection
//        var config = new AppConfiguration(app)
//        {
//            BaseUri = "https://realm.mongodb.com",
//            ServiceName = "mongodb-atlas",
//        };

//        _realm = Realm.GetInstance(config);

//        // Now the _realm variable is initialized and can be used to interact with the Realm database.
//        // You can perform Realm database operations using the _realm variable in other methods of this class.
//    }


//    //------------------------------------Register & Login------------------------------------------

//    // Method to create a player account
//    // Method to create a player account
//    public string Register(string username, string password, string playerName)
//    {
//        // Check if a player with the given username already exists
//        var existingUsername = _realm.All<Player>().Where(p => p.Username == username).FirstOrDefault();
//        if (existingUsername != null)
//        {
//            return "Username already exists.";
//        }

//        // Check if a player with the given playerName already exists
//        var existingPlayerName = _realm.All<Player>().Where(p => p.PlayerName == playerName).FirstOrDefault();
//        if (existingPlayerName != null)
//        {
//            return "PlayerName already exists.";
//        }

//        // If no duplicates, create a new player and save it to the Realm database
//        _realm.Write(() =>
//        {
//            var player = new Player(username, password, playerName, 1); // Default color is set to 1
//            _realm.Add(player);
//        });

//        return "Success!\nYour account has been created.";
//    }

//    // Method to log in a player and retrieve the player data
//    public Player Login(string username, string password)
//    {
//        var player = _realm.All<Player>().Where(p => p.Username == username && p.Password == password).FirstOrDefault();

//        // If a player with the given username and password is found, return the player data
//        return player;
//    }

//    //------------------------------------Levels------------------------------------------
//    public void RecordPlayerRating(string username, int levelNumber, int rating)
//    {
//        var player = _realm.All<Player>().Where(p => p.Username == username).FirstOrDefault();

//        if (player != null)
//        {
//            var existingLevelData = player.Levels.Where(ld => ld.LevelNumber == levelNumber).FirstOrDefault();

//            if (existingLevelData != null)
//            {
//                // Update the existing entry for the level if the new rating is higher
//                if (rating > existingLevelData.Appreciate)
//                {
//                    _realm.Write(() =>
//                    {
//                        existingLevelData.Appreciate = rating;
//                    });
//                }
//            }
//            else
//            {
//                // Add a new entry for the level if it doesn't exist
//                _realm.Write(() =>
//                {
//                    var levelData = new Player.LevelData
//                    {
//                        LevelNumber = levelNumber,
//                        Appreciate = rating
//                    };
//                    player.Levels.Add(levelData);
//                });
//            }
//        }
//        else
//        {
//            // If the player doesn't exist in the database, create a new player with the given level rating
//            _realm.Write(() =>
//            {
//                var newPlayer = new Player(username, "", "", 1); // Create a new player with default color 1
//                newPlayer.Levels.Add(new Player.LevelData
//                {
//                    LevelNumber = levelNumber,
//                    Appreciate = rating
//                });
//                _realm.Add(newPlayer);
//            });
//        }
//    }

//    public int GetPlayerCurrentLevelAppreciate(string username, int levelNumber)
//    {
//        var player = _realm.All<Player>().Where(p => p.Username == username).FirstOrDefault();

//        if (player != null)
//        {
//            var levelData = player.Levels.Where(ld => ld.LevelNumber == levelNumber).FirstOrDefault();
//            if (levelData != null)
//            {
//                return levelData.Appreciate;
//            }
//        }

//        // If the level or player doesn't exist, return a default value (e.g., -1)
//        return -1;
//    }

//    //------------------------------------Scores------------------------------------------

//    public void SaveScoreInPlayMode(string username, int score)
//    {
//        var player = _realm.All<Player>().Where(p => p.Username == username).FirstOrDefault();

//        if (player != null)
//        {
//            _realm.Write(() =>
//            {
//                player.Scores.Add(score);
//            });
//        }
//    }

//    //------------------------------------Color------------------------------------------
//    public void SavePlayerColor(string username, int selectedColor)
//    {
//        var player = _realm.All<Player>().Where(p => p.Username == username).FirstOrDefault();

//        if (player != null)
//        {
//            _realm.Write(() =>
//            {
//                player.Color = selectedColor;
//            });
//        }
//    }

//    public int GetPlayerColor(string username)
//    {
//        var player = _realm.All<Player>().Where(p => p.Username == username).FirstOrDefault();

//        if (player != null)
//        {
//            return player.Color;
//        }

//        return 1; // set default color 
//    }

//    //------------------------------------LeaderBoard------------------------------------------

//    public Dictionary<string, int> GetPlayerNamesAndHighScores()
//    {
//        var playerNamesAndScores = _realm.All<Player>();

//        // Dictionary to store player names and their highest scores
//        var result = new Dictionary<string, int>();

//        foreach (var player in playerNamesAndScores)
//        {
//            string username = player.Username;
//            List<int> scores = player.Scores.ToList(); // Convert IList<int> to List<int>

//            // Find the highest score in the scores list
//            int highestScore = scores.Count > 0 ? scores.Max() : 0;

//            // Add the username and highest score to the result dictionary
//            result.Add(username, highestScore);
//        }

//        return result;
//    }


//    public int GetHighestScoreForPlayer(string username)
//    {
//        var player = _realm.All<Player>().Where(p => p.Username == username).FirstOrDefault();

//        if (player != null)
//        {
//            // Find the highest score in the scores list
//            int highestScore = player.Scores.Count > 0 ? player.Scores.Max() : 0;

//            return highestScore;
//        }

//        // If the player doesn't exist, return 0 as the highest score
//        return 0;
//    }
//}

//public class Player : RealmObject
//{
//    [PrimaryKey]
//    public string Username { get; set; }
//    public string Password { get; set; }
//    public string PlayerName { get; set; }
//    public IList<int> Scores { get;  } // Add private setter
//    public IList<LevelData> Levels { get;  } // Add private setter
//    public int Color { get; set; }

//    // Private constructor for initializing Scores and Levels
//    private Player()
//    {
//        Scores = new List<int>();
//        Levels = new List<LevelData>();
//    }

//    public Player(string username, string password, string playerName, int color)
//    {
//        Username = username;
//        Password = password;
//        PlayerName = playerName;
//        Color = color;
//    }

//    public class LevelData : RealmObject
//    {
//        public int LevelNumber { get; set; }
//        public int Appreciate { get; set; }
//    }
//}
