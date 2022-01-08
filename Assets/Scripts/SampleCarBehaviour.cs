using System.Collections.Generic;
using System.Linq;
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
    public float velocity;
    public float acceleration;
    public float maxTargetVelocity;
    public TextMeshProUGUI velocityText;
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

    private float velocityMetersPerSecond;
    private float nextWantedLane;
    private bool wantLineChange;

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

        velocityMetersPerSecond = CalculateToMetersPerSecond(velocity);
    }

    private void FixedUpdate()
    {
        Accelerate();
        SlowDown();
        Move2();
        LaneChangeDecission();
        if (wantLineChange)
        {
            ChangeLane(nextWantedLane);
        }
        EstablishVelocityInKilometers();
        PrintVelocity();

        targetX = target.position.x;
    }

    private void Accelerate()
    {

    }

    private void SlowDown()
    {

    }

    private void Move()
    {
        var unifiedSpacing = 10f;
        var meterPerSecond = 0.2777f;

        var distanceToMove = (meterPerSecond * velocity) / (unifiedSpacing * 50);

        transform.position += transform.up * distanceToMove;
    }

    private void Move2()
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
            // TODO
            // search for new lane
        }
    }

    private void ChangeLane(float yLayer)
    {
        // when Move()

        // 1. calculate accurate distance in which car wants to change line
        // 2. rotate by calucalted angle
        // 3. move transform.up
        // 4. when transform.position.y == wantedLayer, rotate to lane direction

        // when Move2()

        // 1. calculate accurate distance in which car wants to change line
        // 2. move towards established point in new lane
        // 3. when in target position, change list of points and establish index of current point
    }

    private void LaneChangeDecission()
    {
        // 1. decide which line is your target lane
        // 2. check if there are cars that ride on this lane
        // 3. check if there are in a safe distance
        // 4. establish value of wantLineChange boolean
    }

    private float CalculateToMetersPerSecond(float velocityInKilometers)
    {
        return velocityInKilometers / 3.6f;
    }

    // method only for purpose of displaying velocity
    private void EstablishVelocityInKilometers()
    {
        velocity = velocityMetersPerSecond * 3.6f;
    }

    private void PrintVelocity()
    {
        velocityText.text = velocity.ToString() + "km/h";
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
