using System.Linq.Expressions;
using UnityEngine;

public class ElectricPanelRightDoor : SwitchableObject
{
    GameObject door;
    GameObject doorLock;

    Material doorSelectableMaterial;
    Material doorLockSelectableMaterial;

    protected override void Start()
    {
        base.Start();
        door = transform.Find("r_door").gameObject;
        doorLock = transform.Find("r_door_lock").gameObject;

        doorSelectableMaterial = door.GetComponent<MeshRenderer>().material;
        doorLockSelectableMaterial = doorLock.GetComponent<MeshRenderer>().material;
    }

    public override void ShowSelected()
    {
        doorSelectableMaterial.SetFloat("_IsSelected", 1f);
        doorLockSelectableMaterial.SetFloat("_IsSelected", 1f);
    }

    public override void ShowNormal()
    {
        doorSelectableMaterial.SetFloat("_IsSelected", 0f);
        doorLockSelectableMaterial.SetFloat("_IsSelected", 0f);
    }

    protected override bool SwitchCondition(EInventoryItemID? selectedInventoryItemId = null) => false;
}
