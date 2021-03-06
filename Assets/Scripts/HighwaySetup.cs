using System;
using System.Collections.Generic;
using UnityEngine;

public class HighwaySetup : MonoBehaviour
{
    public ListOfHighwayParts listOfHighwayParts;

    public GameObject highwayLanePrefab;

    public GameObject highwayEntrancePrefab;

    public GameObject highwayExitPrefab;


    public GameObject carPrefab;
    public GameObject truckPrefab;

    void Start()
    {
        listOfHighwayParts = new ListOfHighwayParts();
        SetUpHighway();
    }

    void SetUpHighway()
    {
        var listOfParts = listOfHighwayParts.listOfHighwayPartsInside;

        foreach(var item in listOfParts)
        {
            //TODO
            //create logic of creating parts

            // "3E-BA1-X1-0300-00000-070-70-I" sample
           
            CreateHighwayPart(item.Split('-'));
        }
    }

    private void CreateHighwayPart(string[] infos)
    {
        var yLayer = ReturnFloatLayerValue(infos[0]);
        var description = infos[1];
        var distance = infos[3];
        var xPosition = infos[4];
        var carMaxVelocity = infos[5];
        var truckMaxVelocity = infos[6];
        var prefabType = ReturnTypeOfObjectToInstantiate(infos[7]);

        if (prefabType == highwayLanePrefab)
        {
            var prefabScript = prefabType.GetComponent<PointCreator>();
            prefabScript.type = PointCreator.LaneType.Road;
            prefabScript.yLayer = yLayer;
            prefabScript.description = description;
            prefabScript.distance = float.Parse(distance);
            prefabScript.xPosition = float.Parse(xPosition);
            prefabScript.carMaxVelocity = int.Parse(carMaxVelocity);
            prefabScript.truckMaxVelocity = int.Parse(truckMaxVelocity);
            //add tag if exit and
            var instantiated = Instantiate(prefabType, new Vector3(float.Parse(xPosition), yLayer, 0f), Quaternion.identity);
        }
        else if (prefabType == highwayExitPrefab)
        {
            var boxCollider = prefabType.GetComponent<BoxCollider>();
            boxCollider.center = new Vector3((float.Parse(distance)) / 2, 0, 2);
            boxCollider.size = new Vector3(float.Parse(distance), 2, 1);

            var prefabScript = prefabType.GetComponent<PointCreator>();
            prefabScript.type = PointCreator.LaneType.Exit;
            prefabScript.yLayer = yLayer;
            prefabScript.description = description;
            prefabScript.distance = float.Parse(distance);
            prefabScript.xPosition = float.Parse(xPosition);
            prefabScript.carMaxVelocity = int.Parse(carMaxVelocity);
            prefabScript.truckMaxVelocity = int.Parse(truckMaxVelocity);
            //add tag if exit and
            var instantiated = Instantiate(prefabType, new Vector3(float.Parse(xPosition), yLayer, 0f), Quaternion.identity);
        }
        else
        {
            var prefabScript = prefabType.GetComponent<EntranceBehaviour>();
            prefabScript.entranceType = infos[2] == "EE" ? EntranceBehaviour.EntranceType.End : EntranceBehaviour.EntranceType.Side;
            prefabScript.description = description;
            prefabScript.yLayer = yLayer;
            prefabScript.xPosition = float.Parse(xPosition);
            prefabScript.carPrefab = carPrefab;
            prefabScript.truckPrefab = truckPrefab;
            if (infos[0].Contains("E"))
            {
                prefabScript.direction = "E";
            }
            else
            {
                prefabScript.direction = "W";
            }
            var instantiated = Instantiate(prefabType, new Vector3(float.Parse(xPosition), yLayer, 0f), Quaternion.identity);
        }
    }

    private GameObject ReturnTypeOfObjectToInstantiate(string type)
    {
        switch (type)
        {
            case "I":
                return highwayExitPrefab;
            case "R":
                return highwayLanePrefab;
            case "E":
                return highwayEntrancePrefab;
        }
        return highwayLanePrefab;
    }

    private float ReturnFloatLayerValue(string layerOfLane)
    {
        switch (layerOfLane)
        {
            case "5E":
                return 0.0f;

            case "4E":
                return 2.0f;

            case "3E":
                return 4.0f;

            case "2E":
                return 6.0f;

            case "1E":
                return 8.0f;

            case "1W":
                return 12.5f;

            case "2W":
                return 14.5f;

            case "3W":
                return 16.5f;

            case "4W":
                return 18.5f;
        }
        return 0.0f;
    }

}

[Serializable]
public class ListOfHighwayParts
{
    /*
    towards Rzesz??w = RZE
    Krak??w Bie??an??w interchange = BIZ
    Krak??w Wielicka interchange = WIE
    Krak??w ??agiewniki interchange = LAG
    Krak??w Po??udnie interchange = POL
    Krak??w Skawina interchange = SKA
    Krak??w Tyniec interchange = TYN
    Krak??w Bielany interchange = BIL
    Krak??w Balice II interchange = BA2
    Krak??w Balice I interchange = BA1
    towards Katowice = KAT
    */

    // String ID: "ND-XXX-PP-LLLL-SSSSS-CCC-VV-T";
    // N = lane number, D = direction, XXX = 3 character long intersection code, PP = part indication: name (optional) and number, LLLL = length filled with zeros,
    // SSSSS = xStart filled with zeros, CCC = maxVelocityCars filled with zeros, VV = maxVelocityTrucks, T = type of highway
    public List<string> listOfHighwayPartsInside = new List<string>
        {
            // Krak??w Balice I interchange going east
            "3E-BA1-X1-0300-00000-070-70-I",
            "4E-BA1-X1-0300-00000-070-70-I",
            "3E-BA1-X2-0210-00300-140-80-I",
            "4E-BA1-X2-0210-00300-140-80-I",
            "3E-BA1-X3-0400-00510-120-80-I",
            "4E-BA1-X3-0400-00510-120-80-I",
            // Balice I - Balice II going east
            "1E-BA2-X1-0445-00000-070-70-R",
            "2E-BA2-X1-0445-00000-070-70-R",
            "1E-BA2-X2-0485-00445-080-80-R",
            "2E-BA2-X2-0485-00445-080-80-R",
            "1E-BA2-X3-0590-00930-110-80-R",
            "2E-BA2-X3-0590-00930-110-80-R",
            "4E-BA1-XX-0000-01025-000-00-E",
            "3E-BA2-X4-0495-01025-110-80-R",
            // Krak??w Balice II interchange going east
            "3E-BA2-X1-0330-01520-110-80-I",
            "3E-BA2-X2-0190-01850-050-50-I",
            // Balice II - Bielany going east
            "1E-BIL-X1-4680-01520-110-80-R",
            "2E-BIL-X1-4680-01520-110-80-R",
            "3E-BA2-XX-0000-02185-000-00-E",
            // Krak??w Bielany interchange going east
            "3E-BIL-X1-0255-06200-110-80-I",
            "3E-BIL-X2-0200-06455-060-60-I",
            // Bielany - Tyniec going east
            "1E-TYN-X1-1940-06200-110-80-R",
            "2E-TYN-X1-1940-06200-110-80-R",
            "3E-BIL-XX-0000-06675-000-00-E",
            // Krak??w Tyniec interchange going east
            "3E-TYN-X1-0020-08140-110-80-I",
            "3E-TYN-X2-0210-08160-080-80-I",
            "3E-TYN-X3-0245-08370-050-50-I",
            // Tyniec - Skawina going east
            "1E-SKA-X1-0020-08140-110-80-R",
            "2E-SKA-X1-0020-08140-110-80-R",
            "1E-SKA-X2-0190-08160-080-80-R",
            "2E-SKA-X2-0190-08160-080-80-R",
            "1E-SKA-X3-3140-08350-110-80-R",
            "2E-SKA-X3-3140-08350-110-80-R",
            "3E-TYN-XX-0000-08650-000-00-E",
            // Krak??w Skawina interchange going east
            "3E-SKA-X1-0215-11490-110-80-I",
            "3E-SKA-X2-0240-11705-050-50-I",
            // Skawina - Krak??w Po??udnie going east
            "1E-POL-X1-4550-11490-110-80-R",
            "2E-POL-X1-4550-11490-110-80-R",
            "3E-SKA-XX-0000-12130-000-00-E",
            // Krak??w Po??udnie interchange going east
            "3E-POL-Z1-0200-16040-110-80-I",
            "3E-POL-Z2-0055-16240-050-50-I",
            "1E-POL-X2-0275-16040-140-80-R",
            "2E-POL-X2-0275-16040-140-80-R",
            "3E-POL-K1-0205-16315-140-80-I",
            "3E-POL-K2-0190-16520-050-50-I",
            // Krak??w Po??udnie - ??agiewniki going east
            "1E-LAG-X1-1400-16315-140-80-R",
            "2E-LAG-X1-1400-16315-140-80-R",
            "3E-POL-XX-0000-16730-000-00-E",
            "3E-LAG-X2-0355-17360-140-80-R",
            // Krak??w ??agiewniki interchange going east
            "4E-LAG-X1-0445-17715-140-80-I",
            "4E-LAG-X2-0065-18160-080-80-I",
            "5E-LAG-X2-0045-18180-080-80-I",
            "4E-LAG-X3-0145-18225-060-60-I",
            "5E-LAG-X3-0145-18225-060-60-I",
            "4E-LAG-X4-0475-18370-050-50-I",
            "5E-LAG-X4-0475-18370-050-50-I",
            // ??agiewniki - Wielicka going east
            "1E-WIE-X1-5700-17715-140-80-R",
            "2E-WIE-X1-5700-17715-140-80-R",
            "3E-WIE-X1-5700-17715-140-80-R",
            "4E-LAG-XX-0000-18865-000-00-E",
            // Krak??w Wielicka interchange going east
            "4E-WIE-X1-0650-23415-140-80-I",
            "5E-WIE-X1-0650-23415-140-80-I",
            "4E-WIE-X2-0085-24065-070-70-I",
            "5E-WIE-X2-0085-24065-070-70-I",
            "4E-WIE-K1-0235-24150-070-70-I",
            "4E-WIE-K2-0040-24385-040-40-I",
            "5E-WIE-W1-0225-24150-070-70-I",
            "5E-WIE-W2-0050-24375-050-50-I",
            // Wielicka - Bie??an??w going east
            "1E-BIZ-X1-3300-23415-140-80-R",
            "2E-BIZ-X1-3300-23415-140-80-R",
            "3E-BIZ-X1-3300-23415-140-80-R",
            "4E-WIE-KX-0000-24445-000-00-E",
            "4E-WIE-WX-0000-24875-000-00-E",
            // Krak??w Bie??an??w interchange going east
            "3E-BIZ-X1-0605-26715-140-80-I",
            "4E-BIZ-X1-0605-26715-140-80-I",
            "3E-BIZ-X2-0220-27320-070-70-I",
            "4E-BIZ-X2-0220-27320-070-70-I",
            // going east towards Rzesz??w
            "1E-RZE-X1-1730-26715-140-80-R",
            "2E-RZE-X1-1730-26715-140-80-R",
            "3E-BIZ-XX-0000-27580-000-00-E",
            // MAX = 28445
            // Krak??w Bie??an??w interchange going west
            "3W-BIZ-X1-0470-27975-140-80-I",
            "3W-BIZ-X2-0165-27810-050-50-I",
            // Bie??an??w - Wielicka going west
            "1W-WIE-X1-3390-25055-140-80-R",
            "2W-WIE-X1-3390-25055-140-80-R",
            "3W-WIE-X2-2060-25055-140-80-R",
            "3W-BIZ-XX-0000-27115-000-00-E",
            // Krak??w Wielicka interchange going west
            "4W-WIE-X1-0540-24515-140-80-I",
            "4W-WIE-X2-0270-24245-060-60-I",
            "4W-WIE-X3-0090-24155-050-50-I",
            "4W-WIE-X4-0025-24130-070-70-I",
            // Wielicka - ??agiewniki going west
            "1W-LAG-X1-6200-18855-140-80-R",
            "2W-LAG-X1-6200-18855-140-80-R",
            "3W-LAG-X1-6200-18855-140-80-R",
            "4W-WIE-XX-0000-23575-000-00-E",
            // Krak??w ??agiewniki interchange going west
            "4W-LAG-X1-0240-18615-140-80-I",
            "4W-LAG-X2-0135-18480-060-60-I",
            "4W-LAG-X3-0075-18405-050-50-I",
            // ??agiewniki - Krak??w Po??udnie going west
            "1W-POL-X1-2550-16305-140-80-R",
            "2W-POL-X1-2550-16305-140-80-R",
            "3W-POL-X2-1435-17420-140-80-R",
            "4W-LAG-XX-0000-18305-000-00-E",
            // Krak??w Po??udnie interchange going west
            "3W-POL-X1-0220-16085-140-80-I",
            "3W-POL-X2-0290-15795-050-50-I",
            // Krak??w Po??udnie - Skawina going west
            "1W-SKA-X1-0455-15850-140-80-R",
            "2W-SKA-X1-0455-15850-140-80-R",
            "1W-SKA-X2-3615-12235-110-80-R",
            "2W-SKA-X2-3615-12235-110-80-R",
            "3W-POL-XX-0000-15775-000-00-E",
            // Krak??w Skawina interchange going west
            "3W-SKA-X1-0205-12030-110-80-I",
            "3W-SKA-X2-0380-11650-050-50-I",
            // Skawina - Tyniec going west
            "1W-TYN-X1-3460-08775-110-80-R",
            "2W-TYN-X1-3460-08775-110-80-R",
            "3W-SKA-XX-0000-11550-000-00-E",
            // Krak??w Tyniec interchange going west
            "3W-TYN-X1-0220-08555-110-80-I",
            "3W-TYN-X2-0205-08350-060-60-I",
            "3W-TYN-X3-0015-08335-050-50-I",
            // Tyniec - Bielany going west
            "1W-BIL-X1-0230-08545-110-80-R",
            "2W-BIL-X1-0230-08545-110-80-R",
            "1W-BIL-X2-0160-08385-080-80-R",
            "2W-BIL-X2-0160-08385-080-80-R",
            "1W-BIL-X3-1420-06965-110-80-R",
            "2W-BIL-X3-1420-06965-110-80-R",
            "3W-TYN-XX-0000-08315-000-00-E",
            // Krak??w Bielany interchange going west
            "3W-BIL-X1-0460-06505-110-80-I",
            "3W-BIL-X2-0115-06390-050-50-I",
            // Bielany - Balice II going west
            "1W-BA2-X1-4680-02285-110-80-R",
            "2W-BA2-X1-4680-02285-110-80-R",
            "3W-BIL-XX-0000-06370-000-00-E",
            // Krak??w Balice II interchange going west
            "3W-BA2-X1-0300-01985-110-80-I",
            "3W-BA2-X2-0315-01670-050-50-I",
            // Balice II - Balice I going west
            "1W-BA1-X1-1135-01150-110-80-R",
            "2W-BA1-X1-1135-01150-110-80-R",
            "3W-BA1-X1-0500-01150-110-80-R",
            "1W-BA1-X2-0165-00985-120-80-R",
            "2W-BA1-X2-0165-00985-120-80-R",
            "3W-BA1-X2-0165-00985-120-80-R",
            "1W-BA1-X3-0120-00865-090-80-R",
            "2W-BA1-X3-0120-00865-090-80-R",
            "3W-BA1-X3-0120-00865-090-80-R",
            "3W-BA2-XX-0000-01650-000-00-E",
            // Krak??w Balice I interchange going west
            "3W-BA1-X1-0315-00550-090-80-I",
            "3W-BA1-X2-0265-00285-060-60-I",
            // towards Katowice
            "1W-KAT-X1-0865-00000-090-80-R",
            "2W-KAT-X1-0865-00000-090-80-R",

            // End entrances
            "1E-KAT-EE-0000-00001-000-00-E",
            "2E-KAT-EE-0000-00001-000-00-E",
            "1W-RZE-EE-0000-28444-000-00-E",
            "2W-RZE-EE-0000-28445-000-00-E"
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
    FifthToEast,
    FourthToEast,
    ThirdToEast,
    SecondToEast,
    FirstToEast,
    FirstToWest,
    SecondToWest,
    ThirdToWest,
    FourthToWest
}
