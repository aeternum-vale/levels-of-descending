using UnityEngine;

public class InventoryObject : SelectableObject {
    [SerializeField] protected EInventoryObjectIDs objectId;
    [SerializeField] protected GameObject mesh;

    public override void onClick () {
        GetComponent<Renderer> ().enabled = false;
        Messenger<EInventoryObjectIDs>.Broadcast(Events.ADD_OBJECT_TO_INVENTORY, objectId);
    }
}