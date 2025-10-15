using System.Collections.Generic;
using System.IO;
using System.Linq;
using TestBench2025.Core.Board;
using UnityEngine;

namespace TestBench2025.Core.Game.Save
{
    internal class GameSaveManager : MonoBehaviour
    {
        private static string SavePath => Path.Combine(Application.persistentDataPath, "game_state.json");
        
        public void SaveGame(LevelDifficulty difficulty, ScoreManager score, BoardController board)
        {
            if (board == null || score == null)
            {
                Debug.LogError("[Save] Missing references.");
                return;
            }
            
            var savedGame = new SavedGame()
            {
                difficulty = (int)difficulty,
                coins = score.Coins,
                matches = score.Matches,
                attempts = score.Attempts,
            };
            
            var savedCards = new List<SavedCard>();
            var cards = board.Builder.ActiveCards;

            for (var i = 0; i < cards.Count; i++)
            {
                var c = cards[i];
                var entry = new SavedCard
                {
                    cardId = c.Data.cardId,
                    state = (int)c.State,
                    index = i
                };
                savedCards.Add(entry);
            }
            savedGame.cards = savedCards;
            
            var json = JsonUtility.ToJson(savedGame, true);
            File.WriteAllText(SavePath, json);
        }
        
        public SavedGame LoadGame()
        {
            if (!File.Exists(SavePath))
            {
                Debug.Log("No saved game found.");
                return null;
            }

            var json = File.ReadAllText(SavePath);
            var data = JsonUtility.FromJson<SavedGame>(json);
            data.cards = data.cards.OrderBy(c => c.index).ToList();
            return data;
        }

        public bool HasSave() => File.Exists(SavePath);

        public void DeleteSave()
        {
            if (File.Exists(SavePath))
                File.Delete(SavePath);
        }
    }
}