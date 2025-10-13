using System;
using UnityEngine;

namespace TestBench2025.Core.Cards
{
    internal enum CardState { Hidden, Flipping, Revealed, Matched }
    
    [Serializable]
    internal class CardData
    {
        public string cardId;
        public Sprite symbol;
    }
}