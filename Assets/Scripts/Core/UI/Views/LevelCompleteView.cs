using TestBench2025.Core.Game;
using TMPro;
using UnityEngine;

namespace TestBench2025.Core.UI.Views
{
    internal class LevelCompleteView : UICanvasView
    {
        [SerializeField] private TextMeshProUGUI attemptsText;
        [SerializeField] private TextMeshProUGUI matchesText;
        [SerializeField] private TextMeshProUGUI coinsText;

        public void Initialize(ScoreManager manager)
        {
            attemptsText.text = manager.Attempts.ToString("00");
            matchesText.text = manager.Matches.ToString("00");
            coinsText.text = manager.Coins.ToString("000");
        }
    }
}