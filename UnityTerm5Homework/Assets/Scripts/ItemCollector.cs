using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.SceneManagement;

public class ItemCollector : MonoBehaviour
{
    public int itemsCollected, itemsInLevel;
    public TMP_Text itemHUD;
    public bool allItemsCollected = false;
    FPMovement fP;

    // Start is called before the first frame update
    void Start()
    {
        itemHUD.text = $"Coins {itemsCollected}/{itemsInLevel}";
        fP = GetComponent<FPMovement>();
        
    }

    public void ItemCollect()
    {
        itemsCollected++;
        itemHUD.text = $"Coins {itemsCollected}/{itemsInLevel}";

        if (itemsCollected >= itemsInLevel)
        {
            itemHUD.text = $"{itemsCollected} Coins Collected!";
            allItemsCollected = true ;
            //fP.canDoubleJump = true ;

        }
    }

    IEnumerator GameOver()
    {
        itemHUD.text = $"You Collected all the Coins";
        yield return new WaitForSeconds(5);
        SceneManager.LoadScene(0);
    }
}
