using System;
using UnityEngine;

namespace EventSystem.Models
{
    [Serializable]
    public class DialogOption
    {
        [SerializeField] public string key;

        [SerializeField] public string text;
    }
}