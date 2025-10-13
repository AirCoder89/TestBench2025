using System;
using TestBench2025.Core.Game;
using UnityEngine;

namespace TestBench2025.Core.Board
{
    [Serializable]
    public class LayoutData
    {
        public LevelDifficulty difficulty = LevelDifficulty.Easy;
        public Vector2 cellSize = new Vector2(100, 150);
        public Vector2 spacing = new Vector2(10, 10);
        public int rows = 2;
        public int columns = 2;
        
        [Header("Card Appearance")]
        public Color backgroundColor;
        public Color frontColor;
        public Color backColor;
    }
}