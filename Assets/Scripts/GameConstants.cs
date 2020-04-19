using System.Collections.Generic;

public enum EInventoryItemID
{
    POSTBOX_KEY,
    LETTER,
    SCALPEL
}

public enum ESwitchableObjectID
{
    PAD,
    POSTBOX_DOOR,
    E_PANEL
}

public enum ECameraID
{
    PLAYER,
    INVENTORY
}

public enum EDoorAction
{
    NONE,
    BELL,
    HANDLE
}

public class Events
{
    public static string FLOOR_WAS_TOUCHED = "FLOOR_WAS_TOUCHED";
    public static string INVENTORY_ITEM_WAS_CLICKED = "INVENTORY_ITEM_WAS_CLICKED";
    public static string INVENTORY_WAS_UPDATED = "INVENTORY_WAS_UPDATED";
    public static string INVENTORY_BUTTON_WAS_PRESSED = "INVENTORY_BUTTON_WAS_PRESSED";
    public static string SWITCHABLE_OBJECT_WAS_OPENED = "SWITCHABLE_OBJECT_WAS_OPENED";
    public static string DRAGONFLY_CODE_ACTIVATED = "DRAGONFLY_CODE_ACTIVATED";
};

public class GameConstants
{
    public static Dictionary<EInventoryItemID, string> inventoryItemToInstanceNameMap =
        new Dictionary<EInventoryItemID, string>() {
            { EInventoryItemID.POSTBOX_KEY, "key" },
            { EInventoryItemID.LETTER, "letter" },
            { EInventoryItemID.SCALPEL, "scalpel" },

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

    public static readonly EDoorAction[] dragonflyCode = new EDoorAction[] { EDoorAction.BELL, EDoorAction.BELL, EDoorAction.HANDLE, EDoorAction.BELL, EDoorAction.HANDLE };
}