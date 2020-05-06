using Plugins;
using UnityEngine;

namespace DoorModule
{
    [RequireComponent(typeof(Transform))]
    public abstract class Door : MonoBehaviour
    {
        public GameObject DoorBase { get; private set; }
        public GameObject Details { get; private set; }
        public GameObject Handle { get; private set; }
        public GameObject StaticDetails { get; private set; }
        public GameObject BellButton { get; private set; }
        public GameObject Nameplate { get; private set; }
        public GameObject Peephole { get; private set; }

        public bool IsDragonflyMarked { get; private set; }

        private const string DoorBaseName = "doorBase";
        private static readonly string HandleName = "doorhandle";
        private static readonly string BellButtonName = "door_bell_button";
        private static readonly string StaticDetailsName = "staticDetails";
        private static readonly string DetailsName = "details";
        private static readonly string NameplateName = "nameplate";
        private static readonly string PeepholeName = "peephole";

        private readonly EDoorAction[] _lastActions = new EDoorAction[GameConstants.dragonflyCode.Length];
        private int _lastActionsCursor = 0;

        private void Awake()
        {
            DoorBase = transform.Find(DoorBaseName).gameObject;
            Details = transform.Find(DetailsName).gameObject;
            Handle = Details.transform.Find(HandleName).gameObject;
            Nameplate = Details.transform.Find(NameplateName).gameObject;
            Peephole = Details.transform.Find(PeepholeName).gameObject;

            StaticDetails = transform.Find(StaticDetailsName).gameObject;
            BellButton = StaticDetails.transform.Find(BellButtonName).gameObject;

            Randomize();
        }


        public void Invert()
        {
            transform.localScale = new Vector3(1, 1, -1);
            transform.position -= new Vector3(0, 0, 0.03f);
        }

        public void MarkWithDragonfly()
        {
            Nameplate.SetActive(true);
            Peephole.SetActive(false);
            Nameplate.GetComponent<MeshRenderer>().material.SetFloat("_IsTitleOn", 1f);
            IsDragonflyMarked = true;
        }

        public void UnmarkWithDragonfly()
        {
            Nameplate.SetActive(false);
            Peephole.SetActive(true);
            Nameplate.GetComponent<MeshRenderer>().material.SetFloat("_IsTitleOn", 0f);
            IsDragonflyMarked = false;
        }

        public void Interact(EDoorAction action)
        {
            _lastActions[_lastActionsCursor] = action;
            _lastActionsCursor = (_lastActionsCursor + 1) % _lastActions.Length;

            if (!IsDragonflyMarked) return;

            var dragonflyCode = GameConstants.dragonflyCode;

            for (var i = 0; i < dragonflyCode.Length; i++)
                if (dragonflyCode[i] != _lastActions[(_lastActionsCursor + i) % dragonflyCode.Length])
                    return;

            Messenger<Door>.Broadcast(Events.dragonflyCodeActivated, this);
        }

        protected abstract void Randomize();
    };
}