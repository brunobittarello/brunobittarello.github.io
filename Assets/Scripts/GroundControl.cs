using UnityEngine;

namespace Assets.Scripts
{
    class GroundControl : MonoBehaviour
    {
        Rigidbody RigidBody;
        bool IsGrounded;

        public float MoveForce;
        public float RotationVelocity;
        public float MaxMoveVelocity;
        public float JumpForce;

        public GroundControl()
        {
            MoveForce = 3;
            RotationVelocity = 3;
            MaxMoveVelocity = 15;
            JumpForce = 45;
        }

        void Start()
        {
            RigidBody = GetComponent<Rigidbody>();
        }

        void FixedUpdate()
        {
            if (IsGrounded == false)
                return;

            if (Input.GetAxis("Vertical") != 0)
                Move();

            if (Input.GetAxis("Horizontal") != 0)
                Rotate();

            if (Input.GetAxis("Jump") > 0)
                Jump();
        }

        void Move()
        {
            if (RigidBody.velocity.magnitude > MaxMoveVelocity)
                return;

            //var direction = new Vector3(Input.GetAxis("Horizontal"), 0, Input.GetAxis("Vertical"));
            RigidBody.AddRelativeForce(Input.GetAxis("Vertical") * Vector3.forward * MoveForce);
            
        }

        void Rotate()
        {
            transform.rotation *= Quaternion.AngleAxis(Input.GetAxis("Horizontal") * RotationVelocity, Vector3.up);
            /*
            Quaternion target = Quaternion.Euler(0, Input.GetAxis("Horizontal") * 30, 0);
            transform.localRotation = Quaternion.Slerp(transform.localRotation, target, 2);
             */
        }

        void Jump()
        {
            RigidBody.AddRelativeForce(transform.up * JumpForce);
        }

        void OnCollisionEnter(Collision collision)
        {
            if (collision.gameObject.tag == "Terrain")
                IsGrounded = true;
        }

        void OnCollisionExit(Collision collisionInfo)
        {
            if (collisionInfo.gameObject.tag == "Terrain")
                IsGrounded = false;
        }
    }
}
