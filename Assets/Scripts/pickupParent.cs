using UnityEngine;
using System.Collections;

[RequireComponent(typeof(SteamVR_TrackedObject))]

public class pickupParent : MonoBehaviour {
	SteamVR_TrackedObject trackedObj;
	SteamVR_Controller.Device device;
    public Transform sphere;



	// Use this for initialization
	void Awake () {
		trackedObj = GetComponent<SteamVR_TrackedObject>();
	}

	// Update is called once per frame
	void FixedUpdate()
	{
		device = SteamVR_Controller.Input((int)trackedObj.index);
		if (device.GetTouch(SteamVR_Controller.ButtonMask.Trigger))
		{
			Debug.Log("You are holding 'Touch' on the Trigger.");
		}

		if (device.GetTouchDown(SteamVR_Controller.ButtonMask.Trigger))
		{
			Debug.Log("You activated TouchDown on the Trigger.");
		}

		if (device.GetTouchUp(SteamVR_Controller.ButtonMask.Trigger))
		{
			Debug.Log("You activated TouchUp on the Trigger.");
		}

		if (device.GetPress(SteamVR_Controller.ButtonMask.Trigger))
		{
			Debug.Log("You are holding 'Press' on the Trigger.");
            //device.TriggerHapticPulse();
        }

		if (device.GetPressDown(SteamVR_Controller.ButtonMask.Trigger))
		{
			Debug.Log("You activated PressDown on the Trigger.");
            

        }

		if (device.GetPressUp(SteamVR_Controller.ButtonMask.Trigger))
		{
			Debug.Log("You activated PressUp on the Trigger.");
		}

        if (device.GetPressUp(SteamVR_Controller.ButtonMask.Touchpad))
        {
            Debug.Log("You activated PressUp on the Touchpad.");
            sphere.transform.position = new Vector3(0, 0, 0);
            sphere.GetComponent<Rigidbody>().velocity = Vector3.zero;
            sphere.GetComponent<Rigidbody>().angularVelocity = Vector3.zero;
        }
    }

    void OnTriggerStay (Collider col)
    {
        Debug.Log("You have collided with " + col.name);
        if (device.GetTouch(SteamVR_Controller.ButtonMask.Trigger))
        {
            Debug.Log("You have collided with " + col.name + " while holding down Touch");
            col.attachedRigidbody.isKinematic = true;
            col.gameObject.transform.SetParent(this.gameObject.transform);

            //col.gameObject is the sphere while this.gameObject refers to who
            //the component is written under which is the controller

        }

        if (device.GetTouchUp(SteamVR_Controller.ButtonMask.Trigger))
        {
            Debug.Log("You have released Touch while colliding with" + col);
            col.gameObject.transform.SetParent(null);
            col.attachedRigidbody.isKinematic = false;
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