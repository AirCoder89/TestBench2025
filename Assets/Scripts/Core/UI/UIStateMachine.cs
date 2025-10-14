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
        Transition
    }
    
    internal class UIStateMachine : MonoBehaviour
    {
        [Header("Screen Canvases")]
        [SerializeField] private CanvasGroup mainCanvas;
        [SerializeField] private CanvasGroup settingsCanvas;
        [SerializeField] private CanvasGroup gameplayCanvas;
        [SerializeField] private CanvasGroup levelSelectCanvas;

        [Header("Overlay")]
        [SerializeField] private CanvasGroup pausePanel;
        [SerializeField] private CanvasGroup transitionCanvas;
        [SerializeField] private Image fadeImage;
        [SerializeField] private float fadeDuration = 0.3f;

        private readonly Dictionary<UIState, CanvasGroup> _screens = new();
        private UIState _previousState;
        private UIState _currentState;
        private Coroutine _transitionRoutine;

        private void Awake()
        {
            _screens[UIState.Main] = mainCanvas;
            _screens[UIState.Settings] = settingsCanvas;
            _screens[UIState.Gameplay] = gameplayCanvas;
            _screens[UIState.LevelSelect] = levelSelectCanvas;
            _screens[UIState.Pause] = pausePanel;

            // disable all
            foreach (var kv in _screens)
                SetCanvasActive(kv.Value, false);

            SetCanvasActive(transitionCanvas, false);
            _currentState = UIState.None;
            _previousState = UIState.None;
        }

        public void GoTo(UIState next)
        {
            if (_transitionRoutine != null)
                StopCoroutine(_transitionRoutine);

            _transitionRoutine = StartCoroutine(TransitionRoutine(next));
        }

        private IEnumerator TransitionRoutine(UIState next)
        {
            // fade out
            yield return Fade(true);

            // deactivate all screens
            foreach (var kv in _screens)
                SetCanvasActive(kv.Value, false);

            // activate next screen
            if (_screens.TryGetValue(next, out var screen))
                SetCanvasActive(screen, true);

            if(_currentState != UIState.Pause) _previousState = _currentState;
            _currentState = next;

            // fade back in
            yield return Fade(false);
        }

        private IEnumerator Fade(bool toBlack)
        {
            SetCanvasActive(transitionCanvas, true);

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
                SetCanvasActive(transitionCanvas, false);
        }

        private void SetCanvasActive(CanvasGroup cg, bool active)
        {
            if (cg == null) return;

            cg.alpha = active ? 1 : 0;
            cg.interactable = active;
            cg.blocksRaycasts = active;

            var raycaster = cg.GetComponent<GraphicRaycaster>();
            if (raycaster != null)
                raycaster.enabled = active;
        }

        public bool IsCurrent(UIState state) => _currentState == state;

        public void Back()
        {
            GoTo(_previousState);
        }
    }
}
