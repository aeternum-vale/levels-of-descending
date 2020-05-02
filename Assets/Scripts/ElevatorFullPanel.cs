using System;
using System.Collections.Generic;

public class ElevatorFullPanel : MultiStateObject
{
    enum EElevatorFullPanelState
    {
        START = 0,
        UNCONNECTED,
        CONNECTED,
        BUTTON_ABSENT,
        FINALIZED,
    }

    Dictionary<EElevatorFullPanelState, string> selectablePartPath = new Dictionary<EElevatorFullPanelState, string>()
    {
        {EElevatorFullPanelState.UNCONNECTED,  "wires" },
        {EElevatorFullPanelState.CONNECTED,  "panel" },
        {EElevatorFullPanelState.FINALIZED, "button" },
    };

    protected override void Awake()
    {
        base.Awake();

        stateCount = (byte)((EElevatorFullPanelState[])Enum.GetValues(typeof(EElevatorFullPanelState))).Length;

        anticipatedInventoryItemDict = new Dictionary<byte, EInventoryItemID>()
        {
            {(byte)EElevatorFullPanelState.START, EInventoryItemID.ELEVATOR_BUTTON_PANEL },
            {(byte)EElevatorFullPanelState.UNCONNECTED, EInventoryItemID.INSULATING_TAPE },
            {(byte)EElevatorFullPanelState.BUTTON_ABSENT, EInventoryItemID.ELEVATOR_BUTTON },
        };

        stateNameDict = new Dictionary<byte, string>()
        {
            { (byte)EElevatorFullPanelState.START, "start" },
            { (byte)EElevatorFullPanelState.UNCONNECTED, "unconnected" },
            { (byte)EElevatorFullPanelState.CONNECTED, "connected" },
            { (byte)EElevatorFullPanelState.BUTTON_ABSENT, "button_absent" },
            { (byte)EElevatorFullPanelState.FINALIZED, "finalized" }
        };
    }

}
