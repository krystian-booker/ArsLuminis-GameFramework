using System;
using UnityEngine;

namespace EventSystem.VisualEditor.Nodes.Dialog.Models
{
    [Serializable]
    public class DialogOption
    {
        [SerializeField] public string key;

        [SerializeField] public string text;
    }
}