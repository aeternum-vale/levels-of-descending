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
        private static readonly int IsTitleOn = Shader.PropertyToID("_IsTitleOn");

        private readonly EDoorAction[] _lastActions = new EDoorAction[GameConstants.dragonflyCode.Length];
        private int _lastActionsCursor;

        private DoorPushableDetail[] _pushableDetails;
        public GameObject DoorBase { get; private set; }
        public GameObject Details { get; private set; }
        public GameObject Handle { get; private set; }
        public GameObject StaticDetails { get; private set; }
        public GameObject BellButton { get; private set; }
        public GameObject Nameplate { get; private set; }
        public GameObject Peephole { get; private set; }

        public bool IsDragonflyMarked { get; private set; }

        private void Awake()
        {
            DoorBase = transform.Find(DoorBaseName).gameObject;
            Details = transform.Find(DetailsName).gameObject;
            Handle = Details.transform.Find(HandleName).gameObject;
            Nameplate = Details.transform.Find(NameplateName).gameObject;
            Peephole = Details.transform.Find(PeepholeName).gameObject;

            StaticDetails = transform.Find(StaticDetailsName).gameObject;
            BellButton = StaticDetails.transform.Find(BellButtonName).gameObject;

            _pushableDetails = transform.GetComponentsInChildren<DoorPushableDetail>();

            foreach (var pushableDetail in _pushableDetails)
                pushableDetail.OnStateReached += OnPushableDetailStateReached;

            Randomize();
        }

        private void OnPushableDetailStateReached(object sender, MultiStateObjectEventArgs e)
        {
            Interact(((DoorPushableDetail) sender).action);
        }

        public void Invert()
        {
            var transformValue = transform;
            transformValue.localScale = new Vector3(1, 1, -1);
            transformValue.position -= new Vector3(0, 0, 0.03f);
        }

        public void MarkWithDragonfly()
        {
            Nameplate.SetActive(true);
            Peephole.SetActive(false);
            Nameplate.GetComponent<MeshRenderer>().material.SetFloat(IsTitleOn, 1f);
            IsDragonflyMarked = true;
        }

        public void UnmarkWithDragonfly()
        {
            Nameplate.SetActive(false);
            Peephole.SetActive(true);
            Nameplate.GetComponent<MeshRenderer>().material.SetFloat(IsTitleOn, 0f);
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

            Messenger<Door>.Broadcast(Events.DragonflyCodeActivated, this);
        }

        protected abstract void Randomize();
    }
}