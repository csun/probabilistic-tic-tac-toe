using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

namespace PTTT
{
    public abstract class Highlightable : MonoBehaviour,
        IPointerEnterHandler,
        IPointerExitHandler
    {
        public int TotalBlinks;
        public float FirstBlinksOnTime;
        public float FirstBlinksOffTime;
        public float FinalBlinkHoldTime;

        protected virtual bool ignoreMouseHighlights => true;
        protected virtual bool refreshDefaultValue => DefaultHighlightStateChecker is not null ? DefaultHighlightStateChecker() : false;

        private bool mouseInside;

        public Func<bool> DefaultHighlightStateChecker;

        public abstract void Highlight();
        public abstract void UnHighlight();

        private void Start()
        {
            GameManager.Instance.OnGUIRefresh.AddListener(Refresh);
            Refresh();
        }

        public IEnumerator Blink(Action onBlinkComplete)
        {
            for (var i = 0; i < TotalBlinks - 1; i++)
            {
                UnHighlight();
                yield return new WaitForSeconds(FirstBlinksOffTime);
                Highlight();
                yield return new WaitForSeconds(FirstBlinksOnTime);
            }
            UnHighlight();
            yield return new WaitForSeconds(FirstBlinksOffTime);
            Highlight();
            yield return new WaitForSeconds(FinalBlinkHoldTime);
            onBlinkComplete();
            Refresh();
        }

        public virtual void Refresh()
        {
            if (refreshDefaultValue || (mouseInside && !ignoreMouseHighlights))
            {
                Highlight();
            }
            else
            {
                UnHighlight();
            }
        }

        public void OnPointerEnter(PointerEventData eventData)
        {
            mouseInside = true;
            if (ignoreMouseHighlights) { return; }
            Refresh();
        }

        public void OnPointerExit(PointerEventData eventData)
        {
            mouseInside = false;
            if (ignoreMouseHighlights) { return; }
            Refresh();
        }
    }
}
