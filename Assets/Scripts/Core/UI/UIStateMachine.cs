using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace TestBench2025.Core.UI
{
    public enum UIState
    {
        None,
        Main,
        Settings,
        Gameplay,
        LevelSelect,
        Pause,
        LevelComplete,
        Transition
    }
    
    internal class UIStateMachine : MonoBehaviour
    {
        [Header("Screen Canvases")]
        [SerializeField] private List<UICanvasView> allCanvases;
        
        [Header("Transition Settings")]
        [SerializeField] private UICanvasView transitionCanvas;
        [SerializeField] private Image fadeImage;
        [SerializeField] private float fadeDuration = 0.3f;

        private Dictionary<UIState, UICanvasView> _screens = new();

        private Dictionary<UIState, UICanvasView> Screens
        {
            get
            {
                if(_screens == null || _screens.Count == 0)
                {
                    _screens = new Dictionary<UIState, UICanvasView>();
                    foreach (var canvas in allCanvases)
                    {
                        if (canvas != null && canvas.state != UIState.None)
                        {
                            _screens[canvas.state] = canvas;
                            canvas.SetActive(false);
                        }
                    }
                }
                return _screens;
            }
        }
        private UIState _previousState;
        private UIState _currentState;
        private Coroutine _transitionRoutine;

        public void Initialize()
        {
            if (Screens == null || Screens.Count == 0)
            {
                Debug.LogWarning("No UI Screens found in UIStateMachine.");
            }
            
            transitionCanvas.SetActive(false);
            _currentState = UIState.None;
            _previousState = UIState.None;
        }
        

        public void GoTo(UIState next)
        {
            if (_transitionRoutine != null)
                StopCoroutine(_transitionRoutine);

            var hasNext = _screens.TryGetValue(next, out var nextView);
            if (!hasNext) return;

            var isNextOverlay = nextView.viewType == UIViewType.Overlay;

            // If current is overlay and next is a screen → close overlay first, then transition
            if (_currentState != UIState.None && _screens.TryGetValue(_currentState, out var currentView) && currentView.viewType == UIViewType.Overlay && !isNextOverlay)
            {
                currentView.SetActive(false);
                _currentState = _previousState;
                _transitionRoutine = StartCoroutine(TransitionRoutine(next));
                return;
            }

            // If next is an overlay → show it without no transition
            if (isNextOverlay)
            {
                nextView.SetActive(true);
                _previousState = _currentState;
                _currentState = next;
                return;
            }

            _transitionRoutine = StartCoroutine(TransitionRoutine(next));
        }

        private IEnumerator TransitionRoutine(UIState next)
        {
            yield return Fade(true);

            foreach (var kv in _screens)
            {
                if (kv.Value.viewType == UIViewType.Screen)
                    kv.Value.SetActive(false);
            }

            if (_screens.TryGetValue(next, out var screen))
                screen.SetActive(true);

            if (_currentState != UIState.Pause && _currentState != UIState.LevelComplete)
                _previousState = _currentState;

            _currentState = next;

            yield return Fade(false);
        }

        private IEnumerator Fade(bool toBlack)
        {
            transitionCanvas.SetActive(true);
            var t = 0f;
            while (t < fadeDuration)
            {
                t += Time.unscaledDeltaTime;
                var normalized = Mathf.Clamp01(t / fadeDuration);
                var alpha = toBlack ? normalized : 1f - normalized;

                fadeImage.color = new Color(0f, 0f, 0f, alpha);
                yield return null;
            }

            if (!toBlack)
                transitionCanvas.SetActive(false);
        }

        public bool IsCurrent(UIState state) => _currentState == state;

        public void Back()
        {
            GoTo(_previousState);
        }
    }
}
