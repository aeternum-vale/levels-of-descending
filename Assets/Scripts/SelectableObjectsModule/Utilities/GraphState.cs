using System;

namespace SelectableObjectsModule.Utilities
{
    public class GraphState
    {
        public string Name;
        public Action OnReached;
        public Action OnAnimationEnd;
    }
}