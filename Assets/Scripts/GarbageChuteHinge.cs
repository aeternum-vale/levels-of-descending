public class GarbageChuteHinge : SwitchableObject
{
    GarbageChuteDoor door;

    protected override void Start()
    {
        base.Start();
        door = transform.parent.Find("door").gameObject.GetComponent<GarbageChuteDoor>();
    }

    protected override void Open()
    {
        base.Open();
        door.Unhinge();
    }
}
