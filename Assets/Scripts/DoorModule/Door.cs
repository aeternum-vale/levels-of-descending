using System;
using Plugins;
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

        [SerializeField] private Material leatherMaterial;
        [SerializeField] private Material metalMaterial;

        private readonly EDoorAction?[] _lastActions = new EDoorAction?[GameConstants.dragonflyCode.Length];
        private GameObject _bellButton;
        private GameObject _doorHandle;
        private GameObject _doorHandleBase1;
        private GameObject _doorHandleBase2;
        private GameObject _frames1;
        private GameObject _frames2;
        private int _lastActionsCursor;
        private GameObject _nameplate;
        private Material _nameplateMaterial;
        private GameObject _pad;
        private DoorPushableDetail[] _pushableDetails;
        private GameObject _root;
        private GameObject _staticDetails;
        protected GameObject DoorBase;

        private bool IsDragonflyMarked { get; set; }

        private void Awake()
        {
            _root = transform.GetChild(0).gameObject;

            DoorBase = _root.transform.Find(DoorBaseName).gameObject;

            _doorHandle = _root.transform.Find(DoorHandleName).gameObject;
            _doorHandleBase1 = _root.transform.Find(DoorHandleBase1Name).gameObject;
            _doorHandleBase2 = _root.transform.Find(DoorHandleBase2Name).gameObject;
            _frames1 = _root.transform.Find(Frames1Name).gameObject;
            _frames2 = _root.transform.Find(Frames2Name).gameObject;
            _nameplate = _root.transform.Find(NameplateName).gameObject;
            _bellButton = _root.transform.Find(BellButtonName).gameObject;
            _pad = _root.transform.Find(PadName).gameObject;

            _pushableDetails = transform.GetComponentsInChildren<DoorPushableDetail>();

            _nameplateMaterial = _nameplate.GetComponent<MeshRenderer>().material;

            Randomize();
        }

        private void Start()
        {
            foreach (DoorPushableDetail pushableDetail in _pushableDetails)
                pushableDetail.Opened += OnPushableDetailActivated(pushableDetail);
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
            _nameplateMaterial.SetFloat(GameConstants.isPaintingOnPropertyId, 1f);
            IsDragonflyMarked = true;
        }

        public void UnmarkWithDragonfly()
        {
            _nameplate.SetActive(false);
            _nameplateMaterial.SetFloat(GameConstants.isPaintingOnPropertyId, 0f);
            IsDragonflyMarked = false;
        }

        private void Interact(EDoorAction action)
        {
            _lastActions[_lastActionsCursor] = action;
            _lastActionsCursor = (_lastActionsCursor + 1) % _lastActions.Length;

            if (!IsDragonflyMarked) return;

            var dragonflyCode = GameConstants.dragonflyCode;

            for (int i = 0; i < dragonflyCode.Length; i++)
                if (dragonflyCode[i] != _lastActions[(_lastActionsCursor + i) % dragonflyCode.Length])
                    return;

            Messenger.Broadcast(Events.DragonflyCodeActivated);
        }

        protected virtual void Randomize()
        {
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
             {
                 if (Random.Range(0, 2) == 0)
                 {
                     _doorHandleBase1.SetActive(false);
                     _doorHandleBase2.SetActive(true);
                 }
             }

            // float offset = Random.Range(0f, 0.1f);
            // DoorBase.transform.position -= new Vector3(0, 0, offset);
            // _handle.transform.position -= new Vector3(0, 0, offset);
        }
    }
}