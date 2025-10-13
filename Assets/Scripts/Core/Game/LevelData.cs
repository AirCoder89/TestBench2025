using System;
using TestBench2025.Core.Board;
using UnityEngine;

namespace TestBench2025.Core.Game
{
    [Serializable]
    internal class LevelData
    {
        public LevelDifficulty difficulty = LevelDifficulty.Easy;
        public float revealPreviewDuration = 2f;
        
        public LayoutData layout;
        public LevelAppearance appearance;
    }

    [Serializable]
    internal class LevelAppearance
    {
        public Color boardColor;
        
        [Header("Card Appearance")]
        public Color backgroundColor;
        public Color frontColor;
        public Color backColor;
    }
}