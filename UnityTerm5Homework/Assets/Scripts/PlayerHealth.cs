using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PlayerHealth : MonoBehaviour
{
    public Image healthBar;
    public float playerHealth = 100;
    public float currentHealth;
    // Start is called before the first frame update
    void Start()
    {
        currentHealth = playerHealth;
    }

    public void TakeDamage(float damage)
    {
        if (currentHealth >= 0) {
            currentHealth -= damage;
            healthBar.fillAmount = currentHealth/playerHealth;
        }
        if(currentHealth <= 0)
        {
            //Player Dead
        }
    }
}
