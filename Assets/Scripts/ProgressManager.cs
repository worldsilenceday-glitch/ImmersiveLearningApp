using System;
using System.IO;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace ImmersiveLearningApp
{
    /// <summary>
    /// Manages player progress saving and loading using JSON
    /// </summary>
    public class ProgressManager : MonoBehaviour
    {
        private static ProgressManager instance;
        private PlayerProgress playerProgress;
        private const string SAVE_FILE_NAME = "playerProgress.json";
        
        public static ProgressManager Instance
        {
            get
            {
                if (instance == null)
                {
                    GameObject managerObject = new GameObject("ProgressManager");
                    instance = managerObject.AddComponent<ProgressManager>();
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
            
            LoadProgress();
        }
        
        /// <summary>
        /// Saves the current player progress to JSON file
        /// </summary>
        public void SaveProgress()
        {
            if (playerProgress == null)
            {
                playerProgress = new PlayerProgress();
            }
            
            playerProgress.lastPlayed = DateTime.Now;
            string json = JsonUtility.ToJson(playerProgress, true);
            string path = Path.Combine(Application.persistentDataPath, SAVE_FILE_NAME);
            File.WriteAllText(path, json);
            
            Debug.Log("Progress saved to: " + path);
        }
        
        /// <summary>
        /// Loads player progress from JSON file
        /// </summary>
        public void LoadProgress()
        {
            string path = Path.Combine(Application.persistentDataPath, SAVE_FILE_NAME);
            
            if (File.Exists(path))
            {
                string json = File.ReadAllText(path);
                playerProgress = JsonUtility.FromJson<PlayerProgress>(json);
                Debug.Log("Progress loaded from: " + path);
            }
            else
            {
                playerProgress = new PlayerProgress();
                Debug.Log("No save file found, creating new progress data");
            }
        }
        
        /// <summary>
        /// Updates quiz result for a specific quiz
        /// </summary>
        public void UpdateQuizResult(string quizId, int score, int maxScore)
        {
            if (playerProgress.quizResults.ContainsKey(quizId))
            {
                playerProgress.quizResults[quizId] = new QuizResult(score, maxScore);
            }
            else
            {
                playerProgress.quizResults.Add(quizId, new QuizResult(score, maxScore));
            }
            
            playerProgress.totalScore += score;
            SaveProgress();
        }
        
        /// <summary>
        /// Updates model completion progress
        /// </summary>
        public void UpdateModelCompletion(string modelId, float completionPercentage)
        {
            if (playerProgress.modelCompletion.ContainsKey(modelId))
            {
                playerProgress.modelCompletion[modelId] = completionPercentage;
            }
            else
            {
                playerProgress.modelCompletion.Add(modelId, completionPercentage);
            }
            
            SaveProgress();
        }
        
        /// <summary>
        /// Gets the player progress data
        /// </summary>
        public PlayerProgress GetPlayerProgress()
        {
            return playerProgress;
        }
        
        /// <summary>
        /// Resets all player progress
        /// </summary>
        public void ResetProgress()
        {
            playerProgress = new PlayerProgress();
            SaveProgress();
        }
        
        /// <summary>
        /// Gets the score for a specific quiz
        /// </summary>
        public QuizResult GetQuizResult(string quizId)
        {
            if (playerProgress.quizResults.ContainsKey(quizId))
            {
                return playerProgress.quizResults[quizId];
            }
            
            return null;
        }
    }
}