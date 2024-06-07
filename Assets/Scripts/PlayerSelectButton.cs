using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace PTTT
{
    public class PlayerSelectButton : MouseHighlightable,
        IPointerClickHandler
    {
        public GameManager Manager;

        public TMPro.TMP_Text SoloText;
        public TMPro.TMP_Text CPUText;
        public Image Background;

        public Color InactiveColor;
        public Color ActiveHighlightColor;
        public Color ActiveUnHighlightColor;
        public Color BackgroundHighlightColor;
        public Color BackgroundUnHighlightColor;

        protected override bool canChangeHighlightState => Manager.CurrentState == GameManager.State.Selecting;

        public void OnPointerClick(PointerEventData eventData)
        {
            if (Manager.CurrentState != GameManager.State.Selecting) { return; }
            Manager.ChangePlayerMode();
        }

        protected override void Highlight()
        {
            var activeText = Manager.IsSingleplayer ? SoloText : CPUText;
            var inactiveText = Manager.IsSingleplayer ? CPUText : SoloText;

            activeText.color = ActiveHighlightColor;
            inactiveText.color = InactiveColor;
            Background.color = BackgroundHighlightColor;
        }

        protected override void UnHighlight()
        {
            var activeText = Manager.IsSingleplayer ? SoloText : CPUText;
            var inactiveText = Manager.IsSingleplayer ? CPUText : SoloText;

            activeText.color = ActiveUnHighlightColor;
            inactiveText.color = InactiveColor;
            Background.color = BackgroundUnHighlightColor;
        }
    }
}
