using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class SampleCarBehaviour : MonoBehaviour
{
    public float xLocation;
    public float yLocation;
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

    public Vector2 nextTarget;

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
        xLocation = transform.position.x;
        yLocation = transform.position.y;

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
        Move();
        CheckAndChangeNextPoint();
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

    private void CheckAndChangeNextPoint()
    {

    }
}
