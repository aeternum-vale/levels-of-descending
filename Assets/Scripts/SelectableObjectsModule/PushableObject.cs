using System.Collections.Generic;

namespace SelectableObjectsModule
{
    public class PushableObject : MultiStateObject
    {
        private const string AnimationName = "push";

        protected override void Awake()
        {
            base.Awake();

            States = new List<GraphState>
            {
                new GraphState {Name = AnimationName, OnReached = OnPush}
            };

            StateTransitions = new Dictionary<byte, List<GraphTransition>>
            {
                {
                    0, new List<GraphTransition>
                    {
                        new GraphTransition
                        {
                            NextStateId = 0
                        }
                    }
                }
            };
        }

        protected virtual void OnPush()
        {
        }
    }
}