
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.InputSystem;

public class ActionManager : MonoBehaviour
{
    // Movement Event
    public UnityEvent<Vector2, int> moveCheck;

    // Attack Event
    public UnityEvent<Vector2> attackCheck;

    // Deflect Event
    public UnityEvent deflectCheck;

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
        if (context.phase == InputActionPhase.Performed)
        {
            deflectCheck.Invoke();
        }
    }
}
