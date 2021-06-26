using System;
using Managers;
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
                uiTMPText.text =GameManager.Instance.localizationManager.GetTranslatedString(uiTMPText.text);
            }
        }
    }
}
