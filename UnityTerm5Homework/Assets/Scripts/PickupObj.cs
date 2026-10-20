using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
public class PickupObj : MonoBehaviour
{
    bool pickUp = false;
    Rigidbody rb;
    public Transform destinationObj;


    // Start is called before the first frame update
    void Start()
    {
        rb = GetComponent<Rigidbody>();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void Pickup()
    {
        pickUp = !pickUp;

        if (pickUp)
        {
            rb.useGravity = false;
            rb.isKinematic = true;
            transform.position = destinationObj.position;
            transform.parent = destinationObj.transform;

        }
        else
        {
            rb.useGravity = true;
            rb.isKinematic = false;
            transform.parent = null;
        }
    }
}
