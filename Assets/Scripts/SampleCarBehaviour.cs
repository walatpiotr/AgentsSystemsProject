using System.Collections.Generic;
using System.Linq;
using System;
using UnityEngine;
using TMPro;

public class SampleCarBehaviour : MonoBehaviour
{
    public enum VehicleType
    {
        Car,
        Truck
    }
    public VehicleType vehicleType;
    public float xLocation;
    public float yLayer;
    public float acceleration;
    public float maxTargetVelocity;
    public TextMeshProUGUI velocityText;
    public BoxCollider2D collider;
    public enum Directions
    {
        Right,
        Left
    }
    public Directions direction;
    public GameObject pathObjectToFollow;
    public List<Transform> listOfPoints;
    public Transform target;
    public float targetX;
    public int nextNode;
    public string finalDestination;

    private float velocityMetersPerSecond;
    private float nextWantedLane;
    private bool exitImminent;
    private bool wantLineChange;
    private bool lockLaneChange;
    private const float UNIFIED_SPACING = 10f;

    private float timeInRandomBreaking;

    // Start is called before the first frame update
    void Start()
    {
        

        xLocation = transform.position.x;
        yLayer = transform.position.y;

        if(direction == Directions.Right)
        {
            transform.rotation = Quaternion.Euler(0, 0, -90f);
        }
        else
        {
            transform.rotation = Quaternion.Euler(0, 0, 90f);
        }

        if(velocityMetersPerSecond > 340f || velocityMetersPerSecond < 0f)
        {
            velocityMetersPerSecond = 0f;
        }

        timeInRandomBreaking = 0f;

        wantLineChange = false;
        exitImminent = false;
        lockLaneChange = false;
    }

    private void FixedUpdate()
    {
        if(wantLineChange)
        {
            timeInRandomBreaking = 0f;
        }
        if(timeInRandomBreaking == 0f && !wantLineChange)
        {
            Accelerate();
        }
        SlowDown();
        Move();
        if(!lockLaneChange)
        {
            LaneChangeDecission();
            if (wantLineChange)
            {
                ChangeLane(nextWantedLane);
            }
            PrintVelocity();
        }
        targetX = target.position.x;
    }

    public void SetVelocity(float velocityInKilometers)
    {
        velocityMetersPerSecond = velocityInKilometers / 3.6f;
    }

    public float GetVelocity()
    {
        return velocityMetersPerSecond * 3.6f;
    }

    private void Accelerate()
    {
        float maxVelocityInMeters = maxTargetVelocity / 3.6f;
        if(velocityMetersPerSecond < maxVelocityInMeters)
        {
            if(velocityMetersPerSecond + UNIFIED_SPACING * Time.deltaTime <= maxVelocityInMeters)
            {
                velocityMetersPerSecond += UNIFIED_SPACING * Time.deltaTime;
            }
            else
            {
                velocityMetersPerSecond = maxVelocityInMeters;
            }
        }
    }

    private void SlowDown()
    {
        // Part 1: Avoid collision
        RaycastHit2D detected;
        try
        {
            detected = DetectCars();
            // set velocity so the distance stays consistent with the law
            if(detected.distance - GetVelocity() > 0)
            {
                velocityMetersPerSecond = (detected.distance - GetVelocity()) / Time.deltaTime;
            }
            else
            {
                velocityMetersPerSecond = UNIFIED_SPACING;
            }            
        } catch(Exception e) {
            if(!e.Message.Equals("Undetected"))
            {
                throw e;
            }
        }

        // Part 2: Randomize
        // if speed higher than one unit (unified spacing)
        if(!(wantLineChange) && (velocityMetersPerSecond > UNIFIED_SPACING || timeInRandomBreaking > 0f))
        {
            if(timeInRandomBreaking == 0f && UnityEngine.Random.value < 0.0003)
            {
                timeInRandomBreaking += Time.deltaTime;
            }
            if(timeInRandomBreaking > 0f && timeInRandomBreaking < 1f)
            {
                velocityMetersPerSecond -= UNIFIED_SPACING * Time.deltaTime;
                timeInRandomBreaking += Time.deltaTime;
            }
            else if(timeInRandomBreaking >= 1f || velocityMetersPerSecond < UNIFIED_SPACING)
            {
                timeInRandomBreaking = 0f;
            }
        }
    }

    private void Move()
    {
        float step = velocityMetersPerSecond * Time.deltaTime; // calculate distance to move

        transform.position = Vector3.MoveTowards(transform.position, target.position, step);

        if ((transform.position == target.position) && (((nextNode+1) < listOfPoints.Count) || (nextNode - 1) == -1))
        {
            if(direction == Directions.Left)
            {
                nextNode -= 1;
            }
            else
            {
                nextNode += 1;
            }
            target = listOfPoints.ElementAt(nextNode);
        }
        if ((transform.position == target.position) && (((nextNode+1) == listOfPoints.Count)||(nextNode-1) == -1))
        {
            // if not current lane type is Exit
            if(pathObjectToFollow.GetComponent<PointCreator>().type == PointCreator.LaneType.Road)
            {
                // TODO
                // search for new lane and setupLane
            }
            else
            {
                // deinstantiate
                Destroy(this);
            }
        }
    }

    private RaycastHit2D DetectCars()
    {
        Vector2 convertedDirection = direction == Directions.Right ? Vector2.right : Vector2.left;
        Vector2 offset = vehicleType == VehicleType.Car ? convertedDirection * 4 : convertedDirection * 20;
        Vector3 offset3 = new Vector3(offset.x, offset.y, 0);
        RaycastHit2D hit = Physics2D.Raycast(transform.position + offset3, convertedDirection);
        // if another car is closer than one step of movement + free space required by law
        Debug.DrawLine(transform.position, transform.position + offset3, Color.magenta);
        if((hit.collider != null) && (hit.collider.tag == "car" || hit.collider.tag == "truck") && (hit.distance < velocityMetersPerSecond * Time.deltaTime + GetVelocity()))
        {
            return hit;
        }
        else
        {
            throw new Exception("Undetected");
        }
    }

    private void ChangeLane(float yLayer)
    {
        // 1. calculate accurate distance in which car wants to change line (once)
        // 2. check if there are cars that ride on this lane
        // 3. check if they are in a safe distance
        // 4. move towards established point in new lane
        // 5. when in target position
        //      a. change list of points and establish index of current point
        //      b. set wantLineChange to false
        //      c. if exitImminent and new lane is exit:
        //          - set lockLaneChange to true
    }

    private void LaneChangeDecission()
    {
        // 1. check if already changing
        if(wantLineChange == true)
        {
            return;
        }
        // 2. decide if you want to change lane:
        //      a. there's a slow car in front
        //      b. you can come back to 'slower' lane
        //      c1. next exit is your exit
        //      c2. you have spotted your exit
        
        // if(prev_c1)
        if(exitImminent)
        {
            // c2
            bool exitSpotted = SearchForExit();

            // TODO
            // 3. check if you're on right-most lane
            rightmost = false;
            // if c2 or (not c2 and not rightmost)
            if(exitSpotted != -1 || !rightmost)
            {
                wantLineChange = true;
                // set nextWantedLane
            }          
        }
        else
        {
            // calculate a, b, c1
            //if(a || b || c1)
            wantLineChange = true;
            // 3. decide which line is your target lane: set nextWantedLane
            //if c1 set exitImminent true
        }
    }

    private bool SearchForExit()
    {
        // if next exit is not yours, don't look for exit
        if(!exitImminent)
        {
            return false;
        }
        // 1. Raycast {on higher Z layer} 90 degrees to the right
        // 2. If collider detected and collider is exitPrefab collider:
        //      4. return true
        return false;
    }

    private void PrintVelocity()
    {
        velocityText.text = GetVelocity().ToString() + "km/h";
    }

    public void SetUpLane(Transform nearestPoint, int index)
    {
        pathObjectToFollow = nearestPoint.parent.gameObject;
        foreach (Transform child in pathObjectToFollow.transform)
        {
            listOfPoints.Add(child);
        }
        nextNode = index+1;
    }
}
