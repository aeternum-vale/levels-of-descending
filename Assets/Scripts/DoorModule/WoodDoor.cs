using UnityEngine;

namespace DoorModule
{
    public class WoodDoor : Door
    {
        protected override void Randomize()
        {
            var offset = Random.Range(0f, 0.1f);

            DoorBase.transform.position -= new Vector3(0, 0, offset);
            Details.transform.position -= new Vector3(0, 0, offset);

            var meshRenderer = DoorBase.GetComponent<MeshRenderer>();
            meshRenderer.material.SetColor("_Color",
                new Color(Random.Range(0f, 1f), Random.Range(0f, 1f), Random.Range(0f, 1f)));
        }
    }
}