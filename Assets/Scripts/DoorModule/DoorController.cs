using UnityEngine;

namespace DoorModule
{
    public class DoorController : MonoBehaviour
    {
        [SerializeField] private Door door;

        private Door GetRandomWoodDoor()
        {
            return Instantiate(door);
        }

        public Door GenerateRandomDoor()
        {
            // int randomNumber = Random.Range (0, 3);

            // switch (randomNumber) {
            // 	case 0:
            // 		return getRandomLeatherDoor ();
            // 	case 1:
            // 		return getRandomWoodDoor ();
            // 	case 2:
            // 		return getRandomMetalDoor ();
            // 	default:
            // 		return getRandomLeatherDoor ();
            // }

            return GetRandomWoodDoor();
        }
    }
}