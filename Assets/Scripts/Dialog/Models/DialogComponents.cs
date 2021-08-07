using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.Serialization;

namespace Dialog.Models
{
    /// <summary>
    /// This is attached to the DialogInstance Prefab.
    /// All of these components are needed for dialog management.
    /// This allows us to only do one GetComponent as these all have direct references.
    /// </summary>
    public class DialogComponents : MonoBehaviour
    {
        public RectTransform rectTransform;
        public TMP_Text characterNameTMPText;
        public TMP_Text dialogTMPText;
        public GameObject optionsPanel;
        public List<OptionComponents> optionInstances;
        public GameObject footer;

        public bool IsEnabled { get; set; }
    }
}