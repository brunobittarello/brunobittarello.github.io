using UnityEngine;
using UnityEngine.UI;

namespace Assets.Scripts
{
    class UIDebugger : MonoBehaviour
    {
        Text[] arrText;
        public Rigidbody Rigidbody;

        public static Vector3 JetPeckForce;

        void Start()
        {
            arrText = GetComponentsInChildren<Text>();
        }

        void FixedUpdate()
        {
            arrText[0].text = "Horizontal Axis: " + Input.GetAxis("Horizontal");
            arrText[1].text = "Vertical Axis: " + Input.GetAxis("Vertical");
            arrText[2].text = "Jump Axis: " + Input.GetAxis("RightTurbine");
            arrText[3].text = "Right Turbine Axis: " + Input.GetAxis("RightTurbine");
            arrText[4].text = "Left Turbine Axis: " + Input.GetAxis("LeftTurbine");
            arrText[5].text = "RightX Turbine Axis: " + Input.GetAxis("RightAxisHorizontal");
            arrText[6].text = "RightY Turbine Axis: " + Input.GetAxis("RightAxisVertical");
            arrText[7].text = "Rotate Character Axis: " + Input.GetAxis("RotateCharacter");

            arrText[8].text = "Velocity: " + Rigidbody.velocity;
            arrText[9].text = "Position: " + Rigidbody.position;
            arrText[10].text = "JetPack Force: " + JetPeckForce;
        }
    }
}
