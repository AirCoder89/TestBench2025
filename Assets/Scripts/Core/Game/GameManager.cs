using System.Collections.Generic;
using TestBench2025.Core.Board;
using TestBench2025.Core.Game.Audio;
using TestBench2025.Core.Game.Save;
using TestBench2025.Core.UI;
using TestBench2025.Core.UI.Views;
using TestBench2025.Core.UI.Views.Settings;
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
        [SerializeField] private ScoreManager scoreManager;
        [SerializeField] private SoundManager soundManager;
        [SerializeField] private GameSaveManager saveManager;
        [SerializeField] private SettingsManager settingsManager;
        
        public bool LevelStarted { get; private set; }

        #region Initialization
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
            
            settingsManager.Initialize();
            soundManager.Initialize(settingsManager);
            soundManager.StartMusic();
            boardController.Initialize(settingsManager);
            scoreManager.Initialize();
            InitializeUI();
        }
        
        private void InitializeUI()
        {
            ui.Initialize();
            ui.GoTo(UIState.Main);
            ui.GetView<GameplayView>(UIState.Gameplay).Initialize(scoreManager);
            ui.GetView<MainView>(UIState.Main).UpdateButtonState(HasSaveGame);
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
        #endregion

        #region Save/Load

        public bool HasSaveGame => saveManager.HasSave();
        public void SaveGame()
        {
            saveManager.SaveGame(levelDifficulty, scoreManager, boardController);
            ResumeGame();
        }
        
        public void LoadGame()
        {
            if(!HasSaveGame)
            {
                Debug.Log("No saved game to load.");
                return;
            }
            
            Unpause();
            ui.GoTo(UIState.Gameplay);
            var savedGame = saveManager.LoadGame();
            if (savedGame == null) return;
            levelDifficulty = (LevelDifficulty) savedGame.difficulty;
            var levelData = GetLevelData(levelDifficulty);
            
            scoreManager.ResetScore();
            scoreManager.LoadState(savedGame);
            
            boardController.StartSavedLevel(levelData, savedGame);
        }
        
        public void ClearSaveGame()
        {
            if(!HasSaveGame) return;
            saveManager.DeleteSave();
        }

        #endregion

        #region Navigation
        public void StartEasyGame() => StartLevelWithDifficulty(LevelDifficulty.Easy);
        public void StartMediumGame() => StartLevelWithDifficulty(LevelDifficulty.Medium);
        public void StartHardGame() => StartLevelWithDifficulty(LevelDifficulty.Hard);
        
        private void StartLevelWithDifficulty(LevelDifficulty difficulty)
        {
            levelDifficulty = difficulty;
            Unpause();
            soundManager.Play(SFXName.Start);
            
            ui.GoTo(UIState.Gameplay);
            StartLevel();
        }
        
        public void OpenMain()
        {
            Unpause();
            soundManager.Play(SFXName.ButtonClick);
            
            ui.GetView<MainView>(UIState.Main).UpdateButtonState(HasSaveGame);
            ui.GoTo(UIState.Main);
        }
        
        public void OpenSettings()
        {
            Unpause();
            soundManager.Play(SFXName.ButtonClick);
            
            ui.GetView<SettingsView>(UIState.Settings).Initialize(settingsManager);
            ui.GoTo(UIState.Settings);
        }

        public void ShowLevelSelect()
        {
            Unpause();
            soundManager.Play(SFXName.ButtonClick);
            
            ui.GoTo(UIState.LevelSelect);
        }
        
        public void PauseGame()
        {
            ui.GetView<PauseMenuView>(UIState.Pause).UpdateButtonState(HasSaveGame);
            ui.GoTo(UIState.Pause);
            soundManager.Play(SFXName.ButtonClick);
            soundManager.PauseMusic();
            Time.timeScale = 0f; 
        }
        
        public void ResumeGame()
        {
            Unpause();
            soundManager.Play(SFXName.ButtonClick);
            
            ui.GoTo(UIState.Gameplay);
        }
        
        public void BackToPrevious()
        {
            Unpause();
            soundManager.Play(SFXName.ButtonClick);
            
            ui.Back();
        }
        
        public void RestartLevel()
        {
            Unpause();
            soundManager.Play(SFXName.ButtonClick);
            
            ui.GoTo(UIState.Gameplay);
            LevelStarted = false;
            StartLevel(false);
        }

        private void Unpause()
        {
            Time.timeScale = 1f; 
            soundManager.ResumeMusic();
        }
        #endregion

        #region Level Management
        private void HandleLevelCompleted()
        {
            Unpause();
            soundManager.Play(SFXName.LevelComplete);
            
            LevelStarted = false;
            
            ui.GetView<LevelCompleteView>(UIState.LevelComplete).Initialize(scoreManager);
            ui.GoTo(UIState.LevelComplete);
        }
        
        private void StartLevel(bool resetCoins = true)
        {
            LevelStarted = false;
            scoreManager.ResetScore(resetCoins);
            var levelData = GetLevelData(levelDifficulty);
            boardController.StartLevel(levelData);
        }
        
        private void HandleLevelReady()
        {
            LevelStarted = true;
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
        #endregion
       
    }
}