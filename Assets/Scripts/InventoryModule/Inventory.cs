using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Plugins;
using UnityEngine;
using UnityEngine.UI;
using Utils;

namespace InventoryModule
{
    public class Inventory : MonoBehaviour
    {
        private const float TransitionStep = 0.1f;
        private const float TransitionXOffset = 15f;
        private const string ItemsContainerName = "items";

        private readonly Dictionary<EInventoryItemId, InventoryItemData> _itemsData =
            new Dictionary<EInventoryItemId, InventoryItemData>
            {
                [EInventoryItemId.POSTBOX_KEY] = new InventoryItemData(),
                [EInventoryItemId.LETTER] = new InventoryItemData(),
                [EInventoryItemId.SCALPEL] = new InventoryItemData(),
                [EInventoryItemId.E_PANEL_KEY] = new InventoryItemData(),
                [EInventoryItemId.SCREWDRIVER] = new InventoryItemData {IsInStock = true},
                [EInventoryItemId.INSULATING_TAPE] = new InventoryItemData(),
                [EInventoryItemId.ELEVATOR_CALLER_BUTTON] = new InventoryItemData {IsDisposable = true},
                [EInventoryItemId.ELEVATOR_CALLER_PANEL] = new InventoryItemData {IsDisposable = true}
            };

        private Image _backgroundImageComponent;
        private Texture2D _backgroundTexture;

        private float _currentTransitionOpacity = 1f;
        private InventoryCamera _inventoryCamera;
        private Camera _inventoryCameraCameraComponent;
        private RenderTexture _inventoryCameraTexture;
        private bool _isTransition;
        private bool _isTransitionOut = true;

        public bool IsInventoryModeOn { get; private set; }
        public bool CanActivateInventoryMode => _itemsData.Any(pair => pair.Value.IsInStock);

        private EInventoryItemId[] ArrayOfAvailableItemsIds => _itemsData
            .Where(pair => pair.Value.IsInStock)
            .Select(pair => pair.Key)
            .ToArray();

        private GameObject CurrentInstance => _itemsData[CurrentItemId].ItemGameObject;
        public EInventoryItemId CurrentItemId { get; private set; }

        public bool Contains(EInventoryItemId id)
        {
            return _itemsData[id].IsInStock;
        }

        private void Awake()
        {
            _inventoryCamera = transform.GetComponentInChildren<InventoryCamera>();
            _inventoryCameraCameraComponent = _inventoryCamera.GetComponent<Camera>();

            _backgroundImageComponent = transform.Find("Canvas").Find("Image").GetComponent<Image>();

            Messenger<EInventoryItemId>.AddListener(Events.InventoryObjectWasClicked,
                OnInventoryObjectWasClicked);
            Messenger.AddListener(Events.InventoryButtonWasPressed,
                OnInventorySwitchToNextItem);
            Messenger<EInventoryItemId>.AddListener(Events.InventoryItemWasSuccessfullyUsed,
                OnInventoryItemSuccessfullyUsed);

            var itemsContainerTransform = transform.Find(ItemsContainerName);
            foreach (var pair in _itemsData)
            {
                var itemName = GameUtils.GetNameByPath(GameConstants.inventoryObjectPaths[pair.Key]);
                var go = itemsContainerTransform.Find(itemName).gameObject;
                HideInstance(go);
                pair.Value.ItemGameObject = go;
            }
        }

        private void OnInventoryObjectWasClicked(EInventoryItemId id)
        {
            if (IsInventoryModeOn) return;

            _itemsData[id].IsInStock = true;
            Messenger.Broadcast(Events.InventoryWasUpdated);
        }

        private static void ShowInstance(GameObject instance)
        {
            instance.transform.GetChild(0).gameObject.SetActive(true);
        }

        private static void HideInstance(GameObject instance)
        {
            StopInstanceAnimation(instance);
            instance.transform.GetChild(0).gameObject.SetActive(false);
        }

        private static void StopInstanceAnimation(GameObject instance)
        {
            instance.GetComponent<Animator>().Play("inventoryItemRotation", -1, 0f);
            instance.GetComponent<Animator>().speed = 0f;
        }

        private static void StartInstanceAnimation(GameObject instance)
        {
            instance.GetComponent<Animator>().speed = 1f;
        }

        private void HideAllInstances()
        {
            foreach (var instance in _itemsData.Values.Select(data => data.ItemGameObject))
                HideInstance(instance);
        }

        public void ActivateInventoryMode(Texture2D backgroundTexture)
        {
            if (CanActivateInventoryMode)
            {
                IsInventoryModeOn = true;
                _backgroundTexture = backgroundTexture;
                _backgroundImageComponent.sprite = Sprite.Create(backgroundTexture,
                    new Rect(0, 0, Screen.width, Screen.height), new Vector2(0, 0));

                _inventoryCamera.IsInventoryModeOn = true;
                _inventoryCamera.DrawInventory = DrawInventory;

                HideAllInstances();

                CurrentItemId = ArrayOfAvailableItemsIds.First();
                ShowInstance(CurrentInstance);
                StartInstanceAnimation(CurrentInstance);

                _inventoryCamera.gameObject.SetActive(true);
            }
            else
            {
                throw new Exception("cannot activate inventory mode: there is no items in inventory");
            }
        }

        public void DeactivateInventoryMode()
        {
            IsInventoryModeOn = false;
            _inventoryCamera.IsInventoryModeOn = false;
            _inventoryCamera.gameObject.SetActive(false);
        }

        private void SwitchCurrentItemIdToNext()
        {
            CurrentItemId = CurrentItemId == ArrayOfAvailableItemsIds.Last()
                ? ArrayOfAvailableItemsIds.First()
                : ArrayOfAvailableItemsIds.First(id => (int) id > (int) CurrentItemId);
        }

        private IEnumerator SwitchToNextItem()
        {
            _isTransition = true;

            _backgroundImageComponent.enabled = false;
            _inventoryCameraCameraComponent.targetTexture = new RenderTexture(Screen.width, Screen.height, 24);
            _inventoryCameraCameraComponent.Render();
            _inventoryCameraTexture = _inventoryCameraCameraComponent.targetTexture;
            _inventoryCameraCameraComponent.targetTexture = null;
            HideAllInstances();

            _isTransitionOut = true;
            yield return StartCoroutine(FadeCurrentItem(true));

            SwitchCurrentItemIdToNext();
            ShowInstance(CurrentInstance);

            _inventoryCameraCameraComponent.targetTexture = new RenderTexture(Screen.width, Screen.height, 24);
            _inventoryCameraCameraComponent.Render();
            _inventoryCameraTexture = _inventoryCameraCameraComponent.targetTexture;
            _inventoryCameraCameraComponent.targetTexture = null;

            _isTransitionOut = false;
            yield return StartCoroutine(FadeCurrentItem(false));

            _isTransition = false;
            _inventoryCameraCameraComponent.targetTexture = null;
            _backgroundImageComponent.enabled = true;
            StartInstanceAnimation(CurrentInstance);
        }

        public void OnInventorySwitchToNextItem()
        {
            if (IsInventoryModeOn && !_isTransition && ArrayOfAvailableItemsIds.Length > 1)
                StartCoroutine(SwitchToNextItem());
        }

        private IEnumerator FadeCurrentItem(bool isOut)
        {
            for (_currentTransitionOpacity = isOut ? 1f : 0f;
                _currentTransitionOpacity >= 0 && _currentTransitionOpacity <= 1f;
                _currentTransitionOpacity += (isOut ? -1 : 1) * TransitionStep)
                yield return new WaitForFixedUpdate();
            yield return null;
        }

        public void DrawInventory()
        {
            if (!_isTransition) return;

            GL.PushMatrix();
            GL.LoadPixelMatrix();

            Graphics.DrawTexture(new Rect(0, 0, Screen.width, Screen.height), _backgroundTexture, new Rect(0, 1, 1, -1),
                0,
                0, 0, 0,
                new Color(.5f, .5f, .5f, 1f));

            Graphics.DrawTexture(new Rect(
                    (1 - _currentTransitionOpacity) * TransitionXOffset * (_isTransitionOut ? 1 : -1),
                    0, Screen.width, Screen.height), _inventoryCameraTexture, new Rect(0, 1, 1, -1), 0, 0, 0, 0,
                new Color(.5f, .5f, .5f, _currentTransitionOpacity));

            GL.PopMatrix();
        }

        private void OnInventoryItemSuccessfullyUsed(EInventoryItemId id)
        {
            if (_itemsData[id].IsDisposable)
            {
                _itemsData[id].IsInStock = false;
            }
        }
    }
}