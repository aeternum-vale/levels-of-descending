using UnityEngine;

[RequireComponent(typeof(Transform))]
public abstract class Door : MonoBehaviour
{
    public GameObject DoorBase { get; private set; }
    public GameObject Details { get; private set; }
    public GameObject Handle { get; private set; }
    public GameObject StaticDetails { get; private set; }
    public GameObject BellButton { get; private set; }
    public GameObject Nameplate { get; private set; }
    public GameObject Peephole { get; private set; }

    public bool IsDragonflyMarked { get; private set; }

    static readonly string doorBaseName = "doorBase";
    static readonly string handleName = "doorhandle";
    static readonly string bellButtonName = "door_bell_button";
    static readonly string staticDetailsName = "staticDetails";
    static readonly string detailsName = "details";
    static readonly string nameplateName = "nameplate";
    static readonly string peepholeName = "peephole";

    readonly EDoorAction[] lastActions = new EDoorAction[GameConstants.dragonflyCode.Length];
    int lastActionsCursor = 0;

    void Awake()
    {
        DoorBase = transform.Find(doorBaseName).gameObject;
        Details = transform.Find(detailsName).gameObject;
        Handle = Details.transform.Find(handleName).gameObject;
        Nameplate = Details.transform.Find(nameplateName).gameObject;
        Peephole = Details.transform.Find(peepholeName).gameObject;

        StaticDetails = transform.Find(staticDetailsName).gameObject;
        BellButton = StaticDetails.transform.Find(bellButtonName).gameObject;

        Randomize();
    }


    public void Invert()
    {
        transform.localScale = new Vector3(1, 1, -1);
        transform.position -= new Vector3(0, 0, 0.03f);
    }

    public void MarkWithDragonfly()
    {
        Nameplate.SetActive(true);
        Peephole.SetActive(false);
        Nameplate.GetComponent<MeshRenderer>().material.SetFloat("_IsTitleOn", 1f);
        IsDragonflyMarked = true;
    }

    public void UnmarkWithDragonfly()
    {
        Nameplate.SetActive(false);
        Peephole.SetActive(true);
        Nameplate.GetComponent<MeshRenderer>().material.SetFloat("_IsTitleOn", 0f);
        IsDragonflyMarked = false;
    }

    public void Interact(EDoorAction action)
    {
        lastActions[lastActionsCursor] = action;
        lastActionsCursor = (lastActionsCursor + 1) % lastActions.Length;

        if (!IsDragonflyMarked)
        {
            return;
        }

        EDoorAction[] dragonflyCode = GameConstants.dragonflyCode;

        for (int i = 0; i < dragonflyCode.Length; i++)
        {
            if (dragonflyCode[i] != lastActions[(lastActionsCursor + i) % dragonflyCode.Length])
            {
                return;
            }
        }

        Messenger<Door>.Broadcast(Events.DRAGONFLY_CODE_ACTIVATED, this);
    }

    protected abstract void Randomize();

};