using System;

namespace SelectableObjectsModule.Utilities
{
    public class MultiStateObjectEventArgs : EventArgs
    {
        public readonly byte StateId;

        public MultiStateObjectEventArgs(byte stateId)
        {
            StateId = stateId;
        }
    }
}