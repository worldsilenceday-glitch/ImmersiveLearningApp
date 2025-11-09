using System;
using System.Collections.Generic;
using UnityEngine;

namespace ImmersiveLearningApp
{
    /// <summary>
    /// Data structure for storing question information
    /// </summary>
    [Serializable]
    public class QuestionData
    {
        [Header("Question Information")]
        public string questionText;
        public string topic;
        public List<string> options = new List<string>();
        public int correctAnswerIndex;
        [TextArea(2, 5)]
        public string explanation;
        public int difficulty; // 1 = Easy, 2 = Medium, 3 = Hard
        
        public QuestionData()
        {
            questionText = "Default question";
            topic = "General";
            options = new List<string> { "Option 1", "Option 2", "Option 3", "Option 4" };
            correctAnswerIndex = 0;
            explanation = "Default explanation";
            difficulty = 1;
        }
        
        public QuestionData(string text, string topicName, List<string> opts, int correctIdx, string expl, int diff = 1)
        {
            questionText = text;
            topic = topicName;
            options = opts;
            correctAnswerIndex = correctIdx;
            explanation = expl;
            difficulty = diff;
        }
    }
}