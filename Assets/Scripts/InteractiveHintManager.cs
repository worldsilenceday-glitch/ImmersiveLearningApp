using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections;

namespace ImmersiveLearningApp
{
    /// <summary>
    /// Manages interactive hints and info popups for 3D models
    /// </summary>
    public class InteractiveHintManager : MonoBehaviour
    {
        [Header("UI References")]
        [SerializeField] private GameObject hintPanel;
        [SerializeField] private TextMeshProUGUI hintText;
        [SerializeField] private Button hintCloseButton;
        [SerializeField] private Button hintNextButton;
        [SerializeField] private Button hintPrevButton;
        
        [Header("Model Interaction")]
        [SerializeField] private float hintDisplayDistance = 5.0f;
        [SerializeField] private LayerMask hintLayerMask = -1;
        
        private InteractiveHint currentHint;
        private int currentHintIndex = 0;
        private bool isHintActive = false;
        
        private void Start()
        {
            InitializeUI();
            CloseHintPanel();
        }
        
        /// <summary>
        /// Initializes UI components
        /// </summary>
        private void InitializeUI()
        {
            if (hintCloseButton != null)
            {
                hintCloseButton.onClick.AddListener(CloseHintPanel);
            }
            
            if (hintNextButton != null)
            {
                hintNextButton.onClick.AddListener(NextHint);
            }
            
            if (hintPrevButton != null)
            {
                hintPrevButton.onClick.AddListener(PreviousHint);
            }
        }
        
        /// <summary>
        /// Checks for interactive hints when the player looks at objects
        /// </summary>
        private void Update()
        {
            if (Input.GetMouseButtonDown(0))
            {
                CheckForHintInteraction();
            }
        }
        
        /// <summary>
        /// Checks if the player clicked on an interactive hint object
        /// </summary>
        private void CheckForHintInteraction()
        {
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            RaycastHit hit;
            
            if (Physics.Raycast(ray, out hit, hintDisplayDistance, hintLayerMask))
            {
                InteractiveHint hintComponent = hit.collider.GetComponent<InteractiveHint>();
                
                if (hintComponent != null)
                {
                    ShowHint(hintComponent);
                }
            }
        }
        
        /// <summary>
        /// Shows the hint panel for the specified interactive hint
        /// </summary>
        private void ShowHint(InteractiveHint hint)
        {
            currentHint = hint;
            currentHintIndex = 0;
            
            if (hintPanel != null)
            {
                hintPanel.SetActive(true);
            }
            
            isHintActive = true;
            UpdateHintDisplay();
            
            if (AudioFeedbackManager.Instance != null)
            {
                AudioFeedbackManager.Instance.PlayFeedback(AudioFeedbackType.ModelInteraction);
            }
        }
        
        /// <summary>
        /// Updates the hint display with current hint data
        /// </summary>
        private void UpdateHintDisplay()
        {
            if (currentHint == null || currentHint.hints.Count == 0) return;
            
            // Clamp index to valid range
            currentHintIndex = Mathf.Clamp(currentHintIndex, 0, currentHint.hints.Count - 1);
            
            if (hintText != null)
            {
                hintText.text = currentHint.hints[currentHintIndex];
            }
            
            // Update navigation button states
            if (hintNextButton != null)
            {
                hintNextButton.interactable = currentHintIndex < currentHint.hints.Count - 1;
            }
            
            if (hintPrevButton != null)
            {
                hintPrevButton.interactable = currentHintIndex > 0;
            }
        }
        
        /// <summary>
        /// Moves to the next hint in the sequence
        /// </summary>
        private void NextHint()
        {
            if (currentHint != null && currentHint.hints.Count > 0)
            {
                currentHintIndex++;
                
                if (currentHintIndex >= currentHint.hints.Count)
                {
                    currentHintIndex = currentHint.hints.Count - 1;
                }
                
                UpdateHintDisplay();
            }
        }
        
        /// <summary>
        /// Moves to the previous hint in the sequence
        /// </summary>
        private void PreviousHint()
        {
            if (currentHint != null)
            {
                currentHintIndex--;
                
                if (currentHintIndex < 0)
                {
                    currentHintIndex = 0;
                }
                
                UpdateHintDisplay();
            }
        }
        
        /// <summary>
        /// Closes the hint panel
        /// </summary>
        private void CloseHintPanel()
        {
            if (hintPanel != null)
            {
                hintPanel.SetActive(false);
            }
            
            isHintActive = false;
            currentHint = null;
        }
        
        /// <summary>
        /// Checks if a hint panel is currently active
        /// </summary>
        public bool IsHintActive()
        {
            return isHintActive;
        }
        
        /// <summary>
        /// Shows a simple tooltip hint at the mouse position
        /// </summary>
        public void ShowSimpleHint(string hintMessage, Vector3 worldPosition)
        {
            if (hintPanel != null)
            {
                hintPanel.SetActive(true);
            }
            
            if (hintText != null)
            {
                hintText.text = hintMessage;
            }
            
            isHintActive = true;
            
            // Position the hint near the object (this is a simplified implementation)
            if (Camera.main != null)
            {
                Vector3 screenPos = Camera.main.WorldToScreenPoint(worldPosition);
                hintPanel.transform.position = screenPos;
            }
        }
    }
}