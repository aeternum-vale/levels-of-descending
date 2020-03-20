using System;
using UnityEngine;

[RequireComponent (typeof (Transform))]
public abstract class Door : MonoBehaviour {
    protected GameObject doorBase;
    public GameObject DoorBase { get; private set; }

    protected GameObject details;
    public GameObject Details { get; private set; }

    protected GameObject doorhandle;
    public GameObject Doorhandle { get; private set; }

    protected GameObject staticDetails;
    public GameObject StaticDetails { get; private set; }

    private static readonly string doorBaseName = "doorBase";
    private static readonly string doorhandleName = "doorhandle";
    private static readonly string staticDetailsName = "staticDetails";
    private static readonly string detailsName = "details";

    void Awake () {
        setDoorBase ();
        setDetails ();
        setDoorhandle ();
        setStaticDetails ();
        randomize ();
    }

    protected void setFromRoot (ref GameObject element, string elementName, bool isNecessary = true) {
        Transform searchedElementTransform = transform.Find (elementName);
        if (searchedElementTransform) {
            element = searchedElementTransform.gameObject;
        } else {
            if (isNecessary) {
                throw new Exception ("Child with name " + elementName + " must be provided");
            }
        }
    }

    protected void setDoorBase () {
        setFromRoot (ref this.doorBase, doorBaseName);
    }

    protected void setDoorhandle () {
        Transform searchedDoorhandleTransform = details.transform.Find (doorhandleName);
        if (searchedDoorhandleTransform) {
            this.doorhandle = searchedDoorhandleTransform.gameObject;
        } else {
            throw new Exception ("Child with name " + doorhandleName + " must be provided");
        }
    }

    protected void setDetails () {
        setFromRoot (ref this.details, detailsName);
    }

    protected void setStaticDetails () {
        setFromRoot (ref this.staticDetails, staticDetailsName, false);
    }

    public void invert () {
        transform.localScale = new Vector3 (1, 1, -1);
        transform.position -= new Vector3 (0, 0, 0.03f);
    }

    protected abstract void randomize ();
}