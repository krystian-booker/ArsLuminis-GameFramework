using System;
using System.Linq;
using Saving.Models;
using Tools;
using UnityEditor;
using XNodeEditor;

namespace EventSystem.VisualEditor.Nodes.State.Editor
{
    [CustomNodeEditor(typeof(StateBranchNode))]
    public class StateBranchNodeEditor : NodeEditor
    {
        private bool _initialSetup;
        private int _selectedIndex;
        private string _selectedStateId;

        public override int GetWidth()
        {
            return 300;
        }

        public override void OnBodyGUI()
        {
            serializedObject.Update();

            #region Setup

            var stateNames = Systems.Instance.saveManager.saveTemplate.states.Select(x => x.id).ToArray();
            if (!_initialSetup)
            {
                _initialSetup = true;
                _selectedStateId = serializedObject.FindProperty("selectedStateId").stringValue;
                _selectedIndex = Systems.Instance.saveManager.saveTemplate.states.FindIndex(x => x.id == _selectedStateId);
            }

            #endregion

            #region Draw Entry

            var entry = serializedObject.FindProperty("entry");
            NodeEditorGUILayout.PropertyField(entry);

            #endregion

            #region State Popup

            _selectedIndex = EditorGUILayout.Popup("State", _selectedIndex, stateNames);
            if (_selectedIndex >= 0)
            {
                serializedObject.FindProperty("selectedStateId").stringValue =
                    Systems.Instance.saveManager.saveTemplate.states[_selectedIndex].id;
            }

            #endregion

            #region State outputs

            if (_selectedIndex >= 0)
            {
                var selectedState = Systems.Instance.saveManager.saveTemplate.states[_selectedIndex];
                switch (selectedState.dataType)
                {
                    case DataType.String:
                        var stringOptions = serializedObject.FindProperty("stringOptions");
                        NodeEditorGUILayout.PropertyField(stringOptions);
                        break;
                    case DataType.Integer:
                        var integerOptions = serializedObject.FindProperty("integerOptions");
                        NodeEditorGUILayout.PropertyField(integerOptions);
                        break;
                    case DataType.Float:
                        var floatOptions = serializedObject.FindProperty("floatOptions");
                        NodeEditorGUILayout.PropertyField(floatOptions);
                        break;
                    case DataType.Boolean:
                        var valueTrue = serializedObject.FindProperty("valueTrue");
                        NodeEditorGUILayout.PropertyField(valueTrue);
                        var valueFalse = serializedObject.FindProperty("valueFalse");
                        NodeEditorGUILayout.PropertyField(valueFalse);
                        break;
                    case DataType.Vector3:
                        var vector3Options = serializedObject.FindProperty("vector3Options");
                        NodeEditorGUILayout.PropertyField(vector3Options);
                        break;
                    default:
                        throw new ArgumentOutOfRangeException();
                }
            }

            var defaultOutput = serializedObject.FindProperty("defaultOutput");
            NodeEditorGUILayout.PropertyField(defaultOutput);

            #endregion

            serializedObject.ApplyModifiedProperties();
        }
    }
}