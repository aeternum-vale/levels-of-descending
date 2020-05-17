namespace SelectableObjectsModule.Utilities
{
    public interface IInitStateReturnable
    {
        int InitStateSafeDistanceToPlayer { get; set; }
        void ReturnToInitState(int floorDistanceToPlayer);
    }
}