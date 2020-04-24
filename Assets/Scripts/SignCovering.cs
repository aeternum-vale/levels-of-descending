using UnityEngine;

public class SignCovering : SwitchableObject
{
    GameObject coveringCut;

    protected override void Start()
    {
        base.Start();
        coveringCut = transform.parent.Find("covering_cut").gameObject;
    }
    protected override bool SwitchCondition(EInventoryItemID? selectedInventoryItemId = null)
    {
        return (selectedInventoryItemId == EInventoryItemID.SCALPEL);
    }

    public override void Switch()
    {
        if (!IsOpened)
        {
            IsOpened = true;
            gameObject.SetActive(false);
            coveringCut.SetActive(true);
        }
        else
        {
            IsOpened = false;
            gameObject.SetActive(true);
            coveringCut.SetActive(false);
        }
    }

}
