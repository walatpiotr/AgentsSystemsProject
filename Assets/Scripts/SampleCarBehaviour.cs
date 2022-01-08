using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using TMPro;

public class SampleCarBehaviour : MonoBehaviour
{
    public float xLocation;
    public float yLayer;
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

        if(velocityMetersPerSecond == null)
        {
            velocityMetersPerSecond = 0f;
        } 
    }

    private void FixedUpdate()
    {
        Accelerate();
        SlowDown();
        Move();
        LaneChangeDecission();
        if (wantLineChange)
        {
            ChangeLane(nextWantedLane);
        }
        PrintVelocity();

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

    }

    private void SlowDown()
    {
        var detected = DetectCars();
        if(detected != null)
        {
            // reduce velocity
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
            // TODO
            // search for new lane
        }
    }

    private Collider2D DetectCars()
    {
        Vector2 convertedDirection = direction == Directions.Right ? Vector2.right : Vector2.left;
        RaycastHit2D hit = Physics2D.Raycast(transform.position, convertedDirection);
        if(hit.collider != null)
        {
            if(hit.distance <= velocityMetersPerSecond * Time.deltaTime)
            {
                return hit.collider;
            }
        }
        return null;
    }

    private void ChangeLane(float yLayer)
    {
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
