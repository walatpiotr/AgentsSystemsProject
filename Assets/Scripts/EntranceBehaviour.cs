using System;
using UnityEngine;

public class EntranceBehaviour : MonoBehaviour
{
    public Tuple<float, float> randomRangeForSpawning = new Tuple<float, float>(3.0f, 8.0f);

    public Material meshMaterial;

    public GameObject carPrefab;
    public GameObject truckPrefab;

    public float yLayer;

    private float timer;

    void Awake()
    {
        GenerateMesh();
    }

    void GenerateMesh()
    {
        var transformPosition = transform.position;

        Mesh mesh = new Mesh();

        Vector3[] vertices = new Vector3[6];

        vertices[0] = new Vector3(transform.position.x - 1f, yLayer - 1f + 2f);
        vertices[1] = new Vector3(transform.position.x - 1f, yLayer + 1f + 2f);
        vertices[2] = new Vector3(transform.position.x + 1f, yLayer + 1f + 2f);
        vertices[3] = new Vector3(transform.position.x + 1f, yLayer - 1f + 2f);

        vertices[4] = new Vector3(transform.position.x - 10f, yLayer - 1f + 2f);
        vertices[5] = new Vector3(transform.position.x + 10f, yLayer + 1f + 2f);

        mesh.vertices = vertices;

        mesh.triangles = new int[] { 0, 1, 2, 0, 2, 3, 3, 2, 5, 4, 1, 0};

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
        // parameters to setup: current velocity and target point, assign list of points (lane)
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
