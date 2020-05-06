using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Plugins;
using UnityEngine;
using UnityEngine.UI;

namespace InventoryModule
{
    public class Inventory : MonoBehaviour
    {
        private InventoryCamera _inventoryCamera;
        private Camera _inventoryCameraCameraComponent;

        private Image _backgroundImageComponent;
        private Texture2D _backgroundTexture;
        private RenderTexture _inventoryCameraTexture;

        public Dictionary<EInventoryItemId, bool> AvailableItemsDict { get; } = new Dictionary<EInventoryItemId, bool>()
        {
            //{ EInventoryItemID.E_PANEL_KEY, true },
            //{ EInventoryItemID.LETTER, true },
            //{ EInventoryItemID.POSTBOX_KEY, true },
            //{ EInventoryItemID.SCALPEL, true },
            //{ EInventoryItemID.SCREWDRIVER, true },
            //{ EInventoryItemID.INSULATING_TAPE, true },
            //{ EInventoryItemID.ELEVATOR_BUTTON_PANEL, true }
        };

        private readonly Dictionary<EInventoryItemId, GameObject>
            _instances = new Dictionary<EInventoryItemId, GameObject>();

        private int _currentItemIndex;
        private List<EInventoryItemId> _listOfAvailableItems;
        private float _currentTransitionOpacity = 1f;
        private bool _isTransition;
        private bool _isTransitionOut = true;
        private readonly float _transitionStep = 0.1f;
        private readonly float _transitionXOffset = 15f;
        private static readonly string ItemsContainerName = "items";

        public bool Contains(EInventoryItemId id)
        {
            return AvailableItemsDict.ContainsKey(id);
        }

        public EInventoryItemId CurrentItemId => _listOfAvailableItems[_currentItemIndex];

        public bool IsInventoryModeOn { get; private set; }

        public bool CanActivateInventoryMode
        {
            get
            {
                UpdateListOfAvailableItems();
                return _listOfAvailableItems.Count != 0;
            }
        }

        private void Awake()
        {
            _inventoryCamera = transform.GetComponentInChildren<InventoryCamera>();
            _inventoryCameraCameraComponent = _inventoryCamera.GetComponent<Camera>();

            _backgroundImageComponent = transform.Find("Canvas").Find("Image").GetComponent<Image>();

            Messenger<EInventoryItemId>.AddListener(Events.inventoryItemWasClicked, OnItemAdding);
            Messenger.AddListener(Events.inventoryButtonWasPressed, OnInventorySwitchToNextItem);

            foreach (var item in GameConstants.inventoryItemToInstancePathMap)
            {
                var itemName = GameUtils.GetNameByPath(item.Value);
                var go = transform.Find($"{ItemsContainerName}/{itemName}").gameObject;
                HideInstance(go);
                _instances.Add(item.Key, go);
            }
        }

        private void UpdateListOfAvailableItems()
        {
            _listOfAvailableItems =
                new List<EInventoryItemId>(AvailableItemsDict.Keys.Where(key => AvailableItemsDict[key]));
        }

        private void OnItemAdding(EInventoryItemId id)
        {
            if (IsInventoryModeOn) return;

            AvailableItemsDict.Add(id, true);
            Messenger.Broadcast(Events.inventoryWasUpdated);
        }

        private void ShowInstance(GameObject instance)
        {
            instance.transform.GetChild(0).gameObject.SetActive(true);
        }

        private void HideInstance(GameObject instance)
        {
            StopInstanceAnimation(instance);
            instance.transform.GetChild(0).gameObject.SetActive(false);
        }

        private void StopInstanceAnimation(GameObject instance)
        {
            instance.GetComponent<Animator>().Play("inventoryItemRotation", -1, 0f);
            instance.GetComponent<Animator>().speed = 0f;
        }

        private void StartInstanceAnimation(GameObject instance)
        {
            instance.GetComponent<Animator>().speed = 1f;
        }

        private void HideAllInstances()
        {
            foreach (var instance in _instances.Values) HideInstance(instance);
        }

        public void ActivateInventoryMode(Texture2D backgroundTexture)
        {
            if (CanActivateInventoryMode)
            {
                IsInventoryModeOn = true;
                _currentItemIndex = 0;

                this._backgroundTexture = backgroundTexture;
                _backgroundImageComponent.sprite = Sprite.Create(backgroundTexture,
                    new Rect(0, 0, Screen.width, Screen.height), new Vector2(0, 0));

                _inventoryCamera.IsInventoryModeOn = true;
                _inventoryCamera.DrawInventory = DrawInventory;

                HideAllInstances();
                var currentInstance = _instances[_listOfAvailableItems[_currentItemIndex]];
                ShowInstance(currentInstance);
                StartInstanceAnimation(currentInstance);
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

            _currentItemIndex = (_currentItemIndex + 1) % _listOfAvailableItems.Count;
            var currentInstance = _instances[_listOfAvailableItems[_currentItemIndex]];

            ShowInstance(currentInstance);

            _inventoryCameraCameraComponent.targetTexture = new RenderTexture(Screen.width, Screen.height, 24);
            _inventoryCameraCameraComponent.Render();
            _inventoryCameraTexture = _inventoryCameraCameraComponent.targetTexture;
            _inventoryCameraCameraComponent.targetTexture = null;

            _isTransitionOut = false;
            yield return StartCoroutine(FadeCurrentItem(false));

            _isTransition = false;
            _inventoryCameraCameraComponent.targetTexture = null;
            _backgroundImageComponent.enabled = true;
            StartInstanceAnimation(currentInstance);
        }

        public void OnInventorySwitchToNextItem()
        {
            if (IsInventoryModeOn && !_isTransition && _listOfAvailableItems.Count > 1) StartCoroutine(SwitchToNextItem());
        }

        private IEnumerator FadeCurrentItem(bool isOut)
        {
            for (_currentTransitionOpacity = isOut ? 1f : 0f;
                _currentTransitionOpacity >= 0 && _currentTransitionOpacity <= 1f;
                _currentTransitionOpacity += (isOut ? -1 : 1) * _transitionStep)
                yield return new WaitForFixedUpdate();
            yield return null;
        }

        public void DrawInventory()
        {
            if (!_isTransition) return;

            GL.PushMatrix();
            GL.LoadPixelMatrix();

            Graphics.DrawTexture(new Rect(0, 0, Screen.width, Screen.height), _backgroundTexture, new Rect(0, 1, 1, -1), 0,
                0, 0, 0,
                new Color(.5f, .5f, .5f, 1f));

            Graphics.DrawTexture(new Rect((1 - _currentTransitionOpacity) * _transitionXOffset * (_isTransitionOut ? 1 : -1),
                    0, Screen.width, Screen.height), _inventoryCameraTexture, new Rect(0, 1, 1, -1), 0, 0, 0, 0,
                new Color(.5f, .5f, .5f, _currentTransitionOpacity));

            GL.PopMatrix();
        }
    }
}