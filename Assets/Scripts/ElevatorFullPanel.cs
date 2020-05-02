using System.Collections.Generic;

public class ElevatorFullPanel : SelectableObject
{
    enum EState
    {
        START,
        UNCONNECTED,
        CONNECTED,
        BUTTON_ABSENT,
        FINALIZED
    }

    readonly Dictionary<EState, string> stateNameDict = new Dictionary<EState, string>()
    {
        {EState.START, "start" },
        {EState.UNCONNECTED, "unconnected" },
        {EState.CONNECTED, "connected" },
        {EState.BUTTON_ABSENT, "button_absent" },
        {EState.FINALIZED, "finalized" }
    };

    readonly Dictionary<EState, EInventoryItemID> anticipatedInventoryItemDict = new Dictionary<EState, EInventoryItemID>()
    {
        {EState.START, EInventoryItemID.ELEVATOR_BUTTON_PANEL },
        {EState.UNCONNECTED, EInventoryItemID.INSULATING_TAPE },
        {EState.BUTTON_ABSENT, EInventoryItemID.ELEVATOR_BUTTON },
    };

    Dictionary<EState, string> selectablePartPath = new Dictionary<EState, string>()
    {
        {EState.UNCONNECTED,  "wires" },
        {EState.CONNECTED,  "panel" },
        {EState.FINALIZED, "button" },
    };

    public override void OnClick(EInventoryItemID? selectedInventoryItemId = null)
    {

    }


}
