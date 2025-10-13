using System;
using UnityEngine;

namespace TestBench2025.Core.Board
{
    //Easy = 2x2 , Medium = 2x3 , Hard = 5x6
    public enum LevelDifficulty { Easy, Medium, Hard }
    
    internal class BoardController : MonoBehaviour
    {
        [SerializeField] private LevelDifficulty levelDifficulty;
        [SerializeField] private GridBuilder builder;

        private void Start()
        {
            Initialize();
        }

        private void Initialize()
        {
            builder.Initialize();
            builder.Build(levelDifficulty);
        }
    }
}
