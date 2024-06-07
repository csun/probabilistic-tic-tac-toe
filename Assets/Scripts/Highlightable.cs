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

        private bool mouseInside;

        public abstract void Highlight();
        public abstract void UnHighlight();

        private void Start()
        {
            UnHighlight();
        }

        public IEnumerator Blink(System.Action onBlinkComplete)
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
            Refresh();
            onBlinkComplete();
        }

        public void Refresh()
        {
            if (mouseInside && !ignoreMouseHighlights)
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
