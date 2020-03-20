using System.Collections.Generic;
using UnityEngine;

public class Inventory : MonoBehaviour {

    [SerializeField] private CamerasController camerasController;
    public HashSet<EInventoryObjectIDs> Objects { get; } = new HashSet<EInventoryObjectIDs>();
    public bool IsInventoryModeOn { get; private set; }
    private readonly Dictionary<EInventoryObjectIDs, GameObject> instances  = new Dictionary<EInventoryObjectIDs, GameObject>();

    void Awake () {
        Messenger<EInventoryObjectIDs>.AddListener (Events.ADD_OBJECT_TO_INVENTORY, OnObjectAdding);
        Messenger.AddListener (Events.INVENTORY_BUTTON_PRESSED, OnInventoryButtonPressed);

        foreach (KeyValuePair<EInventoryObjectIDs, string> item in GameConstants.InventoryInstanceNameMap)
        {
            GameObject go = transform.Find(item.Value).gameObject;
            instances.Add(item.Key, go);
        }
    }

    protected void OnObjectAdding (EInventoryObjectIDs id) {
        Objects.Add (id);
        Messenger.Broadcast (Events.INVENTORY_UPDATED);
    }

    protected void OnInventoryButtonPressed () {
        if (!IsInventoryModeOn) {
            IsInventoryModeOn = true;
            camerasController.SetInventoryCameraBackroundTexture();
            camerasController.Activate(ECameraID.INVENTORY);
        } else
        {
            IsInventoryModeOn = false;
            camerasController.Activate(ECameraID.PLAYER);
        }
    }

}