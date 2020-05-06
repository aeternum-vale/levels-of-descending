using System.Collections.Generic;
using Plugins;
using UnityEngine;

namespace SelectableObjectsModule
{
    [RequireComponent(typeof(Animator))]
    public class SwitchableObject : MultiStateObject
    {
        [SerializeField] private ESwitchableObjectId id;
        [SerializeField] private EInventoryItemId necessaryInventoryItem;
        [SerializeField] private bool hasValueOfNecessaryInventoryItem;

        public bool IsOpened { get; set; }

        private static readonly string SwitchStateName = "Switch";

        protected override void Awake()
        {
            base.Awake();

            States = new List<GraphState>()
            {
                new GraphState() {Name = SwitchStateName, OnReached = OnClose}, //0 - close
                new GraphState() {Name = SwitchStateName, OnReached = OnOpen} //1 - open
            };

            StateTransitions = new Dictionary<byte, List<GraphTransition>>()
            {
                {
                    (byte) ESwitchableObjectStateId.CLOSE, new List<GraphTransition>()
                    {
                        new GraphTransition()
                        {
                            NextStateId = (byte) ESwitchableObjectStateId.OPEN,
                            SelectedInventoryItemId = hasValueOfNecessaryInventoryItem
                                ? necessaryInventoryItem
                                : (EInventoryItemId?) null,
                            Condition = OpenCondition
                        }
                    }
                },

                {
                    (byte) ESwitchableObjectStateId.OPEN, new List<GraphTransition>()
                    {
                        new GraphTransition()
                        {
                            NextStateId = (byte) ESwitchableObjectStateId.CLOSE,
                            IsReverse = true
                        }
                    }
                }
            };
        }

        public virtual void Switch()
        {
            MakeTransition(StateTransitions[CurrentStateId][0]);
        }

        protected virtual void OnOpen()
        {
            IsOpened = true;
            Messenger<ESwitchableObjectId>.Broadcast(Events.switchableObjectWasOpened, id);

            if (isDisposable) Seal();
        }

        protected virtual void OnClose()
        {
            IsOpened = false;
        }

        protected virtual bool OpenCondition()
        {
            return true;
        }

        public static string GetPath(ESwitchableObjectId id)
        {
            return GameConstants.switchableObjectToInstancePathMap[id];
        }

        public static string GetName(ESwitchableObjectId id)
        {
            return GameUtils.GetNameByPath(GameConstants.switchableObjectToInstancePathMap[id]);
        }
    }
}