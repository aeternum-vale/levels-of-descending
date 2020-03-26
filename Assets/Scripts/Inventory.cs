using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

public class Inventory : MonoBehaviour
{
    GameObject inventoryCameraGameObject;
    InventoryCamera inventoryCamera;
    Camera inventoryCameraComponent;

    Image backgroundImageComponent;
    Texture2D backgroundTexture;
    RenderTexture inventoryCameraTexture;

    public Dictionary<EInventoryObjectID, bool> AvailableObjectsDict { get; } = new Dictionary<EInventoryObjectID, bool>() {
        { EInventoryObjectID.POSTBOX_KEY, true },
        { EInventoryObjectID.SCREWDRIVER, true }
    };
    public bool IsInventoryModeOn { get; private set; }

    readonly Dictionary<EInventoryObjectID, GameObject> instances = new Dictionary<EInventoryObjectID, GameObject>();
    int currentObjectIndex;
    List<EInventoryObjectID> listOfAvailableObjects;
    float currentTransitionOpacity = 1f;
    bool isTransition;
    bool isTransitionOut = true;
    readonly float transitionStep = 0.03f;
    readonly float transitionStepTime = 0.001f;
    readonly float transitionXOffset = 15f;
    public bool CanActivateInventoryMode
    {
        get
        {
            UpdateListOfAvailableObjects();
            return (listOfAvailableObjects.Count != 0);
        }
    }

    public EInventoryObjectID CurrentObjectID => listOfAvailableObjects[currentObjectIndex];

    void Awake()
    {
        inventoryCameraGameObject = transform.Find("InventoryCamera").gameObject;
        inventoryCameraComponent = inventoryCameraGameObject.GetComponent<Camera>();
        inventoryCamera = inventoryCameraGameObject.GetComponent<InventoryCamera>();

        backgroundImageComponent = transform.Find("Canvas").Find("Image").gameObject.GetComponent<Image>();

        Messenger<EInventoryObjectID>.AddListener(Events.ADD_OBJECT_TO_INVENTORY, OnObjectAdding);
        Messenger.AddListener(Events.INVENTORY_BUTTON_PRESSED, OnInventorySwitchToNextObject);

        foreach (KeyValuePair<EInventoryObjectID, string> item in GameConstants.InventoryInstanceNameMap)
        {
            GameObject go = transform.Find(item.Value).gameObject;
            go.GetComponent<Renderer>().enabled = false;
            instances.Add(item.Key, go);
        }
    }

    private void UpdateListOfAvailableObjects()
    {
        listOfAvailableObjects = new List<EInventoryObjectID>(AvailableObjectsDict.Keys.Where(key => AvailableObjectsDict[key]));
    }

    void OnObjectAdding(EInventoryObjectID id)
    {
        if (IsInventoryModeOn) return;

        AvailableObjectsDict.Add(id, true);
        Messenger.Broadcast(Events.INVENTORY_UPDATED);
    }

    void ShowInstance(GameObject instance)
    {
        instance.GetComponent<Renderer>().enabled = true;
    }

    void HideInstance(GameObject instance)
    {
        instance.GetComponent<Renderer>().enabled = false;
        StopInstanceAnimation(instance);
    }

    void StopInstanceAnimation(GameObject instance)
    {
        instance.GetComponent<Animator>().Play("inventoryObjectRotation", -1, 0f);
        instance.GetComponent<Animator>().speed = 0f;

    }

    void StartInstanceAnimation(GameObject instance)
    {
        instance.GetComponent<Animator>().speed = 1f;
    }


    void HideAllInstances()
    {
        foreach (GameObject instance in instances.Values)
        {
            HideInstance(instance);
        }
    }

    public void ActivateInventoryMode(Texture2D backgroundTexture)
    {
        if (CanActivateInventoryMode)
        {
            IsInventoryModeOn = true;
            currentObjectIndex = 0;

            this.backgroundTexture = backgroundTexture;
            backgroundImageComponent.sprite = Sprite.Create(backgroundTexture, new Rect(0, 0, Screen.width, Screen.height), new Vector2(0, 0));

            inventoryCamera.IsInventoryModeOn = true;
            inventoryCamera.DrawInventory = DrawInventory;

            HideAllInstances();
            GameObject currentInstance = instances[listOfAvailableObjects[currentObjectIndex]];
            ShowInstance(currentInstance);
            StartInstanceAnimation(currentInstance);
            inventoryCameraGameObject.SetActive(true);

        }
        else
            throw new Exception("cannot activate inventory mode: there is no objects in inventory");
    }

    public void DeactivateInventoryMode()
    {
        IsInventoryModeOn = false;
        inventoryCamera.IsInventoryModeOn = false;
        inventoryCameraGameObject.SetActive(false);
    }

    IEnumerator SwitchToNextObject()
    {
        isTransition = true;

        backgroundImageComponent.enabled = false;
        inventoryCameraComponent.targetTexture = new RenderTexture(Screen.width, Screen.height, 24);
        inventoryCameraComponent.Render();
        inventoryCameraTexture = inventoryCameraComponent.targetTexture;
        inventoryCameraComponent.targetTexture = null;
        HideAllInstances();

        isTransitionOut = true;
        yield return StartCoroutine(FadeCurrentObject(true));

        currentObjectIndex = (currentObjectIndex + 1) % listOfAvailableObjects.Count;
        GameObject currentInstance = instances[listOfAvailableObjects[currentObjectIndex]];

        ShowInstance(currentInstance);

        inventoryCameraComponent.targetTexture = new RenderTexture(Screen.width, Screen.height, 24);
        inventoryCameraComponent.Render();
        inventoryCameraTexture = inventoryCameraComponent.targetTexture;
        inventoryCameraComponent.targetTexture = null;

        isTransitionOut = false;
        yield return StartCoroutine(FadeCurrentObject(false));

        isTransition = false;
        inventoryCameraComponent.targetTexture = null;
        backgroundImageComponent.enabled = true;
        StartInstanceAnimation(currentInstance);
    }

    public void OnInventorySwitchToNextObject()
    {
        if (IsInventoryModeOn && !isTransition)
        {
            StartCoroutine(SwitchToNextObject());
        }
    }

    IEnumerator FadeCurrentObject(bool isOut)
    {
        for (currentTransitionOpacity = (isOut ? 1f : 0f);
            currentTransitionOpacity >= 0 && currentTransitionOpacity <= 1f;
            currentTransitionOpacity += (isOut ? -1 : 1) * transitionStep)
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

        Graphics.DrawTexture(new Rect(0, 0, Screen.width, Screen.height), backgroundTexture, new Rect(0, 1, 1, -1), 0, 0, 0, 0,
            new Color(.5f, .5f, .5f, 1f));

        Graphics.DrawTexture(new Rect((1 - currentTransitionOpacity) * transitionXOffset * (isTransitionOut ? 1 : -1),
            0, Screen.width, Screen.height), inventoryCameraTexture, new Rect(0, 1, 1, -1), 0, 0, 0, 0,
            new Color(.5f, .5f, .5f, currentTransitionOpacity));

        GL.PopMatrix();
    }

}