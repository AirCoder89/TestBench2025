using System;
using System.Collections.Generic;

namespace TestBench2025.Core.Game.Save
{
    [Serializable]
    internal class SavedGame
    {
        public int difficulty;
        public List<SavedCard> cards = new();
        public int coins;
        public int matches;
        public int attempts;
        public bool levelInProgress;
    }

    [Serializable]
    internal class SavedCard
    {
        public string cardId;
        public int state;
        public int index;
    }
}