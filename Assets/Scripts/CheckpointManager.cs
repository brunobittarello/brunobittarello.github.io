using UnityEngine;

namespace Assets.Scripts
{
    class CheckpointManager : MonoBehaviour
    {
        int Current;
        Transform[] ListCheckPoints;

        void Start()
        {
            ListCheckPoints = new Transform[this.transform.childCount];
            int i = 0;
            foreach (Transform child in this.transform)
            {
                ListCheckPoints[i] = child;
                ListCheckPoints[i].GetComponent<CameraTargets>().OnReach += CheckpointReached;
                i++;
            }
            Current = 0;
        }

        internal Vector3 GetCheckpointPosition()
        {
            if (Current >= ListCheckPoints.Length)
                return Vector3.zero;
            return ListCheckPoints[Current].position;
        }

        void CheckpointReached(Transform transform)
        {
            for (var i = 0; i < ListCheckPoints.Length; i++ )
                if (ListCheckPoints[i] == transform)
                { 
                    Current = i + 1;
                    return;
                }
        }
    }
}
