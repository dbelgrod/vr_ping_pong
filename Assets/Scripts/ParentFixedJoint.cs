using UnityEngine;
using System.Collections;

[RequireComponent(typeof(SteamVR_TrackedObject))]
public class ParentFixedJoint : MonoBehaviour
{
    SteamVR_TrackedObject trackedObj;
    SteamVR_Controller.Device device;
    Rigidbody rigidBodyAttachPoint;
    FixedJoint fixedJoint;

    
    void Start()
    {
        trackedObj = GetComponent<SteamVR_TrackedObject>();
        device = SteamVR_Controller.Input((int)trackedObj.index);
    }


    void FixedUpdate()
    {
     
    }


    void OnTriggerStay(Collider col)
    {
        if (device.GetPressUp(SteamVR_Controller.ButtonMask.Touchpad))
        {
            Object.Destroy(fixedJoint);
            fixedJoint = null;
            col.gameObject.transform.position = new Vector3(0, .2F, -1);
            col.gameObject.GetComponent<Rigidbody>().velocity = Vector3.zero;
            col.gameObject.GetComponent<Rigidbody>().angularVelocity = Vector3.zero;
        }
        if (fixedJoint == null && device.GetTouch(SteamVR_Controller.ButtonMask.Trigger))
        {
            fixedJoint = col.gameObject.AddComponent<FixedJoint>();
            rigidBodyAttachPoint = this.gameObject.GetComponent<Rigidbody>();
            //added the above myself, dont need because its public so we will pull it in
            //would be better for just any general rigidbody instead of having to fix one
            fixedJoint.connectedBody = rigidBodyAttachPoint;
        }
        else if (fixedJoint != null && device.GetTouchUp(SteamVR_Controller.ButtonMask.Trigger))
        {
            GameObject go = fixedJoint.gameObject;
            Rigidbody rigidBody = go.GetComponent<Rigidbody>();
            Object.Destroy(fixedJoint);
            fixedJoint = null;
            tossObject(col.attachedRigidbody);
        }
        else if (fixedJoint != null && device.GetTouch(SteamVR_Controller.ButtonMask.Trigger) != true)
        {
            GameObject go = fixedJoint.gameObject;
            Rigidbody rigidBody = go.GetComponent<Rigidbody>();
            Object.Destroy(fixedJoint);
            fixedJoint = null;
            tossObject(col.attachedRigidbody);
        }
       
    }

    void tossObject(Rigidbody rigidBody)
    {
        Transform origin = trackedObj.origin ? trackedObj.origin : trackedObj.transform.parent;
        if (origin != null)
        {
            rigidBody.velocity = origin.TransformVector(device.velocity);
            rigidBody.angularVelocity = origin.TransformVector(device.angularVelocity);
        }
        else
        {
            rigidBody.velocity = device.velocity;
            rigidBody.angularVelocity = device.angularVelocity;
        }
    }
}