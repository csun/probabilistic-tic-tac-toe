using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace PTTT
{
    public class FastForwardButton : Highlightable,
        IPointerClickHandler
    {
        public TMPro.TMP_Text Text;
        public TMPro.TMP_Text Icon;
        public Image Background;

        public Color ActiveHighlightColor;
        public Color ActiveUnHighlightColor;
        public Color BackgroundHighlightColor;
        public Color BackgroundUnHighlightColor;

        int currentSpeed = 1;

        protected override bool ignoreMouseHighlights => false;

        private void Start()
        {
            UpdateText();
            UnHighlight();
        }

        public void OnPointerClick(PointerEventData eventData)
        {
            currentSpeed = currentSpeed >= 3 ? 1 : currentSpeed + 1;
            Time.timeScale = currentSpeed;
            UpdateText();
        }

        void UpdateText()
        {
            Text.text = $"{currentSpeed}X";
            if (currentSpeed == 1) { Icon.text = "A"; }
            else if (currentSpeed == 2) { Icon.text = "B"; }
            else { Icon.text = "C"; }
        }

        public override void Highlight()
        {
            Text.color = ActiveHighlightColor;
            Icon.color = ActiveHighlightColor;
            Background.color = BackgroundHighlightColor;
        }

        public override void UnHighlight()
        {
            Text.color = ActiveUnHighlightColor;
            Icon.color = ActiveUnHighlightColor;
            Background.color = BackgroundUnHighlightColor;
        }
    }
}
