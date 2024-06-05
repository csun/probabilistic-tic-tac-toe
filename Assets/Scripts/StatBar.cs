using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace PTTT
{
    public class StatBar : MonoBehaviour
    {
        public TMPro.TMP_Text PlayerText;
        public TMPro.TMP_Text ProbabilityText;
        public RectTransform Slider;

        public float SliderMaxWidth;

        public void UpdateProbability(float chance)
        {
            Slider.SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, chance * SliderMaxWidth);
            ProbabilityText.text = (chance * 100).ToString("0") + "%";
        }

        public void UpdatePlayer(bool isX)
        {
            PlayerText.text = isX ? "X" : "O";
        }
    }
}
