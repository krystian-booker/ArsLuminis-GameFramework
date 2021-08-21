using System;
using System.Collections.Generic;
using UnityEditor;
using XNodeEditor;
using System.Linq;
using Saving.Models;
using Tools;
using UnityEngine;

namespace EventSystem.VisualEditor.Nodes.State.Editor
{
    [CustomNodeEditor(typeof(UpdateStateNode))]
    public class UpdateStateNodeEditor : NodeEditor
    {
        private bool _initialSetup;
        private int _selectedIndex;
        private List<string> _stateNames = new List<string>();

        private string[] _excludes =
        {
            "m_Script",
            "graph",
            "position",
            "ports",
            "stringValue",
            "intValue",
            "floatValue",
            "booleanValue",
            "vector3Value"
        };

        public override int GetWidth()
        {
            return 300;
        }

        public override void OnBodyGUI()
        {
            serializedObject.Update();

            #region Setup
            
            if (!_initialSetup)
            {
                _selectedIndex = serializedObject.FindProperty("selectedStateIndex").intValue;
                _initialSetup = true;
            }
            
            #endregion

            #region Default draw

            // Iterate through serialized properties and draw them like the Inspector (But with ports)
            var iterator = serializedObject.GetIterator();
            var enterChildren = true;
            while (iterator.NextVisible(enterChildren))
            {
                enterChildren = false;
                if (_excludes.Contains(iterator.name)) continue;

                EditorGUIUtility.labelWidth = 120;
                NodeEditorGUILayout.PropertyField(iterator);
            }

            #endregion

            #region State Popup

            //Potential performance issues
            _stateNames = new List<string>();
            var stateNamesProperty = serializedObject.FindProperty("stateNames");
            for (var i = 0; i < stateNamesProperty.arraySize; i++)
            {
                _stateNames.Add(stateNamesProperty.GetArrayElementAtIndex(i).stringValue);
            }

            _selectedIndex = EditorGUILayout.Popup("State", _selectedIndex, _stateNames.ToArray());
            serializedObject.FindProperty("selectedStateIndex").intValue = _selectedIndex;

            #endregion

            #region State Variables

            var selectedState = Systems.SaveManager.gameState.states[_selectedIndex];
            switch (selectedState.dataType)
            {
                case DataType.String:
                    var stringValue = serializedObject.FindProperty("stringValue");
                    NodeEditorGUILayout.PropertyField(stringValue);
                    break;
                case DataType.Integer:
                    var intValue = serializedObject.FindProperty("intValue");
                    NodeEditorGUILayout.PropertyField(intValue);
                    break;
                case DataType.Float:
                    var floatValue = serializedObject.FindProperty("floatValue");
                    NodeEditorGUILayout.PropertyField(floatValue);
                    break;
                case DataType.Boolean:
                    var booleanValue = serializedObject.FindProperty("booleanValue");
                    NodeEditorGUILayout.PropertyField(booleanValue);
                    // NodeEditorGUILayout.PropertyField(booleanValue, new GUIContent("Vector Object"));
                    break;
                case DataType.Vector3:
                    var vector3Value = serializedObject.FindProperty("vector3Value");
                    NodeEditorGUILayout.PropertyField(vector3Value);
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }

            #endregion

            #region Dynamic Ports

            // Iterate through dynamic ports and draw them in the order in which they are serialized
            foreach (var dynamicPort in target.DynamicPorts)
            {
                if (NodeEditorGUILayout.IsDynamicPortListPort(dynamicPort)) continue;
                NodeEditorGUILayout.PortField(dynamicPort);
            }

            #endregion

            serializedObject.ApplyModifiedProperties();
        }
    }
}