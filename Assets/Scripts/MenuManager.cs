using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using System.Collections;

namespace ImmersiveLearningApp
{
    /// <summary>
    /// Manages menu UI with animated transitions
    /// </summary>
    public class MenuManager : MonoBehaviour
    {
        [Header("UI Panels")]
        [SerializeField] private GameObject mainMenuPanel;
        [SerializeField] private GameObject settingsPanel;
        [SerializeField] private GameObject topicSelectionPanel;
        [SerializeField] private GameObject difficultySelectionPanel;
        
        [Header("Animation Settings")]
        [SerializeField] private float fadeDuration = 0.5f;
        [SerializeField] private AnimationCurve fadeCurve = AnimationCurve.EaseInOut(0, 0, 1, 1);
        
        [Header("Audio Feedback")]
        [SerializeField] private bool enableAudioFeedback = true;
        
        private CanvasGroup mainMenuCanvasGroup;
        private CanvasGroup settingsCanvasGroup;
        private CanvasGroup topicSelectionCanvasGroup;
        private CanvasGroup difficultySelectionCanvasGroup;
        
        private void Start()
        {
            InitializeCanvasGroups();
            ShowMainMenu();
        }
        
        /// <summary>
        /// Initializes canvas groups for animation
        /// </summary>
        private void InitializeCanvasGroups()
        {
            if (mainMenuPanel != null)
            {
                mainMenuCanvasGroup = mainMenuPanel.GetComponent<CanvasGroup>();
                if (mainMenuCanvasGroup == null)
                    mainMenuCanvasGroup = mainMenuPanel.AddComponent<CanvasGroup>();
            }
            
            if (settingsPanel != null)
            {
                settingsCanvasGroup = settingsPanel.GetComponent<CanvasGroup>();
                if (settingsCanvasGroup == null)
                    settingsCanvasGroup = settingsPanel.AddComponent<CanvasGroup>();
            }
            
            if (topicSelectionPanel != null)
            {
                topicSelectionCanvasGroup = topicSelectionPanel.GetComponent<CanvasGroup>();
                if (topicSelectionCanvasGroup == null)
                    topicSelectionCanvasGroup = topicSelectionPanel.AddComponent<CanvasGroup>();
            }
            
            if (difficultySelectionPanel != null)
            {
                difficultySelectionCanvasGroup = difficultySelectionPanel.GetComponent<CanvasGroup>();
                if (difficultySelectionCanvasGroup == null)
                    difficultySelectionCanvasGroup = difficultySelectionPanel.AddComponent<CanvasGroup>();
            }
            
            // Initially hide all panels except main menu
            SetCanvasGroupAlpha(mainMenuCanvasGroup, 1);
            SetCanvasGroupAlpha(settingsCanvasGroup, 0);
            SetCanvasGroupAlpha(topicSelectionCanvasGroup, 0);
            SetCanvasGroupAlpha(difficultySelectionCanvasGroup, 0);
            
            // Set all panels inactive initially
            if (settingsPanel != null) settingsPanel.SetActive(true);
            if (topicSelectionPanel != null) topicSelectionPanel.SetActive(true);
            if (difficultySelectionPanel != null) difficultySelectionPanel.SetActive(true);
            
            // Main menu should start active
            if (mainMenuPanel != null) mainMenuPanel.SetActive(true);
        }
        
        /// <summary>
        /// Sets canvas group alpha with immediate effect
        /// </summary>
        private void SetCanvasGroupAlpha(CanvasGroup canvasGroup, float alpha)
        {
            if (canvasGroup != null)
            {
                canvasGroup.alpha = alpha;
                canvasGroup.interactable = alpha > 0;
                canvasGroup.blocksRaycasts = alpha > 0;
            }
        }
        
        /// <summary>
        /// Animates canvas group alpha with fade effect
        /// </summary>
        private IEnumerator AnimateCanvasGroupAlpha(CanvasGroup canvasGroup, float targetAlpha)
        {
            if (canvasGroup == null) yield break;
            
            float startAlpha = canvasGroup.alpha;
            float elapsedTime = 0f;
            
            while (elapsedTime < fadeDuration)
            {
                elapsedTime += Time.deltaTime;
                float progress = elapsedTime / fadeDuration;
                float curvedProgress = fadeCurve.Evaluate(progress);
                
                canvasGroup.alpha = Mathf.Lerp(startAlpha, targetAlpha, curvedProgress);
                canvasGroup.interactable = targetAlpha > 0;
                canvasGroup.blocksRaycasts = targetAlpha > 0;
                
                yield return null;
            }
            
            canvasGroup.alpha = targetAlpha;
            canvasGroup.interactable = targetAlpha > 0;
            canvasGroup.blocksRaycasts = targetAlpha > 0;
        }
        
        /// <summary>
        /// Shows the main menu with animation
        /// </summary>
        public void ShowMainMenu()
        {
            if (enableAudioFeedback && AudioFeedbackManager.Instance != null)
            {
                AudioFeedbackManager.Instance.PlayFeedback(AudioFeedbackType.ButtonClick);
            }
            
            StartCoroutine(SwitchToPanel(mainMenuCanvasGroup, 
                new CanvasGroup[] { settingsCanvasGroup, topicSelectionCanvasGroup, difficultySelectionCanvasGroup }));
        }
        
        /// <summary>
        /// Shows the settings panel with animation
        /// </summary>
        public void ShowSettings()
        {
            if (enableAudioFeedback && AudioFeedbackManager.Instance != null)
            {
                AudioFeedbackManager.Instance.PlayFeedback(AudioFeedbackType.ButtonClick);
            }
            
            StartCoroutine(SwitchToPanel(settingsCanvasGroup, 
                new CanvasGroup[] { mainMenuCanvasGroup, topicSelectionCanvasGroup, difficultySelectionCanvasGroup }));
        }
        
        /// <summary>
        /// Shows the topic selection panel with animation
        /// </summary>
        public void ShowTopicSelection()
        {
            if (enableAudioFeedback && AudioFeedbackManager.Instance != null)
            {
                AudioFeedbackManager.Instance.PlayFeedback(AudioFeedbackType.ButtonClick);
            }
            
            StartCoroutine(SwitchToPanel(topicSelectionCanvasGroup, 
                new CanvasGroup[] { mainMenuCanvasGroup, settingsCanvasGroup, difficultySelectionCanvasGroup }));
        }
        
        /// <summary>
        /// Shows the difficulty selection panel with animation
        /// </summary>
        public void ShowDifficultySelection()
        {
            if (enableAudioFeedback && AudioFeedbackManager.Instance != null)
            {
                AudioFeedbackManager.Instance.PlayFeedback(AudioFeedbackType.ButtonClick);
            }
            
            StartCoroutine(SwitchToPanel(difficultySelectionCanvasGroup, 
                new CanvasGroup[] { mainMenuCanvasGroup, settingsCanvasGroup, topicSelectionCanvasGroup }));
        }
        
        /// <summary>
        /// Switches between UI panels with fade animation
        /// </summary>
        private IEnumerator SwitchToPanel(CanvasGroup activePanel, CanvasGroup[] inactivePanels)
        {
            // Fade out inactive panels
            foreach (CanvasGroup panel in inactivePanels)
            {
                if (panel != null)
                {
                    StartCoroutine(AnimateCanvasGroupAlpha(panel, 0));
                }
            }
            
            // Wait for fade duration before fading in the active panel
            yield return new WaitForSeconds(fadeDuration);
            
            // Hide inactive panels immediately
            foreach (CanvasGroup panel in inactivePanels)
            {
                if (panel != null && panel.gameObject != null)
                {
                    panel.gameObject.SetActive(false);
                }
            }
            
            // Show active panel if not already active
            if (activePanel != null && activePanel.gameObject != null)
            {
                activePanel.gameObject.SetActive(true);
            }
            
            // Fade in the active panel
            if (activePanel != null)
            {
                StartCoroutine(AnimateCanvasGroupAlpha(activePanel, 1));
            }
        }
        
        /// <summary>
        /// Starts the learning experience based on selected topics and difficulty
        /// </summary>
        public void StartLearning()
        {
            if (enableAudioFeedback && AudioFeedbackManager.Instance != null)
            {
                AudioFeedbackManager.Instance.PlayFeedback(AudioFeedbackType.ButtonClick);
            }
            
            // This would typically load the learning hub scene or quiz scene based on user selections
            SceneManager.LoadScene("LearningHub");
        }
        
        /// <summary>
        /// Quits the application
        /// </summary>
        public void QuitApplication()
        {
            if (enableAudioFeedback && AudioFeedbackManager.Instance != null)
            {
                AudioFeedbackManager.Instance.PlayFeedback(AudioFeedbackType.ButtonClick);
            }
            
            #if UNITY_EDITOR
            UnityEditor.EditorApplication.isPlaying = false;
            #else
            Application.Quit();
            #endif
        }
    }
}