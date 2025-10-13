using UnityEngine;

namespace TestBench2025.Utilities
{
    public static class RectTransformExtensions
    {
        public static Vector2 CalculateRelativeAnchoredPos(this Vector3 worldPos, RectTransform target)
        {
            if (target == null) return Vector2.zero;

            Vector2 screenPos = RectTransformUtility.WorldToScreenPoint(null, worldPos);
            RectTransformUtility.ScreenPointToLocalPointInRectangle(target, screenPos, null, out var localPoint);
            return localPoint;
        }
    }
}