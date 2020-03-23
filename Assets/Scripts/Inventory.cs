using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class Inventory : MonoBehaviour
{

    private GameObject inventoryCamera;
    private Camera inventoryCameraComponent;
    private GameObject key;
    private Renderer keyRenderer;

    public Dictionary<EInventoryObjectID, bool> availableObjectsDict { get; } = new Dictionary<EInventoryObjectID, bool>() {
        { EInventoryObjectID.POSTBOX_KEY, true },
        { EInventoryObjectID.SCREWDRIVER, true }
    };
    public bool IsInventoryModeOn { get; private set; }
    private readonly Dictionary<EInventoryObjectID, GameObject> instances = new Dictionary<EInventoryObjectID, GameObject>();
    private int currentObjectIndex;

    private List<EInventoryObjectID> listOfAvailableObjects;
    private float currentTransitionOpacity = 1f;
    private bool isTransition;
    private bool isTransitionOut = true;
    private readonly float transitionStep = 0.03f;
    private readonly float transitionStepTime = 0.0001f;
    private readonly float transitionXOffset = 15f;

    void Awake()
    {
        inventoryCamera = transform.Find("InventoryCamera").gameObject;
        inventoryCameraComponent = inventoryCamera.GetComponent<Camera>();

        key = transform.Find("key").gameObject;
        keyRenderer = key.GetComponent<Renderer>();

        Messenger<EInventoryObjectID>.AddListener(Events.ADD_OBJECT_TO_INVENTORY, OnObjectAdding);
        Messenger.AddListener(Events.INVENTORY_BUTTON_PRESSED, OnInventoryButtonPressed);

        foreach (KeyValuePair<EInventoryObjectID, string> item in GameConstants.InventoryInstanceNameMap)
        {
            GameObject go = transform.Find(item.Value).gameObject;
            go.GetComponent<Renderer>().enabled = false;
            instances.Add(item.Key, go);
        }
    }

    private List<EInventoryObjectID> GetListOfAvailableObjects()
    {
        return new List<EInventoryObjectID>(availableObjectsDict.Keys.Where(key => availableObjectsDict[key]));
    }

    private void OnObjectAdding(EInventoryObjectID id)
    {
        if (IsInventoryModeOn) return;

        availableObjectsDict.Add(id, true);
        Messenger.Broadcast(Events.INVENTORY_UPDATED);
    }

    private void ShowInstance(GameObject instance)
    {
        instance.GetComponent<Renderer>().enabled = true;
    }

    private void HideInstance(GameObject instance)
    {
        instance.GetComponent<Renderer>().enabled = false;
        StopInstanceAnimation(instance);
    }

    private void StopInstanceAnimation(GameObject instance)
    {
        instance.GetComponent<Animator>().Play("inventoryObjectRotation", -1, 0f);
        instance.GetComponent<Animator>().speed = 0f;

    }

    private void StartInstanceAnimation(GameObject instance)
    {
        instance.GetComponent<Animator>().speed = 1f;
    }


    private void HideAllInstances()
    {
        foreach (GameObject instance in instances.Values)
        {
            HideInstance(instance);
        }
    }

    private void InitInventoryMode()
    {
        listOfAvailableObjects = GetListOfAvailableObjects();
        if (listOfAvailableObjects.Count == 0) return;

        IsInventoryModeOn = true;
        currentObjectIndex = 0;

        HideAllInstances();
        GameObject currentInstance = instances[listOfAvailableObjects[currentObjectIndex]];
        ShowInstance(currentInstance);
        StartInstanceAnimation(currentInstance);
        inventoryCamera.SetActive(true);
    }

    private IEnumerator SwitchToNextObject()
    {
        inventoryCameraComponent.targetTexture = new RenderTexture(Screen.width, Screen.height, 24);
        inventoryCameraComponent.Render();
        inventoryCamera.SetActive(false);
        HideAllInstances();

        isTransition = true;
        isTransitionOut = true;
        yield return StartCoroutine(FadeCurrentObject(true));

        currentObjectIndex = (currentObjectIndex + 1) % listOfAvailableObjects.Count;
        GameObject currentInstance = instances[listOfAvailableObjects[currentObjectIndex]];

        ShowInstance(currentInstance);

        inventoryCameraComponent.targetTexture = new RenderTexture(Screen.width, Screen.height, 24);
        inventoryCameraComponent.Render();

        isTransitionOut = false;
        yield return StartCoroutine(FadeCurrentObject(false));

        isTransition = false;
        inventoryCameraComponent.targetTexture = null;
        inventoryCamera.SetActive(true);
        StartInstanceAnimation(currentInstance);
    }

    private void OnInventoryButtonPressed()
    {
        if (!IsInventoryModeOn)
        {
            InitInventoryMode();
        }
        else
        {
            if (!isTransition)
            {
                StartCoroutine(SwitchToNextObject());
            }
        }
    }

    private IEnumerator FadeCurrentObject(bool isOut)
    {
        int outSign = isOut ? -1 : 1;

        for (currentTransitionOpacity = (isOut ? 1f : 0f);
            currentTransitionOpacity >= 0 && currentTransitionOpacity <= 1f;
            currentTransitionOpacity += outSign * transitionStep)
        {
            yield return new WaitForSeconds(transitionStepTime);
        }
        yield return null;
    }

    public void DrawInventory()
    {
        if (!isTransition) return;

        GL.PushMatrix();
        GL.LoadPixelMatrix();
        Graphics.DrawTexture(new Rect((1 - currentTransitionOpacity) * transitionXOffset * (isTransitionOut ? 1 : -1),
            0, Screen.width, Screen.height), inventoryCameraComponent.targetTexture, new Rect(0, 1, 1, -1), 0, 0, 0, 0,
            new Color(.5f, .5f, .5f, currentTransitionOpacity));
        GL.PopMatrix();
    }

}