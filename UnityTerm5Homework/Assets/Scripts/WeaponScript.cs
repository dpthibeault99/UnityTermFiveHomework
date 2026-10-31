using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class WeaponScript : MonoBehaviour
{
    public float weaponDamage = 10.0f;
    public float weaponRange = 50.0f;
    public float fireRate = 20.0f;
    private float nextFire = 0.0f;
    public Camera fpCamera;

    public void Shoot()
    {
        RaycastHit hit;

        if (Physics.Raycast(fpCamera.transform.position, fpCamera.transform.forward, out hit, weaponRange))
        {
            if(hit.transform.gameObject.tag == "Enemy")
            {
                //Apply Damage
                Debug.Log(hit.collider.name);
                hit.collider.GetComponent<EnemyHealth>().TakeDamage(weaponDamage);
            }
        }
    }

    public void FireShot(InputAction.CallbackContext ctx)
    {
        if (ctx.performed && Time.time >= nextFire)
        {
            nextFire = Time.time + 1.0f / fireRate;
            Shoot();
        }
    }
}
