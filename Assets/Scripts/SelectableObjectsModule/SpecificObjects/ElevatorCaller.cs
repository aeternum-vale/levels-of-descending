using System;
using Plugins;
using SelectableObjectsModule.Utilities;
using UnityEngine;

namespace SelectableObjectsModule.SpecificObjects
{
    public class ElevatorCaller : MonoBehaviour, IInitStateReturnable
    {
        private PushableObject _button;
        private SwitchableObject _commonWires;
        private SwitchableObject _connector;

        private GameObject _connectorWires;

        private bool _isButtonAdded;
        private bool _isWiresConnected;
        private SwitchableObject _panel;

        public int InitStateSafeDistanceToPlayer { get; set; }

        public void ReturnToInitState(int floorDistanceToPlayer)
        {
            _panel.gameObject.SetActive(false);
            _connectorWires.SetActive(true);

            _button.gameObject.SetActive(false);

            _isButtonAdded = false;
            _isWiresConnected = false;
        }

        public event EventHandler CallIsDone;

        private void Start()
        {
            _connector = SelectableObject.GetAsChild(gameObject, ESwitchableObjectId.ELEVATOR_CALLER_CONNECTOR);
            _panel = SelectableObject.GetAsChild(_connector.gameObject, ESwitchableObjectId.ELEVATOR_CALLER_PANEL);
            _commonWires = SelectableObject.GetAsChild(_connector.gameObject, ESwitchableObjectId.ELEVATOR_CALLER_WIRES);
            _button = SelectableObject.GetAsChild<PushableObject>(_panel.gameObject, "button");
            _connectorWires = _connector.transform.Find("connector_static_wires").gameObject;

            _connector.Clicked += OnConnectorClicked;
            _connector.Closed += OnConnectorClosed;

            _panel.Clicked += OnPanelClicked;
            _panel.Closed += OnPanelClosed;
            _panel.OpenAnimationCompleted += OnPanelOpenAnimationCompleted;

            _commonWires.Opened += OnWiresConnectedWithTape;

            _button.Opened += OnButtonClicked;
        }

        private void OnButtonClicked(object sender, EventArgs e)
        {
            if (!_isWiresConnected) return;

            CallIsDone?.Invoke(this, EventArgs.Empty);
        }

        private void OnPanelClicked(object sender, SelectableObjectClickedEventArgs e)
        {
            if (e.SelectedInventoryItemId != EInventoryItemId.ELEVATOR_CALLER_BUTTON) return;

            _panel.PreventSwitching = true;
            _button.gameObject.SetActive(true);

            _isButtonAdded = true;
            Messenger<EInventoryItemId>.Broadcast(Events.InventoryItemWasSuccessfullyUsed,
                EInventoryItemId.ELEVATOR_CALLER_BUTTON);
            TryToSealConnectorAndPanel();
        }

        private void OnConnectorClicked(object s, SelectableObjectClickedEventArgs e)
        {
            if (e.SelectedInventoryItemId != EInventoryItemId.ELEVATOR_CALLER_PANEL) return;
            
            _connector.PreventSwitching = true;
            _panel.gameObject.SetActive(true);
            
            _connectorWires.SetActive(false);
            
            Messenger<EInventoryItemId>.Broadcast(Events.InventoryItemWasSuccessfullyUsed,
                EInventoryItemId.ELEVATOR_CALLER_PANEL);
        }

        private void OnWiresConnectedWithTape(object s, EventArgs e)
        {
            _isWiresConnected = true;
        }
        
        private void TryToSealConnectorAndPanel()
        {
            if (_connector.IsOpened || _panel.IsOpened || !_isWiresConnected || !_isButtonAdded) return;

            _connector.SealAndDisableGlowing();
            _panel.SealAndDisableGlowing();
        }

        private void OnPanelOpenAnimationCompleted(object sender, EventArgs e)
        {
            _commonWires.gameObject.SetActive(true);
        }

        private void OnPanelClosed(object sender, EventArgs e)
        {
            _commonWires.gameObject.SetActive(false);
            TryToSealConnectorAndPanel();
        }
        
        private void OnConnectorClosed(object sender, EventArgs e)
        {
            TryToSealConnectorAndPanel();
        }
    }
}