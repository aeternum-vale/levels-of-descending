using System.Collections.Generic;

public enum EInventoryItemID {
	POSTBOX_KEY,
	SCREWDRIVER
}

public enum ECameraID {
	PLAYER,
	INVENTORY
}

public class Events {
	public static string ADD_ITEM_TO_INVENTORY = "ADD_OBJECT_TO_INVENTORY";
	public static string INVENTORY_UPDATED = "INVENTORY_UPDATED";
	public static string INVENTORY_BUTTON_PRESSED = "INVENTORY_BUTTON_PRESSED";
	public static string INVENTORY_MODE_ACTIVATED = "INVENTORY_CURRENT_OBJECT_CHANGED";
};


public class GameConstants {
	public static Dictionary<EInventoryItemID, string> InventoryInstanceNameMap =
		new Dictionary<EInventoryItemID, string> () { 
			{ EInventoryItemID.POSTBOX_KEY, "key" }, 
			{ EInventoryItemID.SCREWDRIVER, "screwdriver" },
		};
}