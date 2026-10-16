using UnityEngine;

public class MovePickUpScript : MonoBehaviour
{
    [Header("Path Objects")]
    public Transform startTransform;
    public Transform endTransform;
    public float moveSpeed = 2f;

    private Transform target;

    void Start()
    {
        transform.position = startTransform.position;
        target = endTransform;
    }

    void Update()
    {
        transform.position = Vector3.MoveTowards(
            transform.position,
            target.position,
            moveSpeed * Time.deltaTime
        );

        if (transform.position == target.position)
        {
            if (target == endTransform)
                target = startTransform;
            else
                target = endTransform;
        }
    }
}