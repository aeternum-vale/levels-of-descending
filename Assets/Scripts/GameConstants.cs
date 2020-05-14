using System.Collections.Generic;
using FloorModule;

public enum EInventoryItemId
{
    POSTBOX_KEY,
    LETTER,
    SCALPEL,
    E_PANEL_KEY,
    SCREWDRIVER,
    ELEVATOR_CALLER_BUTTON,
    ELEVATOR_CALLER_PANEL,
    INSULATING_TAPE
}

public enum ESwitchableObjectId
{
    PAD,
    POSTBOX_DOOR,
    E_PANEL,
    AD,
    AD_COVERING,
    GARBAGE_CHUTE_DOOR,
    GARBAGE_CHUTE_DOOR_HINGE,

    ELEVATOR_CALLER_CONNECTOR,
    ELEVATOR_CALLER_PANEL,
    ELEVATOR_CALLER_WIRES,
}

public enum EDoorAction
{
    BELL,
    HANDLE
}

public enum EFloorMarkId
{
    DRAGONFLY,
    LOST_PET_SIGN
}

public static class Events
{
    public const string FloorWasTouched = "FLOOR_WAS_TOUCHED";
    public const string InventoryObjectWasClicked = "INVENTORY_ITEM_WAS_CLICKED";
    public const string InventoryWasUpdated = "INVENTORY_WAS_UPDATED";
    public const string InventoryButtonWasPressed = "INVENTORY_BUTTON_WAS_PRESSED";
    public const string SwitchableObjectWasOpened = "SWITCHABLE_OBJECT_WAS_OPENED";
    public const string DragonflyCodeActivated = "DRAGONFLY_CODE_ACTIVATED";
    public const string InventoryItemWasSuccessfullyUsed = "INVENTORY_ITEM_WAS_SUCCESSFULLY_USED";
}

public static class GameConstants
{
    public static readonly Dictionary<ESwitchableObjectId, string> switchableObjectPaths =
        new Dictionary<ESwitchableObjectId, string>
        {
            {ESwitchableObjectId.PAD, "pad"},
            {ESwitchableObjectId.POSTBOX_DOOR, "postbox/postbox_door"},
            {ESwitchableObjectId.E_PANEL, "e-panel/right_door"},
            {ESwitchableObjectId.AD, "bulletin_board_elevator/ad"},
            {ESwitchableObjectId.AD_COVERING, "bulletin_board_elevator/covering"},
            {ESwitchableObjectId.GARBAGE_CHUTE_DOOR, "garbage_chute/door"},
            {ESwitchableObjectId.GARBAGE_CHUTE_DOOR_HINGE, "garbage_chute/hinge"},

            {ESwitchableObjectId.ELEVATOR_CALLER_CONNECTOR, "elevator/caller/connector"},
            {ESwitchableObjectId.ELEVATOR_CALLER_PANEL, "elevator/caller/connector/panel"},
            {ESwitchableObjectId.ELEVATOR_CALLER_WIRES, "elevator/caller/connector/panel/wires"},
        };

    public static readonly Dictionary<EInventoryItemId, string> inventoryObjectPaths =
        new Dictionary<EInventoryItemId, string>
        {
            {EInventoryItemId.POSTBOX_KEY, "postbox_key"},
            {EInventoryItemId.LETTER, "letter"},
            {EInventoryItemId.SCALPEL, "scalpel"},
            {EInventoryItemId.E_PANEL_KEY, "e-panel_key"},
            {EInventoryItemId.SCREWDRIVER, "screwdriver"},
            {EInventoryItemId.ELEVATOR_CALLER_BUTTON, "elevator_caller_button"},
            {EInventoryItemId.ELEVATOR_CALLER_PANEL, "garbage_chute/elevator_caller_panel"},
            {EInventoryItemId.INSULATING_TAPE, "insulating_tape"}
        };

    public static readonly string collidersObjectName = "colliders";
    public static readonly string entrywayObjectName = "entryway";

    public static readonly string ground1ColliderObjectName = "ground1";
    public static readonly string ground2ColliderObjectName = "ground2";
    public static readonly string stairs1ColliderObjectName = "stairs1";
    public static readonly string stairs2ColliderObjectName = "stairs2";

    public static readonly EDoorAction[] dragonflyCode =
    {
        EDoorAction.BELL,
        EDoorAction.BELL,
        EDoorAction.HANDLE,
        EDoorAction.BELL,
        EDoorAction.HANDLE
    };

    public static readonly Dictionary<EFloorMarkId, FloorMark> floorMarks = new Dictionary<EFloorMarkId, FloorMark>
    {
        {
            EFloorMarkId.DRAGONFLY,
            new FloorMark
            {
                FirstFloor = 9, Frequency = 10,
                AssociatedInventoryItems = new[]
                    {EInventoryItemId.POSTBOX_KEY, EInventoryItemId.LETTER}
            }
        },
        {
            EFloorMarkId.LOST_PET_SIGN,
            new FloorMark
            {
                FirstFloor = 11, Frequency = 5,
                AssociatedInventoryItems = new[]
                    {EInventoryItemId.E_PANEL_KEY, EInventoryItemId.SCREWDRIVER}
            }
        }
    };
}