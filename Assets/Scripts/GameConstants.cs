using System;
using System.Collections.Generic;
using UnityEngine;

public enum EInventoryItemID
{
    POSTBOX_KEY,
    LETTER,
    SCALPEL,
    E_PANEL_KEY,
    SCREWDRIVER,
    ELEVATOR_BUTTON,
    ELEVATOR_BUTTON_PANEL,
    INSULATING_TAPE
}

public enum ESwitchableObjectID
{
    PAD,
    POSTBOX_DOOR,
    E_PANEL,
    AD,
    AD_COVERING,
    GARBAGE_CHUTE_DOOR,
    GARBAGE_CHUTE_DOOR_HINGE
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

public enum ESwitchableObjectStateId
{
    CLOSE = 0,
    OPEN
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

public static class GameConstants
{
    public static Dictionary<EInventoryItemID, string> inventoryItemToInstancePathMap =
        new Dictionary<EInventoryItemID, string>() {
            { EInventoryItemID.POSTBOX_KEY, "postbox_key" },
            { EInventoryItemID.LETTER, "letter" },
            { EInventoryItemID.SCALPEL, "scalpel" },
            { EInventoryItemID.E_PANEL_KEY, "e-panel_key" },
            { EInventoryItemID.SCREWDRIVER, "screwdriver" },
            { EInventoryItemID.ELEVATOR_BUTTON, "elevator_button" },
            { EInventoryItemID.ELEVATOR_BUTTON_PANEL, "garbage_chute/elevator_button_panel" },
            { EInventoryItemID.INSULATING_TAPE, "insulating_tape" },
        };

    public static Dictionary<ESwitchableObjectID, string> switchableObjectToInstancePathMap =
        new Dictionary<ESwitchableObjectID, string>() {
            { ESwitchableObjectID.PAD, "pad"},
            { ESwitchableObjectID.POSTBOX_DOOR, "postbox/postbox_door"},
            { ESwitchableObjectID.E_PANEL, "e-panel/right_door"},
            { ESwitchableObjectID.AD, "bulletin_board_elevator/ad"},
            { ESwitchableObjectID.AD_COVERING, "bulletin_board_elevator/covering"},
            { ESwitchableObjectID.GARBAGE_CHUTE_DOOR, "garbage_chute/door"},
            { ESwitchableObjectID.GARBAGE_CHUTE_DOOR_HINGE, "garbage_chute/hinge"},
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
       { EFloorMarkID.LOST_PET_SIGN, new FloorMark() {FirstFloor = 11, Frequency = 5,  associatedInventoryItems = new EInventoryItemID[]{ EInventoryItemID.E_PANEL_KEY, EInventoryItemID.SCREWDRIVER} } },
    };
}

public static class GameUtils
{
    public static string GetNameByPath(string path)
    {
        int lastIndexOfSlash = path.LastIndexOf('/');
        return (lastIndexOfSlash == -1) ? path : path.Substring(lastIndexOfSlash + 1);
    }
}

public struct GraphState
{
    public string name;
    public Action onReached;
}

public class GraphTransition
{
    public byte nextStateId;
    public EInventoryItemID? selectedInventoryItemId;
    public Func<bool> condition;
    public bool isReverse;
}

public class MultiStateObjectEventArgs : EventArgs
{
    public readonly byte stateId;

    public MultiStateObjectEventArgs(byte stateId)
    {
        this.stateId = stateId;
    }
}

public static class CameraUtils
{
    public static Texture2D GetCameraTexture(Camera cameraComponent, int width, int height)
    {
        cameraComponent.targetTexture = new RenderTexture(width, height, 24, RenderTextureFormat.ARGB32, 10);
        cameraComponent.Render();

        Texture2D t2d = new Texture2D(width, height, TextureFormat.ARGB32, false);
        Graphics.CopyTexture(cameraComponent.targetTexture, t2d);

        return t2d;
    }
}