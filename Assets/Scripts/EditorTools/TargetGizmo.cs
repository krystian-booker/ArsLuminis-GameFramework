using UnityEngine;

namespace EditorTools
{
    public class TargetGizmo : MonoBehaviour
    {
        private void OnDrawGizmos()
        {
            // Draw a yellow sphere at the transform's position
            Gizmos.color = Color.blue;
            Gizmos.DrawWireSphere(transform.position, 0.5f);
        }
    }
}