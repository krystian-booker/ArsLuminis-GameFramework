using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using NUnit.Framework;
using Tools;
using UnityEditor;
using UnityEditor.Animations;
using UnityEngine;
using UnityEngine.Assertions;
using XNode;
using XNodeEditor;
namespace EventSystem.VisualEditor.Nodes.State.Editor
{
    /// <summary>
    /// This Node allows 'any' script to have its fields updated via the EventSequencer
    /// This should never be used for anything but quick testing, performance is going to be terrible
    /// </summary>
    [CustomNodeEditor(typeof(ComponentUpdateNode))]
    public class ComponentUpdateNodeEditor : NodeEditor
    {
        public ComponentUpdateNodeEditor()
        {
            //Builtin action on xNode
            onUpdateNode += UpdateSelectedComponent;
        }

        /// Node Width
        public override int GetWidth()
        {
            return 350;
        }
        
        private int _selectedVariableIndex;
        private string[] _variables = Array.Empty<string>();
        
        private int _selectedComponentIndex;
        private List<Type> _componentTypes = new List<Type>();
        private string[] _componentNames = Array.Empty<string>();
        
        private GameObject _currentGameObject;
        
        public override void OnBodyGUI()
        {
            serializedObject.Update();
            string[] excludes = {"m_Script", "graph", "position", "ports"};

            // Iterate through serialized properties and draw them like the Inspector (But with ports)
            var iterator = serializedObject.GetIterator();
            var enterChildren = true;

            GetComponents();
            GetPublicVariables();
            while (iterator.NextVisible(enterChildren))
            {
                enterChildren = false;
                if (excludes.Contains(iterator.name)) continue;
                switch (iterator.name)
                {
                    case "continueOnAnimationEvent":
                        EditorGUIUtility.labelWidth = 300;
                        NodeEditorGUILayout.PropertyField(iterator);
                        break;
                    case "targetComponent":
                        _selectedComponentIndex = EditorGUILayout.Popup("Components", _selectedComponentIndex, _componentNames);
                        break;
                    case "targetVariable":
                        _selectedVariableIndex = EditorGUILayout.Popup("Variables", _selectedVariableIndex, _variables);
                        break;
                    default:
                        EditorGUIUtility.labelWidth = 110;
                        NodeEditorGUILayout.PropertyField(iterator);
                        break;
                }
            }

            // Iterate through dynamic ports and draw them in the order in which they are serialized
            foreach (var dynamicPort in target.DynamicPorts)
            {
                if (NodeEditorGUILayout.IsDynamicPortListPort(dynamicPort)) continue;
                NodeEditorGUILayout.PortField(dynamicPort);
            }

            serializedObject.ApplyModifiedProperties();
        }

        private void UpdateSelectedComponent(Node node)
        {
            var componentUpdateNode = node as ComponentUpdateNode;
            if (componentUpdateNode == null || _componentNames == null || !_componentNames.Any())
                return;

            componentUpdateNode.targetComponent = _componentNames[_selectedComponentIndex];
        }

        private void GetComponents()
        {
            var targetGameObjectProperty = serializedObject.FindProperty("targetGameObject");
            var targetGameObject = targetGameObjectProperty.objectReferenceValue as GameObject;

            if (targetGameObject == null || _componentNames?.Length > 0 && _currentGameObject == targetGameObject)
                return;

            _currentGameObject = targetGameObject;
            _componentTypes = _currentGameObject.GetComponents(typeof(Component)).Select(t => t.GetType()).ToList();
            _componentNames = _currentGameObject.GetComponents(typeof(Component)).Select(t => t.GetType().ToString()).ToArray();
        }
        
        private void GetPublicVariables()
        {
            var targetGameObjectProperty = serializedObject.FindProperty("targetGameObject");
            var targetGameObject = targetGameObjectProperty.objectReferenceValue as GameObject;

            if(targetGameObject == null)
                return;

            var typeString = _componentNames[_selectedComponentIndex];
            
            //Remove 'UnityEngine.' from type name
            typeString = typeString.Substring(typeString.IndexOf('.') + 1);
            
            var component = targetGameObject.GetComponent(typeString);
            var fields = component.GetType().GetFields();
            foreach (var field in fields)
            {
                try
                {
                    var propValue = field.GetValue(component);
                    Debug.Log(field.Name);
                }
                catch (Exception e)
                {
                    //
                }
            }
        }
    }
}