using UnityEngine;

namespace Assets.Scripts
{
    class CameraTargets : MonoBehaviour
    {
        public delegate void ClickAction(Transform transform);
        public event ClickAction OnReach;

        void OnTriggerEnter(Collider other)
        {
            OnReach(this.transform);
        }
    }
}
