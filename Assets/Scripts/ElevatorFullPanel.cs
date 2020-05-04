using System;
using System.Collections.Generic;

public class ElevatorFullPanel : MultiStateObject
{
    enum EElevatorFullPanelState
    {
        START = 0,
        UNCONNECTED,
        CONNECTED,
        CLOSING,
        FINALIZED,
        BUTTON_CLICKING
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
            { (byte)EElevatorFullPanelState.START, EInventoryItemID.ELEVATOR_BUTTON_PANEL },
            { (byte)EElevatorFullPanelState.UNCONNECTED, EInventoryItemID.INSULATING_TAPE },
            { (byte)EElevatorFullPanelState.CLOSING, EInventoryItemID.ELEVATOR_BUTTON },
        };

        stateNameDict = new Dictionary<byte, string>()
        {
            { (byte)EElevatorFullPanelState.START, "start" },
            { (byte)EElevatorFullPanelState.UNCONNECTED, "unconnected" },
            { (byte)EElevatorFullPanelState.CONNECTED, "connected" },
            { (byte)EElevatorFullPanelState.CLOSING, "closing" },
            { (byte)EElevatorFullPanelState.FINALIZED, "finalized" },
            { (byte)EElevatorFullPanelState.BUTTON_CLICKING, "button_clicking" }
        };
    }

}
