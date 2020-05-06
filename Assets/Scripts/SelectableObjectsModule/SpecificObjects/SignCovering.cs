using UnityEngine;

namespace SelectableObjectsModule.SpecificObjects
{
    public class SignCovering : SwitchableObject
    {
        private GameObject _coveringCut;

        protected override void Start()
        {
            base.Start();
            _coveringCut = transform.parent.Find("covering_cut").gameObject;
        }

        protected override void OnOpen()
        {
            IsOpened = true;
            gameObject.SetActive(false);
            _coveringCut.SetActive(true);
        }

        protected override void OnClose()
        {
            IsOpened = false;
            gameObject.SetActive(true);
            _coveringCut.SetActive(false);
        }
    }
}