using UnityEngine;

namespace ImmersiveLearningApp
{
    /// <summary>
    /// Enum for different audio feedback types
    /// </summary>
    public enum AudioFeedbackType
    {
        CorrectAnswer,
        IncorrectAnswer,
        ButtonClick,
        QuizCompleted,
        ModelInteraction
    }
    
    /// <summary>
    /// Manages audio feedback for the application
    /// </summary>
    public class AudioFeedbackManager : MonoBehaviour
    {
        [Header("Audio Clips")]
        [SerializeField] private AudioClip correctAnswerClip;
        [SerializeField] private AudioClip incorrectAnswerClip;
        [SerializeField] private AudioClip buttonClickClip;
        [SerializeField] private AudioClip quizCompletedClip;
        [SerializeField] private AudioClip modelInteractionClip;
        
        [Header("Audio Settings")]
        [SerializeField] private AudioSource audioSource;
        [SerializeField] private float volume = 1.0f;
        
        private static AudioFeedbackManager instance;
        
        public static AudioFeedbackManager Instance
        {
            get
            {
                if (instance == null)
                {
                    GameObject managerObject = new GameObject("AudioFeedbackManager");
                    instance = managerObject.AddComponent<AudioFeedbackManager>();
                }
                return instance;
            }
        }
        
        private void Awake()
        {
            if (instance != null && instance != this)
            {
                Destroy(gameObject);
                return;
            }
            
            instance = this;
            DontDestroyOnLoad(gameObject);
            
            // Create audio source if not assigned
            if (audioSource == null)
            {
                audioSource = gameObject.AddComponent<AudioSource>();
            }
        }
        
        /// <summary>
        /// Plays audio feedback based on the type
        /// </summary>
        public void PlayFeedback(AudioFeedbackType feedbackType)
        {
            AudioClip clip = GetAudioClipForType(feedbackType);
            
            if (clip != null && audioSource != null)
            {
                audioSource.PlayOneShot(clip, volume);
            }
            else
            {
                Debug.LogWarning($"No audio clip assigned for feedback type: {feedbackType}");
            }
        }
        
        /// <summary>
        /// Gets the appropriate audio clip for the specified feedback type
        /// </summary>
        private AudioClip GetAudioClipForType(AudioFeedbackType feedbackType)
        {
            switch (feedbackType)
            {
                case AudioFeedbackType.CorrectAnswer:
                    return correctAnswerClip;
                case AudioFeedbackType.IncorrectAnswer:
                    return incorrectAnswerClip;
                case AudioFeedbackType.ButtonClick:
                    return buttonClickClip;
                case AudioFeedbackType.QuizCompleted:
                    return quizCompletedClip;
                case AudioFeedbackType.ModelInteraction:
                    return modelInteractionClip;
                default:
                    return null;
            }
        }
        
        /// <summary>
        /// Plays a specific audio clip with volume control
        /// </summary>
        public void PlayAudioClip(AudioClip clip, float volumeScale = 1.0f)
        {
            if (clip != null && audioSource != null)
            {
                audioSource.PlayOneShot(clip, volume * volumeScale);
            }
        }
        
        /// <summary>
        /// Sets the global volume for all audio feedback
        /// </summary>
        public void SetVolume(float newVolume)
        {
            volume = Mathf.Clamp01(newVolume);
            if (audioSource != null)
            {
                audioSource.volume = volume;
            }
        }
    }
}