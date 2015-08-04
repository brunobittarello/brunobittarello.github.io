using UnityEngine;

namespace Assets.Scripts
{
    class CameraRotateAround : MonoBehaviour
    {
        void Update()
        {
            transform.LookAt(new Vector3(1.32f, 4, -1.14f));
            if (Input.GetAxis("RotateCamera") > 0)
                transform.Translate(Vector3.right * 30 * Time.deltaTime);
            else if (Input.GetAxis("RotateCamera") < 0)
                transform.Translate(Vector3.left * 30*  Time.deltaTime);
        }
    }
}
