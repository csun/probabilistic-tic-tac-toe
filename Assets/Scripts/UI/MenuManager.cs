using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace PTTT
{
    public class MenuManager : MonoBehaviour
    {
        public RectTransform GameScreenRect;
        public RectTransform MenuScreenRect;

        public AnimationCurve TransitionCurve;
        public float TransitionTime;

        private void Start()
        {
            GameScreenRect.anchorMin = new Vector2(0, 0);
            GameScreenRect.anchorMax = new Vector2(1, 1);
            MenuScreenRect.anchorMin = new Vector2(0, 1);
            MenuScreenRect.anchorMax = new Vector2(1, 2);

            GameScreenRect.gameObject.SetActive(true);
            MenuScreenRect.gameObject.SetActive(true);
        }

        public IEnumerator OpenMenu(Action callback)
        {
            yield return RunAnimation(0, -1, callback);
        }

        public IEnumerator CloseMenu(Action callback)
        {
            yield return RunAnimation(-1, 1, callback);
        }

        public IEnumerator RunAnimation(float gameScreenMinStart, float direction, Action callback)
        {
            var animProgress = 0f;

            while (animProgress < TransitionTime)
            {
                var blend = direction * Mathf.Min(1, TransitionCurve.Evaluate(animProgress / TransitionTime));
                GameScreenRect.anchorMin = new Vector2(0, gameScreenMinStart + blend);
                GameScreenRect.anchorMax = new Vector2(1, gameScreenMinStart + 1 + blend);
                MenuScreenRect.anchorMin = new Vector2(0, gameScreenMinStart + 1 + blend);
                MenuScreenRect.anchorMax = new Vector2(1, gameScreenMinStart + 2 + blend);

                yield return null;
                animProgress += Time.unscaledDeltaTime;
            }
        }
    }
}
