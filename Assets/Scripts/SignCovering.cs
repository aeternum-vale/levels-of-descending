using UnityEngine;

public class SignCovering : SwitchableObject
{
    GameObject coveringCut;

    protected override void Start()
    {
        base.Start();
        coveringCut = transform.parent.Find("covering_cut").gameObject;
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
