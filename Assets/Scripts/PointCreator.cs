using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PointCreator : MonoBehaviour
{
    // highway setup section
    public float yLayer;
    public float distance;
    public float xPosition;
    public int carMaxVelocity;
    public int truckMaxVelocity;
    public string description;
    public enum LaneType
    {
        Road,
        Exit
    }
    public LaneType type;

    private float unifiedSpacing = 10f;

    public List<Vector2> listOfPoints;

    private float thiccnessOfCar = 2f;

    public Material meshMaterial;

    public GameObject prefabOfPoints;

    // Start is called before the first frame update
    void Awake()
    {
        var starting = new Vector2(xPosition, yLayer);
        var ending = new Vector2(xPosition + distance, yLayer);

        //How much points will fit between
        var howMuchPoints = distance / unifiedSpacing;

        // TODO
        // new Vector2(beginningX, beginningY).add(new Vector2(distance, 0).rotate(angle) );
        var vectorToAdd = distance / howMuchPoints;

        var angle = Vector2.Angle(starting, ending);

        // Debug.Log(angle);

        CreatePointWithParameters(xPosition, yLayer);

        float tempValue = xPosition;
        while (tempValue < (xPosition + distance - unifiedSpacing))
        {
            tempValue = tempValue + unifiedSpacing;
            CreatePointWithParameters(tempValue, yLayer);
        }

        CreatePointWithParameters(xPosition + distance, yLayer);

        GenerateMesh();
    }

    void GenerateMesh()
    {
        Mesh mesh = new Mesh();

        Vector3[] vertices = new Vector3[4];

        vertices[0] = new Vector3(0, 0 - (thiccnessOfCar / 2));
        vertices[1] = new Vector3(0, 0 + (thiccnessOfCar / 2));
        vertices[2] = new Vector3(0 + distance, 0 + (thiccnessOfCar / 2));
        vertices[3] = new Vector3(0 + distance, 0 - (thiccnessOfCar / 2));

        //vertices[4] = new Vector3(xStarting, yLayer - (thiccnessOfCar / 2) + 0.2f);
        //vertices[5] = new Vector3(xStarting, yLayer + (thiccnessOfCar / 2) - 0.2f);
        //vertices[6] = new Vector3(xEnding, yLayer - (thiccnessOfCar / 2) + 0.2f);
        //vertices[7] = new Vector3(xEnding, yLayer - (thiccnessOfCar / 2) - 0.2f);

        mesh.vertices = vertices;

        mesh.triangles = new int[] { 0, 1, 2, 0, 2, 3 };

        GetComponent<MeshFilter>().mesh = mesh;

        GetComponent<MeshRenderer>().material = meshMaterial;
    }

    void CreatePointWithParameters(float x, float y)
    {
        GameObject childObject = Instantiate(prefabOfPoints, new Vector3(x, y, 0), new Quaternion()) as GameObject;
        childObject.transform.parent = this.transform;
    }
}
