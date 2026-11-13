using UnityEngine;

public class KeypadTrigger : MonoBehaviour
{
    public GameObject keyPadUI;


    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.CompareTag("Player"))
            keyPadUI.GetComponent<KeypadScript>().Open(other.gameObject);
        Debug.Log("Player is herw");
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.gameObject.CompareTag("Player"))
            keyPadUI.GetComponent<KeypadScript>().Close();
    }
}
