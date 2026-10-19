using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TriggerPlate : MonoBehaviour
{
    public RaycastFromPlayer raycastScript;
    [SerializeField]
    string objTag;

    private void OnTriggerStay(Collider other)
    {
        
        if (other.gameObject.name == objTag)
        {
            if (other.gameObject.name == "RedBox")
            {
                raycastScript.redBox = true;
            }
            if (other.gameObject.name == "BlueBox")
            {
                raycastScript.blueBox = true;
            }

        }
        
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.gameObject.name == objTag)
        {
            if (other.gameObject.name == "RedBox")
            {
                raycastScript.redBox = false;
            }
            if (other.gameObject.name == "BlueBox")
            {
                raycastScript.blueBox = false;
            }

        }
    }
}
