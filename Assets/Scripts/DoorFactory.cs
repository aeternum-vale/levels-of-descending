using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DoorFactory : MonoBehaviour
{
    [SerializeField] Door woodDoor;

    Door getRandomWoodDoor()
    {
        return Instantiate(woodDoor);
    }

    public Door generateRandomDoor()
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

        return getRandomWoodDoor();
    }
}