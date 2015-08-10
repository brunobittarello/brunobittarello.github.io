using UnityEngine;

namespace Assets.Scripts
{
    class Checkpoint : MonoBehaviour
    {
        void OnTriggerEnter(Collider other)
        {
            var jetPack = other.GetComponent<JetPackControll2>();
            if (jetPack == null)
                return;

            jetPack.EnableTurboMode = true;
            UIDebugger.TurboFuelEnabled = true;
            Destroy(this);
        }
    }
}
