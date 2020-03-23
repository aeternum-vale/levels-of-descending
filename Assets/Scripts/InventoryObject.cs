using UnityEngine;

public class InventoryObject : SelectableObject {
    [SerializeField] protected EInventoryObjectID objectId;
    [SerializeField] protected GameObject mesh;

    public override void onClick () {
        GetComponent<Renderer> ().enabled = false;
        Messenger<EInventoryObjectID>.Broadcast(Events.ADD_OBJECT_TO_INVENTORY, objectId);
    }
}