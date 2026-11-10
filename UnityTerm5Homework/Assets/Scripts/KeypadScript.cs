using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using TMPro;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.UI;

public class KeypadScript : MonoBehaviour
{
    public string password;
    public string enteredpassword = "";
    public TMP_Text keypadDisplay;
    public int passDigits;
    public GameObject escapePod;
    public GameObject escapePodStand;
    public Camera cutSceneCamera;
    public Camera playerCamera;
    public GameObject firstSelectedButton;

    FPMovement movement;
    MouseLook[] lookScripts;
    bool completed;
    bool isOpen;

    void Awake()
    {
        passDigits = password.Length;
        enteredpassword = "";
        keypadDisplay.text = "Enter Code";
    }

    public void Open(GameObject player)
    {
        if (completed || isOpen) return;
        gameObject.SetActive(true);
        movement = player.GetComponent<FPMovement>();
        lookScripts = player.GetComponentsInChildren<MouseLook>();
        movement.ResetVerticalVelocity();
        movement.enabled = false;
        foreach (MouseLook look in lookScripts)
            look.enabled = false;
        isOpen = true;

        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
        // Select again on every visit, not just when the EventSystem starts.
        if (firstSelectedButton == null)
            firstSelectedButton = GetComponentInChildren<Button>().gameObject;
        EventSystem.current.SetSelectedGameObject(null);
        EventSystem.current.SetSelectedGameObject(firstSelectedButton);
    }

    void Update()
    {
        if (!isOpen) return;
        // Use the UI Cancel binding (Escape / controller B).
        var uiInput = EventSystem.current.currentInputModule as InputSystemUIInputModule;
        if (uiInput != null && uiInput.cancel != null &&
            uiInput.cancel.action.WasPerformedThisFrame())
        {
            Close();
            return;
        }

        if (enteredpassword.Length == passDigits)
        {
            if (enteredpassword == password)
            {
                completed = true;
                keypadDisplay.text = "Correct Passcode";
                playerCamera.enabled = false;
                cutSceneCamera.enabled = true;
                Destroy(escapePodStand);
                escapePod.GetComponent<BoxCollider>().enabled = false;
                Close();
            }
            else
            {
                keypadDisplay.text = "Incorrect Passcode";
                enteredpassword = "";
            }
        }
    }

    // Keep the existing PlayerInput event connection, but close instead of toggling the cursor.
    public void ShowCursor(InputAction.CallbackContext ctx)
    {
        if (ctx.performed && isOpen) Close();
    }

    public void Close()
    {
        gameObject.SetActive(false);
    }

    void OnDisable()
    {
        if (!isOpen) return;
        isOpen = false;
        if (EventSystem.current != null)
            EventSystem.current.SetSelectedGameObject(null);
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
        // Keep the player still during the successful escape sequence.
        if (!completed)
        {
            if (movement != null)
            {
                movement.ResetVerticalVelocity();
                movement.enabled = true;
            }
            foreach (MouseLook look in lookScripts)
                if (look != null) look.enabled = true;
        }
    }

    public void ButtonNumber(string btnNum)
    {
        if (!isOpen || enteredpassword.Length >= passDigits) return;
        enteredpassword += btnNum;
        keypadDisplay.text = enteredpassword;
    }
}
