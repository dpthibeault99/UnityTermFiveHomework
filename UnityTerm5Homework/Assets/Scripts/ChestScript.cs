using UnityEngine;
using TMPro;
using System.Collections;

public class ChestScript : MonoBehaviour
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    public TextMeshProUGUI notification;
    ItemCollector collector;
    FPMovement fP;
    private void Start()
    {
        notification.text = string.Empty;
        //collector = new ItemCollector();
        collector = GameObject.Find("CoinHUD").GetComponent<ItemCollector>();
    }
    private void OnTriggerEnter(Collider other)
    {
        if(other.gameObject.CompareTag("Player"))
        {
            Debug.Log("items " + collector.itemsCollected + " / " + collector.itemsInLevel);
            if (collector.itemsCollected < collector.itemsInLevel)
            {
                //Debug.Log("Player hit chest");
                notification.text = $"You need {collector.itemsInLevel} coins to open";
                StartCoroutine(resetNotification());
            }
            else
            {
                notification.text = "Double Jumped Unlocked";
                Debug.Log("cheastScript can jump true");
                fP = other.GetComponent<FPMovement>(); // the player has FPMovemnt
                fP.canDoubleJump = true;
                StartCoroutine(resetNotification());
            }

        }
   
    }

    private IEnumerator resetNotification()
    {
        yield return new WaitForSeconds(3.0f);
        notification.text = string.Empty;

    }
}
