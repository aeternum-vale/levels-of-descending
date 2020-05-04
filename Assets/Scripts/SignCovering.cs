using UnityEngine;

public class SignCovering : SwitchableObject
{
    GameObject coveringCut;

    protected override void Start()
    {
        base.Start();
        coveringCut = transform.parent.Find("covering_cut").gameObject;
    }

    protected override void OnOpen()
    {
        IsOpened = true;
        gameObject.SetActive(false);
        coveringCut.SetActive(true);
    }

    protected override void OnClose()
    {
        IsOpened = false;
        gameObject.SetActive(true);
        coveringCut.SetActive(false);
    }

}
