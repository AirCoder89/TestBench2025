using System;
using System.Collections.Generic;
using TestBench2025.Core.Board;
using UnityEngine;

namespace TestBench2025.Core.Game
{
    //Easy = 2x2 , Medium = 2x3 , Hard = 5x6
    internal enum LevelDifficulty { Easy, Medium, Hard }

    
    internal class GameManager : MonoBehaviour
    {
        public static GameManager Instance { get; private set; }
        
        [SerializeField] private LevelDifficulty levelDifficulty;
        [SerializeField] private BoardController boardController;
        [SerializeField] private List<LevelData> levels;

        public bool LevelStarted { get; private set; }

        private void Awake()
        {
            if (Instance != null && Instance != this)
            {
                Destroy(gameObject);
                return;
            }
            Instance = this;
        }

        private void Start()
        {
            if (boardController == null)
            {
                Debug.LogError("BoardController is not assigned in GameManager!");
                return;
            }
            
            boardController.Initialize();
            StartLevel();
        }
        
        private void OnEnable()
        {
            boardController.OnLevelReady += HandleLevelReady;
        }

        private void OnDisable()
        {
            boardController.OnLevelReady -= HandleLevelReady;
        }
        
        private void StartLevel()
        {
            LevelStarted = false;
            var levelData = GetLevelData(levelDifficulty);
            boardController.StartLevel(levelData);
        }
        
        [ContextMenu("Restart Level")]
        public void RestartLevel()
        {
            LevelStarted = false;
            var levelData = GetLevelData(levelDifficulty);
            boardController.StartLevel(levelData);
        }


        private void HandleLevelReady()
        {
            LevelStarted = true;
            Debug.Log("Level Started!");
        }

        private LevelData GetLevelData(LevelDifficulty difficulty)
        {
            foreach (var level in levels)
            {
                if (level.difficulty == difficulty)
                    return level;
            }

            Debug.LogError($"No level data found for difficulty {difficulty}");
            return null;
        }
    }
}