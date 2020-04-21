using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SignCovering : SwitchableSelectableObject
{
    GameObject coveringCut;

    protected override void Start()
    {
        base.Start();
        coveringCut = transform.parent.Find("covering_cut").gameObject;
    }
    protected override bool SwitchCondition(EInventoryItemID? selectedInventoryItemId = null) =>
        (selectedInventoryItemId == EInventoryItemID.SCALPEL);

    public override void Switch()
    {
        if (!IsOpened)
        {
            IsOpened = true;
            gameObject.SetActive(false);
            coveringCut.SetActive(true);
        }
    }

}
