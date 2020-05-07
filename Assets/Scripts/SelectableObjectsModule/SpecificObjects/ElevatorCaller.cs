using System;
using InventoryModule;
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
            _wires = SelectableObject.GetAsChild(_connector.gameObject, ESwitchableObjectId.ELEVATOR_CALLER_WIRES);
            _button = SelectableObject.GetAsChild<PushableObject>(_panel.gameObject, "button");

            _connector.States[(byte) ESwitchableObjectStateId.OPEN].OnAnimationEnd +=
                () => _panel.gameObject.SetActive(true);
        }
    }
}