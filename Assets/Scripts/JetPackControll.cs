using UnityEngine;
using System.Collections;

public class JetPackControll : MonoBehaviour
{
    public Rigidbody TurbineRight;
    public Rigidbody TurbineLeft;

    public Transform FireRight;
    public Transform FireLeft;

    Rigidbody RigidBody;

    // Use this for initialization
    void Start()
    {
        RigidBody = GetComponent<Rigidbody>();
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        //Direction(Input.GetAxis("Horizontal"), Input.GetAxis("Vertical"), TurbineLeft.transform);
        Direction(Input.GetAxis("RightAxisHorizontal"), Input.GetAxis("RightAxisVertical"), TurbineRight.transform);

        UpdateJetPack(Input.GetAxis("RightTurbine"), TurbineRight, FireRight);
        UpdateJetPack(Input.GetAxis("LeftTurbine"), TurbineLeft, FireLeft);
    }

    void UpdateJetPack(float force, Rigidbody body, Transform fire)
    {
        if (force > 0)
            body.AddRelativeForce(Vector3.up * 5 * force);

        if (force == 0)
            fire.gameObject.GetComponent<MeshRenderer>().enabled = false;
        else
        {
            fire.gameObject.GetComponent<MeshRenderer>().enabled = true;
            fire.parent.localScale = new Vector3(1, force, 1);
        }
    }

    void Direction(float horizontal, float vertical, Transform transform)
    {
        Debug.Log("horizontal:" + horizontal + " - vertical: " + vertical);
        float tiltAroundZ = horizontal * 30;
        float tiltAroundY = vertical * 30;
        Quaternion target = Quaternion.Euler(tiltAroundY, 0, tiltAroundZ);
        transform.rotation = Quaternion.Slerp(transform.rotation, target, 2);
        /*
        var joint = transform.gameObject.GetComponent<HingeJoint>();
        joint.motor.
         * */
    }
}
