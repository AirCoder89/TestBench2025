using UnityEngine;
using UnityEngine.UI;

namespace TestBench2025.Core.UI
{
    public enum UIViewType { Screen, Overlay }
    
    [RequireComponent(typeof(CanvasGroup))]
    public class UICanvasView : MonoBehaviour
    {
        public UIViewType viewType = UIViewType.Screen;
        public UIState state = UIState.None;
        
        private CanvasGroup _canvasGroup;
        private CanvasGroup CanvasGroup
        {
            get
            {
                if (_canvasGroup == null)
                    _canvasGroup = GetComponent<CanvasGroup>();
                return _canvasGroup;
            }
        }
        
        private GraphicRaycaster _graphicRaycaster;
        private GraphicRaycaster GraphicRaycaster
        {
            get
            {
                if (_graphicRaycaster == null)
                    _graphicRaycaster = GetComponent<GraphicRaycaster>();
                return _graphicRaycaster;
            }
        }

        public void SetActive(bool active)
        {
            CanvasGroup.alpha = active ? 1 : 0;
            CanvasGroup.interactable = active;
            CanvasGroup.blocksRaycasts = active;

            if (GraphicRaycaster != null)
                GraphicRaycaster.enabled = active;
        }
    }
}