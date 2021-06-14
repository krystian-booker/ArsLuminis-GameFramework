using System;
using System.Linq;
using UnityEngine;

namespace EditorTools
{
    public class Tools : MonoBehaviour
    {
        /// <summary>
        /// Used to instantiate gameobjects from in editor.
        /// Example: used to create Targets from prefabs
        /// </summary>
        /// <param name="gameObject">Object to be instantiated</param>
        /// <returns>Reference to the instantiated gameobject</returns>
        public static GameObject InstantiateObject(GameObject gameObject)
        {
            Debug.Log($"Object instantiated at 0,0,0");
            return Instantiate(gameObject, Vector3.zero, Quaternion.identity);
        }
        
        public static void DestroyComponent(Component component)
        {
            Destroy(component);
        }
        
        /// <summary>
        /// Finds enum from a string of the enums name.
        /// This should only be used for editor code and NEVER in game.
        /// </summary>
        /// <param name="enumName">Name of the enum</param>
        /// <returns>enum</returns>
        public static Type GetEnumType(string enumName)
        {
            return AppDomain.CurrentDomain.GetAssemblies().Select(assembly => assembly.GetType(enumName)).Where(type => type != null).FirstOrDefault(type => type.IsEnum);
        }
    }
}