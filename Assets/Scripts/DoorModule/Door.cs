using System;
using Plugins;
using UnityEngine;

namespace DoorModule
{
    [RequireComponent(typeof(Transform))]
    public abstract class Door : MonoBehaviour
    {
        private const string DoorBaseName = "doorBase";
        private const string HandleName = "doorhandle";
        private const string BellButtonName = "door_bell_button";
        private const string StaticDetailsName = "staticDetails";
        private const string DetailsName = "details";
        private const string NameplateName = "nameplate";
        private const string PeepholeName = "peephole";

        private readonly EDoorAction?[] _lastActions = new EDoorAction?[GameConstants.dragonflyCode.Length];
        private GameObject _bellButton;

        private GameObject _handle;
        private int _lastActionsCursor;
        private GameObject _nameplate;

        private Material _nameplateMaterial;
        private GameObject _peephole;

        private DoorPushableDetail[] _pushableDetails;
        private GameObject _staticDetails;
        protected GameObject Details;
        protected GameObject DoorBase;

        private bool IsDragonflyMarked { get; set; }

        private void Awake()
        {
            DoorBase = transform.Find(DoorBaseName).gameObject;
            Details = transform.Find(DetailsName).gameObject;
            _handle = Details.transform.Find(HandleName).gameObject;
            _nameplate = Details.transform.Find(NameplateName).gameObject;
            _peephole = Details.transform.Find(PeepholeName).gameObject;

            _staticDetails = transform.Find(StaticDetailsName).gameObject;
            _bellButton = _staticDetails.transform.Find(BellButtonName).gameObject;

            _nameplateMaterial = _nameplate.GetComponent<MeshRenderer>().material;

            _pushableDetails = transform.GetComponentsInChildren<DoorPushableDetail>();

            Randomize();
        }

        private void Start()
        {
            foreach (var pushableDetail in _pushableDetails)
                pushableDetail.Opened += OnPushableDetailActivated(pushableDetail);
        }

        private EventHandler OnPushableDetailActivated(DoorPushableDetail detail)
        {
            return (s, e) => Interact(detail.action);
        }

        public void Invert()
        {
            var transformValue = transform;
            transformValue.localScale = new Vector3(1, 1, -1);
            transformValue.position -= new Vector3(0, 0, 0.03f);
        }

        public void MarkWithDragonfly()
        {
            _nameplate.SetActive(true);
            _peephole.SetActive(false);
            _nameplateMaterial.SetFloat(GameConstants.isPaintingOnPropertyId, 1f);
            IsDragonflyMarked = true;
        }

        public void UnmarkWithDragonfly()
        {
            _nameplate.SetActive(false);
            _peephole.SetActive(true);
            _nameplateMaterial.SetFloat(GameConstants.isPaintingOnPropertyId, 0f);
            IsDragonflyMarked = false;
        }

        private void Interact(EDoorAction action)
        {
            _lastActions[_lastActionsCursor] = action;
            _lastActionsCursor = (_lastActionsCursor + 1) % _lastActions.Length;

            if (!IsDragonflyMarked) return;

            var dragonflyCode = GameConstants.dragonflyCode;

            for (var i = 0; i < dragonflyCode.Length; i++)
                if (dragonflyCode[i] != _lastActions[(_lastActionsCursor + i) % dragonflyCode.Length])
                    return;

            Messenger.Broadcast(Events.DragonflyCodeActivated);
        }

        protected abstract void Randomize();
    }
}