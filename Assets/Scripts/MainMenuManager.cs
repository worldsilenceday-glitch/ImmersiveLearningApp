using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using TMPro;

namespace ImmersiveLearningApp
{
    /// <summary>
    /// Enum for difficulty levels
    /// </summary>
    public enum DifficultyLevel
    {
        Easy,
        Medium,
        Hard
    }
    
    /// <summary>
    /// Controls the main menu UI and scene navigation
    /// </summary>
    public class MainMenuManager : MonoBehaviour
    {
        [Header("UI References")]
        [SerializeField] private GameObject mainMenuPanel;
        [SerializeField] private GameObject settingsPanel;
        [SerializeField] private GameObject playerNameInput;
        [SerializeField] private TMP_InputField playerNameField;
        [SerializeField] private Button startButton;
        [SerializeField] private Button settingsButton;
        [SerializeField] private Button quitButton;
        [SerializeField] private Toggle easyToggle;
        [SerializeField] private Toggle mediumToggle;
        [SerializeField] private Toggle hardToggle;
        
        [Header("Audio Settings")]
        [SerializeField] private Slider volumeSlider;
        [SerializeField] private Toggle musicToggle;
        [SerializeField] private Toggle sfxToggle;
        
        private DifficultyLevel currentDifficulty = DifficultyLevel.Medium;
        
        private void Start()
        {
            InitializeUI();
            LoadPlayerPreferences();
        }
        
        /// <summary>
        /// Initializes UI components
        /// </summary>
        private void InitializeUI()
        {
            // Set up difficulty toggles
            if (easyToggle != null)
            {
                easyToggle.onValueChanged.AddListener((isOn) => { if (isOn) SetDifficulty(DifficultyLevel.Easy); });
            }
            
            if (mediumToggle != null)
            {
                mediumToggle.onValueChanged.AddListener((isOn) => { if (isOn) SetDifficulty(DifficultyLevel.Medium); });
            }
            
            if (hardToggle != null)
            {
                hardToggle.onValueChanged.AddListener((isOn) => { if (isOn) SetDifficulty(DifficultyLevel.Hard); });
            }
            
            // Set up volume slider
            if (volumeSlider != null)
            {
                volumeSlider.onValueChanged.AddListener(SetVolume);
            }
            
            // Set up audio toggles
            if (musicToggle != null)
            {
                musicToggle.onValueChanged.AddListener(SetMusicEnabled);
            }
            
            if (sfxToggle != null)
            {
                sfxToggle.onValueChanged.AddListener(SetSfxEnabled);
            }
            
            // Set up buttons
            if (startButton != null)
            {
                startButton.onClick.AddListener(StartGame);
            }
            
            if (settingsButton != null)
            {
                settingsButton.onClick.AddListener(ToggleSettings);
            }
            
            if (quitButton != null)
            {
                quitButton.onClick.AddListener(QuitGame);
            }
            
            // Set default difficulty
            mediumToggle.isOn = true;
        }
        
        /// <summary>
        /// Loads player preferences from PlayerPrefs
        /// </summary>
        private void LoadPlayerPreferences()
        {
            // Load player name
            if (playerNameField != null)
            {
                string savedName = PlayerPrefs.GetString("PlayerName", "Player");
                playerNameField.text = savedName;
                
                // Update progress manager with saved name
                if (ProgressManager.Instance != null)
                {
                    var progress = ProgressManager.Instance.GetPlayerProgress();
                    progress.playerName = savedName;
                }
            }
            
            // Load difficulty
            int difficultyInt = PlayerPrefs.GetInt("Difficulty", (int)DifficultyLevel.Medium);
            currentDifficulty = (DifficultyLevel)difficultyInt;
            SetDifficultyUI();
            
            // Load volume
            float volume = PlayerPrefs.GetFloat("Volume", 1.0f);
            if (volumeSlider != null)
            {
                volumeSlider.value = volume;
                SetVolume(volume);
            }
            
            // Load audio settings
            bool musicEnabled = PlayerPrefs.GetInt("MusicEnabled", 1) == 1;
            bool sfxEnabled = PlayerPrefs.GetInt("SfxEnabled", 1) == 1;
            
            if (musicToggle != null)
            {
                musicToggle.isOn = musicEnabled;
                SetMusicEnabled(musicEnabled);
            }
            
            if (sfxToggle != null)
            {
                sfxToggle.isOn = sfxEnabled;
                SetSfxEnabled(sfxEnabled);
            }
        }
        
        /// <summary>
        /// Sets the difficulty level
        /// </summary>
        private void SetDifficulty(DifficultyLevel difficulty)
        {
            currentDifficulty = difficulty;
            PlayerPrefs.SetInt("Difficulty", (int)currentDifficulty);
            PlayerPrefs.Save();
            
            Debug.Log("Difficulty set to: " + currentDifficulty);
        }
        
        /// <summary>
        /// Sets the UI toggles based on current difficulty
        /// </summary>
        private void SetDifficultyUI()
        {
            if (easyToggle != null) easyToggle.isOn = (currentDifficulty == DifficultyLevel.Easy);
            if (mediumToggle != null) mediumToggle.isOn = (currentDifficulty == DifficultyLevel.Medium);
            if (hardToggle != null) hardToggle.isOn = (currentDifficulty == DifficultyLevel.Hard);
        }
        
        /// <summary>
        /// Sets the master volume level
        /// </summary>
        private void SetVolume(float volume)
        {
            PlayerPrefs.SetFloat("Volume", volume);
            PlayerPrefs.Save();
            
            if (AudioFeedbackManager.Instance != null)
            {
                AudioFeedbackManager.Instance.SetVolume(volume);
            }
            
            Debug.Log("Volume set to: " + volume);
        }
        
        /// <summary>
        /// Sets whether background music is enabled
        /// </summary>
        private void SetMusicEnabled(bool enabled)
        {
            PlayerPrefs.SetInt("MusicEnabled", enabled ? 1 : 0);
            PlayerPrefs.Save();
            
            Debug.Log("Music enabled: " + enabled);
        }
        
        /// <summary>
        /// Sets whether sound effects are enabled
        /// </summary>
        private void SetSfxEnabled(bool enabled)
        {
            PlayerPrefs.SetInt("SfxEnabled", enabled ? 1 : 0);
            PlayerPrefs.Save();
            
            Debug.Log("SFX enabled: " + enabled);
        }
        
        /// <summary>
        /// Starts the game by loading the main scene
        /// </summary>
        private void StartGame()
        {
            // Save player name
            if (playerNameField != null && !string.IsNullOrEmpty(playerNameField.text))
            {
                string playerName = playerNameField.text;
                PlayerPrefs.SetString("PlayerName", playerName);
                
                // Update progress manager
                if (ProgressManager.Instance != null)
                {
                    var progress = ProgressManager.Instance.GetPlayerProgress();
                    progress.playerName = playerName;
                    ProgressManager.Instance.SaveProgress();
                }
            }
            
            // Play button click sound
            if (AudioFeedbackManager.Instance != null)
            {
                AudioFeedbackManager.Instance.PlayFeedback(AudioFeedbackType.ButtonClick);
            }
            
            // Load the main learning scene
            SceneManager.LoadScene("LearningHub");
        }
        
        /// <summary>
        /// Toggles the settings panel
        /// </summary>
        private void ToggleSettings()
        {
            if (mainMenuPanel != null && settingsPanel != null)
            {
                mainMenuPanel.SetActive(!mainMenuPanel.activeSelf);
                settingsPanel.SetActive(!settingsPanel.activeSelf);
                
                if (AudioFeedbackManager.Instance != null)
                {
                    AudioFeedbackManager.Instance.PlayFeedback(AudioFeedbackType.ButtonClick);
                }
            }
        }
        
        /// <summary>
        /// Quits the application
        /// </summary>
        private void QuitGame()
        {
            if (AudioFeedbackManager.Instance != null)
            {
                AudioFeedbackManager.Instance.PlayFeedback(AudioFeedbackType.ButtonClick);
            }
            
            #if UNITY_EDITOR
            UnityEditor.EditorApplication.isPlaying = false;
            #else
            Application.Quit();
            #endif
        }
        
        /// <summary>
        /// Returns to main menu from settings
        /// </summary>
        public void ReturnToMainMenu()
        {
            if (mainMenuPanel != null && settingsPanel != null)
            {
                settingsPanel.SetActive(false);
                mainMenuPanel.SetActive(true);
                
                if (AudioFeedbackManager.Instance != null)
                {
                    AudioFeedbackManager.Instance.PlayFeedback(AudioFeedbackType.ButtonClick);
                }
            }
        }
        
        /// <summary>
        /// Opens leaderboard scene
        /// </summary>
        public void OpenLeaderboard()
        {
            if (AudioFeedbackManager.Instance != null)
            {
                AudioFeedbackManager.Instance.PlayFeedback(AudioFeedbackType.ButtonClick);
            }
            
            SceneManager.LoadScene("Leaderboard");
        }
        
        /// <summary>
        /// Opens topics selection scene
        /// </summary>
        public void OpenTopicsSelection()
        {
            if (AudioFeedbackManager.Instance != null)
            {
                AudioFeedbackManager.Instance.PlayFeedback(AudioFeedbackType.ButtonClick);
            }
            
            SceneManager.LoadScene("TopicsSelection");
        }
    }
}