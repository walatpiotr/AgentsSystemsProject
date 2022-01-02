using System;
using System.Collections.Generic;
using UnityEngine;

public class HighwaySetup : MonoBehaviour
{
    public ListOfHighwayParts listOfHighwayParts;

    public GameObject highwayLanePrefab;

    public GameObject highwayEntrancePrefab;

    public GameObject highwayExitPrefab;

    void Awake()
    {
        SetUpHighway();
    }

    void SetUpHighway()
    {

    }

    private float ReturnFloatLayerValue(LayerOfLane layerOfLane)
    {
        switch (layerOfLane)
        {
            case LayerOfLane.ThirdToEast:
                return 0.0f;

            case LayerOfLane.SecondToEast:
                return 2.0f;

            case LayerOfLane.FirstToEast:
                return 4.0f;

            case LayerOfLane.FirstToWest:
                return 6.5f;

            case LayerOfLane.SecondToWest:
                return 8.5f;

            case LayerOfLane.ThirdToWest:
                return 10.5f;
        }
        return 0.0f;
    }

}

[Serializable]
public class ListOfHighwayParts
{
    // Tuple: enum type, length, layer, maxVelocityCars, maxVelocityTrucks
    public List<Tuple<TypeOfHighway, int, LayerOfLane, int, int>> listOfHighwayPartsInside = new List<Tuple<TypeOfHighway, int, LayerOfLane, int, int>>
        {
            new Tuple<TypeOfHighway, int, LayerOfLane, int, int>(TypeOfHighway.Lane, 3300, LayerOfLane.ThirdToEast, 140, 80)
        //TODO enter all lanes with parameters

        };

}

public enum TypeOfHighway
{
    Lane,
    Entrance,
    Exit
}

public enum LayerOfLane
{
    ThirdToEast,
    SecondToEast,
    FirstToEast,
    FirstToWest,
    SecondToWest,
    ThirdToWest
}
