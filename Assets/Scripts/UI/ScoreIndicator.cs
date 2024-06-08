using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace PTTT
{
    public class ScoreIndicator : Highlightable
    {
        public TMPro.TMP_Text HeaderText;
        public TMPro.TMP_Text ScoreText;
        public Image Background;

        public Color TextUnselectedColor;
        public Color TextSelectedColor;
        public Color BackgroundUnselectedColor;
        public Color BackgroundSelectedColor;

        public int Count
        {
            get { return _count; }
            set
            {
                _count = value;
                ScoreText.text = $"{_count}";
            }
        }
        private int _count;

        public override void Highlight()
        {
            HeaderText.color = TextSelectedColor;
            ScoreText.color = TextSelectedColor;
            Background.color = BackgroundSelectedColor;
        }

        public override void UnHighlight()
        {
            HeaderText.color = TextUnselectedColor;
            ScoreText.color = TextUnselectedColor;
            Background.color = BackgroundUnselectedColor;
        }
    }
}
