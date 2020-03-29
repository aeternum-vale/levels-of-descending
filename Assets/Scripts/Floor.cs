using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Floor : MonoBehaviour
{
    public void removeObject(string name)
    {
        transform.Find(name).gameObject.GetComponent<Renderer>().enabled = false;
    }
}
