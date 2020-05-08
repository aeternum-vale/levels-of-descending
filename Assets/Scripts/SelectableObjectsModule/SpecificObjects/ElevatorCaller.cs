using System;
using SelectableObjectsModule.Utilities;
using UnityEngine;

namespace SelectableObjectsModule.SpecificObjects
{
    public class ElevatorCaller : MonoBehaviour
    {
        private PushableObject _button;
        private SwitchableObject _commonWires;
        private SwitchableObject _connector;
        private GameObject _connectorWire1;
        private GameObject _connectorWire2;
        private bool _isButtonAdded;

        private bool _isWiresConnected;
        private SwitchableObject _panel;

        private void Start()
        {
            _connector = SelectableObject.GetAsChild(gameObject, ESwitchableObjectId.ELEVATOR_CALLER_CONNECTOR);
            _panel = SelectableObject.GetAsChild(_connector.gameObject, ESwitchableObjectId.ELEVATOR_CALLER_PANEL);
            _commonWires = SelectableObject.GetAsChild(_panel.gameObject, ESwitchableObjectId.ELEVATOR_CALLER_WIRES);
            _button = SelectableObject.GetAsChild<PushableObject>(_panel.gameObject, "button");
            _connectorWire1 = _connector.transform.Find("wire1_mesh.001").gameObject;
            _connectorWire2 = _connector.transform.Find("wire2_mesh.001").gameObject;

            _connector.Clicked += OnConnectorClicked;
            _panel.Clicked += OnPanelClicked;
            _commonWires.Opened += OnWiresConnectedWithTape;

            _connector.Closed += OnConnectorOrPanelClosed;
            _panel.Closed += OnConnectorOrPanelClosed;
        }

        private void OnPanelClicked(object sender, SelectableObjectClickedEventArgs e)
        {
            if (e.SelectedInventoryItemId != EInventoryItemId.ELEVATOR_CALLER_BUTTON) return;

            _panel.PreventSwitching = true;
            _button.gameObject.SetActive(true);

            _isButtonAdded = true;
        }

        private void OnConnectorClicked(object s, SelectableObjectClickedEventArgs e)
        {
            if (e.SelectedInventoryItemId != EInventoryItemId.ELEVATOR_CALLER_PANEL) return;

            _connector.PreventSwitching = true;
            _panel.gameObject.SetActive(true);

            _connectorWire1.SetActive(false);
            _connectorWire2.SetActive(false);
        }

        private void OnWiresConnectedWithTape(object s, EventArgs e)
        {
            _isWiresConnected = true;
        }

        private void OnConnectorOrPanelClosed(object s, EventArgs e)
        {
            if (_connector.IsOpened || _panel.IsOpened || !_isWiresConnected || !_isButtonAdded) return;

            _connector.SealAndDisableGlowing();
            _panel.SealAndDisableGlowing();
        }
    }
}