using UnityEngine;
using System.Collections;

public class NPCBallManager : BallManager {

    // Use this for initialization
    void Start()
    {
       // GameObject.FindObjectOfType<SteamVR_ControllerManager>().GetComponentInChildren<BallManager>().gameObject
       
        trackedObj = GameObject.FindObjectOfType<SteamVR_ControllerManager>().GetComponentInChildren<BallManager>().gameObject.GetComponentInParent<SteamVR_TrackedObject>();
        device = SteamVR_Controller.Input((int)trackedObj.index);
        //if (GameObject.FindObjectOfType<SteamVR_ControllerManager>().GetComponentInChildren<BallManager>().ballPrefab != null)
         //currentBall = GameObject.FindObjectOfType<SteamVR_ControllerManager>().GetComponentInChildren<BallManager>().ballPrefab;
    }


    void FixedUpdate()
    {

        //FindObjectOfType<SteamVR_ControllerManager>().GetComponentInChildren<BallManager>().gameObject
        if (GameObject.Find("Ball(Clone)").GetComponent<Rigidbody>().isKinematic == true)
            currentBall = GameObject.Find("Ball(Clone)");

        speed = device.velocity;
        accel = Acceleration();

        b = currentBall.GetComponent<Rigidbody>();

        
        NPCCatchCol();
        CatchCol();

        //var g = transform.up * .2f;
        //force = new Vector3(g.x * accel.x, g.y * accel.y, g.z * accel.z);
    }

    private void NPCCatchCol() {
       
        //Debug.DrawRay(this.GetComponent<CapsuleCollider>().bounds.ClosestPoint(currentBall.transform.position), transform.forward);
        
        //Debug.Log(currentBall.transform.position);
        var dist = currentBall.transform.position - this.GetComponent<CapsuleCollider>().ClosestPointOnBounds(currentBall.transform.position);
        //dist = dist.normalized;

        var disp = this.GetComponent<CapsuleCollider>().ClosestPointOnBounds(currentBall.transform.position) - this.transform.position;

        Debug.DrawRay(this.GetComponent<CapsuleCollider>().ClosestPointOnBounds(currentBall.transform.position), dist);
        Debug.DrawRay(this.transform.position, disp, Color.red  );

        Vector3 closPoint = this.GetComponent<CapsuleCollider>().ClosestPointOnBounds(currentBall.transform.position);
        Debug.Log(hitObject);
        if (_hitObject != null)
        {
            var forPos = currentBall.transform.position + initVeloc * Time.fixedDeltaTime;
            var forVeloc = initVeloc + Physics.gravity * Time.fixedDeltaTime;
            _hitObject = null;
            Debug.Log("step1");
            for (int i = 1; i <= 10 * 90; i++)
            {
                forPos = currentBall.transform.position + forVeloc * Time.fixedDeltaTime;
                forVeloc = forVeloc + Physics.gravity * Time.fixedDeltaTime;
            }
            RaycastHit hit;
            if (Physics.Raycast(forPos, forVeloc, out hit, Mathf.Max(Mathf.Abs(forVeloc.x) * Time.fixedDeltaTime, Mathf.Abs(forVeloc.y) * Time.fixedDeltaTime, forVeloc.z * Time.fixedDeltaTime)))
            {
                this.transform.position = closPoint;
            }

        }
        //Debug.DrawRay(this.transform.position, this.transform.position - Vector3.forward, Color.blue);
    }


    protected override Vector3 Acceleration()
    {
        var g = base.Acceleration();
        return g;
    }

    protected override void CatchCol()
    {
        

        Bounds ballBounds = currentBall.GetComponent<SphereCollider>().bounds;
        Bounds racketBounds = FindObjectOfType<SteamVR_ControllerManager>().GetComponentInChildren<BallManager>().gameObject.GetComponent<BoxCollider>().bounds;

        b = currentBall.GetComponent<Rigidbody>();

        Vector3 g = transform.up;

        Vector3 displacement = racketBounds.ClosestPoint(currentBall.transform.position) - currentBall.transform.position;
        float distance = displacement.sqrMagnitude;

        float length = Mathf.Max((speed.x + Mathf.Max(acceleration.x, 0) * Time.fixedDeltaTime) * 1f / 90f, (speed.y + Mathf.Max(acceleration.y, 0) * Time.fixedDeltaTime) * 1f / 90f, (speed.z + Mathf.Max(acceleration.z, 0) * Time.fixedDeltaTime) * 1f / 90f);

        //Debug.DrawRay(racketBounds.ClosestPoint(currentBall.transform.position), transform.up);

        //adjust length for the balls velocity downward
        //reverse ball velocity on this short distance occurring
        if (length * length > distance)
        {
            b.isKinematic = false;
            b.maxAngularVelocity = 10000f;
            Vector3 bVelocity = speed + acceleration * Time.fixedDeltaTime;
            Vector3 bVelocityNeut = speed;
            if (acceleration.y < 0 || acceleration.x < 0 || acceleration.z < 0)
            {
                if (speed.x < 0)
                    speed.x = speed.x * -1f;
                if (speed.y < 0)
                    speed.y = speed.y * -1f;
                if (speed.z < 0)
                    speed.z = speed.z * -1f;
                bVelocity = speed + acceleration * Time.fixedDeltaTime;
                //b.AddForce(g.x * bVelocity.x, g.y * bVelocity.y, g.z * bVelocity.z);
                initPos = currentBall.transform.position + initVeloc * Time.fixedDeltaTime;
                initVeloc = new Vector3(g.x * bVelocity.x, g.y * bVelocity.y, g.z * bVelocity.z) + Physics.gravity * Time.fixedDeltaTime;

            }
            count = count + 1;


        }
        else if (count > 0)
        {
            //projecting frame by frame
            RaycastHit hit;
            if (Physics.Raycast(initPos, initVeloc, out hit, Mathf.Max(Mathf.Abs(initVeloc.x) * Time.fixedDeltaTime, Mathf.Abs(initVeloc.y) * Time.fixedDeltaTime, initVeloc.z * Time.fixedDeltaTime)))
            {
                if (hit.collider.name != "Ball(Clone)") //&& hit.collider.name != "Cylinder")
                {
                    _hitObject = hit.collider;
                   // Debug.Log(hit.collider.name);
                    // initPos = Vector3.zero;
                    initVeloc = Vector3.Reflect(initVeloc, hit.normal);
                }

            }
            else
            {
                initPos = currentBall.transform.position + initVeloc * Time.fixedDeltaTime;
                initVeloc = initVeloc + Physics.gravity * Time.fixedDeltaTime;


            }
        }

    }

   
  
}
