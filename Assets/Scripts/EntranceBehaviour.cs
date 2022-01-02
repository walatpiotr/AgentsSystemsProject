using System;
using UnityEngine;

public class EntranceBehaviour : MonoBehaviour
{
    public Tuple<float, float> randomRangeForSpawning = new Tuple<float, float>(3.0f, 8.0f);

    public Material meshMaterial;

    public GameObject carPrefab;
    public GameObject truckPrefab;

    private float timer;

    void Awake()
    {
        GenerateMesh();
    }

    void GenerateMesh()
    {
        Mesh mesh = new Mesh();

        Vector3[] vertices = new Vector3[4];

        vertices[0] = new Vector3(transform.position.x - 2f + 0.5f, transform.position.y + 2 - 1);
        vertices[1] = new Vector3(transform.position.x - 2f, transform.position.y - 2 + 1);
        vertices[2] = new Vector3(transform.position.x + 2f - 0.5f, transform.position.y - 2 + 1);
        vertices[3] = new Vector3(transform.position.x + 2f, transform.position.y + 2 - 1);

        mesh.vertices = vertices;

        mesh.triangles = new int[] { 0, 1, 2, 0, 2, 3 };

        GetComponent<MeshFilter>().mesh = mesh;

        GetComponent<MeshRenderer>().material = meshMaterial;
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
