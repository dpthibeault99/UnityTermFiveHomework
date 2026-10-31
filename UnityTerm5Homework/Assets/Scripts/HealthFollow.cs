using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HealthFollow : MonoBehaviour
{
    public GameObject enemy;
    public Transform player;
    Vector3 offset;
    // Start is called before the first frame update
    void Start()
    {
        offset = transform.position - enemy.transform.position;
    }

    // Update is called once per frame
    void Update()
    {
        transform.LookAt(player);
        transform.position = enemy.transform.position + offset;
    }
}
