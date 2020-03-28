using System.Collections.Generic;

public enum EInventoryItemID {
	POSTBOX_KEY,
	SCREWDRIVER
}

public enum ESwitchableObjectID
{
	PAD
}

public enum ECameraID {
	PLAYER,
	INVENTORY
}

public class Events {
	public static string ADD_ITEM_TO_INVENTORY = "ADD_OBJECT_TO_INVENTORY";
	public static string INVENTORY_UPDATED = "INVENTORY_UPDATED";
	public static string INVENTORY_BUTTON_PRESSED = "INVENTORY_BUTTON_PRESSED";
	public static string SWITCHABLE_OBJECT_OPENED = "SWITCHABLE_OBJECT_OPENED";
};


public class GameConstants {
	public static Dictionary<EInventoryItemID, string> InventoryInstanceNameMap =
		new Dictionary<EInventoryItemID, string> () { 
			{ EInventoryItemID.POSTBOX_KEY, "key" }, 
			{ EInventoryItemID.SCREWDRIVER, "screwdriver" },
		};
}