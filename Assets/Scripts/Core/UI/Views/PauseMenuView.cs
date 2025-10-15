using UnityEngine;
using UnityEngine.UI;

namespace TestBench2025.Core.UI.Views
{
    internal class PauseMenuView : UICanvasView
    {
        [SerializeField] private Button loadButton;
        
        public void UpdateButtonState(bool canLoad)
        {
            loadButton.interactable = canLoad;
        }
    }
}