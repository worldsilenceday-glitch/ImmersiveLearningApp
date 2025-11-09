using UnityEngine;
using UnityEngine.UI;
using TMPro;

namespace ImmersiveLearningApp
{
    /// <summary>
    /// Represents a single option button in a quiz
    /// </summary>
    public class OptionButton : MonoBehaviour
    {
        [Header("UI References")]
        [SerializeField] private Button button;
        [SerializeField] private TextMeshProUGUI optionText;
        [SerializeField] private Image background;
        
        public int originalIndex { get; private set; }
        private System.Action<int> onSelectedCallback;
        
        private Color defaultColor;
        private Color correctColor = Color.green;
        private Color incorrectColor = Color.red;
        
        private void Start()
        {
            if (background != null)
            {
                defaultColor = background.color;
            }
        }
        
        /// <summary>
        /// Initializes the option button with text and callback
        /// </summary>
        public void Initialize(string text, int index, System.Action<int> callback)
        {
            if (optionText != null)
            {
                optionText.text = text;
            }
            
            originalIndex = index;
            onSelectedCallback = callback;
            
            if (button != null)
            {
                button.onClick.AddListener(OnButtonClicked);
            }
        }
        
        /// <summary>
        /// Handles button click
        /// </summary>
        private void OnButtonClicked()
        {
            if (onSelectedCallback != null)
            {
                onSelectedCallback(originalIndex);
            }
        }
        
        /// <summary>
        /// Disables the button
        /// </summary>
        public void Disable()
        {
            if (button != null)
            {
                button.interactable = false;
            }
        }
        
        /// <summary>
        /// Sets the button color to indicate correct answer
        /// </summary>
        public void SetCorrectAnswer()
        {
            if (background != null)
            {
                background.color = correctColor;
            }
        }
        
        /// <summary>
        /// Sets the button color to indicate incorrect answer
        /// </summary>
        public void SetIncorrectAnswer()
        {
            if (background != null)
            {
                background.color = incorrectColor;
            }
        }
        
        /// <summary>
        /// Resets the button to default state
        /// </summary>
        public void ResetState()
        {
            if (background != null)
            {
                background.color = defaultColor;
            }
            
            if (button != null)
            {
                button.interactable = true;
            }
        }
    }
}