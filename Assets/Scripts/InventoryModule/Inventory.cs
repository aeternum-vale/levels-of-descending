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
        private static readonly int ItemRotationStateNameHash = Animator.StringToHash("inventoryItemRotation");

        private readonly Dictionary<EInventoryItemId, InventoryItemData> _itemsData =
            new Dictionary<EInventoryItemId, InventoryItemData>
            {
                [EInventoryItemId.POSTBOX_KEY] = new InventoryItemData() {},
                [EInventoryItemId.LETTER] = new InventoryItemData() {},
                [EInventoryItemId.SCALPEL] = new InventoryItemData() {},
                [EInventoryItemId.E_PANEL_KEY] = new InventoryItemData() {},
                [EInventoryItemId.SCREWDRIVER] = new InventoryItemData() {},
                [EInventoryItemId.INSULATING_TAPE] = new InventoryItemData() {},
                [EInventoryItemId.ELEVATOR_CALLER_BUTTON] =
                    new InventoryItemData {IsDisposable = true},
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
        private AudioListener _inventoryCameraAudioListener;

        public bool IsInventoryModeOn { get; private set; }
        public bool CanActivateInventoryMode => _itemsData.Any(pair => pair.Value.IsInStock);

        private EInventoryItemId[] ArrayOfAvailableItemsIds => _itemsData
            .Where(pair => pair.Value.IsInStock)
            .Select(pair => pair.Key)
            .ToArray();

        public EInventoryItemId CurrentItemId { get; private set; }

        public bool Contains(EInventoryItemId id)
        {
            return _itemsData[id].IsInStock;
        }

        private void Awake()
        {
            _inventoryCamera = transform.GetComponentInChildren<InventoryCamera>();
            _inventoryCameraCameraComponent = _inventoryCamera.GetComponent<Camera>();
            _backgroundImageComponent = transform.Find("Canvas/Image").GetComponent<Image>();
            _inventoryCameraAudioListener = _inventoryCamera.GetComponent<AudioListener>();

            Messenger<EInventoryItemId>.AddListener(Events.InventoryObjectWasClicked,
                OnInventoryObjectWasClicked);
            Messenger<EInventoryItemId>.AddListener(Events.InventoryItemWasSuccessfullyUsed,
                OnInventoryItemSuccessfullyUsed);

            Transform itemsContainerTransform = transform.Find(ItemsContainerName);
            foreach (var pair in _itemsData)
            {
                string itemName = GameUtils.GetNameByPath(GameConstants.inventoryObjectPaths[pair.Key]);
                GameObject go = itemsContainerTransform.Find(itemName).gameObject;
                pair.Value.Container = go;
                pair.Value.Content = go.transform.GetChild(0).gameObject;
                pair.Value.AnimatorComponent = go.GetComponent<Animator>();
                HideInstance(pair.Key);
            }
        }

        private void OnInventoryObjectWasClicked(EInventoryItemId id)
        {
            if (IsInventoryModeOn) return;

            _itemsData[id].IsInStock = true;
            Messenger.Broadcast(Events.InventoryWasUpdated);
        }

        private void ShowInstance(EInventoryItemId id)
        {
            _itemsData[id].Content.SetActive(true);
        }

        private void HideInstance(EInventoryItemId id)
        {
            StopInstanceAnimation(id);
            _itemsData[id].Content.SetActive(false);
        }

        private void StopInstanceAnimation(EInventoryItemId id)
        {
            _itemsData[id].AnimatorComponent.Play(ItemRotationStateNameHash, -1, 0f);
            _itemsData[id].AnimatorComponent.speed = 0f;
        }

        private void StartInstanceAnimation(EInventoryItemId id)
        {
            _itemsData[id].AnimatorComponent.speed = 1f;
        }

        private void HideAllInstances()
        {
            _itemsData.Keys.ToList().ForEach(HideInstance);
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
                ShowInstance(CurrentItemId);
                StartInstanceAnimation(CurrentItemId);

                _inventoryCamera.gameObject.SetActive(true);
                _inventoryCameraAudioListener.enabled = true;
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
            var arrayOfAvailableItemsIdsCopy = ArrayOfAvailableItemsIds;
            int indexOfCurrentItemId = Array.IndexOf(arrayOfAvailableItemsIdsCopy, CurrentItemId);

            CurrentItemId =
                arrayOfAvailableItemsIdsCopy[(indexOfCurrentItemId + 1) % arrayOfAvailableItemsIdsCopy.Length];
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
            ShowInstance(CurrentItemId);

            _inventoryCameraCameraComponent.targetTexture = new RenderTexture(Screen.width, Screen.height, 24);
            _inventoryCameraCameraComponent.Render();
            _inventoryCameraTexture = _inventoryCameraCameraComponent.targetTexture;
            _inventoryCameraCameraComponent.targetTexture = null;

            _isTransitionOut = false;
            yield return StartCoroutine(FadeCurrentItem(false));

            _isTransition = false;
            _inventoryCameraCameraComponent.targetTexture = null;
            _backgroundImageComponent.enabled = true;
            StartInstanceAnimation(CurrentItemId);
        }

        public void OnInventorySwitchToNextItem()
        {
            if (!IsInventoryModeOn || _isTransition || ArrayOfAvailableItemsIds.Length <= 1) return;

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
                0, 0, 0, new Color(.5f, .5f, .5f, 1f));

            Graphics.DrawTexture(new Rect(
                    (1 - _currentTransitionOpacity) * TransitionXOffset * (_isTransitionOut ? 1 : -1),
                    0, Screen.width, Screen.height), _inventoryCameraTexture, new Rect(0, 1, 1, -1), 0, 0, 0, 0,
                new Color(.5f, .5f, .5f, _currentTransitionOpacity));

            GL.PopMatrix();
        }

        private void OnInventoryItemSuccessfullyUsed(EInventoryItemId id)
        {
            if (_itemsData[id].IsDisposable) _itemsData[id].IsInStock = false;
        }
    }
}