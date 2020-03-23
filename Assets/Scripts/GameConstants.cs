using System.Collections.Generic;

public enum EInventoryObjectID {
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
	public static string INVENTORY_MODE_ACTIVATED = "INVENTORY_CURRENT_OBJECT_CHANGED";
};


public class GameConstants {
	public static Dictionary<EInventoryObjectID, string> InventoryInstanceNameMap =
		new Dictionary<EInventoryObjectID, string> () { 
			{ EInventoryObjectID.POSTBOX_KEY, "key" }, 
			{ EInventoryObjectID.SCREWDRIVER, "screwdriver" },
		};
}