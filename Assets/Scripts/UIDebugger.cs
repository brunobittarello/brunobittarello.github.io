using UnityEngine;
using UnityEngine.UI;

namespace Assets.Scripts
{
    class UIDebugger : MonoBehaviour
    {
        Text[] arrText;
        public Rigidbody Rigidbody;

        public static Vector3 JetPackForce;
        public static float Fuel;
        public static float TurboFuel;

        void Start()
        {
            arrText = GetComponentsInChildren<Text>();
        }

        void FixedUpdate()
        {
            arrText[0].text = "Horizontal Axis: " + Input.GetAxis("Horizontal").ToString("N2");
            arrText[1].text = "Vertical Axis: " + Input.GetAxis("Vertical").ToString("N2");
            arrText[2].text = "Jump Axis: " + Input.GetAxis("RightTurbine").ToString("N2");
            arrText[3].text = "Right Turbine Axis: " + Input.GetAxis("RightTurbine").ToString("N2");
            arrText[4].text = "Left Turbine Axis: " + Input.GetAxis("LeftTurbine").ToString("N2");
            arrText[5].text = "RightX Turbine Axis: " + Input.GetAxis("RightAxisHorizontal").ToString("N2");
            arrText[6].text = "RightY Turbine Axis: " + Input.GetAxis("RightAxisVertical").ToString("N2");
            arrText[7].text = "Rotate Character Axis: " + Input.GetAxis("RotateCharacter").ToString("N2");

            arrText[8].text = "Velocity: " + Rigidbody.velocity;
            arrText[9].text = "Velocity(mag): " + Rigidbody.velocity.magnitude.ToString("N2");
            arrText[10].text = "Position: " + Rigidbody.position;
            arrText[11].text = "JetPack Force: " + JetPackForce;
            arrText[12].text = "Fuel: " + (Fuel * 100).ToString("N2") + "%";
            arrText[13].text = "Turbo Fuel: " + (TurboFuel * 100).ToString("N2") + "%";
        }
    }
}
