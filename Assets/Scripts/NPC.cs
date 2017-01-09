using UnityEngine;
using System.Collections;

public class NPC : MonoBehaviour {

    // Use this for initialization
    private SteamVR_ControllerManager camRig;
    private GameObject camHead;
    private GameObject currentBall;
	void Start () {
        
    }
	
	// Update is called once per frame
	void FixedUpdate () {
        if (Time.time < Time.fixedDeltaTime * 180f)
        {
            camRig = FindObjectOfType<SteamVR_TrackedObject>().GetComponentInParent<SteamVR_ControllerManager>();
            GameObject.Find("Camera (head)").transform.localPosition = camRig.gameObject.GetComponentInChildren<Camera>().gameObject.transform.localPosition;

            this.GetComponent<SteamVR_ControllerManager>().left.transform.localPosition = new Vector3(.3f, -.5f, 0);
            this.GetComponent<SteamVR_ControllerManager>().right.transform.localPosition = new Vector3(-.3f, -.5f, 0);

            
            


        }
        //currentBall = FindObjectOfType<NPCBallManager>().currentBall;
        if (currentBall != null)
        {
           // Vector3 relativePos = currentBall.transform.position;
           // Quaternion rotation = Quaternion.LookRotation(relativePos);
            //transform.rotation = rotation;
        }
    }
}
