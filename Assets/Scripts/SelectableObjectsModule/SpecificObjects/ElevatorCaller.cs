using System;
using SelectableObjectsModule.Utilities;
using UnityEngine;

namespace SelectableObjectsModule.SpecificObjects
{
    public class ElevatorCaller : MonoBehaviour
    {
        private PushableObject _button;
        private SwitchableObject _connector;
        private SwitchableObject _panel;
        private SwitchableObject _wires;

        private void Start()
        {
            _connector = SelectableObject.GetAsChild(gameObject, ESwitchableObjectId.ELEVATOR_CALLER_CONNECTOR);
            _panel = SelectableObject.GetAsChild(_connector.gameObject, ESwitchableObjectId.ELEVATOR_CALLER_PANEL);
            _wires = SelectableObject.GetAsChild(_panel.gameObject, ESwitchableObjectId.ELEVATOR_CALLER_WIRES);
            _button = SelectableObject.GetAsChild<PushableObject>(_panel.gameObject, "button");

            _connector.CloseAnimationCompleted += OnPanelAdded;

            _panel.Clicked += OnPanelClicked;
        }

        private void OnPanelClicked(object sender, SelectableObjectClickedEventArgs e)
        {
            if (e.SelectedInventoryItemId == EInventoryItemId.ELEVATOR_CALLER_BUTTON)
                _button.gameObject.SetActive(true);
        }

        private void OnPanelAdded(object s, EventArgs e)
        {
            _panel.gameObject.SetActive(true);
            _connector.NecessaryInventoryItem = null;
        }
    }
}