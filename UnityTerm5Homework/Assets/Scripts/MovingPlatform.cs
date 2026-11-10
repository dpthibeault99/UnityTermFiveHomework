using UnityEngine;

public class MovingPlatform : MonoBehaviour
{
    public Transform startPoint;
    public Transform endPoint;

    public float speed = 1.0f;
    public float distance = 3.0f;
    public float startTime;
    [Min(0f)] public float pauseDuration = 2.0f;

    float progress;
    float pauseRemaining;
    bool movingToEnd = true;

    void FixedUpdate()
    {
        if (Time.time < startTime) return;

        if (pauseRemaining > 0f)
        {
            pauseRemaining -= Time.fixedDeltaTime;
            return;
        }

        if (speed <= 0f || distance <= 0f) return;

        // At speed 1, distance is the number of seconds for each trip.
        float target = movingToEnd ? 1f : 0f;
        progress = Mathf.MoveTowards(progress, target,
            speed / distance * Time.fixedDeltaTime);
        transform.position = Vector3.Lerp(startPoint.position, endPoint.position, progress);

        if (progress == target)
            PauseAtEndpoint();
    }

    void PauseAtEndpoint()
    {
        pauseRemaining = pauseDuration;
        movingToEnd = !movingToEnd;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.CompareTag("Player"))
            other.transform.SetParent(transform);
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.gameObject.CompareTag("Player"))
            other.transform.SetParent(null);
    }
}
