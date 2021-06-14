using System;
using EventSystem.Models;
using UnityEngine;

//Namespace is used as path in option menu
// ReSharper disable once CheckNamespace
namespace EventSystem
{
    public class WaitNode : BaseNode
    {
        [Input] public Empty entry;
        [Output] public Empty exit;

        [SerializeField] public float delayTime;
    }
}