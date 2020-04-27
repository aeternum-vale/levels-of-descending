using System.Collections.Generic;

public enum EInventoryItemID
{
    POSTBOX_KEY,
    LETTER,
    SCALPEL,
    E_PANEL_KEY
}

public enum ESwitchableObjectID
{
    PAD,
    POSTBOX_DOOR,
    E_PANEL,
    SIGN,
    SIGN_COVERING,
    //GARBAGE_CHUTE_DOOR
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

public enum EFloorMarkID
{
    DRAGONFLY,
    LOST_PET_SIGN
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
            { EInventoryItemID.POSTBOX_KEY, "postbox_key" },
            { EInventoryItemID.LETTER, "letter" },
            { EInventoryItemID.SCALPEL, "scalpel" },
            { EInventoryItemID.E_PANEL_KEY, "e-panel_key" },

        };

    public static Dictionary<ESwitchableObjectID, string> switchableObjectToInstancePathMap =
        new Dictionary<ESwitchableObjectID, string>() {
            { ESwitchableObjectID.PAD, "pad"},
            { ESwitchableObjectID.POSTBOX_DOOR, "postbox/postbox_door"},
            { ESwitchableObjectID.E_PANEL, "e-panel/right_door"},
            { ESwitchableObjectID.SIGN, "bulletin_board_elevator/sign"},
            { ESwitchableObjectID.SIGN_COVERING, "bulletin_board_elevator/covering"},
            //{ ESwitchableObjectID.GARBAGE_CHUTE_DOOR, "garbage_chute/door"},
        };

    public static readonly string collidersObjectName = "colliders";
    public static readonly string entrywayObjectName = "entryway";

    public static readonly string ground1ColliderObjectName = "ground1";
    public static readonly string ground2ColliderObjectName = "ground2";
    public static readonly string stairs1ColliderObjectName = "stairs1";
    public static readonly string stairs2ColliderObjectName = "stairs2";

    public static readonly EDoorAction[] dragonflyCode = new EDoorAction[] { EDoorAction.BELL, EDoorAction.BELL, EDoorAction.HANDLE, EDoorAction.BELL, EDoorAction.HANDLE };

    public static readonly Dictionary<EFloorMarkID, FloorMark> floorMarksDict = new Dictionary<EFloorMarkID, FloorMark> {
       { EFloorMarkID.DRAGONFLY,     new FloorMark() {FirstFloor = 9,  Frequency = 10, associatedInventoryItems = new EInventoryItemID[]{ EInventoryItemID.POSTBOX_KEY, EInventoryItemID.LETTER } } },
       { EFloorMarkID.LOST_PET_SIGN, new FloorMark() {FirstFloor = 11, Frequency = 5,  associatedInventoryItems = new EInventoryItemID[]{ EInventoryItemID.E_PANEL_KEY} } },
    };
}