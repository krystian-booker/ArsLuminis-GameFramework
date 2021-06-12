using System;
using System.Collections.Generic;
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
    }
}