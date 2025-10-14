using TMPro;
using UnityEngine;

namespace TestBench2025.Core.UI
{
    public class GameplayHUD : MonoBehaviour
    {
        [SerializeField] private TextMeshProUGUI coinsText;
        [SerializeField] private TextMeshProUGUI matchesText;
        [SerializeField] private TextMeshProUGUI attemptsText;

        public void Initialize()
        {
            coinsText.text = "000";
            matchesText.text = "00";
            attemptsText.text = "00";
        }

        public void UpdateCoins(int coins)
        {
            coinsText.text = coins.ToString("000");
        }

        public void UpdateProgress(int matches, int attempts)
        {
            matchesText.text = matches.ToString("00");
            attemptsText.text = attempts.ToString("00");
        }
    }
}