using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class SampleCarBehaviour : MonoBehaviour
{
    public float xLocation;
    public float yLayer;
    public float velocity;
    public float acceleration;
    public float maxTargetVelocity;

    public enum Directions
    {
        Right,
        Left
    }

    public Directions direction;

    public GameObject pathObjectToFollow;
    public List<Transform> listOfPoints;

    //public Vector2 nextTarget;

    private Transform target;

    private float nextWantedLane;
    private bool wantLineChange;

    private int nextNode;

    // Start is called before the first frame update
    void Start()
    {
        foreach (Transform child in pathObjectToFollow.transform)
        {
            listOfPoints.Add(child);
        }

        Debug.Log(listOfPoints);

        //temp solution
        velocity = maxTargetVelocity;

        //
        transform.position = listOfPoints.First().position;

        target = listOfPoints.ElementAt(1);
        nextNode = 1;

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
    }

    private void Accelerate()
    {

    }

    private void SlowDown()
    {

    }

    private void Move()
    {
        var unifiedSpaceing = pathObjectToFollow.GetComponent<PointCreator>().unifiedSpacing;
        var meterPerSecond = 0.2777f;

        var distanceToMove = (meterPerSecond * velocity) / (unifiedSpaceing * 50);

        transform.position += transform.up * distanceToMove;
    }

    private void Move2()
    {
        float step = velocity * Time.deltaTime; // calculate distance to move

        if(transform.position != target.position)
        {
            transform.position = Vector3.MoveTowards(transform.position, target.position, step);
        }
        else
        {
            nextNode += 1;
            target = listOfPoints.ElementAt(nextNode);
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
}
