using System;
using Plugins;
using SelectableObjectsModule;
using UnityEngine;
using Random = UnityEngine.Random;

namespace DoorModule
{
    [RequireComponent(typeof(Transform))]
    public class Door : MonoBehaviour
    {
        private const string DoorBaseName = "base";
        private const string DoorHandleName = "doorhandle";
        private const string DoorHandleBase1Name = "doorhandle-base_1";
        private const string DoorHandleBase2Name = "doorhandle-base_2";
        private const string Frames1Name = "frames_1";
        private const string Frames2Name = "frames_2";
        private const string BellButtonName = "bell-button";
        private const string NameplateName = "nameplate";
        private const string PadName = "pad";
        private const string TapeName = "tape";
        private const string PeepholeName = "peephole";

        private readonly EDoorAction?[] _lastActions = new EDoorAction?[GameConstants.cowCode.Length];
        private GameObject _bellButton;
        private GameObject _doorHandle;
        private GameObject _doorHandleBase1;
        private GameObject _doorHandleBase2;
        private GameObject _frames1;
        private GameObject _frames2;

        private bool _isSealedWithTape;
        private int _lastActionsCursor;
        private GameObject _nameplate;
        private Material _nameplateMatComponent;
        private GameObject _pad;
        private SwitchableObject _peephole;
        private DoorPushableDetail[] _pushableDetails;
        private GameObject _root;
        private GameObject _staticDetails;
        private GameObject _tape;

        protected GameObject DoorBase;
        private Vector3 _initRootPosition;
        private Vector3 _framelessRootPosition;
        [SerializeField] private Material leatherMaterial;
        [SerializeField] private Material metalMaterial;
        [SerializeField] private Material padMaterial1;
        [SerializeField] private Material padMaterial2;
        [SerializeField] private Material woodMaterial;

        private bool IsDragonflyMarked { get; set; }
        private bool IsCowMarked { get; set; }

        private void Awake()
        {
            _root = transform.GetChild(0).gameObject;
            _initRootPosition = _root.transform.localPosition;
            _framelessRootPosition = _initRootPosition + new Vector3(0, 0, -0.06f);

            DoorBase = _root.transform.Find(DoorBaseName).gameObject;

            _doorHandle = _root.transform.Find(DoorHandleName).gameObject;
            _doorHandleBase1 = _root.transform.Find(DoorHandleBase1Name).gameObject;
            _doorHandleBase2 = _root.transform.Find(DoorHandleBase2Name).gameObject;
            _frames1 = _root.transform.Find(Frames1Name).gameObject;
            _frames2 = _root.transform.Find(Frames2Name).gameObject;
            _nameplate = _root.transform.Find(NameplateName).gameObject;
            _bellButton = _root.transform.Find(BellButtonName).gameObject;
            _pad = _root.transform.Find(PadName).gameObject;
            _tape = _root.transform.Find(TapeName).gameObject;
            _peephole = SelectableObject.GetAsChild<SwitchableObject>(_root, PeepholeName);

            _pushableDetails = transform.GetComponentsInChildren<DoorPushableDetail>();

            _nameplateMatComponent = _nameplate.GetComponent<MeshRenderer>().material;

            Randomize();
        }

        private void Start()
        {
            foreach (DoorPushableDetail pushableDetail in _pushableDetails)
                pushableDetail.Opened += OnPushableDetailActivated(pushableDetail);

            _peephole.Opened += (s, e) =>
            {
                _tape.SetActive(true);
                _isSealedWithTape = true;
            };
            _peephole.Closed += (s, e) =>
            {
                _tape.SetActive(false);
                _isSealedWithTape = false;
            };
        }

        private EventHandler OnPushableDetailActivated(DoorPushableDetail detail)
        {
            return (s, e) => Interact(detail.action);
        }

        public void Invert()
        {
            Transform transformValue = transform;
            transformValue.localScale = new Vector3(1, 1, -1);
            transformValue.position -= new Vector3(0, 0, 0.03f);
        }

        public void MarkWithDragonfly()
        {
            _nameplate.SetActive(true);
            _nameplate.GetComponent<MeshRenderer>().material = woodMaterial;
            IsDragonflyMarked = true;
        }

        public void MarkWithCow()
        {
            _nameplate.SetActive(true);
            _nameplate.GetComponent<MeshRenderer>().material = leatherMaterial;
            IsCowMarked = true;
        }

        public void Unmark()
        {
            _nameplate.SetActive(false);
            IsDragonflyMarked = false;
            IsCowMarked = false;
        }

        private void Interact(EDoorAction action)
        {
            _lastActions[_lastActionsCursor] = action;
            _lastActionsCursor = (_lastActionsCursor + 1) % _lastActions.Length;

            if (!IsCowMarked || !_isSealedWithTape) return;

            var cowCode = GameConstants.cowCode;

            for (int i = 0; i < cowCode.Length; i++)
                if (cowCode[i] != _lastActions[(_lastActionsCursor + i) % cowCode.Length])
                    return;

            Messenger.Broadcast(Events.CowCodeActivated);
        }

        private void SetInitViewType()
        {
            _frames1.SetActive(true);
            _frames2.SetActive(false);
            _doorHandleBase1.SetActive(true);
            _doorHandleBase2.SetActive(false);

            DoorBase.GetComponent<MeshRenderer>().material = woodMaterial;
            _pad.GetComponent<MeshRenderer>().material = padMaterial1;

            _root.transform.localPosition = _initRootPosition;
        }

        public void Randomize()
        {
            SetInitViewType();

            if (Random.Range(0, 2) == 0) _pad.GetComponent<MeshRenderer>().material = padMaterial2;

            int doorType = Random.Range(0, 3);

            switch (doorType)
            {
                case 0: //wood
                    break;

                case 1: //leather
                    DoorBase.GetComponent<MeshRenderer>().material = leatherMaterial;
                    break;

                case 2: //metal
                    DoorBase.GetComponent<MeshRenderer>().material = metalMaterial;

                    _frames1.SetActive(false);
                    _frames2.SetActive(true);

                    break;
            }

            if (doorType == 0 || doorType == 2)
                if (Random.Range(0, 2) == 0)
                {
                    _doorHandleBase1.SetActive(false);
                    _doorHandleBase2.SetActive(true);
                }

            if (Random.Range(0, 3) == 0) //frameless
            {
                _frames1.SetActive(false);
                _frames2.SetActive(false);

                _root.transform.localPosition = _framelessRootPosition;
            }
        }
    }
}