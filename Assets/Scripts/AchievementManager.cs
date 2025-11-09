using System;
using System.Collections.Generic;
using UnityEngine;

namespace ImmersiveLearningApp
{
    /// <summary>
    /// Represents an achievement in the game
    /// </summary>
    [Serializable]
    public class Achievement
    {
        public string id;
        public string title;
        public string description;
        public bool isUnlocked;
        public DateTime unlockedDate;
        public int unlockConditionValue;
        public string conditionType; // "score", "completion", "time", etc.
        
        public Achievement(string achievementId, string achievementTitle, string achievementDescription, string type, int conditionValue)
        {
            id = achievementId;
            title = achievementTitle;
            description = achievementDescription;
            conditionType = type;
            unlockConditionValue = conditionValue;
            isUnlocked = false;
            unlockedDate = DateTime.MinValue;
        }
    }
    
    /// <summary>
    /// Manages achievements and tracks unlocked achievements
    /// </summary>
    public class AchievementManager : MonoBehaviour
    {
        private static AchievementManager instance;
        private List<Achievement> achievements = new List<Achievement>();
        private const string ACHIEVEMENTS_FILE_NAME = "achievements.json";
        
        public static AchievementManager Instance
        {
            get
            {
                if (instance == null)
                {
                    GameObject managerObject = new GameObject("AchievementManager");
                    instance = managerObject.AddComponent<AchievementManager>();
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
            
            InitializeAchievements();
            LoadAchievements();
        }
        
        /// <summary>
        /// Initializes the default achievements
        /// </summary>
        private void InitializeAchievements()
        {
            // Score-based achievements
            achievements.Add(new Achievement("score_100", "First Steps", "Score 100 points in a quiz", "score", 100));
            achievements.Add(new Achievement("score_500", "Quiz Master", "Score 500 points in a quiz", "score", 500));
            achievements.Add(new Achievement("score_1000", "Knowledge Legend", "Score 1000 points in a quiz", "score", 1000));
            
            // Completion-based achievements
            achievements.Add(new Achievement("complete_5", "Persistent Learner", "Complete 5 quizzes", "completion", 5));
            achievements.Add(new Achievement("complete_10", "Dedicated Scholar", "Complete 10 quizzes", "completion", 10));
            achievements.Add(new Achievement("complete_all", "Master Explorer", "Complete all topics", "completion", 100)); // Special achievement
            
            // Topic-based achievements
            achievements.Add(new Achievement("biology_100", "Biologist", "Score 100 points in biology topic", "topic_score", 100));
            achievements.Add(new Achievement("planets_100", "Astronomer", "Score 100 points in planets topic", "topic_score", 100));
            achievements.Add(new Achievement("mixed_100", "Versatile Learner", "Score 100 points in mixed topic", "topic_score", 100));
            
            // Speed-based achievements
            achievements.Add(new Achievement("fast_quiz", "Speed Demon", "Complete a quiz in under 2 minutes", "time", 120));
        }
        
        /// <summary>
        /// Checks if an achievement condition is met and unlocks it if so
        /// </summary>
        public void CheckAchievement(string conditionType, int value, string additionalParam = "")
        {
            foreach (Achievement achievement in achievements)
            {
                if (achievement.isUnlocked) continue; // Skip already unlocked achievements
                
                bool conditionMet = false;
                
                switch (achievement.conditionType)
                {
                    case "score":
                        if (conditionType == "score" && value >= achievement.unlockConditionValue)
                        {
                            conditionMet = true;
                        }
                        break;
                    case "completion":
                        if (conditionType == "completion" && value >= achievement.unlockConditionValue)
                        {
                            conditionMet = true;
                        }
                        break;
                    case "topic_score":
                        if (conditionType == "topic_score" && value >= achievement.unlockConditionValue && 
                            additionalParam == achievement.id.Split('_')[0]) // Match topic type
                        {
                            conditionMet = true;
                        }
                        break;
                    case "time":
                        if (conditionType == "time" && value <= achievement.unlockConditionValue)
                        {
                            conditionMet = true;
                        }
                        break;
                }
                
                if (conditionMet)
                {
                    UnlockAchievement(achievement.id);
                }
            }
        }
        
        /// <summary>
        /// Unlocks an achievement by ID
        /// </summary>
        public void UnlockAchievement(string achievementId)
        {
            Achievement achievement = achievements.Find(a => a.id == achievementId);
            
            if (achievement != null && !achievement.isUnlocked)
            {
                achievement.isUnlocked = true;
                achievement.unlockedDate = DateTime.Now;
                
                Debug.Log($"Achievement unlocked: {achievement.title} - {achievement.description}");
                
                // Save achievements
                SaveAchievements();
                
                // Optional: Trigger UI notification here
                // AchievementUIManager.Instance?.ShowAchievementNotification(achievement);
            }
        }
        
        /// <summary>
        /// Gets all achievements
        /// </summary>
        public List<Achievement> GetAllAchievements()
        {
            return new List<Achievement>(achievements);
        }
        
        /// <summary>
        /// Gets unlocked achievements
        /// </summary>
        public List<Achievement> GetUnlockedAchievements()
        {
            List<Achievement> unlocked = new List<Achievement>();
            
            foreach (Achievement achievement in achievements)
            {
                if (achievement.isUnlocked)
                {
                    unlocked.Add(achievement);
                }
            }
            
            return unlocked;
        }
        
        /// <summary>
        /// Gets locked achievements
        /// </summary>
        public List<Achievement> GetLockedAchievements()
        {
            List<Achievement> locked = new List<Achievement>();
            
            foreach (Achievement achievement in achievements)
            {
                if (!achievement.isUnlocked)
                {
                    locked.Add(achievement);
                }
            }
            
            return locked;
        }
        
        /// <summary>
        /// Saves achievements to JSON file
        /// </summary>
        public void SaveAchievements()
        {
            AchievementData data = new AchievementData();
            data.achievements = achievements.ToArray();
            
            string json = JsonUtility.ToJson(data, true);
            string path = System.IO.Path.Combine(Application.persistentDataPath, ACHIEVEMENTS_FILE_NAME);
            System.IO.File.WriteAllText(path, json);
            
            Debug.Log("Achievements saved to: " + path);
        }
        
        /// <summary>
        /// Loads achievements from JSON file
        /// </summary>
        public void LoadAchievements()
        {
            string path = System.IO.Path.Combine(Application.persistentDataPath, ACHIEVEMENTS_FILE_NAME);
            
            if (System.IO.File.Exists(path))
            {
                string json = System.IO.File.ReadAllText(path);
                AchievementData data = JsonUtility.FromJson<AchievementData>(json);
                
                // Update achievements with loaded data
                if (data.achievements != null)
                {
                    for (int i = 0; i < data.achievements.Length; i++)
                    {
                        Achievement loadedAchievement = data.achievements[i];
                        
                        for (int j = 0; j < achievements.Count; j++)
                        {
                            if (achievements[j].id == loadedAchievement.id)
                            {
                                achievements[j].isUnlocked = loadedAchievement.isUnlocked;
                                achievements[j].unlockedDate = loadedAchievement.unlockedDate;
                                break;
                            }
                        }
                    }
                }
                
                Debug.Log("Achievements loaded from: " + path);
            }
            else
            {
                Debug.Log("No achievements file found, starting with default achievements");
            }
        }
        
        /// <summary>
        /// Checks if a specific achievement is unlocked
        /// </summary>
        public bool IsAchievementUnlocked(string achievementId)
        {
            Achievement achievement = achievements.Find(a => a.id == achievementId);
            return achievement != null && achievement.isUnlocked;
        }
    }
    
    /// <summary>
    /// Wrapper class for achievement data serialization
    /// </summary>
    [Serializable]
    public class AchievementData
    {
        public Achievement[] achievements;
    }
}