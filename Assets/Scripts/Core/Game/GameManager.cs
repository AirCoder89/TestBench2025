using System.Collections.Generic;
using TestBench2025.Core.Board;
using TestBench2025.Core.Game.Audio;
using TestBench2025.Core.Game.Save;
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
        [SerializeField] private List<LevelData> levels;
        [Header("Managers")]
        [SerializeField] private BoardController boardController;
        [SerializeField] private UIStateMachine ui;
        [SerializeField] private GameplayUIController gameplayUI;
        [SerializeField] private ScoreManager scoreManager;
        [SerializeField] private SoundManager soundManager;
        [SerializeField] private GameSaveManager saveManager;
        
        public bool LevelStarted { get; private set; }

        private void Update()
        {
            if (Input.GetKeyDown(KeyCode.S))
            {
                Debug.Log("Saving game...");
                SaveGame();
            }

            if (Input.GetKeyDown(KeyCode.L))
            {
                Debug.Log("Loading game...");
                LoadGame();
            }
        }

        public void SaveGame()
        {
            saveManager.SaveGame(levelDifficulty, scoreManager, boardController);
        }
        
        public void LoadGame()
        {
            if(!saveManager.HasSave())
            {
                Debug.Log("No saved game to load.");
                return;
            }
            
            var savedGame = saveManager.LoadGame();
            if (savedGame == null) return;
            levelDifficulty = (LevelDifficulty) savedGame.difficulty;
            var levelData = GetLevelData(levelDifficulty);
            
            scoreManager.ResetScore();
            scoreManager.LoadState(savedGame);
            
            boardController.StartSavedLevel(levelData, savedGame);
        }
        
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
            Unpause();
            soundManager.Play(SFXName.LevelComplete);
            
            LevelStarted = false;
            ui.GoTo(UIState.LevelComplete);
        }

        public void StartEasyGame()
        {
            Unpause();
            soundManager.Play(SFXName.ButtonClick);
            
            levelDifficulty = LevelDifficulty.Easy;
            ui.GoTo(UIState.Gameplay);
            StartLevel();
        }
        
        public void StartMediumGame()
        {
            Unpause();
            soundManager.Play(SFXName.ButtonClick);
            
            levelDifficulty = LevelDifficulty.Medium;
            ui.GoTo(UIState.Gameplay);
            StartLevel();
        }
        
        public void StartHardGame()
        {
            Unpause();
            soundManager.Play(SFXName.ButtonClick);
            
            levelDifficulty = LevelDifficulty.Hard;
            ui.GoTo(UIState.Gameplay);
            StartLevel();
        }
        
        public void OpenMain()
        {
            Unpause();
            ui.GoTo(UIState.Main);
        }
        
        public void OpenSettings()
        {
            Unpause();
            ui.GoTo(UIState.Settings);
        }

        public void ShowLevelSelect()
        {
            Unpause();
            ui.GoTo(UIState.LevelSelect);
        }
        
        public void PauseGame()
        {
            ui.GoTo(UIState.Pause);
            soundManager.Play(SFXName.ButtonClick);
            soundManager.PauseMusic();
            Time.timeScale = 0f; 
        }
        
        public void ResumeGame()
        {
            Unpause();
            ui.GoTo(UIState.Gameplay);
            soundManager.Play(SFXName.ButtonClick);
        }
        
        private void Unpause()
        {
            Time.timeScale = 1f; 
            soundManager.ResumeMusic();
        }
        
        public void BackToPrevious()
        {
            Unpause();
            ui.Back();
        }
        
        public void RestartLevel()
        {
            Unpause();
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