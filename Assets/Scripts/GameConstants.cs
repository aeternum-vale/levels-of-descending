using System;
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
    ELEVATOR_BUTTON,
    ELEVATOR_BUTTON_PANEL,
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
    GARBAGE_CHUTE_DOOR_HINGE
}

public enum EDoorAction
{
    NONE,
    BELL,
    HANDLE
}

public enum EFloorMarkId
{
    DRAGONFLY,
    LOST_PET_SIGN
}

public enum ESwitchableObjectStateId
{
    CLOSE = 0,
    OPEN
}

public static class Events
{
    public static readonly string floorWasTouched = "FLOOR_WAS_TOUCHED";
    public static readonly string inventoryItemWasClicked = "INVENTORY_ITEM_WAS_CLICKED";
    public static readonly string inventoryWasUpdated = "INVENTORY_WAS_UPDATED";
    public static readonly string inventoryButtonWasPressed = "INVENTORY_BUTTON_WAS_PRESSED";
    public static readonly string switchableObjectWasOpened = "SWITCHABLE_OBJECT_WAS_OPENED";
    public static readonly string dragonflyCodeActivated = "DRAGONFLY_CODE_ACTIVATED";
}

public static class GameConstants
{
    public static readonly Dictionary<EInventoryItemId, string> inventoryItemToInstancePathMap =
        new Dictionary<EInventoryItemId, string>
        {
            {EInventoryItemId.POSTBOX_KEY, "postbox_key"},
            {EInventoryItemId.LETTER, "letter"},
            {EInventoryItemId.SCALPEL, "scalpel"},
            {EInventoryItemId.E_PANEL_KEY, "e-panel_key"},
            {EInventoryItemId.SCREWDRIVER, "screwdriver"},
            {EInventoryItemId.ELEVATOR_BUTTON, "elevator_button"},
            {EInventoryItemId.ELEVATOR_BUTTON_PANEL, "garbage_chute/elevator_button_panel"},
            {EInventoryItemId.INSULATING_TAPE, "insulating_tape"}
        };

    public static readonly Dictionary<ESwitchableObjectId, string> switchableObjectToInstancePathMap =
        new Dictionary<ESwitchableObjectId, string>
        {
            {ESwitchableObjectId.PAD, "pad"},
            {ESwitchableObjectId.POSTBOX_DOOR, "postbox/postbox_door"},
            {ESwitchableObjectId.E_PANEL, "e-panel/right_door"},
            {ESwitchableObjectId.AD, "bulletin_board_elevator/ad"},
            {ESwitchableObjectId.AD_COVERING, "bulletin_board_elevator/covering"},
            {ESwitchableObjectId.GARBAGE_CHUTE_DOOR, "garbage_chute/door"},
            {ESwitchableObjectId.GARBAGE_CHUTE_DOOR_HINGE, "garbage_chute/hinge"}
        };

    public static readonly string collidersObjectName = "colliders";
    public static readonly string entrywayObjectName = "entryway";

    public static readonly string ground1ColliderObjectName = "ground1";
    public static readonly string ground2ColliderObjectName = "ground2";
    public static readonly string stairs1ColliderObjectName = "stairs1";
    public static readonly string stairs2ColliderObjectName = "stairs2";

    public static readonly EDoorAction[] dragonflyCode =
        {EDoorAction.BELL, EDoorAction.BELL, EDoorAction.HANDLE, EDoorAction.BELL, EDoorAction.HANDLE};

    public static readonly Dictionary<EFloorMarkId, FloorMark> floorMarksDict = new Dictionary<EFloorMarkId, FloorMark>
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

public static class GameUtils
{
    public static string GetNameByPath(string path)
    {
        var lastIndexOfSlash = path.LastIndexOf('/');
        return lastIndexOfSlash == -1 ? path : path.Substring(lastIndexOfSlash + 1);
    }
}

public struct GraphState
{
    public string Name;
    public Action OnReached;
}

public class GraphTransition
{
    public Func<bool> Condition;
    public bool IsReverse;
    public byte NextStateId;
    public EInventoryItemId? SelectedInventoryItemId;
}

public class MultiStateObjectEventArgs : EventArgs
{
    public readonly byte StateId;

    public MultiStateObjectEventArgs(byte stateId)
    {
        StateId = stateId;
    }
}

public static class CameraUtils
{
    public static Texture2D GetCameraTexture(Camera cameraComponent, int width, int height)
    {
        cameraComponent.targetTexture = new RenderTexture(width, height, 24, RenderTextureFormat.ARGB32, 10);
        cameraComponent.Render();

        var t2d = new Texture2D(width, height, TextureFormat.ARGB32, false);
        Graphics.CopyTexture(cameraComponent.targetTexture, t2d);

        return t2d;
    }
}