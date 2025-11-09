using UnityEngine;
using UnityEngine.SceneManagement;

namespace ImmersiveLearningApp
{
    /// <summary>
    /// Controls the main learning hub scene with navigation
    /// </summary>
    public class LearningHubManager : MonoBehaviour
    {
        [Header("Scene Names")]
        [SerializeField] private string mainMenuScene = "MainMenu";
        [SerializeField] private string quizScene = "QuizScene";
        [SerializeField] private string modelViewerScene = "ModelViewer";
        [SerializeField] private string leaderboardScene = "Leaderboard";
        
        /// <summary>
        /// Loads the main menu scene
        /// </summary>
        public void LoadMainMenu()
        {
            if (AudioFeedbackManager.Instance != null)
            {
                AudioFeedbackManager.Instance.PlayFeedback(AudioFeedbackType.ButtonClick);
            }
            
            SceneManager.LoadScene(mainMenuScene);
        }
        
        /// <summary>
        /// Loads the quiz scene
        /// </summary>
        public void LoadQuizScene()
        {
            if (AudioFeedbackManager.Instance != null)
            {
                AudioFeedbackManager.Instance.PlayFeedback(AudioFeedbackType.ButtonClick);
            }
            
            SceneManager.LoadScene(quizScene);
        }
        
        /// <summary>
        /// Loads the model viewer scene
        /// </summary>
        public void LoadModelViewer()
        {
            if (AudioFeedbackManager.Instance != null)
            {
                AudioFeedbackManager.Instance.PlayFeedback(AudioFeedbackType.ButtonClick);
            }
            
            SceneManager.LoadScene(modelViewerScene);
        }
        
        /// <summary>
        /// Loads the leaderboard scene
        /// </summary>
        public void LoadLeaderboard()
        {
            if (AudioFeedbackManager.Instance != null)
            {
                AudioFeedbackManager.Instance.PlayFeedback(AudioFeedbackType.ButtonClick);
            }
            
            SceneManager.LoadScene(leaderboardScene);
        }
        
        /// <summary>
        /// Quits the application
        /// </summary>
        public void QuitApplication()
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
    }
}