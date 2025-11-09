using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

namespace ImmersiveLearningApp
{
    /// <summary>
    /// Controls the leaderboard UI display
    /// </summary>
    public class LeaderboardUI : MonoBehaviour
    {
        [Header("UI References")]
        [SerializeField] private Transform leaderboardContainer;
        [SerializeField] private GameObject leaderboardEntryPrefab;
        
        [Header("UI Elements")]
        [SerializeField] private TextMeshProUGUI playerNameText;
        
        private List<GameObject> entryObjects = new List<GameObject>();
        
        private void Start()
        {
            RefreshLeaderboard();
            
            // Set the current player name
            if (ProgressManager.Instance != null)
            {
                var progress = ProgressManager.Instance.GetPlayerProgress();
                playerNameText.text = "Player: " + progress.playerName;
            }
        }
        
        /// <summary>
        /// Refreshes the leaderboard display with current data
        /// </summary>
        public void RefreshLeaderboard()
        {
            // Clear existing entries
            foreach (GameObject obj in entryObjects)
            {
                Destroy(obj);
            }
            entryObjects.Clear();
            
            // Get the leaderboard entries
            List<LeaderboardEntry> entries = LeaderboardManager.Instance.GetLeaderboardEntries();
            
            // Create UI for each entry
            for (int i = 0; i < entries.Count; i++)
            {
                if (leaderboardEntryPrefab != null)
                {
                    GameObject entryObj = Instantiate(leaderboardEntryPrefab, leaderboardContainer);
                    LeaderboardEntryUI entryUI = entryObj.GetComponent<LeaderboardEntryUI>();
                    
                    if (entryUI != null)
                    {
                        entryUI.SetEntryData(i + 1, entries[i]);
                    }
                    
                    entryObjects.Add(entryObj);
                }
            }
        }
        
        /// <summary>
        /// Returns to main menu
        /// </summary>
        public void ReturnToMainMenu()
        {
            UnityEngine.SceneManagement.SceneManager.LoadScene("MainMenu");
        }
    }
}