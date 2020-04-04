using System.Collections.Generic;

public enum EInventoryItemID
{
    POSTBOX_KEY,
    SCREWDRIVER
}

public enum ESwitchableObjectID
{
    PAD,
    POSTBOX_DOOR
}

public enum ECameraID
{
    PLAYER,
    INVENTORY
}

public class Events
{
    public static string FLOOR_TOUCHED = "FLOOR_TOUCHED";
    public static string ADD_ITEM_TO_INVENTORY = "ADD_OBJECT_TO_INVENTORY";
    public static string INVENTORY_UPDATED = "INVENTORY_UPDATED";
    public static string INVENTORY_BUTTON_PRESSED = "INVENTORY_BUTTON_PRESSED";
    public static string SWITCHABLE_OBJECT_OPENED = "SWITCHABLE_OBJECT_OPENED";
};

public class GameConstants
{
    public static Dictionary<EInventoryItemID, string> inventoryItemToInstanceNameMap =
        new Dictionary<EInventoryItemID, string>() {
            { EInventoryItemID.POSTBOX_KEY, "key" },
            { EInventoryItemID.SCREWDRIVER, "screwdriver" },
        };

    public static Dictionary<ESwitchableObjectID, string> switchableObjectToInstanceNameMap =
        new Dictionary<ESwitchableObjectID, string>() {
            { ESwitchableObjectID.PAD, "pad"},
            { ESwitchableObjectID.POSTBOX_DOOR, "postbox_door"},
        };

    public static readonly string collidersObjectName = "colliders";
    public static readonly string entrywayObjectName = "entryway";

    public static readonly string ground1ColliderObjectName = "ground1";
    public static readonly string ground2ColliderObjectName = "ground2";
    public static readonly string stairs1ColliderObjectName = "stairs1";
    public static readonly string stairs2ColliderObjectName = "stairs2";
}