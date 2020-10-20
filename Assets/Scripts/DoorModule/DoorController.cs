using UnityEngine;

namespace DoorModule
{
    public class DoorController : MonoBehaviour
    {
        [SerializeField] private Door door;

        public Door GenerateRandomDoor()
        {
            return Instantiate(door);
        }
    }
}