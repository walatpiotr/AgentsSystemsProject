using System.Collections.Generic;
using System.Linq;
using System;
using UnityEngine;
using TMPro;
using System.Threading.Tasks;
using Assets.Scripts;

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
    private bool exitImminent;
    private bool wantLineChange;
    private bool lockLaneChange;
    private const float UNIFIED_SPACING = 10f;

    private float timeInRandomBreaking;

    private bool rightmost;
    private Transform laneChangingTarget;

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

    private async Task Move()
    {
        float step = velocityMetersPerSecond * Time.deltaTime; // calculate distance to move

        transform.position = Vector3.MoveTowards(transform.position, target.position, step);

        if((transform.position == target.position) && ((direction == Directions.Left && nextNode == 0) || (direction == Directions.Right && nextNode == listOfPoints.Count - 1)))
        {
            var currLane = pathObjectToFollow.GetComponent<PointCreator>();
            
            if(currLane.type == PointCreator.LaneType.Exit || (currLane.type == PointCreator.LaneType.Road && ((currLane.description == "KAT" && finalDestination == "KAT") || (currLane.description == "RZE" && finalDestination == "RZE"))))
            {
                // deinstantiate
                Destroy(gameObject);
            }
            else if (currLane.type == PointCreator.LaneType.Road)
            {
                // search for new lane and setupLane
                var nearestTuple = await FindAndSetUpNearestLaneAndPoint(0);
                SetUpLane(nearestTuple.Item1, nearestTuple.Item2, nearestTuple.Item3, true);
            }
        }
        else if (transform.position == target.position && nextNode < listOfPoints.Count && nextNode >= 0)
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
        if(transform.position == laneChangingTarget.position)
        {
            wantLineChange = false;
            laneChangingTarget = null;
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

    private async Task LaneChangeDecission()
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
            if(pathObjectToFollow.GetComponent<PointCreator>().type == PointCreator.LaneType.Exit)
            {
                lockLaneChange = true;
                return;
            }

            // c2
            bool exitSpotted = SearchForExit();

            // TODO
            // 3. check if you're on right-most lane
            int offset = (direction == Directions.Right) ? -1 : 1;
            var laneToTheRight = await FindAndSetUpNearestLaneAndPoint(offset);
            rightmost = laneToTheRight.Item1 == null;
            // Debug.Log(rightmost);

            // if c2 or (not c2 and not rightmost)
            if(exitSpotted || !rightmost)
            {
                wantLineChange = true;

                Tuple<GameObject, Transform, int> nearestTuple;
                if(direction == Directions.Right)
                {
                    nearestTuple = await FindAndSetUpNearestLaneAndPoint(-1);
                }
                else
                {
                    nearestTuple = await FindAndSetUpNearestLaneAndPoint(1);
                }
                SetUpLane(nearestTuple.Item1, nearestTuple.Item2, nearestTuple.Item3);
            }          
        }
        else
        {
            // calculate a, b, c1
            // if(a || b || c1)

            if (CheckIfNextExitIsDestination())
            {
                exitImminent = true;
                wantLineChange = false;
            }
            else
            {
                Tuple<GameObject, Transform, int> nearestTuple;
                if (direction == Directions.Right)
                {
                    nearestTuple = await FindAndSetUpNearestLaneAndPoint(1);
                }
                else
                {
                    nearestTuple = await FindAndSetUpNearestLaneAndPoint(-1);
                }

                if (SimulationUtility.CheckLane(nearestTuple.Item2, GetVelocity(), 3))
                {
                    SetUpLane(nearestTuple.Item1, nearestTuple.Item2, nearestTuple.Item3);

                    wantLineChange = true;
                }
                else
                {
                    if (direction == Directions.Right)
                    {
                        nearestTuple = await FindAndSetUpNearestLaneAndPoint(-1);
                    }
                    else
                    {
                        nearestTuple = await FindAndSetUpNearestLaneAndPoint(1);
                    }
                    if (SimulationUtility.CheckLane(nearestTuple.Item2, GetVelocity(), 3))
                    {
                        SetUpLane(nearestTuple.Item1, nearestTuple.Item2, nearestTuple.Item3);
                        wantLineChange = true;
                    }
                }

            }
        }
    }

    private bool SearchForExit()
    {
        // if next exit is not yours, don't look for exit
        if(!exitImminent)
        {
            return false;
        }
        Vector2 rightDirection = direction == Directions.Right ? Vector2.down : Vector2.up;
        RaycastHit hit;
        if(Physics.Raycast(new Vector3(transform.position.x, transform.position.y, 2), new Vector3(rightDirection.x, rightDirection.y, 2), out hit))
        {
            if(hit.collider.tag == "highwayLane")
            {
                Debug.Log("strzelłem");
                return true;
            }
        }
        return false;
    }

    private void PrintVelocity()
    {
        velocityText.text = GetVelocity().ToString() + "km/h";
    }

    public void SetUpLane(GameObject lane, Transform nearestPoint, int index, bool isForward=false)
    {
        pathObjectToFollow = lane;
        listOfPoints = new List<Transform>();
        foreach (Transform child in pathObjectToFollow.transform)
        {
            listOfPoints.Add(child);
        }

        if(direction == Directions.Right)
        {
            nextNode = index+1;
            target = listOfPoints.ElementAt(nextNode);
        }
        else
        {
            nextNode = index - 1;
            target = listOfPoints.ElementAt(nextNode);
        }
        if(!isForward)
        {
            laneChangingTarget = target;
        }

        if(vehicleType == VehicleType.Car)
        {
            maxTargetVelocity = pathObjectToFollow.GetComponent<PointCreator>().carMaxVelocity;
        }
        else
        {
            maxTargetVelocity = pathObjectToFollow.GetComponent<PointCreator>().truckMaxVelocity;
        }

    }

    private async Task<Tuple<GameObject, Transform, int>> FindAndSetUpNearestLaneAndPoint(int layerChange)
    {
        var listOfLanes = GameObject.FindGameObjectsWithTag("highwayLane");
        GameObject nearestLane = null;
        float distanceToLane = 100000000000f;
        float distanceToNode = 100000000000f;
        Transform nearestPoint = null;

        foreach (var lane in listOfLanes)
        {
            if(lane.transform.position.y == transform.position.y+(layerChange)*2 && lane != pathObjectToFollow)
            {
                if(direction == Directions.Right)
                {
                    if(lane.transform.position.x >= transform.position.x)
                    {
                        var tempDistance = Vector3.Distance(lane.transform.position, transform.position);
                        if (tempDistance < distanceToLane)
                        {
                            distanceToLane = tempDistance;
                            nearestLane = lane;
                        }
                    }
                }
                else
                {
                    if (lane.transform.position.x <= transform.position.x)
                    {
                        var tempDistance = Vector3.Distance(lane.transform.position, transform.position);
                        if (tempDistance < distanceToLane)
                        {
                            distanceToLane = tempDistance;
                            nearestLane = lane;
                        }
                    }
                }
            }
        }

        int index = 0;
        int i = 0;
        foreach (Transform node in nearestLane.transform)
        {
            var tempDistance = Vector3.Distance(node.transform.position, transform.position);
            if (tempDistance < distanceToNode)
            {
                distanceToNode = tempDistance;
                nearestPoint = node;
                index = i;
            }
            i++;
        }

        return new Tuple<GameObject, Transform, int>(nearestLane, nearestPoint, index);
    }

    private bool CheckIfNextExitIsDestination()
    {
        return pathObjectToFollow.GetComponent<PointCreator>().description == finalDestination;
    }
}
