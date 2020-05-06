using SelectableObjectsModule;
using UnityEngine;

namespace DoorModule
{
    public class DoorPushableDetail : PushableObject
    {
        [SerializeField] public EDoorAction action;
    }
}