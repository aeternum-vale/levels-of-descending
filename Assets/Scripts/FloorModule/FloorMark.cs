namespace FloorModule
{
    public struct FloorMark
    {
        public int Frequency { get; set; }
        public int FirstFloor { get; set; }

        public EInventoryItemId[] AssociatedInventoryItems;

        public bool IsFloorMarked(int floorNumber)
        {
            return (floorNumber - FirstFloor) % Frequency == 0;
        }
    }
}