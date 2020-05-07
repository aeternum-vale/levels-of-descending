using System;
using InventoryModule;
using SelectableObjectsModule.Utilities;
using UnityEngine;

namespace SelectableObjectsModule.SpecificObjects
{
    public class ElevatorCaller : MonoBehaviour
    {
        private SwitchableObject _connector;
        private SwitchableObject _panel;
        private SwitchableObject _wires;
        private PushableObject _button;

        private bool _isPanelAdded;

        private void Start()
        {
            _connector = SelectableObject.GetAsChild(gameObject, ESwitchableObjectId.ELEVATOR_CALLER_CONNECTOR);
            _panel = SelectableObject.GetAsChild(_connector.gameObject, ESwitchableObjectId.ELEVATOR_CALLER_PANEL);
            _wires = SelectableObject.GetAsChild(_panel.gameObject, ESwitchableObjectId.ELEVATOR_CALLER_WIRES);
            _button = SelectableObject.GetAsChild<PushableObject>(_panel.gameObject, "button");

            _connector.States[(byte) ESwitchableObjectStateId.OPEN].OnAnimationEnd += OnPanelAdded;
            
            _panel.Clicked += OnPanelClicked;
        }

        private void OnPanelClicked(object sender, SelectableObjectClickedEventArgs e)
        {
            if (e.SelectedInventoryItemId == EInventoryItemId.ELEVATOR_CALLER_BUTTON)
            {
                _button.gameObject.SetActive(true);
            }
        }

        private void OnPanelAdded()
        {
            _panel.gameObject.SetActive(true);
            _isPanelAdded = true;

            _connector.StateTransitions[(byte) ESwitchableObjectStateId.CLOSE][0].SelectedInventoryItemId = null;
        }
        
    }
}