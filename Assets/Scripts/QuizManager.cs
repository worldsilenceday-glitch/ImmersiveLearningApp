using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections;
using System.Collections.Generic;

namespace ImmersiveLearningApp
{
    /// <summary>
    /// Represents a single quiz question
    /// </summary>
    [System.Serializable]
    public class QuizQuestion
    {
        public string questionText;
        public List<string> options = new List<string>();
        public int correctAnswerIndex;
        public string explanation;
        public string topic;
        public int difficulty; // 1 = Easy, 2 = Medium, 3 = Hard
    }
    
    /// <summary>
    /// Controls the quiz gameplay and UI
    /// </summary>
    public class QuizManager : MonoBehaviour
    {
        [Header("Quiz Data")]
        public List<QuizQuestion> questions = new List<QuizQuestion>();
        public string quizId = "defaultQuiz";
        public List<string> selectedTopics = new List<string>();
        
        [Header("UI References")]
        [SerializeField] private TextMeshProUGUI questionText;
        [SerializeField] private Transform optionsContainer;
        [SerializeField] private GameObject optionPrefab;
        [SerializeField] private Button nextButton;
        [SerializeField] private TextMeshProUGUI scoreText;
        [SerializeField] private TextMeshProUGUI feedbackText;
        [SerializeField] private GameObject feedbackPanel;
        [SerializeField] private Slider progressBar;
        
        [Header("Quiz Settings")]
        [SerializeField] private float timePerQuestion = 30f;
        [SerializeField] private bool canNavigateBack = false;
        [SerializeField] private bool randomizeOptions = true;
        [SerializeField] private DifficultyLevel difficulty = DifficultyLevel.Medium;
        
        private List<GameObject> optionObjects = new List<GameObject>();
        private int currentQuestionIndex = 0;
        private int score = 0;
        private List<int> selectedAnswers = new List<int>();
        private List<bool> questionResults = new List<bool>();
        private float timeRemaining;
        private bool questionAnswered = false;
        private Coroutine timerCoroutine;
        private List<QuizQuestion> activeQuestions = new List<QuizQuestion>();
        
        // Events for UI communication
        public System.Action<int, QuizQuestion> OnQuestionLoaded;
        public System.Action<int> OnQuizCompleted;
        
        private void Start()
        {
            InitializeQuiz();
        }
        
        /// <summary>
        /// Initializes the quiz by setting up the first question
        /// </summary>
        private void InitializeQuiz()
        {
            // Filter questions based on selected topics and difficulty
            FilterQuestions();
            
            if (activeQuestions.Count == 0)
            {
                Debug.LogError("No questions available for the selected topics and difficulty!");
                return;
            }
            
            currentQuestionIndex = 0;
            score = 0;
            selectedAnswers.Clear();
            questionResults.Clear();
            
            for (int i = 0; i < activeQuestions.Count; i++)
            {
                selectedAnswers.Add(-1); // -1 means not answered
                questionResults.Add(false);
            }
            
            LoadQuestion(currentQuestionIndex);
            UpdateScoreDisplay();
            
            if (progressBar != null)
            {
                progressBar.maxValue = activeQuestions.Count;
                progressBar.value = 0;
            }
        }
        
        /// <summary>
        /// Filters questions based on selected topics and difficulty
        /// </summary>
        private void FilterQuestions()
        {
            activeQuestions.Clear();
            
            foreach (QuizQuestion question in questions)
            {
                // Check if question topic is in selected topics
                bool topicMatch = selectedTopics.Count == 0 || selectedTopics.Contains(question.topic) || selectedTopics.Contains("Mixed");
                
                // Check if question difficulty matches selected difficulty
                bool difficultyMatch = true;
                switch (difficulty)
                {
                    case DifficultyLevel.Easy:
                        difficultyMatch = question.difficulty <= 1;
                        break;
                    case DifficultyLevel.Medium:
                        difficultyMatch = question.difficulty <= 2;
                        break;
                    case DifficultyLevel.Hard:
                        difficultyMatch = question.difficulty == 3;
                        break;
                }
                
                if (topicMatch && difficultyMatch)
                {
                    activeQuestions.Add(question);
                }
            }
            
            // Shuffle questions to randomize order
            ShuffleList(activeQuestions);
        }
        
        /// <summary>
        /// Loads the specified question
        /// </summary>
        private void LoadQuestion(int questionIndex)
        {
            if (questionIndex < 0 || questionIndex >= activeQuestions.Count)
            {
                EndQuiz();
                return;
            }
            
            QuizQuestion currentQuestion = activeQuestions[questionIndex];
            
            // Notify UI that a question is loaded
            OnQuestionLoaded?.Invoke(questionIndex, currentQuestion);
            
            // Update UI
            if (questionText != null)
            {
                questionText.text = currentQuestion.questionText;
            }
            
            // Create option buttons
            ClearOptions();
            List<string> questionOptions = new List<string>(currentQuestion.options);
            
            if (randomizeOptions)
            {
                ShuffleList(questionOptions);
            }
            
            for (int i = 0; i < questionOptions.Count; i++)
            {
                if (optionPrefab != null)
                {
                    GameObject optionObj = Instantiate(optionPrefab, optionsContainer);
                    OptionButton optionButton = optionObj.GetComponent<OptionButton>();
                    
                    if (optionButton != null)
                    {
                        // Find the original index of this option
                        int originalIndex = currentQuestion.options.IndexOf(questionOptions[i]);
                        optionButton.Initialize(questionOptions[i], originalIndex, OnOptionSelected);
                    }
                    
                    optionObjects.Add(optionObj);
                }
            }
            
            // Reset state
            questionAnswered = false;
            
            // Enable next button initially if we want to allow skipping
            if (nextButton != null)
            {
                nextButton.interactable = false;
            }
            
            // Start timer
            if (timerCoroutine != null)
            {
                StopCoroutine(timerCoroutine);
            }
            timeRemaining = timePerQuestion;
            timerCoroutine = StartCoroutine(QuestionTimer());
            
            // Update progress bar
            if (progressBar != null)
            {
                progressBar.value = questionIndex + 1;
            }
        }
        
        /// <summary>
        /// Handles option selection
        /// </summary>
        private void OnOptionSelected(int optionIndex)
        {
            if (questionAnswered) return; // Prevent multiple answers
            
            QuizQuestion currentQuestion = activeQuestions[currentQuestionIndex];
            bool isCorrect = optionIndex == currentQuestion.correctAnswerIndex;
            
            // Store the answer and result
            selectedAnswers[currentQuestionIndex] = optionIndex;
            questionResults[currentQuestionIndex] = isCorrect;
            
            if (isCorrect)
            {
                // Award points based on difficulty
                int points = 10;
                switch (currentQuestion.difficulty)
                {
                    case 2: points = 15; break; // Medium
                    case 3: points = 20; break; // Hard
                }
                
                score += points;
                ShowFeedback("Correct! " + currentQuestion.explanation, true);
                
                if (AudioFeedbackManager.Instance != null)
                {
                    AudioFeedbackManager.Instance.PlayFeedback(AudioFeedbackType.CorrectAnswer);
                }
                
                // Check achievement for scoring
                if (AchievementManager.Instance != null)
                {
                    AchievementManager.Instance.CheckAchievement("score", score);
                    AchievementManager.Instance.CheckAchievement("topic_score", score, currentQuestion.topic.ToLower());
                }
            }
            else
            {
                ShowFeedback("Incorrect. " + currentQuestion.explanation, false);
                
                if (AudioFeedbackManager.Instance != null)
                {
                    AudioFeedbackManager.Instance.PlayFeedback(AudioFeedbackType.IncorrectAnswer);
                }
            }
            
            questionAnswered = true;
            
            // Disable all option buttons
            foreach (GameObject optionObj in optionObjects)
            {
                OptionButton optionButton = optionObj.GetComponent<OptionButton>();
                if (optionButton != null)
                {
                    optionButton.Disable();
                    
                    // Highlight correct answer
                    if (optionButton.originalIndex == currentQuestion.correctAnswerIndex)
                    {
                        optionButton.SetCorrectAnswer();
                    }
                    else if (optionButton.originalIndex == optionIndex)
                    {
                        optionButton.SetIncorrectAnswer();
                    }
                }
            }
            
            // Enable next button
            if (nextButton != null)
            {
                nextButton.interactable = true;
            }
        }
        
        /// <summary>
        /// Shows feedback to the player
        /// </summary>
        private void ShowFeedback(string message, bool isCorrect)
        {
            if (feedbackPanel != null)
            {
                feedbackPanel.SetActive(true);
            }
            
            if (feedbackText != null)
            {
                feedbackText.text = message;
                
                // Color feedback text based on correctness
                if (isCorrect)
                {
                    feedbackText.color = Color.green;
                }
                else
                {
                    feedbackText.color = Color.red;
                }
            }
        }
        
        /// <summary>
        /// Navigates to the next question
        /// </summary>
        public void NextQuestion()
        {
            if (currentQuestionIndex < activeQuestions.Count - 1)
            {
                currentQuestionIndex++;
                LoadQuestion(currentQuestionIndex);
                
                if (nextButton != null)
                {
                    nextButton.interactable = false;
                }
                
                if (feedbackPanel != null)
                {
                    feedbackPanel.SetActive(false);
                }
            }
            else
            {
                EndQuiz();
            }
        }
        
        /// <summary>
        /// Ends the quiz and saves results
        /// </summary>
        private void EndQuiz()
        {
            int maxPossibleScore = activeQuestions.Count * 20; // Max points for hard questions
            
            if (ProgressManager.Instance != null)
            {
                ProgressManager.Instance.UpdateQuizResult(quizId, score, maxPossibleScore);
            }
            
            if (LeaderboardManager.Instance != null)
            {
                var progress = ProgressManager.Instance.GetPlayerProgress();
                LeaderboardManager.Instance.AddScore(progress.playerName, score);
            }
            
            if (AudioFeedbackManager.Instance != null)
            {
                AudioFeedbackManager.Instance.PlayFeedback(AudioFeedbackType.QuizCompleted);
            }
            
            // Trigger quiz completion event
            OnQuizCompleted?.Invoke(score);
            
            // Check completion achievements
            if (AchievementManager.Instance != null)
            {
                AchievementManager.Instance.CheckAchievement("completion", 1);
            }
            
            Debug.Log($"Quiz completed! Final score: {score}/{maxPossibleScore}");
        }
        
        /// <summary>
        /// Updates the score display
        /// </summary>
        private void UpdateScoreDisplay()
        {
            if (scoreText != null)
            {
                scoreText.text = $"Score: {score}";
            }
        }
        
        /// <summary>
        /// Clears existing option buttons
        /// </summary>
        private void ClearOptions()
        {
            foreach (GameObject optionObj in optionObjects)
            {
                if (optionObj != null)
                    Destroy(optionObj);
            }
            optionObjects.Clear();
        }
        
        /// <summary>
        /// Coroutine for the question timer
        /// </summary>
        private IEnumerator QuestionTimer()
        {
            while (timeRemaining > 0 && !questionAnswered)
            {
                timeRemaining -= Time.deltaTime;
                
                if (timeRemaining <= 0)
                {
                    // Time's up! Process as unanswered
                    if (!questionAnswered)
                    {
                        OnOptionSelected(-1); // -1 indicates no answer
                    }
                    yield break;
                }
                
                yield return null;
            }
        }
        
        /// <summary>
        /// Shuffles a list randomly
        /// </summary>
        private void ShuffleList<T>(List<T> list)
        {
            for (int i = 0; i < list.Count; i++)
            {
                int randomIndex = Random.Range(i, list.Count);
                T temp = list[i];
                list[i] = list[randomIndex];
                list[randomIndex] = temp;
            }
        }
        
        /// <summary>
        /// Skips to the next question
        /// </summary>
        public void SkipQuestion()
        {
            if (!questionAnswered)
            {
                OnOptionSelected(-1);
            }
        }
        
        /// <summary>
        /// Sets the topics for the quiz
        /// </summary>
        public void SetTopics(List<string> topics)
        {
            selectedTopics = new List<string>(topics);
            Debug.Log($"Quiz topics set to: {string.Join(", ", selectedTopics)}");
        }
        
        /// <summary>
        /// Sets the difficulty level for the quiz
        /// </summary>
        public void SetDifficulty(DifficultyLevel diff)
        {
            difficulty = diff;
            Debug.Log($"Quiz difficulty set to: {difficulty}");
        }
        
        /// <summary>
        /// Gets the total number of questions in the active set
        /// </summary>
        public int GetTotalQuestions()
        {
            return activeQuestions.Count;
        }
        
        /// <summary>
        /// Gets the current score
        /// </summary>
        public int GetCurrentScore()
        {
            return score;
        }
        
        /// <summary>
        /// Gets the current question
        /// </summary>
        public QuizQuestion GetCurrentQuestion()
        {
            if (currentQuestionIndex >= 0 && currentQuestionIndex < activeQuestions.Count)
            {
                return activeQuestions[currentQuestionIndex];
            }
            return null;
        }
    }
}