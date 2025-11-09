using System;
using System.Collections.Generic;
using UnityEngine;

namespace ImmersiveLearningApp
{
    /// <summary>
    /// Represents a leaderboard entry
    /// </summary>
    [Serializable]
    public class LeaderboardEntry
    {
        public string playerName;
        public int score;
        public DateTime dateAchieved;
        
        public LeaderboardEntry(string name, int score)
        {
            this.playerName = name;
            this.score = score;
            this.dateAchieved = DateTime.Now;
        }
    }
    
    /// <summary>
    /// Manages the leaderboard with top scores
    /// </summary>
    public class LeaderboardManager : MonoBehaviour
    {
        private const int MAX_LEADERBOARD_ENTRIES = 10;
        private List<LeaderboardEntry> leaderboardEntries = new List<LeaderboardEntry>();
        private const string LEADERBOARD_FILE_NAME = "leaderboard.json";
        
        private static LeaderboardManager instance;
        
        public static LeaderboardManager Instance
        {
            get
            {
                if (instance == null)
                {
                    GameObject managerObject = new GameObject("LeaderboardManager");
                    instance = managerObject.AddComponent<LeaderboardManager>();
                }
                return instance;
            }
        }
        
        private void Awake()
        {
            if (instance != null && instance != this)
            {
                Destroy(gameObject);
                return;
            }
            
            instance = this;
            DontDestroyOnLoad(gameObject);
            
            LoadLeaderboard();
        }
        
        /// <summary>
        /// Adds a new score to the leaderboard
        /// </summary>
        public void AddScore(string playerName, int score)
        {
            LeaderboardEntry newEntry = new LeaderboardEntry(playerName, score);
            leaderboardEntries.Add(newEntry);
            
            // Sort the entries by score (descending)
            leaderboardEntries.Sort((x, y) => y.score.CompareTo(x.score));
            
            // Keep only the top entries
            if (leaderboardEntries.Count > MAX_LEADERBOARD_ENTRIES)
            {
                leaderboardEntries.RemoveRange(MAX_LEADERBOARD_ENTRIES, 
                    leaderboardEntries.Count - MAX_LEADERBOARD_ENTRIES);
            }
            
            SaveLeaderboard();
        }
        
        /// <summary>
        /// Gets the sorted list of leaderboard entries
        /// </summary>
        public List<LeaderboardEntry> GetLeaderboardEntries()
        {
            return leaderboardEntries;
        }
        
        /// <summary>
        /// Saves the leaderboard to JSON file
        /// </summary>
        private void SaveLeaderboard()
        {
            LeaderboardData data = new LeaderboardData();
            data.entries = leaderboardEntries.ToArray();
            
            string json = JsonUtility.ToJson(data, true);
            string path = Path.Combine(Application.persistentDataPath, LEADERBOARD_FILE_NAME);
            File.WriteAllText(path, json);
            
            Debug.Log("Leaderboard saved to: " + path);
        }
        
        /// <summary>
        /// Loads the leaderboard from JSON file
        /// </summary>
        private void LoadLeaderboard()
        {
            string path = Path.Combine(Application.persistentDataPath, LEADERBOARD_FILE_NAME);
            
            if (File.Exists(path))
            {
                string json = File.ReadAllText(path);
                LeaderboardData data = JsonUtility.FromJson<LeaderboardData>(json);
                leaderboardEntries = new List<LeaderboardEntry>(data.entries);
                Debug.Log("Leaderboard loaded from: " + path);
            }
            else
            {
                leaderboardEntries = new List<LeaderboardEntry>();
                Debug.Log("No leaderboard file found, creating new leaderboard");
            }
        }
    }
    
    /// <summary>
    /// Wrapper class for leaderboard data serialization
    /// </summary>
    [Serializable]
    public class LeaderboardData
    {
        public LeaderboardEntry[] entries;
    }
}