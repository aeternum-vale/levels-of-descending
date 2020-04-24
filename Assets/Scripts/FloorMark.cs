﻿public struct FloorMark
{
    public int Frequency { get; set; }
    public int FirstFloor { get; set; }

    public EInventoryItemID[] associatedInventoryItems;
    public bool IsFloorMarked(int floorNumber) => (floorNumber - FirstFloor) % Frequency == 0;
}
