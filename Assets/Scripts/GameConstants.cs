using System.Collections.Generic;

public enum EInventoryObjectIDs {
	POSTBOX_KEY,
	SCREWDRIVER
}

public enum ECameraID {
	PLAYER,
	INVENTORY
}

public class Events {
	public static string ADD_OBJECT_TO_INVENTORY = "ADD_OBJECT_TO_INVENTORY";
	public static string INVENTORY_UPDATED = "INVENTORY_UPDATED";
	public static string INVENTORY_BUTTON_PRESSED = "INVENTORY_BUTTON_PRESSED";
	public static string INVENTORY_MODE_ACTIVATED = "INVENTORY_MODE_ACTIVATED";
};


public class GameConstants {
	public static Dictionary<EInventoryObjectIDs, string> InventoryInstanceNameMap =
		new Dictionary<EInventoryObjectIDs, string> () { 
			{ EInventoryObjectIDs.POSTBOX_KEY, "key" }, 
			{ EInventoryObjectIDs.SCREWDRIVER, "screwdriver" },
		};
}