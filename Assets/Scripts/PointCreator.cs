using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PointCreator : MonoBehaviour
{
    public float xStarting;
    public float yStarting;

    public float xEnding;
    public float yEnding;

    public float unifiedSpacing;

    public List<Vector2> listOfPoints;

    public float thiccnessOfCar = 2f;

    public Material meshMaterial;

    public GameObject prefabOfPoints;

    private Vector2 starting;
    private Vector2 ending;

    // Start is called before the first frame update
    void Awake()
    {
        float xTemp = xStarting;
        float yTemp = yStarting;

        starting = new Vector2(xStarting, yStarting);
        ending = new Vector2(xEnding, yEnding);

        var difference = ending - starting;
        var distance = Vector2.Distance(ending, starting);

        //How much points will fit between
        var howMuchPoints = distance / unifiedSpacing;

        // TODO
        // new Vector2(beginningX, beginningY).add(new Vector2(distance, 0).rotate(angle) );
        var vectorToAdd = distance / howMuchPoints;

        var angle = Vector2.Angle(starting, ending);

        Debug.Log(angle);

        CreatePointWithParameters(xStarting, yStarting);

        float tempValue = xStarting;
        while (tempValue < xEnding - unifiedSpacing)
        {
            tempValue = tempValue + unifiedSpacing;
            CreatePointWithParameters(tempValue, yStarting);
        }

        CreatePointWithParameters(xEnding, yEnding);

        GenerateMesh();
    }

    void GenerateMesh()
    {
        Mesh mesh = new Mesh();

        Vector3[] vertices = new Vector3[4];

        vertices[0] = new Vector3(xStarting, yStarting - (thiccnessOfCar / 2));
        vertices[1] = new Vector3(xStarting, yStarting + (thiccnessOfCar / 2));
        vertices[2] = new Vector3(xEnding, yEnding + (thiccnessOfCar / 2));
        vertices[3] = new Vector3(xEnding, yEnding - (thiccnessOfCar / 2));

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
