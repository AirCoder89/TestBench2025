using System;
using System.Collections;
using TestBench2025.Utilities;
using UnityEngine;

namespace TestBench2025.Core.Cards
{
    internal class CardAnimator : MonoBehaviour
    {
        [SerializeField] private RectTransform cardContent;
        [SerializeField] private GameObject frontFace;
        [SerializeField] private GameObject backFace;
        [SerializeField] private float flipDuration = 0.4f;

        private Coroutine _flipRoutine;
        
        public void Initialize()
        {
            ResetCard();
        }
        
        public void FlipToFront(Action onComplete = null, float startDelay = 0f)
        {
            if (_flipRoutine != null) StopCoroutine(_flipRoutine);
            _flipRoutine = StartCoroutine(FlipRoutine(0, 90, () =>
            {
                frontFace.SetActive(true); 
                backFace.SetActive(false); 
                StartCoroutine(FlipRoutine(90, 0, onComplete));
            }, startDelay));
        }

        public void FlipToBack(Action onComplete = null, float startDelay = 0f)
        {
            if (_flipRoutine != null) StopCoroutine(_flipRoutine);
            _flipRoutine = StartCoroutine(FlipRoutine(0, 90, () =>
            {
                frontFace.SetActive(false); 
                backFace.SetActive(true); 
                StartCoroutine(FlipRoutine(90, 0, onComplete));
            }, startDelay));
        }
        
        public void PlayEntryAnimation(Vector2 relativeOrigin,float speed, float delay, Action onComplete = null)
        {
            StopAllCoroutines();
            StartCoroutine(MoveTo(relativeOrigin, Vector2.zero, speed, delay, onComplete));
        }

        public void PlayEntryReveal(float previewDuration, Action onComplete = null)
        {
            StartCoroutine(EntryReveal(previewDuration, onComplete));
        }
        
        public void Matched(Action onComplete = null, float startDelay = 0f)
        {
            cardContent.gameObject.SetActive(false); //todo
        }

        public void ResetCard()
        {
            frontFace.SetActive(false);
            backFace.SetActive(true);
            cardContent.gameObject.SetActive(true);
            cardContent.localRotation = Quaternion.Euler(0, 0, 0);
            cardContent.anchoredPosition = Vector2.zero;
        }
        
        private IEnumerator FlipRoutine(float from, float to, Action onComplete, float startDelay = 0f)
        {
            if (startDelay > 0) yield return new WaitForSeconds(startDelay); 
            var elapsed = 0F;
            while (elapsed < flipDuration)
            {
                elapsed += Time.deltaTime; var t = Mathf.Clamp01(elapsed / flipDuration); 
                var eased = MiniTween.BackEaseOut(t * flipDuration, 0, 1, flipDuration); 
                var rotation = Mathf.Lerp(from, to, eased); 
                cardContent.localRotation = Quaternion.Euler(0, rotation, 0); 
                yield return null;
            } 
            
            onComplete?.Invoke();
        }
        
        
        private IEnumerator EntryReveal(float previewDuration, Action onComplete)
        {
            if (!this || !gameObject) yield break;  // destroyed or disabled

            FlipToFront();

            float timer = 0f;
            while (timer < previewDuration)
            {
                if (!this || !gameObject) yield break;
                timer += Time.deltaTime;
                yield return null;
            }

            if (!this || !gameObject) yield break;
            FlipToBack(onComplete);
        }


        private IEnumerator MoveTo(Vector2 startPos, Vector2 endPos, float speed, float delay, Action onComplete = null)
        {
            if (!this || !gameObject) yield break;
            if (delay > 0f)
            {
                float wait = 0f;
                while (wait < delay)
                {
                    if (!this || !gameObject) yield break;
                    wait += Time.deltaTime;
                    yield return null;
                }
            }

            if (!this || !gameObject) yield break;
            cardContent.anchoredPosition = startPos;

            var distance = Vector2.Distance(startPos, endPos);
            if (distance < 0.01f)
            {
                cardContent.anchoredPosition = endPos;
                onComplete?.Invoke();
                yield break;
            }

            var duration = distance / speed;
            var elapsed = 0f;
            while (elapsed < duration)
            {
                if (!this || !gameObject) yield break;

                elapsed += Time.deltaTime;
                var t = Mathf.Clamp01(elapsed / duration);
                var eased = MiniTween.BackEaseOut(t * duration, 0, 1, duration);
                cardContent.anchoredPosition = Vector2.LerpUnclamped(startPos, endPos, eased);
                yield return null;
            }

            if (this && gameObject)
                cardContent.anchoredPosition = endPos;

            onComplete?.Invoke();
        }


        public void KillAnimation()
        {
            if (_flipRoutine != null) StopCoroutine(_flipRoutine);
            StopAllCoroutines();
            ResetCard();
        }
    }
}
