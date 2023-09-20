using UnityEngine;
using Sirenix.OdinInspector;
using System;
using Assets.Scripts.Nodes;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using UnityEngine.Assertions;

namespace Nodes.Logic
{
    [NodeTint(85, 107, 47)]
    public class ExecuteNode : ExecutableNode
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
        [SerializeField]
        private string method;

        bool isFinished = false;
        private Action cachedMethod;

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
            method = null;
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
                AddFilteredMembers("Method", type.GetMethods().Where(m => m.IsPublic).ToArray(), list);
            }
            return list;
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
                    list.Add(_member.Name);
                }
            }
        }

        /// <summary>
        /// Executes an evaluation on a specified member (either a property, field, or method) of a given component.
        /// The evaluation result is stored in the 'evaluationResult' field, and the method sets 'isFinished' to true upon completion.
        /// If the component or member is not properly set, a warning is logged and no further action is taken.
        /// </summary>
        public override void Execute()
        {
            isFinished = false;
            Assert.IsNotNull(component, "Component is not set");
            Assert.IsNotNull(method, "Method is not set");

            InvokeMethod(method);
            isFinished = true;
        }


        /// <summary>
        /// Invokes a method on the attached component using a cached delegate for better performance.
        /// </summary>
        /// <param name="memberName">The name of the method to be invoked on the component.</param>
        private void InvokeMethod(string memberName)
        {
            if (cachedMethod == null)
            {
                CacheMethod(memberName);
            }

            // Assert that the cached delegate is not null
            Assert.IsNotNull(cachedMethod, $"Method {memberName} not found.");

            // Use the delegate to invoke the method
            cachedMethod.Invoke();
        }

        /// <summary>
        /// Caches the method as a delegate for faster invocation. This eliminates the need for reflection every time the method is called.
        /// </summary>
        /// <param name="memberName">The name of the method to be cached.</param>
        private void CacheMethod(string memberName)
        {
            Type type = component.GetType();
            MethodInfo methodInfo = type.GetMethod(memberName);
            if (methodInfo != null)
            {
                cachedMethod = (Action)Delegate.CreateDelegate(typeof(Action), component, methodInfo);
            }
        }

        /// <summary>
        /// Checks whether the current operation or task has finished execution.
        /// </summary>
        /// <returns>
        /// Returns true if the operation is finished, as indicated by the 'isFinished' field; otherwise, returns false.
        /// </returns>
        public override bool IsFinished()
        {
            return isFinished;
        }
    }
}
