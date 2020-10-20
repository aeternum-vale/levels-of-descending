using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using AdGeneratorModule;
using BackgroundMusicModule;
using FloorModule;
using GameCanvasModule;
using InventoryModule;
using PlayerModule;
using Plugins;
using ResourcesModule;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using Utils;

public class GameController : MonoBehaviour
{
    private const float FloorHeight = 3.99f;
    private const int FloorCount = 5;
    private const int FirstFloorNumber = 7;
    
    private const int SuspenseStartFloorNumber = 9;
    private const int SuspenseEndFloorNumber = 42;
    
    private const int DemoCameraMoveDurationSec = 25;
    private const float TipAlpha = 0.6f;
    private const float TipAlphaRate = 0.05f;

    private readonly Dictionary<Floor, GameObject> _ground1Colliders = new Dictionary<Floor, GameObject>();

    private AudioSource _audioSource;
    private Floor _currentDemoCameraFloor;
    private GameObject _demoCameraInnerObject;
    private GameObject _demoCameraRotateContainer;

    private int _fakeFloorNumber = FirstFloorNumber;

    private Floor[] _floors;
    private int _floorsHalf;

    private bool _gameIsOver;
    private int _highestFloorIndex;

    private Player _player;
    private int _realFloorNumber = FirstFloorNumber;
    
    private string _useButtonName;

    private bool _wasInventoryTipShown;
    [SerializeField] private AdGenerator adGenerator;
    [SerializeField] private BackgroundMusicController backgroundMusicController;
    [SerializeField] private GameObject demoCamera;
    [SerializeField] private Text exitTipText;
    [SerializeField] private GameObject floorPrefab;
    [SerializeField] private Inventory inventory;
    [SerializeField] private Text inventoryTipText;

    [SerializeField] private bool isItMenuScene;
    [SerializeField] private GameObject playerGameObject;
    [SerializeField] private ResourcesController resourcesController;
    [SerializeField] private GameCanvas gameCanvas;

    private int LowestFloorIndex => (_highestFloorIndex + 1) % FloorCount;

    private void Start()
    {
        _highestFloorIndex = FloorCount - 1;
        _floors = new Floor[FloorCount];

        for (int i = 0; i < FloorCount; i++)
        {
            _floors[i] = GenerateRandomFloor(floorPrefab.transform.position + new Vector3(0, FloorHeight * i, 0));
            _ground1Colliders.Add(_floors[i],
                _floors[i].transform.Find(GameConstants.collidersObjectName)
                    .Find(GameConstants.ground1ColliderObjectName).gameObject);
        }

        _floorsHalf = FloorCount / 2;

        Floor initPlayerFloor = _floors[_floorsHalf];
        initPlayerFloor.SetFloorDrawnNumber(_fakeFloorNumber--);

        Vector3 localPosition = playerGameObject.transform.localPosition;

        localPosition = new Vector3(
            localPosition.x,
            initPlayerFloor.transform.localPosition.y,
            localPosition.z
        );

        playerGameObject.transform.localPosition = localPosition;
        initPlayerFloor.SetHandsTextureToElevatorAd();

        foreach (Floor floor in _floors)
            floor.GenerateRandomTextureProjectorsAndGarbageProps();

        if (isItMenuScene)
        {
            Cursor.lockState = CursorLockMode.None;
            Cursor.visible = true;

            backgroundMusicController.BackgroundMusicIntensity = .45f;
            _currentDemoCameraFloor = initPlayerFloor;
            GetNextHigherFloor().SetFloorDrawnNumber(++_fakeFloorNumber + 1);
            demoCamera.transform.position = initPlayerFloor.DemoCameraPlaceholder.transform.position;
            MoveDemoCameraToNextPlaceholder();
        }
        else
        {
            gameCanvas.FadeIn();
        }
    }

    private void Awake()
    {
        if (!isItMenuScene)
        {
            _audioSource = GetComponent<AudioSource>();
            _player = playerGameObject.GetComponent<Player>();

            Messenger.AddListener(Events.FloorWasTouched, OnFloorWasTouched);
            Messenger.AddListener(Events.InventoryWasUpdated, OnInventoryWasUpdated);
            Messenger.AddListener(Events.InventoryModeBeforeActivating, OnInventoryModeBeforeActivating);
            Messenger.AddListener(Events.CowCodeActivated, OnCowCodeActivated);
            Messenger.AddListener(Events.ElevatorFloorWasTouched, OnElevatorFloorWasTouched);
            Messenger.AddListener(Events.PlayerCutSceneMoveCompleted, OnPlayerCutSceneMoveCompleted);
            Messenger.AddListener(Events.Elevating, OnElevating);
            Messenger.AddListener(Events.InventoryItemUsedIncorrectly, OnInventoryItemUsedIncorrectly);
            Messenger.AddListener(Events.FullBlackoutReached, OnFullBlackoutReached);
            Messenger.AddListener(Events.ExitButtonClicked, OnExitButtonClicked);
        }
        else
        {
            _demoCameraRotateContainer = demoCamera.transform.GetChild(0).gameObject;
            _demoCameraInnerObject = _demoCameraRotateContainer.transform.GetChild(0).gameObject;
        }
    }

    private bool IsFloorCurrent(Floor floor)
    {
        if (isItMenuScene)
            return floor == _currentDemoCameraFloor;

        return _player.LastGround1ColliderTouched == _ground1Colliders[floor];
    }

    private Floor GetNextHigherFloor()
    {
        int i = GetCurrentFloorIndex();
        return _floors[(i + 1) % FloorCount];
    }

    private Floor GetNextLowerFloor()
    {
        int i = GetCurrentFloorIndex();
        return _floors[i - 1 >= 0 ? i - 1 : FloorCount - 1];
    }

    private Floor GetCurrentFloor()
    {
        return _floors[GetCurrentFloorIndex()];
    }

    private int GetCurrentFloorIndex()
    {
        for (int i = 0; i < _floors.Length; i++)
            if (IsFloorCurrent(_floors[i]))
                return i;

        throw new Exception("Can't compute current floor index");
    }

    private int GetFloorIndex(Floor floor)
    {
        for (int i = 0; i < _floors.Length; i++)
            if (_floors[i] == floor)
                return i;
        throw new Exception("Can't compute provided floor index");
    }

    private byte GetFloorDistanceToPlayer(Floor floor)
    {
        int dist = Math.Abs(GetFloorIndex(floor) - GetCurrentFloorIndex());
        return (byte) Math.Min(dist, FloorCount - dist);
    }

    private bool? IsPlayerGoingUp()
    {
        if (_player.PrevLastGround1ColliderTouched == null) return null;

        return _player.PrevLastGround1ColliderTouched.transform.position.y <
               _player.LastGround1ColliderTouched.transform.position.y;
    }

    private Floor GenerateRandomFloor(Vector3 position)
    {
        Floor floor = Instantiate(floorPrefab).GetComponent<Floor>();
        floor.AdGenerator = adGenerator;
        floor.ResourcesController = resourcesController;
        floor.transform.position = position;
        floor.UpdateDoors();
        return floor;
    }

    private void RandomizeFloor(Floor floor)
    {
        //updateFloorDoors (floor);
    }

    private void OnFloorWasTouched()
    {
        _fakeFloorNumber++;

        UpdateSuspenseIntensity();
        UpdateRealFloorNumber();
        RearrangeFloors();
        FloorsSelectiveUpdate();
    }

    private void UpdateSuspenseIntensity()
    {
         float si = Mathf.Clamp(
            (float) (_fakeFloorNumber - SuspenseStartFloorNumber) / (SuspenseEndFloorNumber - SuspenseStartFloorNumber),
            0f,
            1f);

         backgroundMusicController.BackgroundMusicIntensity = si;
         gameCanvas.SetFlickerIntensity(si);
         
         //Debug.Log("suspense intensity is: " + si);
    }

    private void UpdateRealFloorNumber()
    {
        var isPlayerGoingUpCopy = IsPlayerGoingUp();
        if (isPlayerGoingUpCopy.HasValue)
            _realFloorNumber += isPlayerGoingUpCopy.GetValueOrDefault() ? 1 : -1;
    }

    private void RearrangeFloors()
    {
        var isPlayerGoingUpCopy = isItMenuScene ? true : IsPlayerGoingUp();
        if (!isPlayerGoingUpCopy.HasValue) return;

        if (isPlayerGoingUpCopy.GetValueOrDefault())
        {
            _floors[LowestFloorIndex].transform.localPosition =
                _floors[_highestFloorIndex].transform.localPosition + new Vector3(0, FloorHeight, 0);
            RandomizeFloor(_floors[LowestFloorIndex]);

            _highestFloorIndex = LowestFloorIndex;
        }
        else
        {
            _floors[_highestFloorIndex].transform.localPosition =
                _floors[LowestFloorIndex].transform.localPosition + new Vector3(0, -FloorHeight, 0);
            RandomizeFloor(_floors[LowestFloorIndex]);

            _highestFloorIndex = _highestFloorIndex == 0 ? FloorCount - 1 : _highestFloorIndex - 1;
        }
    }

    private void OnInventoryWasUpdated()
    {
        if (!_wasInventoryTipShown)
        {
            _wasInventoryTipShown = true;
            StartCoroutine(ShowTip(inventoryTipText));
        }

        UpdateInventoryObjectsPresence();
    }

    private IEnumerator ShowTip(Text tip)
    {
        yield return HideAllTips();

        tip.gameObject.SetActive(true);
        tip.color = GameUtils.SetColorAlpha(tip.color, 0f);

        yield return StartCoroutine(GameUtils.AnimateValue(
            () => tip.color.a,
            v => tip.color = GameUtils.SetColorAlpha(tip.color, v),
            TipAlpha, TipAlphaRate));
    }

    private IEnumerator HideTip(Text tip)
    {
        yield return StartCoroutine(GameUtils.AnimateValue(
            () => tip.color.a,
            v => tip.color = GameUtils.SetColorAlpha(tip.color, v),
            0f, TipAlphaRate));

        tip.gameObject.SetActive(false);
    }

    private IEnumerator HideAllTips()
    {
        if (exitTipText.gameObject.activeSelf)
            yield return HideTip(exitTipText);

        if (inventoryTipText.gameObject.activeSelf)
            yield return HideTip(inventoryTipText);
    }

    private void OnInventoryModeBeforeActivating()
    {
        inventoryTipText.gameObject.SetActive(false);
    }

    private void OnCowCodeActivated()
    {
        if (!inventory.Contains(EInventoryItemId.SCALPEL)) GetCurrentFloor().EmergeScalpel();
    }

    private void FloorsSelectiveUpdate()
    {
        ForEachFloorExceptCurrent(floor =>
        {
            byte floorDistanceToPlayer = GetFloorDistanceToPlayer(floor);

            if (floorDistanceToPlayer <= 0 || floorDistanceToPlayer > 2) return;

            floor.ReturnAllObjectsToInitState(floorDistanceToPlayer);

            if (floorDistanceToPlayer == 2)
            {
                floor.GenerateRandomTextureProjectorsAndGarbageProps();
                floor.SetGcAdsRandomTextures();
            }

            if (floorDistanceToPlayer == 1) floor.SetElevatorAdRandomTexture();
        });

        UpdateInventoryObjectsPresence();
        ImplementFloorMarksAndFakeFloorNumber();
    }

    private void UpdateInventoryObjectsPresence()
    {
        ForEachFloorExceptCurrent(floor =>
        {
            floor.HideSomeInventoryObjects(id => inventory.Contains(id));

            if (!inventory.Contains(EInventoryItemId.ELEVATOR_CALLER_PANEL))
                floor.ShowInventoryObject(EInventoryItemId.ELEVATOR_CALLER_PANEL);

            if (!inventory.Contains(EInventoryItemId.ELEVATOR_CALLER_BUTTON))
                floor.ShowInventoryObject(EInventoryItemId.ELEVATOR_CALLER_BUTTON);

            if (!inventory.Contains(EInventoryItemId.INSULATING_TAPE))
                floor.ShowInventoryObject(EInventoryItemId.INSULATING_TAPE);
        });
    }

    private void ForEachFloorExceptCurrent(Action<Floor> action)
    {
        foreach (Floor floor in _floors)
        {
            if (IsFloorCurrent(floor))
                continue;

            action.Invoke(floor);
        }
    }

    private void ImplementFloorMarksAndFakeFloorNumber()
    {
        int nextFakeFloorNumber = _fakeFloorNumber + 1;

        Floor nextHighFloor = GetNextHigherFloor();
        Floor nextLowFloor = GetNextLowerFloor();

        nextHighFloor.SetFloorDrawnNumber(nextFakeFloorNumber);
        nextLowFloor.SetFloorDrawnNumber(nextFakeFloorNumber);

        nextHighFloor.ResetAllMarks();
        nextLowFloor.ResetAllMarks();

        foreach (var floorMarkKeyValuePair in GameConstants.floorMarks)
        {
            EFloorMarkId id = floorMarkKeyValuePair.Key;
            FloorMark floorMarkValue = floorMarkKeyValuePair.Value;

            foreach (EInventoryItemId inventoryItemId in floorMarkValue.AssociatedInventoryItems)
            {
                nextHighFloor.HideInventoryObject(inventoryItemId);
                nextLowFloor.HideInventoryObject(inventoryItemId);
            }

            if (!floorMarkValue.IsFloorMarked(nextFakeFloorNumber)) continue;

            bool playerHaveAllAssociatedInventoryItems =
                floorMarkValue.AssociatedInventoryItems.All(itemId => inventory.Contains(itemId));

            if (playerHaveAllAssociatedInventoryItems) continue;

            nextHighFloor.SetMark(id);
            nextLowFloor.SetMark(id);

            foreach (EInventoryItemId inventoryItemId in floorMarkValue.AssociatedInventoryItems)
            {
                if (inventory.Contains(inventoryItemId)) continue;

                nextHighFloor.ShowInventoryObject(inventoryItemId);
                nextLowFloor.ShowInventoryObject(inventoryItemId);
            }

            return;
        }
    }

    private void OnElevatorFloorWasTouched()
    {
        StartEndingCutScene();
    }

    private void StartEndingCutScene()
    {
        backgroundMusicController.BackgroundMusicIntensity = 0f;
        gameCanvas.StopFlicker();

        Transform playerPhTransform = GetCurrentFloor().PlayerPlaceholder.transform;
        Transform cameraPhTransform = GetCurrentFloor().PlayerPlaceholder.transform.GetChild(0);

        _player.CutSceneMoveToPosition(playerPhTransform.position, playerPhTransform.eulerAngles,
            cameraPhTransform.eulerAngles);
    }

    private void OnPlayerCutSceneMoveCompleted()
    {
        ForEachFloorExceptCurrent(floor => floor.HideElevator());

        Floor f = GetCurrentFloor();
        f.CloseAndElevateElevator();
        _player.BindYTo(f.Elevator.gameObject);
    }

    private void OnElevating()
    {
        _gameIsOver = true;
        gameCanvas.FadeOut();
    }

    private void OnFullBlackoutReached()
    {
        if (_gameIsOver)
            Application.Quit();
    }

    private void OnInventoryItemUsedIncorrectly()
    {
        PlayErrorSound();
    }

    private void PlayErrorSound()
    {
        _audioSource.Play();
    }

    private void MoveDemoCameraToNextPlaceholder()
    {
        GameObject ph = GetNextHigherFloor().DemoCameraPlaceholder;

        iTween.MoveTo(demoCamera, iTween.Hash(
            "position", ph.transform.position,
            "time", DemoCameraMoveDurationSec,
            "EaseType", "linear",
            "oncomplete", "OnDemoCameraMoveComplete",
            "oncompletetarget", gameObject
        ));

        iTween.RotateBy(_demoCameraRotateContainer, iTween.Hash(
            "y", -1,
            "time", DemoCameraMoveDurationSec,
            "EaseType", "linear"
        ));

        if (_fakeFloorNumber % 2 == 0)
            iTween.RotateBy(_demoCameraInnerObject, iTween.Hash(
                "z", -1,
                "time", DemoCameraMoveDurationSec,
                "EaseType", "easeInOutSine"
            ));
    }

    private void OnDemoCameraMoveComplete()
    {
        _fakeFloorNumber++;

        _currentDemoCameraFloor = GetNextHigherFloor();
        RearrangeFloors();

        GetNextHigherFloor().SetFloorDrawnNumber(_fakeFloorNumber + 1);

        MoveDemoCameraToNextPlaceholder();
    }

    private void OnExitButtonClicked()
    {
        if (exitTipText.gameObject.activeSelf)
            SceneManager.LoadScene("menu");
        else
            StartCoroutine(ShowExitTip());
    }

    private IEnumerator ShowExitTip()
    {
        yield return ShowTip(exitTipText);
        yield return new WaitForSeconds(3f);
        yield return HideTip(exitTipText);
    }

    private void OnDestroy()
    {
        StopAllCoroutines();

        if (isItMenuScene) return;

        Messenger.RemoveListener(Events.FloorWasTouched, OnFloorWasTouched);
        Messenger.RemoveListener(Events.InventoryWasUpdated, OnInventoryWasUpdated);
        Messenger.RemoveListener(Events.InventoryModeBeforeActivating, OnInventoryModeBeforeActivating);
        Messenger.RemoveListener(Events.CowCodeActivated, OnCowCodeActivated);
        Messenger.RemoveListener(Events.ElevatorFloorWasTouched, OnElevatorFloorWasTouched);
        Messenger.RemoveListener(Events.PlayerCutSceneMoveCompleted, OnPlayerCutSceneMoveCompleted);
        Messenger.RemoveListener(Events.Elevating, OnElevating);
        Messenger.RemoveListener(Events.InventoryItemUsedIncorrectly, OnInventoryItemUsedIncorrectly);
        Messenger.RemoveListener(Events.FullBlackoutReached, OnFullBlackoutReached);
        Messenger.RemoveListener(Events.ExitButtonClicked, OnExitButtonClicked);
    }
}