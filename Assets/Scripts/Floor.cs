using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Floor : MonoBehaviour
{
    static readonly string frontWallName  = "front_wall";
    static readonly string frontWallNumberMatPropertyName  = "_FloorNumber";

    Material frontWallMaterial;
    Material postboxPartMaterialWithDragonFly;
    Door leftDoor;
    Door rightDoor;
    Scalpel scalpel;

    public readonly Dictionary<ESwitchableObjectID, SwitchableObject> switchableInstancesDict = new Dictionary<ESwitchableObjectID, SwitchableObject>();

    private void Start()
    {
        postboxPartMaterialWithDragonFly = gameObject.transform.Find("postbox").Find("Cube.003").gameObject.GetComponent<MeshRenderer>().material;
        leftDoor = gameObject.transform.Find("left_door_prefab").gameObject.GetComponent<Door>();
        rightDoor = gameObject.transform.Find("right_door_prefab").gameObject.GetComponent<Door>();
        scalpel = gameObject.transform.Find(GameConstants.inventoryItemToInstanceNameMap[EInventoryItemID.SCALPEL]).gameObject.GetComponent<Scalpel>();

        foreach (ESwitchableObjectID id in (ESwitchableObjectID[])Enum.GetValues(typeof(ESwitchableObjectID)))
        {
            switchableInstancesDict.Add(id, transform.Find(GameConstants.switchableObjectToInstancePathMap[id]).gameObject.GetComponent<SwitchableObject>());
        }
    }

    private void Awake()
    {
        frontWallMaterial = gameObject.transform.Find(GameConstants.entrywayObjectName).Find(frontWallName).gameObject.GetComponent<MeshRenderer>().material;
    }

    public void HideObject(string name)
    {
        transform.Find(name).gameObject.SetActive(false);
    }

    public void ShowObject(string name)
    {
        transform.Find(name).gameObject.SetActive(true);
    }

    public void SetFloorDrawnNumber(int number)
    {
        frontWallMaterial.SetFloat(frontWallNumberMatPropertyName, number);
    }

    public void MarkWithDragonfly()
    {
        postboxPartMaterialWithDragonFly.SetFloat("_IsTitleOn", 1f);
        leftDoor.MarkWithDragonfly();
    }

    public void UnmarkWithDragonfly()
    {
        postboxPartMaterialWithDragonFly.SetFloat("_IsTitleOn", 0f);
        leftDoor.UnmarkWithDragonfly();
    }

    public void EmergeScalpel()
    {
        scalpel.Emerge();
    }
}
