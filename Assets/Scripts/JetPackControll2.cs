using UnityEngine;
using System.Collections;
using Assets.Scripts;

namespace Assets.Scripts
{
    [RequireComponent(typeof(CustomThirdPersonCharacter))]
    public class JetPackControll2 : MonoBehaviour
    {
        CustomThirdPersonCharacter ThirdPersonCharacter;

        public Transform TurbineRight;
        public Transform TurbineLeft;

        public Transform FireRight;
        public Transform FireLeft;

        Rigidbody RigidBody;
        public Vector3 RightForceDirection;
        public Vector3 LeftForceDirection;

        public float TurbineMaxAngle;
        public float TurbineMaxForce;
        public float RotationVelocity;

        public float MaxVelocity;
        public float TerminalVelocity;

        public float FuelRecoveryFactor;
        public float FuelConsumeFactor;
        public float FuelRecoveryDelay;
        float Fuel;
        float FuelTimer;

        public bool TurboMode;


        public JetPackControll2()
        {
            RightForceDirection = new Vector3(-0.4f, 0.6f, 0);
            LeftForceDirection = new Vector3(0.4f, 0.6f, 0);
            TurbineMaxAngle = 30;
            TurbineMaxForce = 5;
            RotationVelocity = 3;
            MaxVelocity = 70;
            TerminalVelocity = 40;

            FuelRecoveryFactor = 0.2f;
            FuelConsumeFactor = 0.1f;
            FuelRecoveryDelay = 2;
        }

        void Start()
        {
            ThirdPersonCharacter = GetComponent<CustomThirdPersonCharacter>();
            RigidBody = GetComponent<Rigidbody>();
        }

        void FixedUpdate()
        {
            if (Input.GetKeyDown(KeyCode.Space))
                ToggleMode();

            if (TurboMode)
                UpdateTurboMode();
            else
                UpdateHoverMode();

            VerifyMaxVelocity();
            RecoverFuel();
            UIDebugger.Fuel = Fuel;
        }

        void ToggleMode()
        {
            TurboMode = !TurboMode;
            var angles = transform.localRotation.eulerAngles;
            if (TurboMode)
            {
                angles.x = 70;
                transform.localRotation = Quaternion.Euler(angles);
                GetComponent<CustomThirdPersonUserControl>().enabled = false;
                GetComponent<CustomThirdPersonCharacter>().enabled = false;
            }
            else
            {
                angles.x = 0;
                transform.localRotation = Quaternion.Euler(angles);
                GetComponent<CustomThirdPersonCharacter>().enabled = true;
                GetComponent<CustomThirdPersonUserControl>().enabled = true;
            }
        }

        void UpdateTurboMode()
        {
            if (ThirdPersonCharacter.m_IsGrounded || Fuel <= 0)
            {
                ToggleMode();
                return;
            }

            if (Input.GetAxis("RotateCharacter") != 0)
            {
                DierctionBothBoosters(Input.GetAxis("RotateCharacter"), 0);
            }
            else
            {
                DierctionBothBoosters(Input.GetAxis("Horizontal"), Input.GetAxis("Vertical"));
            }
            ConsumeFuel(0.5f, 0.5f);
            UpdateJetPack(Input.GetAxis("RightTurbine"), Input.GetAxis("LeftTurbine"));
            RigidBody.AddRelativeForce(Vector3.up * 20);
            RigidBody.AddForce(Vector3.up * 40);
        }

        void UpdateHoverMode()
        {
            if (ThirdPersonCharacter.m_IsGrounded)
            {
                if (ThirdPersonCharacter.m_Rigidbody.velocity.magnitude > 1)
                    DierctionBothBoosters(0, 1);
                else
                    DierctionBothBoosters(0, 0);
            }
            else
            {
                if (Input.GetAxis("RotateCharacter") != 0)
                {
                    DierctionBothBoosters(Input.GetAxis("RotateCharacter"), 0);
                }
                else
                {
                    var AbsHorizontal = Mathf.Abs(Input.GetAxis("Horizontal"));
                    if (AbsHorizontal > Mathf.Abs(Input.GetAxis("Vertical")))
                        DierctionBothBoosters(0, AbsHorizontal);
                    else
                        DierctionBothBoosters(0, Mathf.Abs(Input.GetAxis("Vertical")));
                    //Direction(-Input.GetAxis("Horizontal"), Input.GetAxis("Vertical"), TurbineLeft);
                    //Direction(-Input.GetAxis("Horizontal"), Input.GetAxis("Vertical"), TurbineRight);
                }
            }

            UpdateJetPack(Input.GetAxis("RightTurbine"), Input.GetAxis("LeftTurbine"));
        }

        void UpdateJetPack(float rightPower, float leftPower)
        {
            if (Fuel <= 0)
            {
                FireLeft.parent.localScale = FireRight.parent.localScale = new Vector3(1, 0, 1);
                return;
            }

            Vector3 turbineDirectionL = Vector3.zero;
            Vector3 turbineDirectionR = Vector3.zero;

            if (rightPower > 0)
            {
                turbineDirectionR = transform.InverseTransformDirection((TurbineRight.up + RightForceDirection).normalized) * (rightPower * TurbineMaxForce);

                FireRight.gameObject.GetComponent<MeshRenderer>().enabled = true;
                FireRight.parent.localScale = new Vector3(1, rightPower, 1);
            }
            else
                FireRight.gameObject.GetComponent<MeshRenderer>().enabled = false;

            if (leftPower > 0)
            {
                turbineDirectionL = transform.InverseTransformDirection((TurbineLeft.up + LeftForceDirection).normalized) * (leftPower * TurbineMaxForce);

                FireLeft.gameObject.GetComponent<MeshRenderer>().enabled = true;
                FireLeft.parent.localScale = new Vector3(1, leftPower, 1);
            }
            else
                FireLeft.gameObject.GetComponent<MeshRenderer>().enabled = false;

            if (rightPower != 0 || leftPower != 0)
                ConsumeFuel(rightPower, leftPower);
            UIDebugger.JetPackForce = (turbineDirectionR + turbineDirectionL);
            RigidBody.AddRelativeForce((turbineDirectionR + turbineDirectionL));
        }

        void DierctionBothBoosters(float horizonal, float vertical)
        {
            Direction(horizonal, vertical, TurbineRight);
            TurbineLeft.localRotation = TurbineRight.localRotation;
        }

        void Direction(float horizontal, float vertical, Transform transform)
        {
            float tiltAroundZ = horizontal * TurbineMaxAngle;
            float tiltAroundY = vertical * TurbineMaxAngle;
            Quaternion target = Quaternion.Euler(tiltAroundY, 0, tiltAroundZ);
            transform.localRotation = Quaternion.Slerp(transform.localRotation, target, 0.5f);
        }

        void Rotate()
        {
            transform.rotation *= Quaternion.AngleAxis(Input.GetAxis("RotateCharacter") * RotationVelocity, Vector3.up);
        }

        void ConsumeFuel(float rightPower, float leftPower)
        {
            FuelTimer = Time.time;
            Fuel = Mathf.Clamp01(Fuel - ((FuelConsumeFactor * leftPower) + (FuelConsumeFactor * rightPower)));
        }

        void RecoverFuel()
        {
            if (Time.time > FuelTimer + FuelRecoveryDelay)
                Fuel = Mathf.Clamp01(Fuel + FuelRecoveryFactor);
        }

        void VerifyMaxVelocity()
        {
            if (RigidBody.velocity.magnitude > MaxVelocity)
            {
                var oldY = Mathf.Clamp(RigidBody.velocity.y, -TerminalVelocity, 0);
                RigidBody.velocity = RigidBody.velocity.normalized * MaxVelocity;
                if (oldY < 0)
                    RigidBody.velocity = new Vector3(RigidBody.velocity.x, oldY, RigidBody.velocity.z);
            }
        }
    }
}