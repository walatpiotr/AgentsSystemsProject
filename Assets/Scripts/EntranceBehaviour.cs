using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;

public class EntranceBehaviour : MonoBehaviour
{
    public Tuple<float, float> randomRangeForSpawning = new Tuple<float, float>(0.0f, 2.0f);

    public Material meshMaterial;

    public GameObject carPrefab;
    public GameObject truckPrefab;

    public string direction;

    public float yLayer;
    public float xPosition;

    private float timer;
    private Transform nearestPoint;
    private int indexOfNearest = 0;
    private GameObject nearest;
    private readonly string[] allDestinations = {"BIZ", "WIE", "LAG", "POL", "SKA", "TYN", "BIL", "BA2", "BA1", "RZE", "KAT"};
    private const float UNIFIED_SPACING = 10f;
    private const float SPAWNING_SPEED = 70f;

    void Awake()
    {
        GenerateMesh();
    }

    private async Task Start()
    {
        var listOfLanes = GameObject.FindGameObjectsWithTag("highwayLane");
        nearest = listOfLanes[0];
        nearestPoint = await FindNearest(listOfLanes);
        timer = 0f;
    }

    void GenerateMesh()
    {
        var transformPosition = transform.position;

        Mesh mesh = new Mesh();

        Vector3[] vertices = new Vector3[4];

        vertices[0] = new Vector3(0f, 0f - 1f);
        vertices[1] = new Vector3(0f, 0f + 1f);
        vertices[2] = new Vector3(0f - 8f, 0f - 1f);
        vertices[3] = new Vector3(0f + 8f, 0f + 1f);

        mesh.vertices = vertices;

        mesh.triangles = new int[] { 2, 1, 0, 0, 1, 3};

        GetComponent<MeshFilter>().mesh = mesh;

        GetComponent<MeshRenderer>().material = meshMaterial;

        this.transform.position = transformPosition;
    }

    void FixedUpdate()
    {
        if (timer == 0f)
        {
            if(CheckLane())
            {
                SpawnCar();
            }
            SetupTimer();
        }

        TimerTick();
    }

    bool CheckLane()
    {
        Vector2 backwardDirection;
        Vector2 forwardDirection;
        if(nearestPoint.position.y < 9f)
        {
            backwardDirection = Vector2.left;
            forwardDirection = Vector2.right;
        }
        else
        {
            backwardDirection = Vector2.right;
            forwardDirection = Vector2.left;
        }
        RaycastHit2D hitBackward = Physics2D.Raycast(nearestPoint.position, backwardDirection);
        RaycastHit2D hitForward = Physics2D.Raycast(nearestPoint.position, forwardDirection);
        PointCreator lane = nearestPoint.parent.GetComponent<PointCreator>();
        // distance < (2v1 - v2p - v2k)(v2k - v2p)/2a2 + buffer [km i h]
        // we assume v1 = lane.carMaxVelocity; buffer = v1; v2p = 70km/h 
        if((hitBackward.collider != null))
        {
            if(hitBackward.collider.tag == "car" && (hitBackward.distance < ((2*lane.carMaxVelocity - SPAWNING_SPEED - lane.carMaxVelocity)*(lane.carMaxVelocity - SPAWNING_SPEED))/(2*UNIFIED_SPACING*3.6f)/1000+lane.carMaxVelocity))
            {
                return false;
            }
            if(hitBackward.collider.tag == "truck" && (hitBackward.distance < ((2*lane.carMaxVelocity - SPAWNING_SPEED - lane.truckMaxVelocity)*(lane.truckMaxVelocity - SPAWNING_SPEED))/(2*UNIFIED_SPACING*3.6f)/1000+lane.carMaxVelocity))
            {
                return false;
            }
        }
        if((hitForward.collider != null) && (hitForward.collider.tag == "car" || hitForward.collider.tag == "truck") && (hitForward.distance < SPAWNING_SPEED))
        {
            return false;
        }
        return true;
    }

    void SpawnCar()
    {
        // TODO
        // determine probability
        SampleCarBehaviour carBehaviourScript;
        if(UnityEngine.Random.value >= 0.3)
        {
            carBehaviourScript = carPrefab.GetComponent<SampleCarBehaviour>();
        }
        else
        {
            carBehaviourScript = truckPrefab.GetComponent<SampleCarBehaviour>();
        }
        carBehaviourScript.target = nearestPoint;

        int destination = UnityEngine.Random.Range(0, 10);
        if(destination < 9)
        {
            carBehaviourScript.finalDestination = allDestinations[destination];
        }
        else
        {
            if(direction == "E")
            {
                carBehaviourScript.finalDestination = allDestinations[9];
            }
            else
            {
                carBehaviourScript.finalDestination = allDestinations[10];
            }
        }

        if (direction == "E")
        {
            carBehaviourScript.direction = SampleCarBehaviour.Directions.Right;
        }
        else
        {
            carBehaviourScript.direction = SampleCarBehaviour.Directions.Left;
        }

        carBehaviourScript.pathObjectToFollow = carBehaviourScript.target.parent.gameObject;
        // TODO: change to SampleCarBehaviour.Setup() function
        int i = 0;
        carBehaviourScript.listOfPoints = new List<Transform>() { };
        foreach (Transform child in carBehaviourScript.pathObjectToFollow.transform)
        {
            if (child == carBehaviourScript.target)
            {
                if (carBehaviourScript.direction == SampleCarBehaviour.Directions.Left)
                {
                    carBehaviourScript.nextNode = i - 1;
                }
                else
                {
                    carBehaviourScript.nextNode = i + 1;
                }
            }
            carBehaviourScript.listOfPoints.Add(child);
            i++;
        }
        //carBehaviourScript.SetUpLane(nearestPoint, indexOfNearest);
        GameObject spawnedCar;
        if(carBehaviourScript.vehicleType == SampleCarBehaviour.VehicleType.Car)
        {
            spawnedCar = Instantiate(carPrefab, transform.position, Quaternion.identity);
        }
        else
        {
            spawnedCar = Instantiate(truckPrefab, transform.position, Quaternion.identity);
        }
        spawnedCar.GetComponent<SampleCarBehaviour>().SetVelocity(SPAWNING_SPEED);
        // parameters to setup: current velocity and target point, assign list of points (lane)
    }

    async Task<Transform> FindNearest(GameObject[] listOfLanes)
    {
        var nearest = listOfLanes[0];
        float distance = 100000000000f;
        Transform nearestPoint = null;
        foreach (var lane in listOfLanes)
        {
            int i = 0;
            foreach (Transform node in lane.transform)
            {
                var tempDistance = Vector3.Distance(node.transform.position, transform.position);
                if (tempDistance < distance)
                {
                    distance = tempDistance;
                    nearest = lane;
                    nearestPoint = node;
                    indexOfNearest = i;
                }
                i++;
            }
        }
        return nearestPoint;
    }

    void SetupTimer()
    {
        timer = UnityEngine.Random.Range(randomRangeForSpawning.Item1, randomRangeForSpawning.Item2);
    }

    void TimerTick()
    {
        timer -= Time.deltaTime;
    }
}
