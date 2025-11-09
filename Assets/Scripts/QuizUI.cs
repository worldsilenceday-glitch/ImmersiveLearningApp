using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace ImmersiveLearningApp
{
    /// <summary>
    /// Manages the Quiz UI with animated transitions
    /// </summary>
    public class QuizUI : MonoBehaviour
    {
        [Header("UI References")]
        [SerializeField] private GameObject questionPanel;
        [SerializeField] private TextMeshProUGUI questionText;
        [SerializeField] private Transform answerOptionsContainer;
        [SerializeField] private Button nextButton;
        [SerializeField] private Button skipButton;
        [SerializeField] private TextMeshProUGUI scoreText;
        [SerializeField] private TextMeshProUGUI timerText;
        [SerializeField] private Slider progressSlider;
        [SerializeField] private TextMeshProUGUI progressText;
        
        [Header("Feedback UI")]
        [SerializeField] private GameObject feedbackPanel;
        [SerializeField] private TextMeshProUGUI feedbackText;
        [SerializeField] private Image feedbackBackground;
        
        [Header("Animation Settings")]
        [SerializeField] private float panelTransitionDuration = 0.5f;
        [SerializeField] private float answerRevealDuration = 0.3f;
        [SerializeField] private AnimationCurve transitionCurve = AnimationCurve.EaseInOut(0, 0, 1, 1);
        [SerializeField] private Color correctFeedbackColor = Color.green;
        [SerializeField] private Color incorrectFeedbackColor = Color.red;
        
        [Header("Prefabs")]
        [SerializeField] private GameObject answerButtonPrefab;
        
        private List<GameObject> answerButtons = new List<GameObject>();
        private QuizManager quizManager;
        private Coroutine timerCoroutine;
        private float timePerQuestion = 30f; // Default time
        private float timeRemaining;
        
        private void Start()
        {
            quizManager = FindObjectOfType<QuizManager>();
            if (quizManager != null)
            {
                quizManager.OnQuestionLoaded += OnQuestionLoaded;
                quizManager.OnQuizCompleted += OnQuizCompleted;
                timePerQuestion = quizManager.timePerQuestion;
            }
            
            InitializeUI();
        }
        
        /// <summary>
        /// Initializes the UI components
        /// </summary>
        private void InitializeUI()
        {
            if (nextButton != null)
            {
                nextButton.onClick.AddListener(NextQuestion);
                nextButton.interactable = false;
            }
            
            if (skipButton != null)
            {
                skipButton.onClick.AddListener(SkipQuestion);
            }
            
            if (progressSlider != null)
            {
                progressSlider.minValue = 0;
                progressSlider.maxValue = 1;
                progressSlider.value = 0;
            }
            
            // Initially hide feedback panel
            if (feedbackPanel != null)
            {
                feedbackPanel.SetActive(false);
            }
        }
        
        /// <summary>
        /// Called when a new question is loaded
        /// </summary>
        private void OnQuestionLoaded(int index, QuestionData question)
        {
            StartCoroutine(AnimateQuestionTransition(index, question));
        }
        
        /// <summary>
        /// Animates the transition to a new question
        /// </summary>
        private IEnumerator AnimateQuestionTransition(int index, QuestionData question)
        {
            // Fade out current content
            if (questionPanel != null)
            {
                yield return StartCoroutine(FadeOutObject(questionPanel.GetComponent<CanvasGroup>()));
            }
            
            // Clear previous answer buttons
            ClearAnswerButtons();
            
            // Update UI with new question
            if (questionText != null)
            {
                questionText.text = question.questionText;
            }
            
            // Create answer buttons
            CreateAnswerButtons(question);
            
            // Update progress
            UpdateProgress(index, quizManager.GetTotalQuestions());
            
            // Start timer
            timeRemaining = timePerQuestion;
            if (timerCoroutine != null)
            {
                StopCoroutine(timerCoroutine);
            }
            timerCoroutine = StartCoroutine(UpdateTimer());
            
            // Fade in new content
            if (questionPanel != null)
            {
                yield return StartCoroutine(FadeInObject(questionPanel.GetComponent<CanvasGroup>()));
            }
        }
        
        /// <summary>
        /// Updates the progress display
        /// </summary>
        private void UpdateProgress(int currentQuestion, int totalQuestions)
        {
            if (progressSlider != null)
            {
                float progress = (float)currentQuestion / totalQuestions;
                progressSlider.maxValue = totalQuestions;
                progressSlider.value = currentQuestion + 1;
            }
            
            if (progressText != null)
            {
                progressText.text = $"{currentQuestion + 1}/{totalQuestions}";
            }
            
            if (scoreText != null)
            {
                scoreText.text = $"Score: {quizManager.GetCurrentScore()}";
            }
        }
        
        /// <summary>
        /// Creates answer buttons for the current question
        /// </summary>
        private void CreateAnswerButtons(QuestionData question)
        {
            // Shuffle the options to randomize order
            List<string> options = new List<string>(question.options);
            ShuffleList(options);
            
            for (int i = 0; i < options.Count; i++)
            {
                if (answerButtonPrefab != null && answerOptionsContainer != null)
                {
                    GameObject buttonObj = Instantiate(answerButtonPrefab, answerOptionsContainer);
                    AnswerButton answerButton = buttonObj.GetComponent<AnswerButton>();
                    
                    if (answerButton != null)
                    {
                        // Find the original index of this option
                        int originalIndex = question.options.IndexOf(options[i]);
                        bool isCorrect = originalIndex == question.correctAnswerIndex;
                        
                        answerButton.Initialize(options[i], originalIndex, isCorrect, OnAnswerSelected);
                    }
                    
                    answerButtons.Add(buttonObj);
                }
            }
        }
        
        /// <summary>
        /// Handles answer selection
        /// </summary>
        private void OnAnswerSelected(int answerIndex)
        {
            // Disable all buttons to prevent multiple selections
            foreach (GameObject buttonObj in answerButtons)
            {
                AnswerButton button = buttonObj.GetComponent<AnswerButton>();
                if (button != null)
                {
                    button.DisableButton();
                }
            }
            
            // Determine if answer is correct
            QuestionData currentQuestion = quizManager.GetCurrentQuestion();
            bool isCorrect = answerIndex == currentQuestion.correctAnswerIndex;
            
            // Show feedback
            ShowFeedback(isCorrect, currentQuestion.explanation);
            
            // Highlight correct/incorrect answers
            StartCoroutine(HighlightAnswers(answerIndex, currentQuestion.correctAnswerIndex));
        }
        
        /// <summary>
        /// Shows feedback to the user
        /// </summary>
        private void ShowFeedback(bool isCorrect, string explanation)
        {
            if (feedbackPanel != null)
            {
                feedbackPanel.SetActive(true);
            }
            
            if (feedbackText != null)
            {
                feedbackText.text = isCorrect ? $"Correct! {explanation}" : $"Incorrect. {explanation}";
                feedbackText.color = isCorrect ? correctFeedbackColor : incorrectFeedbackColor;
            }
            
            if (feedbackBackground != null)
            {
                feedbackBackground.color = isCorrect ? 
                    new Color(correctFeedbackColor.r, correctFeedbackColor.g, correctFeedbackColor.b, 0.3f) : 
                    new Color(incorrectFeedbackColor.r, incorrectFeedbackColor.g, incorrectFeedbackColor.b, 0.3f);
            }
            
            // Enable next button
            if (nextButton != null)
            {
                nextButton.interactable = true;
            }
            
            // Play audio feedback
            if (AudioFeedbackManager.Instance != null)
            {
                AudioFeedbackManager.Instance.PlayFeedback(
                    isCorrect ? AudioFeedbackType.CorrectAnswer : AudioFeedbackType.IncorrectAnswer);
            }
        }
        
        /// <summary>
        /// Highlights correct and incorrect answers
        /// </summary>
        private IEnumerator HighlightAnswers(int selectedAnswerIndex, int correctAnswerIndex)
        {
            yield return new WaitForSeconds(answerRevealDuration);
            
            foreach (GameObject buttonObj in answerButtons)
            {
                AnswerButton button = buttonObj.GetComponent<AnswerButton>();
                
                if (button != null)
                {
                    if (button.GetAnswerIndex() == correctAnswerIndex)
                    {
                        button.ShowCorrectAnswer();
                    }
                    else if (button.GetAnswerIndex() == selectedAnswerIndex && selectedAnswerIndex != correctAnswerIndex)
                    {
                        button.ShowIncorrectAnswer();
                    }
                }
            }
        }
        
        /// <summary>
        /// Moves to the next question
        /// </summary>
        private void NextQuestion()
        {
            if (nextButton != null)
            {
                nextButton.interactable = false;
            }
            
            if (feedbackPanel != null)
            {
                feedbackPanel.SetActive(false);
            }
            
            quizManager.NextQuestion();
        }
        
        /// <summary>
        /// Skips the current question
        /// </summary>
        private void SkipQuestion()
        {
            if (feedbackPanel != null)
            {
                feedbackPanel.SetActive(false);
            }
            
            quizManager.SkipQuestion();
        }
        
        /// <summary>
        /// Updates the question timer
        /// </summary>
        private IEnumerator UpdateTimer()
        {
            while (timeRemaining > 0)
            {
                if (timerText != null)
                {
                    timerText.text = Mathf.CeilToInt(timeRemaining).ToString();
                }
                
                timeRemaining -= Time.deltaTime;
                
                if (timeRemaining <= 0)
                {
                    // Time's up - handle as unanswered
                    OnAnswerSelected(-1); // -1 indicates no answer selected
                    break;
                }
                
                yield return null;
            }
        }
        
        /// <summary>
        /// Called when the quiz is completed
        /// </summary>
        private void OnQuizCompleted(int finalScore)
        {
            // Show final score or navigate to results screen
            if (timerCoroutine != null)
            {
                StopCoroutine(timerCoroutine);
            }
            
            if (AudioFeedbackManager.Instance != null)
            {
                AudioFeedbackManager.Instance.PlayFeedback(AudioFeedbackType.QuizCompleted);
            }
            
            // Add achievement for completing quiz
            if (AchievementManager.Instance != null)
            {
                AchievementManager.Instance.CheckAchievement("completion", 1);
            }
        }
        
        /// <summary>
        /// Clears all answer buttons
        /// </summary>
        private void ClearAnswerButtons()
        {
            foreach (GameObject buttonObj in answerButtons)
            {
                if (buttonObj != null)
                {
                    Destroy(buttonObj);
                }
            }
            answerButtons.Clear();
        }
        
        /// <summary>
        /// Fades out an object using CanvasGroup
        /// </summary>
        private IEnumerator FadeOutObject(CanvasGroup canvasGroup)
        {
            if (canvasGroup == null) yield break;
            
            float startAlpha = canvasGroup.alpha;
            float elapsedTime = 0f;
            
            while (elapsedTime < panelTransitionDuration)
            {
                elapsedTime += Time.deltaTime;
                float progress = elapsedTime / panelTransitionDuration;
                float curvedProgress = transitionCurve.Evaluate(progress);
                
                canvasGroup.alpha = Mathf.Lerp(startAlpha, 0, curvedProgress);
                
                yield return null;
            }
            
            canvasGroup.alpha = 0;
        }
        
        /// <summary>
        /// Fades in an object using CanvasGroup
        /// </summary>
        private IEnumerator FadeInObject(CanvasGroup canvasGroup)
        {
            if (canvasGroup == null) yield break;
            
            float startAlpha = canvasGroup.alpha;
            float elapsedTime = 0f;
            
            while (elapsedTime < panelTransitionDuration)
            {
                elapsedTime += Time.deltaTime;
                float progress = elapsedTime / panelTransitionDuration;
                float curvedProgress = transitionCurve.Evaluate(progress);
                
                canvasGroup.alpha = Mathf.Lerp(startAlpha, 1, curvedProgress);
                
                yield return null;
            }
            
            canvasGroup.alpha = 1;
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
        
        private void OnDestroy()
        {
            if (quizManager != null)
            {
                quizManager.OnQuestionLoaded -= OnQuestionLoaded;
                quizManager.OnQuizCompleted -= OnQuizCompleted;
            }
        }
    }
}