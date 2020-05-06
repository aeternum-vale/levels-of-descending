using UnityEngine;

namespace DoorModule
{
    public class WoodDoor : Door
    {
        private static readonly int Color = Shader.PropertyToID("_Color");

        protected override void Randomize()
        {
            var offset = Random.Range(0f, 0.1f);

            DoorBase.transform.position -= new Vector3(0, 0, offset);
            Details.transform.position -= new Vector3(0, 0, offset);

            var meshRenderer = DoorBase.GetComponent<MeshRenderer>();
            meshRenderer.material.SetColor(Color,
                new Color(Random.Range(0f, 1f), Random.Range(0f, 1f), Random.Range(0f, 1f)));
        }
    }
}