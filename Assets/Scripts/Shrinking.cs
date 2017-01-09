using UnityEngine;
using System.Collections;


public class Shrinking : MonoBehaviour {
    SteamVR_TrackedObject trackedObj;
    SteamVR_Controller.Device device1;
    SteamVR_Controller.Device device2;
    SteamVR_ControllerManager HMD;
    Transform sizeObj;
    EnemyHealth enem = new EnemyHealth();
    int i = 0;
    

    void Awake () {

        var controllerManager = GameObject.FindObjectOfType<SteamVR_ControllerManager>();
        trackedObj = controllerManager.left.GetComponent<SteamVR_TrackedObject>();
        //trackedObj = leftHand.GetComponent<SteamVR_TrackedObject>();
        sizeObj = GetComponent<Transform>();
    }
	
	void FixedUpdate () {
        //device = SteamVR_Controller.Input((int)trackedObj.index);
       
        //Shrink(trackedObj);
        
    }
    void Update()
            {
        device1 = SteamVR_Controller.Input((int)trackedObj.index);
        if (device1.GetPressDown(SteamVR_Controller.ButtonMask.Grip))
            Shrink();
    }
    

    void Shrink()
    {

        //{
        //    Debug.Log(HMD.name);
        //    Debug.Log("You are shrinking");
        //    trackedObj.GetComponent<Transform>().localScale = new Vector3(1, 1, 1) * .4F;
        //}
        //else
        //    sizeObj.localScale = leftHand.GetComponent<Transform>().localScale;
        if (sizeObj.localScale == Vector3.one)
        {
            sizeObj.transform.position = this.transform.position;
            sizeObj.localScale = new Vector3(1, 1, 1) * .045F;
            
        }
        
        else
        {
            sizeObj.transform.position = this.transform.position;
            sizeObj.localScale = new Vector3(1, 1, 1);

        }

    }

}
