using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerBody : MonoBehaviour
{
    private void OnCollisionEnter(Collision collision)
    {
        Debug.Log("collision");
        if (collision.collider.gameObject.name == "floor")
        {
            Debug.Log("hit the floor");
        }
    }
}
