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
        public Transform FireCenter;

        Rigidbody RigidBody;
        public Vector3 RightForceDirection;
        public Vector3 LeftForceDirection;

        public float TurbineMaxAngle;
        public float TurbineMaxForce;
        public float RotationVelocity;

        public float MaxVelocity;
        public float MaxVelocityTurboMode;
        public float TerminalVelocity;
        public float MinimumYPosition;

        public float FuelRecoveryFactor;
        public float FuelConsumeFactor;
        public float FuelRecoveryDelay;
        float Fuel;
        float FuelTimer;

        public bool TurboMode;

        public float TurboFuelRecoveryFactor;
        public float TurboFuelConsumeFactor;
        public float TurboFuelRecoveryDelay;
        float TurboFuel;
        float TurboFuelTimer;

        public float ChangeModeDelay;
        float ChangeModeTimer;

        public bool EnableTurboMode;

        public JetPackControll2()
        {
            RightForceDirection = new Vector3(-0.4f, 0.6f, 0);
            LeftForceDirection = new Vector3(0.4f, 0.6f, 0);
            TurbineMaxAngle = 30;
            TurbineMaxForce = 5;
            RotationVelocity = 3;
            MaxVelocity = 70;
            TerminalVelocity = 40;
            MinimumYPosition = -100;

            FuelRecoveryFactor = 0.2f;
            FuelConsumeFactor = 0.1f;
            FuelRecoveryDelay = 2;

            TurboFuelRecoveryFactor = 0.2f;
            TurboFuelConsumeFactor = 0.1f;
            TurboFuelRecoveryDelay = 2;

            ChangeModeDelay = 1;
        }

        void Start()
        {
            ThirdPersonCharacter = GetComponent<CustomThirdPersonCharacter>();
            RigidBody = GetComponent<Rigidbody>();
            FireCenter.gameObject.GetComponent<MeshRenderer>().enabled = false;

            if (transform.position.y < MinimumYPosition)
            {
                Debug.LogError("Minimun Y position can be less than start Y. Arruma isso Natanael!");
                MinimumYPosition = transform.position.y - 100;
            }
        }

        void FixedUpdate()
        {
            if (TurboMode)
                UpdateTurboMode();
            else
                UpdateHoverMode();

            VerifyMinimumPosition();
            VerifyMaxVelocity();
            RecoverFuel();
            UIDebugger.Fuel = Fuel;
            UIDebugger.TurboFuel = TurboFuel;
        }

        void ToggleMode()
        {
            TurboMode = !TurboMode;
            var angles = transform.localRotation.eulerAngles;
            if (TurboMode)
            {
                angles.x = 70;
                transform.localRotation = Quaternion.Euler(angles);
                FireCenter.parent.localScale = new Vector3(1, 0, 1);
                FireCenter.gameObject.GetComponent<MeshRenderer>().enabled = true;
                GetComponent<CustomThirdPersonUserControl>().enabled = false;
                GetComponent<CustomThirdPersonCharacter>().enabled = false;
            }
            else
            {
                angles.x = 0;
                transform.localRotation = Quaternion.Euler(angles);
                FireCenter.gameObject.GetComponent<MeshRenderer>().enabled = false;
                TurboFuelTimer = Time.time;
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

            if (Input.GetAxis("Horizontal") != 0 || Input.GetAxis("Vertical") != 0)
                TurboModeRotation(Input.GetAxis("Horizontal"), Input.GetAxis("Vertical"));


            if (FireCenter.parent.localScale.y != 1)
                FireCenter.parent.localScale = new Vector3(1, Mathf.Lerp(FireCenter.parent.localScale.y, 1, 0.08f), 1);

            DierctionBothBoosters(-Input.GetAxis("Horizontal"), Input.GetAxis("Vertical"));
            //Rotate();
            ConsumeFuel(0.5f, 0.5f);
            UpdateJetPack(Input.GetAxis("RightTurbine"), Input.GetAxis("LeftTurbine"));
            RigidBody.AddRelativeForce(Vector3.up * 20);
            RigidBody.AddForce(Vector3.up * 25);

            if (Input.GetAxis("L1") == 0 || Input.GetAxis("R1") == 0)
                ToggleMode();
        }

        private void TurboModeRotation(float horizontal, float vertical)
        {
            var m_CamForward = Vector3.Scale(Camera.main.transform.forward, new Vector3(1, 0, 1)).normalized;
            var m_Move = vertical * m_CamForward + horizontal * Camera.main.transform.right;
            m_Move = transform.InverseTransformDirection(m_Move);
            m_Move = Vector3.ProjectOnPlane(m_Move, Vector3.up);
            var m_TurnAmount = Mathf.Atan2(m_Move.x, m_Move.z);
            Debug.Log(m_TurnAmount);
            //transform.Rotate(0, m_TurnAmount * 0.5f, 0, Space.Self);

            //Debug.Log(m_Move);

            var rotation = transform.rotation.eulerAngles;
            rotation.y += m_TurnAmount;
            rotation.z = Mathf.Clamp(rotation.z + Mathf.Atan2(m_Move.x, m_Move.y), -(Mathf.Deg2Rad * 20), (Mathf.Deg2Rad * 20));
            transform.rotation = Quaternion.Slerp(transform.rotation, Quaternion.Euler(rotation), 0.1f);
        }

        void ChangeModeManager()
        {
            if (EnableTurboMode && !ThirdPersonCharacter.m_IsGrounded && Input.GetAxis("L1") != 0 && Input.GetAxis("R1") != 0)
            {
                ChangeModeTimer += Time.fixedDeltaTime;
                if (ChangeModeTimer > ChangeModeDelay)
                {
                    ChangeModeTimer = 0;
                    ToggleMode();
                }
            }
            else
                ChangeModeTimer = 0;
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
            ChangeModeManager();
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
            transform.localRotation *= Quaternion.AngleAxis(-Input.GetAxis("Horizontal"), Vector3.forward);
        }

        void ConsumeFuel(float rightPower, float leftPower)
        {
            if (TurboMode)
                TurboFuel = Mathf.Clamp01(TurboFuel - ((TurboFuelConsumeFactor * leftPower) + (TurboFuelConsumeFactor * rightPower)));
            else
            {
                FuelTimer = Time.time;
                Fuel = Mathf.Clamp01(Fuel - ((FuelConsumeFactor * leftPower) + (FuelConsumeFactor * rightPower)));
            }
        }

        void RecoverFuel()
        {
            if (Time.time > FuelTimer + FuelRecoveryDelay)
                Fuel = Mathf.Clamp01(Fuel + FuelRecoveryFactor);

            if (TurboMode == false && Time.time > TurboFuelTimer + TurboFuelRecoveryDelay)
                TurboFuel = Mathf.Clamp01(TurboFuel + TurboFuelRecoveryFactor);
        }

        void VerifyMinimumPosition()
        {
            if (transform.position.y < MinimumYPosition)
                Application.LoadLevel(Application.loadedLevel);
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