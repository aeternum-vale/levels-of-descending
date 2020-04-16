using System;
using UnityEngine;

[RequireComponent(typeof(Transform))]
public abstract class Door : MonoBehaviour
{
    protected GameObject doorBase;
    public GameObject DoorBase { get; private set; }

    protected GameObject details;
    public GameObject Details { get; private set; }

    protected GameObject doorhandle;
    public GameObject Doorhandle { get; private set; }

    protected GameObject staticDetails;
    public GameObject StaticDetails { get; private set; }

    static readonly string doorBaseName = "doorBase";
    static readonly string doorhandleName = "doorhandle";
    static readonly string staticDetailsName = "staticDetails";
    static readonly string detailsName = "details";

    void Awake()
    {
        SetDoorBase();
        SetDetails();
        SetDoorhandle();
        SetStaticDetails();
        Randomize();
    }

    protected void SetFromRoot(ref GameObject element, string elementName, bool isNecessary = true)
    {
        Transform searchedElementTransform = transform.Find(elementName);
        if (searchedElementTransform)
        {
            element = searchedElementTransform.gameObject;
        }
        else
        {
            if (isNecessary)
            {
                throw new Exception("Child with name " + elementName + " must be provided");
            }
        }
    }

    protected void SetDoorBase()
    {
        SetFromRoot(ref this.doorBase, doorBaseName);
    }

    protected void SetDoorhandle()
    {
        Transform searchedDoorhandleTransform = details.transform.Find(doorhandleName);
        if (searchedDoorhandleTransform)
        {
            this.doorhandle = searchedDoorhandleTransform.gameObject;
        }
        else
        {
            throw new Exception("Child with name " + doorhandleName + " must be provided");
        }
    }

    protected void SetDetails()
    {
        SetFromRoot(ref this.details, detailsName);
    }

    protected void SetStaticDetails()
    {
        SetFromRoot(ref this.staticDetails, staticDetailsName, false);
    }

    public void Invert()
    {
        transform.localScale = new Vector3(1, 1, -1);
        transform.position -= new Vector3(0, 0, 0.03f);
    }

    public void MarkWithDragonfly()
    {
        GameObject nameplate = details.transform.Find("nameplate").gameObject;
        nameplate.SetActive(true);
        nameplate.GetComponent<MeshRenderer>().material.SetFloat("_IsTitleOn", 1f);
    }

    public void UnmarkWithDragonfly()
    {
        GameObject nameplate = details.transform.Find("nameplate").gameObject;
        nameplate.SetActive(false);
        nameplate.GetComponent<MeshRenderer>().material.SetFloat("_IsTitleOn", 0f);
    }


    protected abstract void Randomize();
}