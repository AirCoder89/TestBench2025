using System.Collections.Generic;
using TestBench2025.Core.Board;
using TestBench2025.Core.Game.Audio;
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
        [SerializeField] private SoundManager soundManager;
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
            
            soundManager.Initialize();
            soundManager.StartMusic();
            
            ui.Initialize();
            ui.GoTo(UIState.Main);
            boardController.Initialize();
            scoreManager.Initialize();
            gameplayUI.Initialize(scoreManager);
        }
        
        private void OnEnable()
        {
            boardController.OnLevelReady += HandleLevelReady;
            boardController.OnLevelCompleted += HandleLevelCompleted;
        }

        private void OnDisable()
        {
            boardController.OnLevelReady -= HandleLevelReady;
            boardController.OnLevelCompleted -= HandleLevelCompleted;
        }
        
        private void HandleLevelCompleted()
        {
            soundManager.ResumeMusic();
            soundManager.Play(SFXName.LevelComplete);
            
            LevelStarted = false;
            ui.GoTo(UIState.LevelComplete);
        }

        public void StartEasyGame()
        {
            soundManager.ResumeMusic();
            soundManager.Play(SFXName.ButtonClick);
            
            levelDifficulty = LevelDifficulty.Easy;
            ui.GoTo(UIState.Gameplay);
            StartLevel();
        }
        
        public void StartMediumGame()
        {
            soundManager.ResumeMusic();
            soundManager.Play(SFXName.ButtonClick);
            
            levelDifficulty = LevelDifficulty.Medium;
            ui.GoTo(UIState.Gameplay);
            StartLevel();
        }
        
        public void StartHardGame()
        {
            soundManager.ResumeMusic();
            soundManager.Play(SFXName.ButtonClick);
            
            levelDifficulty = LevelDifficulty.Hard;
            ui.GoTo(UIState.Gameplay);
            StartLevel();
        }
        
        public void OpenMain()
        {
            soundManager.ResumeMusic();
            ui.GoTo(UIState.Main);
        }
        
        public void OpenSettings()
        {
            soundManager.ResumeMusic();
            ui.GoTo(UIState.Settings);
        }

        public void ShowLevelSelect()
        {
            soundManager.ResumeMusic();
            ui.GoTo(UIState.LevelSelect);
        }
        
        public void PauseGame()
        {
            ui.GoTo(UIState.Pause);
            soundManager.Play(SFXName.ButtonClick);
            soundManager.PauseMusic();
        }
        
        public void BackToPrevious()
        {
            soundManager.ResumeMusic();
            ui.Back();
        }
        
        public void RestartLevel()
        {
            soundManager.ResumeMusic();
            ui.GoTo(UIState.Gameplay);
            LevelStarted = false;
            StartLevel();
        }
        
        private void StartLevel()
        {
            LevelStarted = false;
            scoreManager.ResetScore();
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