using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace PTTT
{
    public class StatBar : MonoBehaviour
    {
        public TMPro.TMP_Text PlayerText;
        public TMPro.TMP_Text ProbabilityText;
        public Image Slider;

        public float SliderMaxWidth;

        public void UpdateColor(Color color)
        {
            PlayerText.color = color;
            ProbabilityText.color = color;
            Slider.color = color;
        }

        public void UpdateProbability(float chance)
        {
            // Take into account that max possible chance is only 90%
            Slider.rectTransform.SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, (chance / 0.9f) * SliderMaxWidth);
            ProbabilityText.text = (chance * 100).ToString("0") + "%";
        }

        public void UpdatePlayer(bool isX)
        {
            PlayerText.text = isX ? "X" : "O";
        }
    }
}
