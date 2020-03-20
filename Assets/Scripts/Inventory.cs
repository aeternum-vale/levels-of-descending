using System.Collections.Generic;
using UnityEngine;

public class Inventory : MonoBehaviour {

    [SerializeField] private CamerasController camerasController;

    private HashSet<EInventoryObjectIDs> objects = new HashSet<EInventoryObjectIDs> ();
    public HashSet<EInventoryObjectIDs> Objects { get { return this.objects; } }

    private bool isInventoryModeOn;
    public bool IsInventoryModeOn { get { return isInventoryModeOn; } }

    private Dictionary<EInventoryObjectIDs, GameObject> instances  = new Dictionary<EInventoryObjectIDs, GameObject>();

    void Awake () {

        Messenger<EInventoryObjectIDs>.AddListener (Events.ADD_OBJECT_TO_INVENTORY, onObjectAdding);
        Messenger.AddListener (Events.INVENTORY_BUTTON_PRESSED, onInventoryButtonPressed);

        foreach (KeyValuePair<EInventoryObjectIDs, string> item in GameConstants.InventoryInstanceNameMap)
        {
            GameObject go = transform.Find(item.Value).gameObject;
            instances.Add(item.Key, go);
        }
    }

    protected void onObjectAdding (EInventoryObjectIDs id) {
        objects.Add (id);
        Messenger.Broadcast (Events.INVENTORY_UPDATED);
    }

    protected void onInventoryButtonPressed () {
        if (!isInventoryModeOn) {
            isInventoryModeOn = true;

            camerasController.setActive(ECameraID.INVENTORY);
        }
    }

}