using System;
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
        
        //Is this a bad idea?
        public static Type GetEnumType(string enumName)
        {
            foreach (var assembly in AppDomain.CurrentDomain.GetAssemblies())
            {
                var type = assembly.GetType(enumName);
                if (type == null)
                    continue;
                if (type.IsEnum)
                    return type;
            }
            return null;
        }
    }
}