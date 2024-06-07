using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

namespace PTTT
{
    public abstract class MouseHighlightable : MonoBehaviour,
        IPointerEnterHandler,
        IPointerExitHandler
    {
        protected virtual bool canChangeHighlightState => true;
        protected virtual bool ignoreMouseHighlights => true;

        private bool currentHighlightState;
        private bool desiredHighlightState;

        protected abstract void Highlight();
        protected abstract void UnHighlight();

        public void ForceUpdateHighlight()
        {
            if (currentHighlightState == desiredHighlightState)
            {
                return;
            }

            if (desiredHighlightState)
            {
                Highlight();
            }
            else
            {
                UnHighlight();
            }

            currentHighlightState = desiredHighlightState;
        }    

        public void ChangeDesiredHighlightState(bool state)
        {
            desiredHighlightState = state;
        }

        public void OnPointerEnter(PointerEventData eventData)
        {
            if (ignoreMouseHighlights) { return; }
            desiredHighlightState = true;
            if (!canChangeHighlightState) { return; }
            ForceUpdateHighlight();
        }

        public void OnPointerExit(PointerEventData eventData)
        {
            if (ignoreMouseHighlights) { return; }
            desiredHighlightState = false;
            if (!canChangeHighlightState) { return; }
            ForceUpdateHighlight();
        }
    }
}
