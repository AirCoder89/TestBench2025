using System;
using System.Collections.Generic;
using TestBench2025.Core.Board;
using TestBench2025.Core.UI;
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
        [SerializeField] private UIStateMachine ui;
        [SerializeField] private GameplayUIController gameplayUI;
        [SerializeField] private ScoreManager scoreManager;
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
            
            ui.GoTo(UIState.Main);
            boardController.Initialize();
            scoreManager.Initialize();
            gameplayUI.Initialize(scoreManager);
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
        
        [ContextMenu("Start Easy Game")]
        public void StartEasyGame()
        {
            levelDifficulty = LevelDifficulty.Easy;
            ui.GoTo(UIState.Gameplay);
            StartLevel();
        }
        
        [ContextMenu("Start Medium Game")]
        public void StartMediumGame()
        {
            levelDifficulty = LevelDifficulty.Medium;
            ui.GoTo(UIState.Gameplay);
            StartLevel();
        }
        
        [ContextMenu("Start Hard Game")]
        public void StartHardGame()
        {
            levelDifficulty = LevelDifficulty.Hard;
            ui.GoTo(UIState.Gameplay);
            StartLevel();
        }
        
        [ContextMenu("OpenMain")]
        public void OpenMain()
        {
            ui.GoTo(UIState.Main);
        }
        
        [ContextMenu("OpenSettings")]
        public void OpenSettings()
        {
            ui.GoTo(UIState.Settings);
        }

        [ContextMenu("ReturnToMenu")]
        public void ReturnToMenu()
        {
            ui.GoTo(UIState.Main);
        }

        [ContextMenu("ShowLevelSelect")]
        public void ShowLevelSelect()
        {
            ui.GoTo(UIState.LevelSelect);
        }
        
        [ContextMenu("PauseGame")]
        public void PauseGame()
        {
            ui.GoTo(UIState.Pause);
        }
        
        [ContextMenu("BackToPrevious")]
        public void BackToPrevious()
        {
            ui.Back();
        }
        
        private void StartLevel()
        {
            LevelStarted = false;
            scoreManager.ResetScore();
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