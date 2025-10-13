using System;
using TestBench2025.Core.Board;
using UnityEngine;

namespace TestBench2025.Core.Game
{
    //Easy = 2x2 , Medium = 2x3 , Hard = 5x6
    public enum LevelDifficulty { Easy, Medium, Hard }

    
    public class GameManager : MonoBehaviour
    {
        [SerializeField] private LevelDifficulty levelDifficulty;
        [SerializeField] private BoardController boardController;

        private void Start()
        {
            boardController.Initialize();
            boardController.StartLevel(levelDifficulty);
        }
    }
}