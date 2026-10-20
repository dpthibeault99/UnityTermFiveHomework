using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class RaycastFromPlayer : MonoBehaviour
{
    public float raycastDistance = 5.0f;
    bool holdingItem = false;
    GameObject heldObj;

    public bool redBox = false;
    public bool blueBox = false;

    public GameObject doorButton;
    public Animator leftDoor;
    public Animator rightDoor;
    bool doorUnlocked = false;
    MeshRenderer hitObj;
    public GameObject messageBox;

    
    

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        Debug.DrawRay(transform.position, transform.forward * raycastDistance, Color.green);


        if (redBox && blueBox)
        {
            doorButton.GetComponent<Renderer>().material.color = Color.green;
            doorUnlocked = true;
        }
        else
        {
            doorButton.GetComponent<Renderer>().material.color = Color.red;
            doorUnlocked = false;
        }

        // Clear the previous highlight and prompt.
        if (hitObj != null)
        {
            hitObj.materials[1].SetFloat("_Scale", 1.0f);
            hitObj = null;
        }

        //messageBox.SetActive(false);

        //// Check what the player is looking at now.
        RaycastHit hit;

        if (Physics.Raycast(transform.position, transform.forward,
            out hit, raycastDistance))
        {
            bool canPickUp = hit.collider.CompareTag("PickupItem")
                             && !holdingItem;

            bool canUseDoor = hit.collider.CompareTag("DoorButton") && doorUnlocked;

            if (canPickUp || canUseDoor)
            {
                hitObj = hit.collider.GetComponent<MeshRenderer>();
                hitObj.materials[1].SetFloat("_Scale", 1.03f);
            }

            //messageBox.SetActive(canUseDoor);
        }

    }

    public void PickupItem(InputAction.CallbackContext ctx)
    {
        if (ctx.performed)
        {
            RaycastHit hit;

            if (Physics.Raycast(transform.position, transform.forward, out hit, raycastDistance))
            {
                Debug.Log(hit.collider.name);
                if (hit.collider.CompareTag("PickupItem"))
                {
                    hit.collider.GetComponent<PickupObj>().Pickup();
                    heldObj = hit.collider.gameObject;
                    holdingItem = true;
                }
                
            }
        }
        if (ctx.canceled)
        {
            if (holdingItem)
            {
                heldObj.GetComponent<PickupObj>().Pickup();
                holdingItem = false;
                heldObj = null;
            }
        }
    }

    //public void InteractableObject(InputAction.CallbackContext ctx)
    //{
    //    if (ctx.performed)
    //    {

    //        RaycastHit hit;

    //        if (Physics.Raycast(transform.position, transform.forward, out hit, raycastDistance))
    //        {
        
    //            if (hit.collider.CompareTag("DoorButton") && doorUnlocked)
    //            {
    //                leftDoor.SetTrigger("OpenDoor");
    //                rightDoor.SetTrigger("OpenDoor");
    //            }

    //        }
    //    }
    //}
}
