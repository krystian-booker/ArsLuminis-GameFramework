using UnityEngine;

namespace Assets.Scripts.Components
{
    [ExecuteInEditMode]
    public class GizmoComponent : MonoBehaviour
    {
        public Color gizmoColor = Color.green; // Color of the gizmo

        [HideInInspector]
        public float gizmoRadius = 0.5f;       // Radius of the gizmo

        private void OnDrawGizmos()
        {
            Gizmos.color = gizmoColor;
            Gizmos.DrawSphere(transform.position, 0.1f);
            Gizmos.DrawWireSphere(transform.position, gizmoRadius);
        }
    }
}
