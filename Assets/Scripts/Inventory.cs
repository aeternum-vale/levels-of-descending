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

    public Dictionary<EInventoryItemID, bool> AvailableItemsDict { get; } = new Dictionary<EInventoryItemID, bool>() {
        //{ EInventoryItemID.E_PANEL_KEY, true },
        //{ EInventoryItemID.LETTER, true },
        //{ EInventoryItemID.POSTBOX_KEY, true },
        //{ EInventoryItemID.SCALPEL, true },
        { EInventoryItemID.SCREWDRIVER, true },
        { EInventoryItemID.INSULATING_TAPE, true },
        { EInventoryItemID.ELEVATOR_BUTTON_PANEL, true }
    };
    readonly Dictionary<EInventoryItemID, GameObject> instances = new Dictionary<EInventoryItemID, GameObject>();
    int currentItemIndex;
    List<EInventoryItemID> listOfAvailableItems;
    float currentTransitionOpacity = 1f;
    bool isTransition;
    bool isTransitionOut = true;
    readonly float transitionStep = 0.1f;
    readonly float transitionStepTime = 0.001f;
    readonly float transitionXOffset = 15f;
    static readonly string itemsContainerName = "items";

    public bool Contains(EInventoryItemID id)
    {
        return AvailableItemsDict.ContainsKey(id);
    }

    public EInventoryItemID CurrentItemID => listOfAvailableItems[currentItemIndex];

    public bool IsInventoryModeOn { get; private set; }

    public bool CanActivateInventoryMode
    {
        get
        {
            UpdateListOfAvailableItems();
            return (listOfAvailableItems.Count != 0);
        }
    }

    void Awake()
    {
        inventoryCameraGameObject = transform.Find("InventoryCamera").gameObject;
        inventoryCameraComponent = inventoryCameraGameObject.GetComponent<Camera>();
        inventoryCamera = inventoryCameraGameObject.GetComponent<InventoryCamera>();

        backgroundImageComponent = transform.Find("Canvas").Find("Image").gameObject.GetComponent<Image>();

        Messenger<EInventoryItemID>.AddListener(Events.INVENTORY_ITEM_WAS_CLICKED, OnItemAdding);
        Messenger.AddListener(Events.INVENTORY_BUTTON_WAS_PRESSED, OnInventorySwitchToNextItem);

        foreach (KeyValuePair<EInventoryItemID, string> item in GameConstants.inventoryItemToInstanceNameMap)
        {
            GameObject go = transform.Find($"{itemsContainerName}/{item.Value}").gameObject;
            HideInstance(go);
            instances.Add(item.Key, go);
        }
    }

    private void UpdateListOfAvailableItems()
    {
        listOfAvailableItems = new List<EInventoryItemID>(AvailableItemsDict.Keys.Where(key => AvailableItemsDict[key]));
    }

    void OnItemAdding(EInventoryItemID id)
    {
        if (IsInventoryModeOn) return;

        AvailableItemsDict.Add(id, true);
        Messenger.Broadcast(Events.INVENTORY_WAS_UPDATED);
    }

    void ShowInstance(GameObject instance)
    {
        instance.transform.GetChild(0).gameObject.SetActive(true);
    }

    void HideInstance(GameObject instance)
    {
        StopInstanceAnimation(instance);
        instance.transform.GetChild(0).gameObject.SetActive(false);
    }

    void StopInstanceAnimation(GameObject instance)
    {
        instance.GetComponent<Animator>().Play("inventoryItemRotation", -1, 0f);
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
            currentItemIndex = 0;

            this.backgroundTexture = backgroundTexture;
            backgroundImageComponent.sprite = Sprite.Create(backgroundTexture, new Rect(0, 0, Screen.width, Screen.height), new Vector2(0, 0));

            inventoryCamera.IsInventoryModeOn = true;
            inventoryCamera.DrawInventory = DrawInventory;

            HideAllInstances();
            GameObject currentInstance = instances[listOfAvailableItems[currentItemIndex]];
            ShowInstance(currentInstance);
            StartInstanceAnimation(currentInstance);
            inventoryCameraGameObject.SetActive(true);

        }
        else
            throw new Exception("cannot activate inventory mode: there is no items in inventory");
    }

    public void DeactivateInventoryMode()
    {
        IsInventoryModeOn = false;
        inventoryCamera.IsInventoryModeOn = false;
        inventoryCameraGameObject.SetActive(false);
    }

    IEnumerator SwitchToNextItem()
    {
        isTransition = true;

        backgroundImageComponent.enabled = false;
        inventoryCameraComponent.targetTexture = new RenderTexture(Screen.width, Screen.height, 24);
        inventoryCameraComponent.Render();
        inventoryCameraTexture = inventoryCameraComponent.targetTexture;
        inventoryCameraComponent.targetTexture = null;
        HideAllInstances();

        isTransitionOut = true;
        yield return StartCoroutine(FadeCurrentItem(true));

        currentItemIndex = (currentItemIndex + 1) % listOfAvailableItems.Count;
        GameObject currentInstance = instances[listOfAvailableItems[currentItemIndex]];

        ShowInstance(currentInstance);

        inventoryCameraComponent.targetTexture = new RenderTexture(Screen.width, Screen.height, 24);
        inventoryCameraComponent.Render();
        inventoryCameraTexture = inventoryCameraComponent.targetTexture;
        inventoryCameraComponent.targetTexture = null;

        isTransitionOut = false;
        yield return StartCoroutine(FadeCurrentItem(false));

        isTransition = false;
        inventoryCameraComponent.targetTexture = null;
        backgroundImageComponent.enabled = true;
        StartInstanceAnimation(currentInstance);
    }

    public void OnInventorySwitchToNextItem()
    {
        if (IsInventoryModeOn && !isTransition && listOfAvailableItems.Count > 1)
        {
            StartCoroutine(SwitchToNextItem());
        }
    }

    IEnumerator FadeCurrentItem(bool isOut)
    {
        for (currentTransitionOpacity = (isOut ? 1f : 0f);
            currentTransitionOpacity >= 0 && currentTransitionOpacity <= 1f;
            currentTransitionOpacity += (isOut ? -1 : 1) * transitionStep)
        {
            yield return new WaitForFixedUpdate();
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