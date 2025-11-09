using System;
using System.Collections.Generic;
using UnityEngine;

namespace ImmersiveLearningApp
{
    /// <summary>
    /// Represents the player's progress data that will be saved/loaded
    /// </summary>
    [Serializable]
    public class PlayerProgress
    {
        public string playerName;
        public Dictionary<string, QuizResult> quizResults = new Dictionary<string, QuizResult>();
        public Dictionary<string, float> modelCompletion = new Dictionary<string, float>();
        public int totalScore;
        public DateTime lastPlayed;
        
        public PlayerProgress()
        {
            playerName = "Player";
            lastPlayed = DateTime.Now;
        }
    }
    
    /// <summary>
    /// Stores the results of a specific quiz attempt
    /// </summary>
    [Serializable]
    public class QuizResult
    {
        public int score;
        public int maxScore;
        public DateTime completedAt;
        public bool completed;
        
        public QuizResult(int score, int maxScore)
        {
            this.score = score;
            this.maxScore = maxScore;
            this.completed = true;
            this.completedAt = DateTime.Now;
        }
    }
}