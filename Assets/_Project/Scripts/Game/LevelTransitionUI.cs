using System;
using System.Collections;
using TMPro;
using UnityEngine;

namespace _Project.Scripts.Game
{
    public class LevelTransitionUI : MonoBehaviour
    {
        [Header("UI")]
        [SerializeField] private CanvasGroup canvasGroup;
        [SerializeField] private TMP_Text levelText;

        [Header("Timing")]
        [SerializeField] private float fadeInDuration = 0.35f;
        [SerializeField] private float showDuration = 0.7f;
        [SerializeField] private float fadeOutDuration = 0.35f;

        private void Awake()
        {
            if (canvasGroup == null)
            {
                canvasGroup = GetComponent<CanvasGroup>();
            }

            HideInstant();
        }

        public IEnumerator PlayTransition(int nextLevel, Action onMiddle)
        {
            if (canvasGroup == null)
            {
                onMiddle?.Invoke();
                yield break;
            }

            if (levelText != null)
            {
                levelText.text = "Уровень " + nextLevel;
            }

            canvasGroup.blocksRaycasts = true;

            yield return Fade(1f, fadeInDuration);

            onMiddle?.Invoke();

            yield return new WaitForSeconds(showDuration);

            yield return Fade(0f, fadeOutDuration);

            canvasGroup.blocksRaycasts = false;
        }

        private IEnumerator Fade(float targetAlpha, float duration)
        {
            float startAlpha = canvasGroup.alpha;
            float timer = 0f;

            if (duration <= 0f)
            {
                canvasGroup.alpha = targetAlpha;
                yield break;
            }

            while (timer < duration)
            {
                timer += Time.deltaTime;
                float progress = timer / duration;

                canvasGroup.alpha = Mathf.Lerp(startAlpha, targetAlpha, progress);

                yield return null;
            }

            canvasGroup.alpha = targetAlpha;
        }

        private void HideInstant()
        {
            if (canvasGroup == null)
                return;

            canvasGroup.alpha = 0f;
            canvasGroup.blocksRaycasts = false;
        }
    }
}