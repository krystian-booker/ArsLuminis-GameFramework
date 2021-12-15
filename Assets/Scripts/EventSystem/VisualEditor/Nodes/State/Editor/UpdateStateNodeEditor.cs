using System;
using UnityEditor;
using XNodeEditor;
using System.Linq;
using Saving.Models;
using Tools;

namespace EventSystem.VisualEditor.Nodes.State.Editor
{
    [CustomNodeEditor(typeof(UpdateStateNode))]
    public class UpdateStateNodeEditor : NodeEditor
    {
        private bool _initialSetup;
        private int _selectedIndex;
        private string _selectedStateId;

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

            var stateNames = Systems.saveManager.saveTemplate.states.Select(x => x.id).ToArray();
            if (!_initialSetup)
            {
                _initialSetup = true;
                _selectedStateId = serializedObject.FindProperty("selectedStateId").stringValue;
                _selectedIndex = Systems.saveManager.saveTemplate.states.FindIndex(x => x.id == _selectedStateId);
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

            _selectedIndex = EditorGUILayout.Popup("State", _selectedIndex, stateNames);
            if (_selectedIndex >= 0)
            {
                serializedObject.FindProperty("selectedStateId").stringValue = Systems.saveManager.saveTemplate.states[_selectedIndex].id;
            }

            #endregion

            #region State Variables

            if (_selectedIndex >= 0)
            {
                var selectedState = Systems.saveManager.saveTemplate.states[_selectedIndex];
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
                        break;
                    case DataType.Vector3:
                        var vector3Value = serializedObject.FindProperty("vector3Value");
                        NodeEditorGUILayout.PropertyField(vector3Value);
                        break;
                    default:
                        throw new ArgumentOutOfRangeException();
                }
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