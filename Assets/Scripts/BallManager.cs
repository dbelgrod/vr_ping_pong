/*
    Copyright (C) 2016 FusionEd

    This program is free software: you can redistribute it and/or modify
    it under the terms of the GNU General Public License as published by
    the Free Software Foundation, either version 3 of the License, or
    (at your option) any later version.

    This program is distributed in the hope that it will be useful,
    but WITHOUT ANY WARRANTY; without even the implied warranty of
    MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
    GNU General Public License for more details.

    You should have received a copy of the GNU General Public License
    along with this program.  If not, see <http://www.gnu.org/licenses/>.
 */

//going to try to make this easy to read
using UnityEngine;
using System.Collections;
public class BallManager : MonoBehaviour
{

    public static BallManager Instance;

    public SteamVR_TrackedObject trackedObj;

//as we keep spawning new ping pong balls, each time we spawn we have a currentBall
//there will be many balls but only one currentBall
    protected GameObject currentBall;
    protected Vector3 distance;

//we dont want our currentBall to collide and get stuck midframe inside our racket
//so we want to find acceleration (velocity2-velocity1) / (time2-time1)

//to get velocity we need distance velocity2 = ((distance2-distance1) / (time2-time1))

//so we essentially need 3 frames to occur to even begin calculating acceleration
//if we velocity1 then we have data from time0,time1, and time2
    protected Vector3 lastVelocity;
    protected Vector3 lastPosition1;

    protected float curTime;
    protected Vector3 accel;
    protected Vector3 lastPos;
	
//ignore minimumExtent for now
    protected float minimumExtent;
  
 //ballPrefab is 
    public GameObject ballPrefab;

    protected Rigidbody b;
    protected Rigidbody g;
    protected Vector3 force;
    protected Vector3 curVelocity;
    protected Vector3 speed;
    protected Vector3 acceleration;
    protected Vector3 initVeloc;
    protected Vector3 initPos;

    protected bool isAttached = false;
    protected int count = 0;


    protected SteamVR_Controller.Device device;

    public GameObject CurrentBall
    {
        
        get { return currentBall; }

    }

    public Vector3 InitVeloc {
        get { return initVeloc; }
    }


    void OnDestroy()
    {
        if (Instance == this)
            Instance = null;
    }

    // Use this for initialization
	//this starts before anything
    void Start()
    {
        //we are at time 0
        curTime = 0.0f;
		//we say the ball starts in (0,0,0) in 3D
        lastPosition1 = Vector3.zero;
		//our trackedObj is going to be our right hand where we hold the racket
        trackedObj = this.GetComponentInParent<SteamVR_TrackedObject>();
		//device takes trackedObj and is able to say this is the right controller
        device = SteamVR_Controller.Input((int)trackedObj.index);
        
		//ignore for now
        if (currentBall != null)
        minimumExtent = Mathf.Min(Mathf.Min(currentBall.GetComponent<Collider>().bounds.extents.x, currentBall.GetComponent<Collider>().bounds.extents.y), currentBall.GetComponent<Collider>().bounds.extents.z);
    }
	
	//this occurs after Start
    void Awake()
    {
        
        if (Instance == null)
            Instance = this;
        
    }

//since we are dealing with physics, we do FixedUpdate to look do calclulations
//every 1/90 of a sec 
    void FixedUpdate()
    {
        
        AttachArrow();
		//we end up not using position because SteamVR gives us the velocity
        speed = device.velocity;
		//this is a useless function since we basically return speed which is above
        accel = Acceleration();
       
        //we are going to use speed to determine collisions, it was much
		//smoother than using acceleration because it changed constantly
		//and the ball would have volatile movement
		
		//b is the rigidBody ie how ball intercts with forces, collisions
        b = currentBall.GetComponent<Rigidbody>();
        CatchCol();
        //simulatePath();
        var g = transform.up * .2f;
        force = new Vector3(g.x * accel.x, g.y * accel.y, g.z * accel.z);

        lastPosition1 = this.transform.position;

		//if the ball is ever below .2meters and im pressing the trigger
		//I reset the currentBall 
        if (currentBall.transform.position.y < .2f)
            if (device.GetPress(SteamVR_Controller.ButtonMask.Trigger))
        {
   
            currentBall = null;
            count = 0;
        }

       

    }

	//dont bother reading since we dont use it 
    protected virtual Vector3 Acceleration() {
        var b = FindObjectOfType<SteamVR_ControllerManager>().GetComponentInChildren<BallManager>().gameObject.GetComponent<Rigidbody>();
        curVelocity = (FindObjectOfType<SteamVR_ControllerManager>().GetComponentInChildren<BallManager>().gameObject.transform.position - lastPosition1) / Time.fixedDeltaTime;
        distance = FindObjectOfType<SteamVR_ControllerManager>().GetComponentInChildren<BallManager>().gameObject.transform.position - lastPosition1;
        if (curVelocity.x < 0)
            curVelocity.x = curVelocity.x * -1f;
        if (curVelocity.y < 0)
            curVelocity.y = curVelocity.y * -1f;
        if (curVelocity.z < 0)
            curVelocity.z = curVelocity.z * -1f;     
        acceleration = (speed - lastVelocity) / Time.fixedDeltaTime; 
        lastVelocity = speed;
       
        return speed;
    }

    protected virtual void CatchCol()
    {
  
		//although we have might have a ping pong ball as a sphere
		//we can make it act like a box by giving it a BoxCollider
		//that means if we pushed the ball it would just flip onto its side like a box
		//we use different shape colliders than the object because its costly for the computer
		//here our ball has a SphereCollider and our racket has a MeshCollider
		//ie we fit our collider around the shape of the racket, this case was not costly
        Bounds ballBounds = currentBall.GetComponent<SphereCollider>().bounds;
      
        Bounds racketBounds = this.GetComponent<MeshCollider>().bounds;
		
		
        b = currentBall.GetComponent<Rigidbody>();
		//g is (0,1,0) as in pointing straight up
		
        Vector3 g = transform.up;
        
        
        //we are looking at at the ball position and finding the difference btwn that
		//and the bound on the ball that is closet to the racket's bounds 
        Vector3 displacement = currentBall.transform.position - racketBounds.ClosestPoint(currentBall.transform.position);
        
		//we square that value
		float distance = displacement.sqrMagnitude;
		
		//length is the rackets speed + the calculated change in velocity
		//we are looking at each the x,y,z predicted speed in next frame
		//and using the max as length 
        float length = Mathf.Max( (speed.x+ Mathf.Max(acceleration.x,0)*Time.fixedDeltaTime) * 1f / 90f, (speed.y + Mathf.Max(acceleration.y,0) * Time.fixedDeltaTime) * 1f / 90f, (speed.z+ Mathf.Max(acceleration.z, 0) * Time.fixedDeltaTime) * 1f / 90f);
		//ignore 2 comments below, they are for me
        //adjust length for the balls velocity downward
        //reverse ball velocity on this short distance occurring
		
		//if the distance we will travel in a frame is greater than the distance
		//btwn the ball and the racket at closest bounds 
        if (length * length > distance)
        {
			
            b.isKinematic = false;
            b.maxAngularVelocity = 10000f;
			//we set b to the velocity and change in velocity as our racket 
            Vector3 bVelocity = speed + acceleration * Time.fixedDeltaTime;
            Vector3 bVelocityNeut = speed;
            
			//when our hand slows down swinging we will set b to that speed and
			//let gravity apply onto it 
            if (acceleration.y < 0 || acceleration.x < 0 || acceleration.z < 0)
            {
                if (speed.x < 0)
                    speed.x = speed.x * -1f;
                if (speed.y < 0)
                    speed.y = speed.y * -1f;
                if (speed.z < 0)
                    speed.z = speed.z * -1f;
                bVelocity = speed + acceleration * Time.fixedDeltaTime; 
                b.AddForce(g.x * bVelocity.x, g.y * bVelocity.y, g.z * bVelocity.z);
                initPos = currentBall.transform.position + initVeloc * Time.fixedDeltaTime;
                initVeloc = new Vector3(g.x * bVelocity.x, g.y * bVelocity.y, g.z * bVelocity.z) + Physics.gravity * Time.fixedDeltaTime;
                
            }
            else
            {
				//otherwise, we keep applying a force on b that is the same as our rackets velocity
                b.AddForce(g.x * bVelocity.x, g.y * bVelocity.y, g.z * bVelocity.z);
                

            }
            count = count + 1;


        }
		//ignore this portion
        else if (count > 0)
        {
            //projecting frame by frame
                RaycastHit hit;
            if (Physics.Raycast(initPos, initVeloc, out hit, Mathf.Max(Mathf.Abs(initVeloc.x) * Time.fixedDeltaTime, Mathf.Abs(initVeloc.y) * Time.fixedDeltaTime, initVeloc.z * Time.fixedDeltaTime)))
                {
                if (hit.collider.name != "Ball(Clone)") //&& hit.collider.name != "Cylinder")
                {
                    //_hitObject = hit.collider;
                    Debug.Log(hit.collider.name);
                    // initPos = Vector3.zero;
                    initVeloc = Vector3.Reflect(initVeloc, hit.normal); 
                }

            }
            else
                {
                    initPos = currentBall.transform.position + initVeloc * Time.fixedDeltaTime;
                    initVeloc = initVeloc + Physics.gravity * Time.fixedDeltaTime;
                    

                }
                // this.GetComponent<Collider>().enabled = false;
                //simulatePath();
                //this.GetComponent<Collider>().enabled = true;

            
        }
       
    }

          private void AttachArrow()
          {
			  
              if (currentBall == null)
              {
				  
				//create an instance of ballPrefab (ball)
				currentBall = Instantiate(ballPrefab);
				//ignore
				currentBall.GetComponent<Rigidbody>().sleepThreshold = 0f;
				//put the ball in position (-1.5, 1, -.26), this can change
				//I have it at this because it appears right in front of me
				currentBall.transform.position = new Vector3(-1.5f, 1f,- .26f);
				//ignore
				currentBall.transform.rotation = Quaternion.identity;
				//we are looking at the ball and its RigidBody ie how the ball
				//interacts with gravity, with collisions with other objects
				//isKinematic = true means that nothing like gravity or collisions will affect the ball
				//this is why it is still in middair
				currentBall.GetComponent<Rigidbody>().isKinematic = true;
        }
        
        
    }
	//ignore below 
    // Reference to the LineRenderer we will use to display the simulated path


    // Number of segments to calculate - more gives a smoother line
    public int segmentCount = 20;

    // Length scale for each segment
    public float segmentScale = 1;

    // gameobject we're actually pointing at (may be useful for highlighting a target, etc.)
    protected Collider _hitObject;
    public Collider hitObject { get { return _hitObject; } }
    
   
}
