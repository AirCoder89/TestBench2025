using UnityEngine;

namespace TestBench2025.Core.Cards
{
    public enum CardState { Hidden, Flipping, Revealed, Matched }
    
    [CreateAssetMenu(fileName = "New Card Data", menuName = "TestBench2025/Card Data")]
    internal class CardData : ScriptableObject
    {
        public string cardId;
        public Sprite symbol;
        
    }
}