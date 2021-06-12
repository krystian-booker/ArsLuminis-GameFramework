using System;
using TMPro;
using UnityEngine;

namespace Localization
{
    public class UITextLocalization : MonoBehaviour
    {
        private void Start()
        {
            var uiTMPTexts = GetComponentsInChildren<TMP_Text>();
            foreach (var uiTMPText in uiTMPTexts)
            {
                uiTMPText.text = Localization.Instance.GetTranslatedString(uiTMPText.text);
            }
        }
    }
}
