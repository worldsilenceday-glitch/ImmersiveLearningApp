using UnityEngine;
using UnityEngine.UI;
using TMPro;

namespace ImmersiveLearningApp
{
    /// <summary>
    /// Script for answer buttons in quiz system
    /// </summary>
    public class AnswerButton : MonoBehaviour
    {
        [Header("UI References")]
        [SerializeField] private Button button;
        [SerializeField] private TextMeshProUGUI answerText;
        [SerializeField] private Image background;
        
        [Header("Visual Settings")]
        [SerializeField] private Color defaultColor = Color.white;
        [SerializeField] private Color correctColor = Color.green;
        [SerializeField] private Color incorrectColor = Color.red;
        [SerializeField] private Color selectedColor = Color.yellow;
        
        private int answerIndex;
        private bool isCorrect;
        private bool isAnswered = false;
        private System.Action<int> onAnswerSelected;
        
        /// <summary>
        /// Initializes the answer button with text and callback
        /// </summary>
        public void Initialize(string text, int index, bool correct, System.Action<int> callback)
        {
            if (answerText != null)
            {
                answerText.text = text;
            }
            
            answerIndex = index;
            isCorrect = correct;
            onAnswerSelected = callback;
            isAnswered = false;
            
            // Set default color
            if (background != null)
            {
                background.color = defaultColor;
            }
            
            // Set up button click
            if (button != null)
            {
                button.onClick.RemoveAllListeners();
                button.onClick.AddListener(OnButtonClicked);
            }
        }
        
        /// <summary>
        /// Handles button click event
        /// </summary>
        private void OnButtonClicked()
        {
            if (isAnswered || onAnswerSelected == null) return;
            
            onAnswerSelected(answerIndex);
            isAnswered = true;
            
            // Visual feedback
            if (background != null)
            {
                background.color = selectedColor;
            }
        }
        
        /// <summary>
        /// Shows the correct answer state
        /// </summary>
        public void ShowCorrectAnswer()
        {
            if (background != null)
            {
                background.color = correctColor;
            }
        }
        
        /// <summary>
        /// Shows the incorrect answer state
        /// </summary>
        public void ShowIncorrectAnswer()
        {
            if (background != null)
            {
                background.color = incorrectColor;
            }
        }
        
        /// <summary>
        /// Resets the button to default state
        /// </summary>
        public void ResetButton()
        {
            isAnswered = false;
            if (background != null)
            {
                background.color = defaultColor;
            }
        }
        
        /// <summary>
        /// Disables the button interaction
        /// </summary>
        public void DisableButton()
        {
            if (button != null)
            {
                button.interactable = false;
            }
        }
        
        /// <summary>
        /// Enables the button interaction
        /// </summary>
        public void EnableButton()
        {
            if (button != null)
            {
                button.interactable = true;
            }
        }
        
        /// <summary>
        /// Gets whether this is the correct answer
        /// </summary>
        public bool IsCorrectAnswer()
        {
            return isCorrect;
        }
        
        /// <summary>
        /// Gets the answer index
        /// </summary>
        public int GetAnswerIndex()
        {
            return answerIndex;
        }
    }
}