using UnityEngine;
using Sirenix.OdinInspector;
using System;
using Assets.Scripts.Nodes;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Assets.Scripts.Constants;
using Assets.Scripts.Models.Interfaces;
using XNode;
using UnityEngine.Assertions;

namespace Nodes.Logic
{
    [NodeTint(30, 30, 160)]
    public class IfNode : BranchingNode
    {
        [Tooltip("Target GameObject")]
        [SerializeField, Required]
        private GameObject gameObject;

        [Tooltip("Selected Component")]
        [ValueDropdown("GetComponentsInGameObject")]
        [OnValueChanged("OnComponentChanged")]
        [SerializeField]
        private Component component;

        [Tooltip("Selected Property or Method")]
        [ValueDropdown("GetMembersInComponent")]
        [OnValueChanged("OnMembersChanged")]
        [SerializeField]
        private string member;

        [Tooltip("Comparison Value")]
        [ShowIf("ShouldShowComparisonValue")]
        [SerializeField]
        private string compareValue;

        private bool evaluationResult = false;

        private Dictionary<string, PropertyInfo> propertyCache = new Dictionary<string, PropertyInfo>();
        private Dictionary<string, FieldInfo> fieldCache = new Dictionary<string, FieldInfo>();
        private Dictionary<string, MethodInfo> methodCache = new Dictionary<string, MethodInfo>();

        public IfNode()
        {

            AddExit(Constants.IfNodeOutputs.TRUE_EXIT);
            AddExit(Constants.IfNodeOutputs.FALSE_EXIT);
        }

        /// <summary>
        /// Retrieves all the components attached to the current GameObject and returns them as a ValueDropdownList.
        /// This list can be used for populating a dropdown for selecting a component in a Unity Editor.
        /// </summary>
        /// <returns>A ValueDropdownList containing the names and references to the components attached to the current GameObject.</returns>
        private ValueDropdownList<Component> GetComponentsInGameObject()
        {
            var list = new ValueDropdownList<Component>();
            if (gameObject != null)
            {
                foreach (var comp in gameObject.GetComponents<Component>())
                {
                    list.Add(comp.GetType().Name, comp);
                }
            }
            return list;
        }

        /// <summary>
        /// Resets the 'member' and 'compareValue' fields to null when the component is changed.
        /// This method is typically invoked to ensure that old values do not persist when the component is updated.
        /// </summary>
        private void OnComponentChanged()
        {
            member = null;
            compareValue = null;
        }

        /// <summary>
        /// Retrieves the properties, methods, and fields of the selected component and returns them as a ValueDropdownList.
        /// This is useful for populating a dropdown in a Unity Editor to allow selection of a specific member within the selected component.
        /// </summary>
        /// <returns>A ValueDropdownList containing the names of the properties, methods, and fields belonging to the selected component.</returns>
        private ValueDropdownList<string> GetMembersInComponent()
        {
            var list = new ValueDropdownList<string>();
            if (component != null)
            {
                var type = component.GetType();
                AddFilteredMembers("Property", type.GetProperties(), list);
                AddFilteredMembers("Method", type.GetMethods().Where(m => m.IsPublic).ToArray(), list);
                AddFilteredMembers("Field", type.GetFields(), list);
            }
            return list;
        }

        /// <summary>
        /// Resets the 'compareValue' field to null when the selected member changes.
        /// This method is usually invoked to ensure that outdated compare values do not persist when the selected member is updated.
        /// </summary>
        private void OnMembersChanged()
        {
            compareValue = null;
        }

        /// <summary>
        /// Adds filtered members of a given type to a ValueDropdownList with a specified prefix.
        /// Excludes members that are declared in MonoBehaviour, Behaviour, or Component.
        /// </summary>
        /// <typeparam name="TMemberInfo">The type of the MemberInfo (could be PropertyInfo, MethodInfo, FieldInfo, etc.)</typeparam>
        /// <param name="prefix">A string prefix to prepend to each member name in the list (e.g., "Property", "Method", "Field").</param>
        /// <param name="members">An array of members to filter and add to the list.</param>
        /// <param name="list">The ValueDropdownList to which filtered members will be added.</param>
        private void AddFilteredMembers<TMemberInfo>(string prefix, TMemberInfo[] members, ValueDropdownList<string> list) where TMemberInfo : MemberInfo
        {
            var excludedTypes = new HashSet<Type> { typeof(MonoBehaviour), typeof(Behaviour), typeof(Component) };
            foreach (var _member in members)
            {
                if (!excludedTypes.Contains(_member.DeclaringType))
                {
                    list.Add($"{prefix}:{_member.Name}");
                }
            }
        }

        /// <summary>
        /// Determines whether a comparison value should be shown in the UI, based on the selected member and component.
        /// Excludes boolean types, as they don't typically require a comparison value.
        /// </summary>
        /// <returns>
        /// True if a comparison value should be shown; otherwise, false.
        /// This will return false if the selected member is of boolean type or if the member or component is not properly set.
        /// </returns>
        private bool ShouldShowComparisonValue()
        {
            if (member != null && component != null)
            {
                string[] parts = member.Split(':');
                if (parts.Length != 2) return false;

                string memberType = parts[0];
                string memberName = parts[1];
                Type type = component.GetType();

                if (memberType == "Property")
                {
                    var propInfo = type.GetProperty(memberName);
                    return propInfo != null && propInfo.PropertyType != typeof(bool);
                }
                else if (memberType == "Field")
                {
                    var fieldInfo = type.GetField(memberName);
                    return fieldInfo != null && fieldInfo.FieldType != typeof(bool);
                }
            }
            return false;
        }

        /// <summary>
        /// Executes an evaluation on a specified member (either a property, field, or method) of a given component.
        /// The evaluation result is stored in the 'evaluationResult' field.
        /// If the component or member is not properly set, a warning is logged and no further action is taken.
        /// </summary>
        public override void Execute()
        {
            Assert.IsNotNull(component, "Component is not set.");
            Assert.IsNotNull(member, "Member is not set.");

            evaluationResult = false;

            string[] parts = member.Split(':');
            if (parts.Length != 2) return;

            string memberType = parts[0];
            string memberName = parts[1];
            Type type = component.GetType();

            switch (memberType)
            {
                case "Property":
                    if (!propertyCache.TryGetValue(memberName, out var propInfo))
                    {
                        propInfo = type.GetProperty(memberName);
                        propertyCache[memberName] = propInfo;
                    }
                    if (propInfo != null)
                    {
                        object value = propInfo.GetValue(component);
                        evaluationResult = Evaluate(value);
                    }
                    break;
                case "Field":
                    if (!fieldCache.TryGetValue(memberName, out var fieldInfo))
                    {
                        fieldInfo = type.GetField(memberName);
                        fieldCache[memberName] = fieldInfo;
                    }
                    if (fieldInfo != null)
                    {
                        object value = fieldInfo.GetValue(component);
                        evaluationResult = Evaluate(value);
                    }
                    break;
                case "Method":
                    if (!methodCache.TryGetValue(memberName, out var methodInfo))
                    {
                        methodInfo = type.GetMethod(memberName);
                        methodCache[memberName] = methodInfo;
                    }
                    if (methodInfo != null)
                    {
                        object value = methodInfo.Invoke(component, null);
                        evaluationResult = Evaluate(value);
                    }
                    break;
            }
        }

        /// <summary>
        /// Evaluates the given object value against a predefined 'compareValue'.
        /// If the object value is a boolean, it returns the boolean value itself.
        /// If the object value is non-null, it compares the object's string representation to the 'compareValue' and returns the comparison result.
        /// </summary>
        /// <param name="value">The object value to evaluate.</param>
        /// <returns>
        /// Returns true if the evaluation is successful based on the criteria defined; otherwise, returns false.
        /// </returns>
        private bool Evaluate(object value)
        {
            if (value is bool boolValue)
            {
                return boolValue;
            }

            if (value != null)
            {
                return value.ToString() == compareValue;
            }

            return false;
        }

        /// <summary>
        /// Checks whether the current operation or task has finished execution.
        /// </summary>
        /// <returns>
        /// Returns true if the operation is finished.
        /// </returns>
        public override bool IsFinished()
        {
            return true;
        }

        /// <summary>
        /// Retrieves a list of connected output nodes based on the evaluation result.
        /// If 'evaluationResult' is true, it returns the nodes connected to the "TrueExit" output port.
        /// If 'evaluationResult' is false, it returns the nodes connected to the "FalseExit" output port.
        /// </summary>
        /// <returns>
        /// A list of IBaseNode objects that are connected to the selected output port based on the evaluation result.
        /// Returns an empty list if there are no connected output nodes.
        /// </returns>
        public override List<IBaseNode> GetConnectedOutputs()
        {
            var selectedOutputName = evaluationResult ? Constants.IfNodeOutputs.TRUE_EXIT : Constants.IfNodeOutputs.FALSE_EXIT;

            var selectedOutputs = new List<IBaseNode>();
            foreach (NodePort outputPort in Outputs)
            {
                if (outputPort.fieldName == selectedOutputName)
                {
                    foreach (NodePort connectedPort in outputPort.GetConnections())
                    {
                        selectedOutputs.Add(connectedPort.node as IBaseNode);
                    }
                }
            }

            return selectedOutputs;
        }
    }
}
