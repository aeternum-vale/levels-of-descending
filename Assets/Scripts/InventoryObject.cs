using UnityEngine;

public class InventoryObject : SelectableObject {
    [SerializeField] protected EInventoryObjectID objectId;
    [SerializeField] protected GameObject mesh;

    public override void OnClick (EInventoryObjectID? selectedInventoryObjectId = null) {
        GetComponent<Renderer> ().enabled = false;
        Messenger<EInventoryObjectID>.Broadcast(Events.ADD_OBJECT_TO_INVENTORY, objectId);
    }
}