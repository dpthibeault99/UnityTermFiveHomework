using System.Collections;
using UnityEngine;
using UnityEngine.AI;

[RequireComponent(typeof(NavMeshAgent))]
public class EnemyJump : MonoBehaviour
{
    [SerializeField, Min(0f)]
    float jumpHeight = 0.5f;
    [SerializeField, Min(0.01f)]
    float durationTime = 0.5f;
    public Animator anim;

    IEnumerator Start()
    {
        NavMeshAgent agent = GetComponent<NavMeshAgent>();
        agent.autoTraverseOffMeshLink = false;
        while (true)
        {
            if (agent.isActiveAndEnabled && agent.isOnNavMesh && agent.isOnOffMeshLink)
            {
                yield return StartCoroutine(Jump(agent, jumpHeight, durationTime));
                if (agent.isActiveAndEnabled && agent.isOnNavMesh && agent.isOnOffMeshLink)
                    agent.CompleteOffMeshLink();
            }
            yield return null;
        }
    }

    IEnumerator Jump(NavMeshAgent agent, float height, float duration)
    {
        OffMeshLinkData data = agent.currentOffMeshLinkData;
        Vector3 startPos = agent.transform.position;
        Vector3 endPos = data.endPos + Vector3.up * agent.baseOffset;
        Vector3 direction = Vector3.ProjectOnPlane(endPos - startPos, Vector3.up);

        // Face the landing point without tilting the character up or down.
        Quaternion jumpRotation = agent.transform.rotation;
        if (direction.sqrMagnitude > 0.001f)
            jumpRotation = Quaternion.LookRotation(direction, Vector3.up);

        bool previousUpdateRotation = agent.updateRotation;
        int jumpLayer = anim != null ? anim.GetLayerIndex("Jump") : -1;
        agent.updateRotation = false;
        try
        {
            agent.transform.rotation = jumpRotation;
            if (jumpLayer >= 0)
            {
                // Start from Empty even if a second link follows the first immediately.
                anim.Play("Jump.Empty", jumpLayer, 0f);
                anim.SetLayerWeight(jumpLayer, 1f);
                anim.SetTrigger("Jump");
            }
            float time = 0f;
            duration = Mathf.Max(0.01f, duration);

            while (time < 1f)
            {
                if (!agent.isActiveAndEnabled || !agent.isOnNavMesh)
                    yield break;

                // Peak offset above the straight start-to-end path is jumpHeight.
                float upDist = 4f * height * time * (1f - time);
                agent.transform.position = Vector3.Lerp(startPos, endPos, time)
                                           + upDist * Vector3.up;
                agent.transform.rotation = jumpRotation;
                time += Time.deltaTime / duration;
                yield return null;
            }

            agent.transform.position = endPos;
            agent.transform.rotation = jumpRotation;
        }
        finally
        {
            if (anim != null && jumpLayer >= 0)
            {
                anim.ResetTrigger("Jump");
                anim.SetLayerWeight(jumpLayer, 0f);
            }
            if (agent != null)
                agent.updateRotation = previousUpdateRotation;
        }
    }
}
