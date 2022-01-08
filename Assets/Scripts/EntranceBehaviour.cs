using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;

public class EntranceBehaviour : MonoBehaviour
{
    public Tuple<float, float> randomRangeForSpawning = new Tuple<float, float>(3.0f, 8.0f);

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
            // TODO
            // add method to check if lane is clear. if not set timer and dont spawn car
            SpawnCar();
            SetupTimer();
        }

        Timer();
    }

    void SpawnCar()
    {
        // TODO
        // decide which car to spawn : car or truck
        var carBehaviourScript = carPrefab.GetComponent<SampleCarBehaviour>();
        carBehaviourScript.SetVelocity(70f);
        carBehaviourScript.target = nearestPoint;

        if (direction == "E")
        {
            carBehaviourScript.direction = SampleCarBehaviour.Directions.Right;
        }
        else
        {
            carBehaviourScript.direction = SampleCarBehaviour.Directions.Left;
        }

        carBehaviourScript.pathObjectToFollow = carBehaviourScript.target.parent.gameObject;
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
        Instantiate(carPrefab, transform.position, Quaternion.identity);
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

    void Timer()
    {
        timer -= Time.deltaTime;
    }
}
