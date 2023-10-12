using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using MongoDB.Bson;
using MongoDB.Driver;
using TMPro;
using UnityEngine;

public class MongoDBManager
{
    private const string connectionString = "mongodb+srv://sang23820011:sang123456@cluster0.6vlf5nv.mongodb.net/test"; // Thay đổi chuỗi kết nối theo cấu hình của bạn
    private const string databaseName = "XepGach1";

    private IMongoClient client;
    public IMongoDatabase database;
    private ModelPlayer mdplayer;


    public MongoDBManager()
    {
        client = new MongoClient(connectionString);
        database = client.GetDatabase(databaseName);
    }


    //------------------------------------Register & Login------------------------------------------

    // Phương thức để tạo tài khoản người chơi
    public string Register(string username, string password, string playerName)
    {
        var collection = database.GetCollection<BsonDocument>("players");

        // Kiểm tra nếu đã tồn tại username trong cơ sở dữ liệu
        var usernameFilter = Builders<BsonDocument>.Filter.Eq("username", username);
        var existingUsername = collection.Find(usernameFilter).FirstOrDefault();
        if (existingUsername != null)
        {
            //return "Username already exists.";
            return "SIGNUP_USERNAME_E003";
        }

        // Kiểm tra nếu đã tồn tại playerName trong cơ sở dữ liệu
        var playerNameFilter = Builders<BsonDocument>.Filter.Eq("playerName", playerName);
        var existingPlayerName = collection.Find(playerNameFilter).FirstOrDefault();
        if (existingPlayerName != null)
        {
            //return "PlayerName already exists.";
            return "SIGNUP_PLAYERNAME_E003";
        }

        // Nếu không có tài khoản nào trùng, tiến hành thêm tài khoản mới vào cơ sở dữ liệu
        var document = new BsonDocument
        {
            { "username", username },
            { "password", password },
            { "playerName", playerName },
            { "scores", new BsonArray() },
            { "levels", new BsonArray() },
            { "color", 1 },
            { "playSession", 1 },
            { "playTime", 1 },
            { "playMoments", new BsonArray() },
        };

        collection.InsertOne(document);
        //return "Success!\nYour account has been created";
        return "SUCCESS!";
    }

    public ModelPlayer Login(string username, string password)
    {
        var collection = database.GetCollection<BsonDocument>("players");

        // Kiểm tra xem có tài khoản nào trùng khớp với username và password không
        var filter = Builders<BsonDocument>.Filter.And(
            Builders<BsonDocument>.Filter.Eq("username", username),
            Builders<BsonDocument>.Filter.Eq("password", password)
        );

        var existingPlayer = collection.Find(filter).FirstOrDefault();

        if (existingPlayer != null)
        {
            // Thời điểm đăng nhập
            DateTime loginTime = DateTime.Now;
            // Thêm thời điểm đăng nhập vào danh sách playMoments của người chơi trong cơ sở dữ liệu
            var updateDefinition = Builders<BsonDocument>.Update.Push("playMoments", loginTime.ToString());
            collection.UpdateOne(filter, updateDefinition);

            // If correct, read in4, return ModelPlayer
            ModelPlayer player = new ModelPlayer
            {
                Username = existingPlayer.GetValue("username").AsString,
                Password = existingPlayer.GetValue("password").AsString,
                PlayerName = existingPlayer.GetValue("playerName").AsString,
                Scores = existingPlayer.GetValue("scores").AsBsonArray.Select(x => x.ToInt32()).ToList(),
                Levels = existingPlayer.GetValue("levels").AsBsonArray.Select(x => new ModelPlayer.LevelData
                {
                    LevelNumber = x["levelnumber"].AsInt32,
                    Appreciate = x["appreciate"].AsInt32
                }).ToList(),
                Color = existingPlayer.GetValue("color").AsInt32,
                playSession = existingPlayer.GetValue("playSession").AsInt32,
                playTime = existingPlayer.GetValue("playTime").AsInt32,
                playMoments = existingPlayer.GetValue("playMoments").AsBsonArray.Select(x => x.AsString).ToList()
            };
            // Thêm thời điểm đăng nhập vào danh sách playMoments của người chơi
            //player.playMoments.Add(loginTime.ToString());
            return player;
        }
        else
        {
            return null; // Trả về null nếu không tìm thấy tài khoản trùng khớp
        }
    }





    //------------------------------------Levels------------------------------------------
    public void RecordPlayerRating(string username, int levelNumber, int rating)
    {
        var collection = database.GetCollection<BsonDocument>("players");
        var filter = Builders<BsonDocument>.Filter.Eq("username", username);

        // Check if the player exists in the database
        var playerExists = collection.Find(filter).Any();

        var newLevelData = new BsonDocument
    {
        { "levelnumber", levelNumber },
        { "appreciate", rating }
    };

        if (playerExists)
        {
            // Check if the player has already played the level
            var levelFilter = Builders<BsonDocument>.Filter.Eq($"levels.levelnumber", levelNumber);
            var existingPlayer = collection.Find(filter & levelFilter).FirstOrDefault();

            if (existingPlayer != null)
            {
                // Update the existing entry for the level if the new rating is higher
                var levelDataArray = existingPlayer["levels"].AsBsonArray;
                var levelData = levelDataArray.FirstOrDefault(x => x["levelnumber"].AsInt32 == levelNumber);

                if (levelData != null)
                {
                    int existingRating = levelData["appreciate"].AsInt32;
                    if (rating > existingRating)
                    {
                        var update = Builders<BsonDocument>.Update.Set($"levels.$.appreciate", rating);
                        collection.UpdateOne(filter & levelFilter, update);
                    }
                }
                else
                {
                    // Add a new entry for the level if it doesn't exist
                    var update = Builders<BsonDocument>.Update.Push("levels", newLevelData);
                    collection.UpdateOne(filter, update);
                }
            }
            else
            {
                // If the player has not played any level yet, add a new entry for the current level
                var update = Builders<BsonDocument>.Update.Push("levels", newLevelData);
                collection.UpdateOne(filter, update);
            }
        }
        else
        {
            // If the player doesn't exist in the database, add them with the given level rating
            var levelsList = new BsonArray { newLevelData }; // Create a new array containing the initial level data
            for (int prevLevel = 1; prevLevel < levelNumber; prevLevel++)
            {
                // Set "appreciate" value to 0 for any previous levels that haven't been played
                var prevLevelData = new BsonDocument
            {
                { "levelnumber", prevLevel },
                { "appreciate", 0 }
            };
                levelsList.Add(prevLevelData);
            }

            var update = Builders<BsonDocument>.Update.Set("levels", levelsList);
            collection.InsertOne(new BsonDocument { { "username", username }, { "levels", levelsList } });
        }
    }


    public int GetPlayerCurrentLevelAppreciate(string username, int levelNumber)
    {
        var collection = database.GetCollection<BsonDocument>("players");

        var filter = Builders<BsonDocument>.Filter.Eq("username", username);
        var levelFilter = Builders<BsonDocument>.Filter.Eq($"levels.levelnumber", levelNumber);

        var projection = Builders<BsonDocument>.Projection.Include("levels");
        var result = collection.Find(filter & levelFilter).Project(projection).FirstOrDefault();

        if (result != null)
        {
            var levelsArray = result["levels"].AsBsonArray;
            var levelData = levelsArray.FirstOrDefault(l => l["levelnumber"].AsInt32 == levelNumber);

            if (levelData != null)
            {
                return levelData["appreciate"].AsInt32;
            }
        }

        // If the level or player doesn't exist, return a default value (e.g., -1)
        return -1;
    }

    //------------------------------------Scores------------------------------------------

    public void SaveScoreInPlayMode(string username, int score)
    {
        var collection = database.GetCollection<BsonDocument>("players");

        var filter = Builders<BsonDocument>.Filter.Eq("username", username);
        var existingPlayer = collection.Find(filter).FirstOrDefault();

        if (existingPlayer != null)
        {
            // Get the current scores array from the existing player document
            List<int> currentScores = existingPlayer.GetValue("scores", new BsonArray()).AsBsonArray.ToList().Select(x => x.AsInt32).ToList();

            // Add the new score to the scores array
            currentScores.Add(score);

            // Update the "scores" field with the updated scores array
            var update = Builders<BsonDocument>.Update.Set("scores", new BsonArray(currentScores));
            collection.UpdateOne(filter, update);
        }
    }

    //------------------------------------Color------------------------------------------
    // Record curr Color
    public void SavePlayerColor(string username, int selectedColor)
    {
        var collection = database.GetCollection<BsonDocument>("players");

        // Find the player by their username
        var filter = Builders<BsonDocument>.Filter.Eq("username", username);
        var existingPlayer = collection.Find(filter).FirstOrDefault();

        if (existingPlayer != null)
        {
            // Update the "color" field with the selected color
            var update = Builders<BsonDocument>.Update.Set("color", selectedColor);
            collection.UpdateOne(filter, update);
        }
    }

    public int GetPlayerColor(string username)
    {
        var collection = database.GetCollection<BsonDocument>("players");

        // Find the player by their username
        var filter = Builders<BsonDocument>.Filter.Eq("username", username);
        var existingPlayer = collection.Find(filter).FirstOrDefault();

        if (existingPlayer != null)
        {
            // Get the value of the "color" field as an integer
            int color = existingPlayer.GetValue("color").AsInt32;
            return color;
        }
        return 1; // set default color 
    }



    //------------------------------------LeaderBoard Scores------------------------------------------

    public Dictionary<string, int> GetPlayerNamesAndHighScores()
    {
        var collection = database.GetCollection<BsonDocument>("players");

        // Projection to get only the "playerName" and "scores" fields from the documents
        var projection = Builders<BsonDocument>.Projection.Include("playerName").Include("scores");

        // Sort the documents based on the highest score in descending order
        var sort = Builders<BsonDocument>.Sort.Descending("scores");

        // Find all players and project only "playerName" and "scores" fields
        var playerNamesAndScores = collection.Find(new BsonDocument())
            .Project(projection)
            .Sort(sort)
            .ToList();

        // Dictionary to store player names and their highest scores
        var result = new Dictionary<string, int>();

        foreach (var player in playerNamesAndScores)
        {
            string playerName = player.GetValue("playerName").AsString;
            List<int> scores = player.GetValue("scores").AsBsonArray.Select(x => x.ToInt32()).ToList();

            // Find the highest score in the scores list
            int highestScore = scores.Count > 0 ? scores.Max() : 0;

            // Add the playerName and highest score to the result dictionary
            result.Add(playerName, highestScore);
        }

        return result;
    }

    public int GetHighestScoreForPlayer(string username)
    {
        var collection = database.GetCollection<BsonDocument>("players");

        // Find the player by their username
        var filter = Builders<BsonDocument>.Filter.Eq("username", username);
        var existingPlayer = collection.Find(filter).FirstOrDefault();

        if (existingPlayer != null)
        {
            // Get the value of the "scores" field as a List<int>
            List<int> scores = existingPlayer.GetValue("scores", new BsonArray()).AsBsonArray.ToList().Select(x => x.AsInt32).ToList();

            // Find the highest score in the scores list
            int highestScore = scores.Count > 0 ? scores.Max() : 0;

            return highestScore;
        }

        // If the player doesn't exist, return 0 as the highest score
        return 0;
    }

    //------------------------------------LeaderBoard Stars------------------------------------------

    public Dictionary<string, int> GetPlayerNamesAndTotalAppreciate()
    {
        var collection = database.GetCollection<BsonDocument>("players");

        // Projection to get only the "playerName" and "levels" fields from the documents
        var projection = Builders<BsonDocument>.Projection.Include("playerName").Include("levels");

        // Find all players and project only "playerName" and "levels" fields
        var playerNamesAndLevels = collection.Find(new BsonDocument())
            .Project(projection)
            .ToList();

        // Dictionary to store player names and their total appreciate stars
        var result = new Dictionary<string, int>();

        foreach (var player in playerNamesAndLevels)
        {
            string playerName = player.GetValue("playerName").AsString;
            var levelsArray = player.GetValue("levels").AsBsonArray;

            // Calculate the total "appreciate" star for the player
            int totalAppreciate = 0;
            foreach (var levelData in levelsArray)
            {
                totalAppreciate += levelData["appreciate"].AsInt32;
            }

            // Add the player name and total "appreciate" star to the result dictionary
            result.Add(playerName, totalAppreciate);
        }

        // Sort the dictionary in descending order of totalAppreciate stars
        var sortedResult = result.OrderByDescending(x => x.Value).ToDictionary(x => x.Key, x => x.Value);

        return sortedResult;
    }

    public int GetTotalStarsForPlayer(string username)
    {
        var collection = database.GetCollection<BsonDocument>("players");

        // Find the player by their username
        var filter = Builders<BsonDocument>.Filter.Eq("username", username);
        var existingPlayer = collection.Find(filter).FirstOrDefault();

        if (existingPlayer != null)
        {
            var levelsArray = existingPlayer.GetValue("levels").AsBsonArray;

            // Calculate the total "appreciate" score for the player
            int totalAppreciate = 0;
            foreach (var levelData in levelsArray)
            {
                totalAppreciate += levelData["appreciate"].AsInt32;
            }

            return totalAppreciate;
        }

        // If the player doesn't exist or is not logged in, return 0 as the total appreciate
        return 0;
    }

    //------------------------------------History------------------------------------------

    public List<int> GetAllScoresForLoggedInPlayer(string username)
    {
        var collection = database.GetCollection<BsonDocument>("players");

        // Find the player by their username
        var filter = Builders<BsonDocument>.Filter.Eq("username", username);
        var existingPlayer = collection.Find(filter).FirstOrDefault();

        if (existingPlayer != null)
        {
            var scoresArray = existingPlayer.GetValue("scores").AsBsonArray;

            // Convert the BsonArray to a List<int> of scores
            List<int> scores = scoresArray.Select(x => x.ToInt32()).ToList();

            return scores;
        }

        // If the player doesn't exist or is not logged in, return an empty list
        return new List<int>();
    }

    //------------------------------------Play Session------------------------------------------
    // Record curr play session
    public void SavePlaySession(string username, int nextPlaySession)
    {
        var collection = database.GetCollection<BsonDocument>("players");

        var filter = Builders<BsonDocument>.Filter.Eq("username", username);
        var existingPlayer = collection.Find(filter).FirstOrDefault();

        if (existingPlayer != null)
        {
            var update = Builders<BsonDocument>.Update.Set("playSession", nextPlaySession);
            collection.UpdateOne(filter, update);
        }
    }

    public int GetPlaySession(string username)
    {
        var collection = database.GetCollection<BsonDocument>("players");

        // Find the player by their username
        var filter = Builders<BsonDocument>.Filter.Eq("username", username);
        var existingPlayer = collection.Find(filter).FirstOrDefault();

        if (existingPlayer != null)
        {
            int playSession = existingPlayer.GetValue("playSession").AsInt32;
            return playSession;
        }
        return 1;
    }

    //------------------------------------Total Play Time------------------------------------------
    // Record curr play time
    public void SavePlayTime(string username, int nextPlayTime)
    {
        var collection = database.GetCollection<BsonDocument>("players");

        var filter = Builders<BsonDocument>.Filter.Eq("username", username);
        var existingPlayer = collection.Find(filter).FirstOrDefault();

        if (existingPlayer != null)
        {
            var update = Builders<BsonDocument>.Update.Set("playTime", nextPlayTime);
            collection.UpdateOne(filter, update);
        }
    }

    public int GetPlayTime(string username)
    {
        var collection = database.GetCollection<BsonDocument>("players");

        // Find the player by their username
        var filter = Builders<BsonDocument>.Filter.Eq("username", username);
        var existingPlayer = collection.Find(filter).FirstOrDefault();

        if (existingPlayer != null)
        {
            int playTime = existingPlayer.GetValue("playTime").AsInt32;
            return playTime;
        }
        return 1;
    }


    //------------------------------------Report List------------------------------------------
    public Dictionary<string, (int playTime, int playSession)> GetPlayerNamesAndStats()
    {
        var collection = database.GetCollection<BsonDocument>("players");

        // Projection to get only the required fields from the documents
        var projection = Builders<BsonDocument>.Projection
            .Include("playerName")
            .Include("playTime")
            .Include("playSession");

        var playerNamesAndStats = collection.Find(new BsonDocument())
            .Project(projection)
            .ToList();

        var result = new Dictionary<string, (int playTime, int playSession)>();

        foreach (var player in playerNamesAndStats)
        {
            string playerName = player.GetValue("playerName").AsString;
            int playTime = player.GetValue("playTime").AsInt32;
            int playSession = player.GetValue("playSession").AsInt32;

            result.Add(playerName, (playTime, playSession));
        }

        var sortedResult = result.OrderByDescending(x => x.Value.playSession)
            .ToDictionary(x => x.Key, x => x.Value);

        return sortedResult;
    }

    //------------------------------------Detail Report------------------------------------------
    public List<(string PlayerName, List<string> PlayMoments)> GetPlayerPlayMoments()
    {
        var collection = database.GetCollection<BsonDocument>("players");

        // Lấy ra danh sách người chơi với thông tin cần thiết
        var projection = Builders<BsonDocument>.Projection.Include("playerName").Include("playMoments");
        var players = collection.Find(new BsonDocument()).Project(projection).ToList();

        var playerPlayMomentsList = new List<(string PlayerName, List<string> PlayMoments)>();
        foreach (var player in players)
        {
            var playerName = player.GetValue("playerName").AsString;
            var playMoments = player.GetValue("playMoments").AsBsonArray.Select(x => x.AsString).ToList();
            playerPlayMomentsList.Add((playerName, playMoments));
        }

        return playerPlayMomentsList;
    }

    //public List<(string PlayerName, List<string> PlayMoments)> GetPlayerPlayMomentsWithinAWeek()
    //{
    //    var collection = database.GetCollection<ModelPlayer>("players");

    //    // Lấy ngày hiện tại và ngày 7 ngày trước đó
    //    DateTime currentDate = DateTime.Now;
    //    DateTime oneWeekAgo = currentDate.AddDays(-7);

    //    // Tạo biểu thức tìm kiếm: playMoments trong vòng 1 tuần tính đến hiện tại
    //    var filter = Builders<ModelPlayer>.Filter.And(
    //        Builders<ModelPlayer>.Filter.ElemMatch(player => player.playMoments, moment => DateTime.Parse(moment) >= oneWeekAgo && DateTime.Parse(moment) <= currentDate),
    //        Builders<ModelPlayer>.Filter.Exists("playMoments", true)
    //    );

    //    // Lấy ra danh sách người chơi thỏa mãn điều kiện
    //    var players = collection.Find(filter).ToList();

    //    // Chuyển đổi dữ liệu từ ModelPlayer sang danh sách cần thiết
    //    var playerPlayMomentsList = new List<(string PlayerName, List<string> PlayMoments)>();
    //    foreach (var player in players)
    //    {
    //        var playerName = player.PlayerName;
    //        var playMoments = player.playMoments;
    //        playerPlayMomentsList.Add((playerName, playMoments));
    //    }

    //    return playerPlayMomentsList;
    //}
}
