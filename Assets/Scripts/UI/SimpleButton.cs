using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace PTTT
{
    public class SimpleButton : Highlightable,
        IPointerClickHandler
    {
        public UnityEvent Action;

        public TMPro.TMP_Text Text;
        public Image Background;

        public Color InactiveColor;
        public Color ActiveHighlightColor;
        public Color ActiveUnHighlightColor;
        public Color BackgroundHighlightColor;
        public Color BackgroundUnHighlightColor;

        public bool IsMenuButton;

        protected override bool ignoreMouseHighlights => IsMenuButton ?
            GameManager.Instance.CurrentState != GameManager.State.InMenu : 
            GameManager.Instance.CurrentState != GameManager.State.Selecting;

        public void OnPointerClick(PointerEventData eventData)
        {
            if (ignoreMouseHighlights) { return; }
            Action.Invoke();
        }

        public override void Highlight()
        {
            Text.color = ActiveHighlightColor;
            Background.color = BackgroundHighlightColor;
        }

        public override void UnHighlight()
        {
            Text.color = ActiveUnHighlightColor;
            Background.color = BackgroundUnHighlightColor;
        }
    }
}
