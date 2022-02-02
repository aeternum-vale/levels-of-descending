using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using AdGeneratorModule;
using DoorModule;
using FloorModule.PropsGenerator;
using ResourcesModule;
using SelectableObjectsModule;
using SelectableObjectsModule.SpecificObjects;
using SelectableObjectsModule.Utilities;
using UnityEngine;
using Utils;
using Random = UnityEngine.Random;

namespace FloorModule
{
	public class Floor : MonoBehaviour
	{
		private const string FrontWallName = "front_wall";
		private const string EntrywayObjectName = "entryway";
		private const string LeftDoorBaseObjectName = "door_left";
		private const string RightDoorBaseObjectName = "door_right";
		private const string LeftDoorObjectName = "left_door_prefab";
		private const string RightDoorObjectName = "right_door_prefab";
		private static readonly int MainTexPropertyId = Shader.PropertyToID("_MainTex");
		private static readonly int FloorNumberPropertyId = Shader.PropertyToID("_FloorNumber");

		private readonly Dictionary<EInventoryItemId, InventoryObject> _inventoryObjects =
			new Dictionary<EInventoryItemId, InventoryObject>();

		private readonly Dictionary<EFloorMarkId, bool> _markStates = new Dictionary<EFloorMarkId, bool>
		{
			{EFloorMarkId.DRAGONFLY, false},
			{EFloorMarkId.RABBIT_AD, false},
			{EFloorMarkId.RABBIT_SYMBOL, false}
		};

		private readonly string _playerPlaceholderName = "player_placeholder";
		private Light _backLight;
		private float _backLightInitIntensity;

		private Material _elevatorAdMaterial;

		private SwitchableObject _ePanelDoor;
		private Material _ePanelDoorMaterial;
		private Light _frontLight;
		private float _frontLightInitIntensity;
		private GarbagePropsGenerator _garbagePropsGenerator;
		private Material _gc1AdMaterial;
		private Material _gc2AdMaterial;
		private Door _leftDoor;
		private Material _postboxBaseMaterial;
		private SwitchableObject _postboxDoor;
		private List<IInitStateReturnable> _returnableObjects;
		private Door _rightDoor;
		private Scalpel _scalpel;
		private TextureProjectorPropsGenerator _textureProjectorPropsGenerator;

		[SerializeField] private DoorController doorController;

		public ResourcesController ResourcesController { get; set; }
		public AdGenerator AdGenerator { get; set; }
		public Elevator Elevator { get; private set; }
		public GameObject PlayerPlaceholder { get; private set; }
		public GameObject DemoCameraPlaceholder { get; private set; }
		public string Id { get; set; }

		[SerializeField] private GameObject _frontWall;
		[SerializeField] private Material _frontWallMaterial;
		[SerializeField] Transform _floorLeftDoorBaseTransform;
		[SerializeField] Transform _floorRightDoorBaseTransform;

		[SerializeField] private List<GameObject> _rendererHolders1 = new List<GameObject>();
		[SerializeField] private List<GameObject> _rendererHolders2 = new List<GameObject>();
		[SerializeField] private List<GameObject> _colliderHolders1 = new List<GameObject>();
		[SerializeField] private List<GameObject> _colliderHolders2 = new List<GameObject>();

		private List<MeshRenderer> _toggableRenderers1 = new List<MeshRenderer>();
		private List<MeshRenderer> _toggableRenderers2 = new List<MeshRenderer>();
		private List<Collider> _toggableColliders1 = new List<Collider>();
		private List<Collider> _toggableColliders2 = new List<Collider>();


		private void Start()
		{
			_postboxBaseMaterial =
				transform.Find("postbox/postbox-base").GetComponent<MeshRenderer>().material;
			_leftDoor = transform.Find("left_door_prefab").GetComponent<Door>();
			_rightDoor = transform.Find("right_door_prefab").GetComponent<Door>();
			_scalpel = SelectableObject.GetAsChild<Scalpel>(gameObject, EInventoryItemId.SCALPEL);
			_ePanelDoor = SelectableObject.GetAsChildByPath(gameObject, ESwitchableObjectId.E_PANEL);
			_postboxDoor = SelectableObject.GetAsChildByPath(gameObject, ESwitchableObjectId.POSTBOX_LEFT_DOOR);
			_returnableObjects = transform.GetComponentsInChildren<IInitStateReturnable>(true).ToList();
			PlayerPlaceholder = transform.Find(_playerPlaceholderName).gameObject;
			Elevator = transform.Find("elevator").GetComponent<Elevator>();
			Transform lights = transform.Find("lights");
			_frontLight = lights.GetChild(0).GetComponent<Light>();
			_backLight = lights.GetChild(1).GetComponent<Light>();

			_frontLightInitIntensity = _frontLight.intensity;
			_backLightInitIntensity = _backLight.intensity;

			transform.GetComponentsInChildren<InventoryObject>(true)
				.ToList()
				.ForEach(io => _inventoryObjects.Add(io.objectId, io));

			_ePanelDoor.OpenCondition = () => _markStates[EFloorMarkId.RABBIT_SYMBOL];
			_postboxDoor.OpenCondition = () => _markStates[EFloorMarkId.DRAGONFLY];

			if (AdGenerator == null) throw new Exception("AdGenerator is not provided to the floor");
			if (ResourcesController == null) throw new Exception("ResourcesController is not provided to the floor");
			if (doorController == null) throw new Exception("doorController is not provided to the floor");

			SetGcAdsRandomTextures();
		}

		private void Awake()
		{
			DemoCameraPlaceholder = transform.Find("demoCamera_placeholder").gameObject;
			_textureProjectorPropsGenerator = transform.GetComponentInChildren<TextureProjectorPropsGenerator>();
			_garbagePropsGenerator = transform.GetComponentInChildren<GarbagePropsGenerator>();

			_frontWallMaterial = _frontWall.GetComponent<MeshRenderer>().material;

			_elevatorAdMaterial = transform.Find(SelectableObject.GetPath(ESwitchableObjectId.ELEVATOR_AD))
				.GetComponent<MeshRenderer>()
				.material;

			_gc1AdMaterial = transform.Find("bulletin-board-gc/ad").GetComponent<MeshRenderer>().material;
			_gc2AdMaterial = transform.Find("bulletin-board-gc_2/ad").GetComponent<MeshRenderer>().material;

			_ePanelDoorMaterial = transform.Find("e-panel/right_door").GetComponent<MeshRenderer>().material;


			_rendererHolders1.ToList().ForEach(rh => rh.GetComponentsInChildren<MeshRenderer>(true).ToList().ForEach(mr => _toggableRenderers1.Add(mr)));
			_rendererHolders2.ToList().ForEach(rh => rh.GetComponentsInChildren<MeshRenderer>(true).ToList().ForEach(mr => _toggableRenderers2.Add(mr)));

			_colliderHolders1.ToList().ForEach(ch => ch.GetComponentsInChildren<Collider>(true).ToList().ForEach(c => _toggableColliders1.Add(c)));
			_colliderHolders2.ToList().ForEach(ch => ch.GetComponentsInChildren<Collider>(true).ToList().ForEach(c => _toggableColliders2.Add(c)));
		}

		public void ReturnAllObjectsToInitState(int floorDistanceToPlayer)
		{
			_returnableObjects.ForEach(returnable => returnable.ReturnToInitState(floorDistanceToPlayer));
		}

		private void SetActivityOfInventoryObject(EInventoryItemId id, bool active)
		{
			_inventoryObjects[id].gameObject.SetActive(active);
		}

		public void HideInventoryObject(EInventoryItemId id)
		{
			SetActivityOfInventoryObject(id, false);
		}

		public void ShowInventoryObject(EInventoryItemId id)
		{
			SetActivityOfInventoryObject(id, true);
		}

		public void HideAllInventoryObjects()
		{
			_inventoryObjects.Keys.ToList().ForEach(HideInventoryObject);
		}

		public void HideSomeInventoryObjects(Func<EInventoryItemId, bool> condition)
		{
			_inventoryObjects.Keys.Where(condition).ToList().ForEach(HideInventoryObject);
		}

		public void SetFloorDrawnNumber(int number)
		{
			_frontWallMaterial.SetFloat(FloorNumberPropertyId, number);
		}

		public void EmergeScalpel()
		{
			_scalpel.Emerge();
		}

		public void SetMark(EFloorMarkId id)
		{
			_markStates[id] = true;

			switch (id)
			{
				case EFloorMarkId.DRAGONFLY:
					_postboxBaseMaterial.SetFloat(GameConstants.isPaintingOnPropertyId, 1f);
					_leftDoor.MarkWithDragonfly();
					break;

				case EFloorMarkId.RABBIT_AD:
					SetLostRabbitTextureToElevatorAd();
					break;

				case EFloorMarkId.RABBIT_SYMBOL:
					_ePanelDoorMaterial.SetFloat(GameConstants.isPaintingOnPropertyId, 1f);
					break;

				case EFloorMarkId.COW:
					_rightDoor.MarkWithCow();
					break;

				default:
					throw new ArgumentOutOfRangeException(nameof(id), id, null);
			}
		}

		public void ResetMark(EFloorMarkId id)
		{
			_markStates[id] = false;

			switch (id)
			{
				case EFloorMarkId.DRAGONFLY:
					_postboxBaseMaterial.SetFloat(GameConstants.isPaintingOnPropertyId, 0f);
					_leftDoor.Unmark();
					break;

				case EFloorMarkId.RABBIT_SYMBOL:
					_ePanelDoorMaterial.SetFloat(GameConstants.isPaintingOnPropertyId, 0f);
					break;

				case EFloorMarkId.RABBIT_AD:
					SetElevatorAdRandomTexture();
					break;

				case EFloorMarkId.COW:
					_rightDoor.Unmark();
					break;

				default:
					throw new ArgumentOutOfRangeException(nameof(id), id, null);
			}
		}

		public void ResetAllMarks()
		{
			foreach (EFloorMarkId key in _markStates.Keys.ToList()) ResetMark(key);
		}

		public void SetElevatorAdRandomTexture()
		{
			SetAdRandomTexture(_elevatorAdMaterial);
		}

		public void SetAdRandomTexture(Material adMaterial)
		{
			if (Random.Range(0, 10) <= 3)
			{
				int randomIndex = Random.Range(0, ResourcesController.StaticAdPicTextures.Length);
				adMaterial.SetTexture(MainTexPropertyId, ResourcesController.StaticAdPicTextures[randomIndex]);
			}
			else
			{
				adMaterial.SetTexture(MainTexPropertyId, AdGenerator.GetRandomAdTexture());
			}
		}

		public void SetLostRabbitTextureToElevatorAd()
		{
			_elevatorAdMaterial.SetTexture(MainTexPropertyId, ResourcesController.LostRabbitAdTexture);
		}

		public void SetHandsTextureToElevatorAd()
		{
			_elevatorAdMaterial.SetTexture(MainTexPropertyId, ResourcesController.HandsTexture);
		}

		public void SetGcAdsRandomTextures()
		{
			SetAdRandomTexture(_gc1AdMaterial);
			SetAdRandomTexture(_gc2AdMaterial);
		}


		public void GenerateDoors()
		{

			Door leftDoor = doorController.GenerateRandomDoor();
			Door rightDoor = doorController.GenerateRandomDoor();

			leftDoor.transform.position = _floorLeftDoorBaseTransform.position;

			rightDoor.transform.position = _floorRightDoorBaseTransform.position;
			rightDoor.Invert();

			leftDoor.name = LeftDoorObjectName;
			rightDoor.name = RightDoorObjectName;

			Transform transformValue = transform;
			leftDoor.transform.SetParent(transformValue);
			rightDoor.transform.SetParent(transformValue);
		}

		public void GenerateRandomTextureProjectorsAndGarbageProps()
		{
			GenerateRandomTextureProjectors();
			GenerateRandomGarbageProps();
		}

		public void GenerateRandomTextureProjectors()
		{
			_textureProjectorPropsGenerator.GenerateProps();
		}

		public void GenerateRandomGarbageProps()
		{
			_garbagePropsGenerator.GenerateProps();
		}

		public void CloseAndElevateElevator()
		{
			Elevator.CloseAndElevate();
		}

		public void HideElevator()
		{
			Elevator.gameObject.SetActive(false);
		}

		public void RandomizeDoors()
		{
			_leftDoor.Randomize();
			_rightDoor.Randomize();
		}

		public void SetLightsState(bool front, bool back)
		{
			StopAllCoroutines();
			StartCoroutine(SetLightState(_frontLight, _frontLightInitIntensity, front));
			StartCoroutine(SetLightState(_backLight, _backLightInitIntensity, back));
		}

		private IEnumerator SetLightState(Light lightComponent, float initIntensity, bool state)
		{
			const float tolerance = 0.01f;
			if (lightComponent.gameObject.activeSelf == state &&
				Math.Abs(lightComponent.intensity - initIntensity) < tolerance) yield break;

			float target = state ? initIntensity : 0f;

			lightComponent.gameObject.SetActive(true);

			yield return StartCoroutine(GameUtils.AnimateValue(
				() => lightComponent.intensity,
				v => lightComponent.intensity = v,
				target, 0.04f));

			lightComponent.gameObject.SetActive(state);
		}

		public void SetСolliders1Active(bool areActive)
		{
			_toggableColliders1.ForEach(c => c.enabled = areActive);
		}

		public void SetСolliders2Active(bool areActive)
		{
			_toggableColliders2.ForEach(c => c.enabled = areActive);
		}

		public void SetRenderers1Visibility(bool areVisible)
		{
			_toggableRenderers1.ForEach(mr => mr.enabled = areVisible);
		}

		public void SetRenderers2Visibility(bool areVisible)
		{
			_toggableRenderers2.ForEach(mr => mr.enabled = areVisible);
		}
	}
}