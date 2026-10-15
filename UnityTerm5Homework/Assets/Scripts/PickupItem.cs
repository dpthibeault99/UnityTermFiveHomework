using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PickupItem : MonoBehaviour
{
    ItemCollector collector;
   
    [SerializeField]
    float rotationSpeed = 50f;
    [SerializeField] private Vector3 rotationStart = new Vector3(0, 0,100);
    // Start is called before the first frame update
    private void Awake() 
        // this came from the first thing i found on google
        // i have no idea how to know when to use a vector3
    {
        // 1. Get the current rotation in Euler angles
        Vector3 currentEuler = transform.eulerAngles;

        // 2. Assign a random angle (0 to 360) to the Y axis
        currentEuler.z = Random.Range(0f, 360f);

        // 3. Apply it back to the object
        transform.eulerAngles = currentEuler;
    }
    void Start()
    {
        collector = GameObject.Find("CoinHUD").GetComponent<ItemCollector>();
    }

    // Update is called once per frame
    void Update()
    {
        transform.Rotate(0, 0, rotationSpeed * Time.deltaTime);
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.CompareTag("Player"))
        {
            collector.ItemCollect();
            Destroy(gameObject);
        }
    }
}
