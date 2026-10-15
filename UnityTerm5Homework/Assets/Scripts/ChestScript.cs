using UnityEngine;
using TMPro;
using System.Collections;

public class ChestScript : MonoBehaviour
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    public TextMeshProUGUI notification;
    private void Start()
    {
        notification.text = string.Empty;
    }
    private void OnTriggerEnter(Collider other)
    {
        if(other.gameObject.CompareTag("Player"))
        {
            Debug.Log("Player hit chest");
            notification.text = "Player hit chest";
        }
    }

    private IEnumerator resetNotification()
    {
        yield return new WaitForSeconds(3.0f);
        notification.text = string.Empty;

    }
}
