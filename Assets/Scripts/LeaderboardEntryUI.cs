using UnityEngine;
using UnityEngine.UI;
using TMPro;

namespace ImmersiveLearningApp
{
    /// <summary>
    /// Displays a single leaderboard entry
    /// </summary>
    public class LeaderboardEntryUI : MonoBehaviour
    {
        [Header("UI References")]
        [SerializeField] private TextMeshProUGUI rankText;
        [SerializeField] private TextMeshProUGUI playerNameText;
        [SerializeField] private TextMeshProUGUI scoreText;
        [SerializeField] private TextMeshProUGUI dateText;
        [SerializeField] private Image background;
        
        /// <summary>
        /// Sets the data for this leaderboard entry
        /// </summary>
        public void SetEntryData(int rank, LeaderboardEntry entry)
        {
            if (rankText != null)
            {
                rankText.text = rank.ToString();
                
                // Highlight top 3 entries
                if (rank <= 3)
                {
                    rankText.color = Color.yellow;
                }
            }
            
            if (playerNameText != null)
            {
                playerNameText.text = entry.playerName;
            }
            
            if (scoreText != null)
            {
                scoreText.text = entry.score.ToString();
            }
            
            if (dateText != null)
            {
                dateText.text = entry.dateAchieved.ToString("dd/MM/yyyy");
            }
            
            // Change background color for top 3
            if (background != null)
            {
                if (rank == 1)
                {
                    background.color = new Color(1.0f, 0.84f, 0.0f, 0.3f); // Gold
                }
                else if (rank == 2)
                {
                    background.color = new Color(0.6f, 0.6f, 0.6f, 0.3f); // Silver
                }
                else if (rank == 3)
                {
                    background.color = new Color(0.8f, 0.5f, 0.2f, 0.3f); // Bronze
                }
                else
                {
                    background.color = new Color(0.2f, 0.2f, 0.2f, 0.3f); // Normal
                }
            }
        }
    }
}