using System.Collections.Generic;
using FloorModule;
using UnityEngine;

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
    POSTBOX_LEFT_DOOR,
    POSTBOX_RIGHT_DOOR,
    E_PANEL,
    
    ELEVATOR_AD,
    ELEVATOR_AD_COVERING,
    
    GC_1_AD,
    GC_1_AD_COVERING,
    
    GC_2_AD,
    GC_2_AD_COVERING,
    
    GARBAGE_CHUTE_DOOR,
    GARBAGE_CHUTE_DOOR_HINGE,

    ELEVATOR_CALLER_CONNECTOR,
    ELEVATOR_CALLER_PANEL,
    ELEVATOR_CALLER_WIRES,
    PEEPHOLE
}

public enum EDoorAction
{
    BELL,
    HANDLE
}

public enum EFloorMarkId
{
    DRAGONFLY,
    RABBIT_AD,
    RABBIT_SYMBOL,
    COW
}

public static class Events
{
    public const string FloorWasTouched = "FLOOR_WAS_TOUCHED";
    public const string InventoryObjectWasClicked = "INVENTORY_ITEM_WAS_CLICKED";
    public const string InventoryWasUpdated = "INVENTORY_WAS_UPDATED";
    public const string InventoryModeBeforeActivating = "INVENTORY_BUTTON_WAS_PRESSED";
    public const string SwitchableObjectWasOpened = "SWITCHABLE_OBJECT_WAS_OPENED";
    public const string CowCodeActivated = "COW_CODE_ACTIVATED";
    public const string InventoryItemWasSuccessfullyUsed = "INVENTORY_ITEM_WAS_SUCCESSFULLY_USED";
    public const string InventoryItemUsedIncorrectly = "INVENTORY_ITEM_USED_INCORRECTLY";
    public const string ElevatorFloorWasTouched = "ELEVATOR_FLOOR_WAS_TOUCHED";
    public const string PlayerCutSceneMoveCompleted = "PLYER_CUT_SCENE_MOVE_COMPLETED";
    public const string Elevating = "ELEVATING";
    public const string ButtonClicked = "BUTTON_CLICKED";
    public const string MenuBackClicked = "MENU_BACK_CLICKED";
    public const string FullBlackoutReached = "FULL_BLACKOUT_REACHED";
}

public static class GameConstants
{
    public static readonly Dictionary<ESwitchableObjectId, string> switchableObjectPaths =
        new Dictionary<ESwitchableObjectId, string>
        {
            {ESwitchableObjectId.POSTBOX_LEFT_DOOR, "postbox/left_door"},
            {ESwitchableObjectId.POSTBOX_RIGHT_DOOR, "postbox/right_door"},
            {ESwitchableObjectId.E_PANEL, "e-panel/right_door"},
            
            {ESwitchableObjectId.ELEVATOR_AD, "bulletin-board-elevator/ad"},
            {ESwitchableObjectId.ELEVATOR_AD_COVERING, "bulletin-board-elevator/covering"},
            
            {ESwitchableObjectId.GC_1_AD, "bulletin-board-gc/ad"},
            {ESwitchableObjectId.GC_1_AD_COVERING, "bulletin-board-gc/covering"},
            
            {ESwitchableObjectId.GC_2_AD, "bulletin-board-gc_2/ad"},
            {ESwitchableObjectId.GC_2_AD_COVERING, "bulletin-board-gc_2/covering"},
            
            {ESwitchableObjectId.GARBAGE_CHUTE_DOOR, "garbage_chute/gc-door"},
            {ESwitchableObjectId.GARBAGE_CHUTE_DOOR_HINGE, "garbage_chute/gc-hinge"},

            {ESwitchableObjectId.ELEVATOR_CALLER_CONNECTOR, "elevator/caller/connector"},
            {ESwitchableObjectId.ELEVATOR_CALLER_PANEL, "elevator/caller/connector/panel"},
            {ESwitchableObjectId.ELEVATOR_CALLER_WIRES, "elevator/caller/connector/wires"}
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
    
    public static readonly string elevatorFloorColliderObjectName = "elevator_floor";

    public static readonly int isPaintingOnPropertyId = Shader.PropertyToID("_IsPaintingOn");
    
    public static readonly int idleStateNameHash = Animator.StringToHash("Idle");


    public static readonly EDoorAction[] cowCode =
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
                FirstFloor = 9, Frequency = 3,
                AssociatedInventoryItems = new[]
                    {EInventoryItemId.POSTBOX_KEY, EInventoryItemId.LETTER}
            }
        },
        {
            EFloorMarkId.RABBIT_AD,
            new FloorMark
            {
                FirstFloor = 8, Frequency = 4,
                AssociatedInventoryItems = new[]
                    {EInventoryItemId.E_PANEL_KEY}
            }
        },
        {
            EFloorMarkId.RABBIT_SYMBOL,
            new FloorMark
            {
                FirstFloor = 9, Frequency = 5,
                AssociatedInventoryItems = new[]
                    {EInventoryItemId.SCREWDRIVER}
            }
        },
        {
            EFloorMarkId.COW,
            new FloorMark
            {
                FirstFloor = 10, Frequency = 7,
                AssociatedInventoryItems = new[]
                    {EInventoryItemId.SCALPEL}
            }
        }
    };
}