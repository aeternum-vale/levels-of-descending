using UnityEngine;

namespace DoorModule
{
    public class DoorFactory : MonoBehaviour
    {
        [SerializeField] private Door woodDoor;

        private Door GetRandomWoodDoor()
        {
            return Instantiate(woodDoor);
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
            // 		return getRandomMetallDoor ();
            // 	default:
            // 		return getRandomLeatherDoor ();
            // }

            return GetRandomWoodDoor();
        }
    }
}