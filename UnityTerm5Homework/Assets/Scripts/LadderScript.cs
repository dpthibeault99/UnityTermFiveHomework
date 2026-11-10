using UnityEngine;

public class LadderScript : MonoBehaviour
{
    public Transform playerController;
    bool insideLadder;
    public float ladderSpeed = 3.5f;
    public FPMovement playerInput;
    CharacterController controller;

    void Start()
    {
        controller = playerController.GetComponent<CharacterController>();
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.CompareTag("Ladder"))
        {
            insideLadder = true;
            playerInput.ResetVerticalVelocity();
            playerInput.enabled = false;
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.gameObject.CompareTag("Ladder"))
        {
            LeaveLadder();
        }
    }

    void Update()
    {
        if (!insideLadder) return;

        Vector2 input = playerInput.MovementInput;
        // Keep horizontal control so the player can step off the ladder.
        Vector3 movement = playerController.right * input.x
                         + playerController.forward * input.y;
        movement = Vector3.ClampMagnitude(movement, 1f) * ladderSpeed;
        movement.y = input.y * ladderSpeed;
        controller.Move(movement * Time.deltaTime);
    }

    void LeaveLadder()
    {
        if (!insideLadder) return;
        insideLadder = false;
        playerInput.ResetVerticalVelocity();
        playerInput.enabled = true;
    }

    void OnDisable()
    {
        LeaveLadder();
    }
}
