using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace PTTT
{
    public class ToggleGroup : MonoBehaviour
    {
        public bool SettingDesired;

        public Highlightable OnButton;
        public Highlightable OffButton;

        public void UpdateSetting(bool on)
        {
            SettingDesired = on;
            OnButton.Refresh();
            OffButton.Refresh();
        }

        private void Start()
        {
            OnButton.DefaultHighlightStateChecker = () => SettingDesired;
            OffButton.DefaultHighlightStateChecker = () => !SettingDesired;
        }
    }
}
