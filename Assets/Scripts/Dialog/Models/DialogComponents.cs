using TMPro;
using UnityEngine;

namespace Dialog.Models
{
    /// <summary>
    /// This is attached to the DialogInstance Prefab.
    /// All of these components are needed for dialog management.
    /// This allows us to only do one GetComponent as these all have direct references.
    /// </summary>
    public class DialogComponents : MonoBehaviour
    {
        public GameObject dialogGameObject;
        public RectTransform rectTransform;
        public TMP_Text characterNameTMPText;
        public TMP_Text dialogTMPText;
        public GameObject optionsPanel;
        public GameObject footer;

        public bool IsEnabled { get; set; }
    }
}