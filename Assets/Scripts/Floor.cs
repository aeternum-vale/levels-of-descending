using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Floor : MonoBehaviour
{
    static readonly string frontWallName  = "front_wall";
    static readonly string frontWallNumberMatPropertyName  = "_FloorNumber";

    Material frontWallMaterial;

    private void Awake()
    {
        frontWallMaterial = gameObject.transform.Find(GameConstants.entrywayObjectName).Find(frontWallName).gameObject.GetComponent<MeshRenderer>().material;
    }
    public void RemoveObject(string name)
    {
        transform.Find(name).gameObject.GetComponent<Renderer>().enabled = false;
    }

    public void SetFloorNumber(int number)
    {
        frontWallMaterial.SetFloat(frontWallNumberMatPropertyName, number);
    }
}
