
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.InputSystem;

public class ActionManager : MonoBehaviour
{
    // Movement Event (Move with WASD)
    public UnityEvent<Vector2, int> moveCheck;

    // Attack Event (Shoot with Arrows) -> to be removed for final game
    public UnityEvent<Vector2> attackCheck;

    // Deflect Event (Parry with LMB)
    public UnityEvent deflectCheck;

    // Evade Event (Dodge with RMB)
    public UnityEvent evadeCheck;

    // Hotwire Event (Interact with E)
    public UnityEvent<Vector3> hotwireCheck;
    public UnityEvent hotwireCancel;

    // Attack Event (Serve with Q)
    public UnityEvent serveCheck;

    // Item Event (Use actives with Spacebar)
    public UnityEvent itemCheck;

    public void OnMoveAction(InputAction.CallbackContext context)
    {
        if (context.performed)
        {
            Vector2 moveInput = context.ReadValue<Vector2>();
            int faceRight = moveInput.x > 0 ? 1 : moveInput.x < 0 ? -1 : 0;
            moveCheck.Invoke(moveInput, faceRight);
        }
        else if (context.canceled)
        {
            moveCheck.Invoke(Vector2.zero, 0);
        }
    }

    // TO BE REMOVED
    public void OnAttackAction(InputAction.CallbackContext context)
    {
        if (context.started)
        {
            Vector2 attackInput = context.ReadValue<Vector2>();
            attackCheck.Invoke(attackInput);
        }
        else if (context.canceled)
        {
            attackCheck.Invoke(Vector2.zero);
        }
    }

    public void OnDeflectAction(InputAction.CallbackContext context)
    {
        // if (context.phase == InputActionPhase.Performed)
        if (context.performed)
        {
            deflectCheck.Invoke();
        }
    }

    public void OnEvadeAction(InputAction.CallbackContext context)
    {
        if (context.performed)
        {
            evadeCheck.Invoke();
        }
    }

    public void OnServeAction(InputAction.CallbackContext context)
    {
        if (context.performed)
        {
            serveCheck.Invoke();
        }
    }
    public void OnHotwireAction(InputAction.CallbackContext context)
    {
        if (context.performed)
        {
            hotwireCheck.Invoke(this.gameObject.transform.position);
        }
        else if (context.canceled)
        {
            hotwireCancel.Invoke();
        }
    }
}
