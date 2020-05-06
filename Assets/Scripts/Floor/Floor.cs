using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class Floor : MonoBehaviour
{
    static readonly string frontWallName = "front_wall";
    static readonly string frontWallNumberMatPropertyName = "_FloorNumber";

    Material frontWallMaterial;
    Material postboxPartMaterialWithDragonFly;
    Door leftDoor;
    Door rightDoor;
    Scalpel scalpel;

    Material adMaterial;

    public readonly Dictionary<ESwitchableObjectID, SwitchableObject> switchableInstancesDict = new Dictionary<ESwitchableObjectID, SwitchableObject>();
    readonly Dictionary<EFloorMarkID, bool> markStatesDict = new Dictionary<EFloorMarkID, bool>() {
        { EFloorMarkID.DRAGONFLY, false },
        { EFloorMarkID.LOST_PET_SIGN, false },
    };

    private void Start()
    {
        postboxPartMaterialWithDragonFly = transform.Find("postbox").Find("Cube.003").GetComponent<MeshRenderer>().material;
        leftDoor = gameObject.transform.Find("left_door_prefab").GetComponent<Door>();
        rightDoor = gameObject.transform.Find("right_door_prefab").GetComponent<Door>();
        scalpel = transform.Find(InventoryObject.GetPath(EInventoryItemID.SCALPEL)).GetComponent<Scalpel>();

        foreach (ESwitchableObjectID id in (ESwitchableObjectID[])Enum.GetValues(typeof(ESwitchableObjectID)))
        {
            switchableInstancesDict.Add(id, transform.Find(SwitchableObject.GetPath(id)).GetComponent<SwitchableObject>());
        }
    }

    private void Awake()
    {
        frontWallMaterial = transform.Find(GameConstants.entrywayObjectName).Find(frontWallName).GetComponent<MeshRenderer>().material;
        adMaterial = transform.Find(SwitchableObject.GetPath(ESwitchableObjectID.AD)).GetComponent<MeshRenderer>().material;
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

    public void EmergeScalpel()
    {
        scalpel.Emerge();
    }

    public void SetMark(EFloorMarkID id)
    {
        markStatesDict[id] = true;

        switch (id)
        {
            case EFloorMarkID.DRAGONFLY:
                postboxPartMaterialWithDragonFly.SetFloat("_IsTitleOn", 1f);
                leftDoor.MarkWithDragonfly();
                break;

            case EFloorMarkID.LOST_PET_SIGN:
                adMaterial.SetFloat("_ActiveTextureNumber", 1f);
                break;
        }
    }

    public void ResetMark(EFloorMarkID id)
    {
        markStatesDict[id] = false;

        switch (id)
        {
            case EFloorMarkID.DRAGONFLY:
                postboxPartMaterialWithDragonFly.SetFloat("_IsTitleOn", 0f);
                leftDoor.UnmarkWithDragonfly();
                break;
        }
    }

    public void ResetAllMarks()
    {
        foreach (var key in markStatesDict.Keys.ToList())
        {
            ResetMark(key);
        }
    }

    public void SetFrontWallAd(Texture2D texture)
    {
        adMaterial.SetTexture("_MainTex", texture);
    }
}
