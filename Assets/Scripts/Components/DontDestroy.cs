using UnityEngine;

namespace Assets.Scripts.Components
{
    public class DontDestroy : MonoBehaviour
    {
        void Awake()
        {
            DontDestroyOnLoad(gameObject);
        }
    }
}
