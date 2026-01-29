using UnityEngine;
using UnityEngine.InputSystem;

public class InputManager: Singleton<InputManager>, InputSystem_Actions.IPlayerActions
{
    private InputSystem_Actions input;

    public event System.Action<Vector2> OnMoveEvent;
    public event System.Action<bool> OnJumpEvent;

    void Awake()
    {
        input = new InputSystem_Actions();
        input.Player.SetCallbacks(this);
    }

    void OnEnable()
    {
        input.Enable();
    }

    void OnDisable()
    {
        input.Disable();
        input.Dispose();
    }

    public void OnMove(InputAction.CallbackContext context)
    {
        if (context.started || context.performed)
        {
            OnMoveEvent?.Invoke(context.ReadValue<Vector2>());
            return;
        }

        OnMoveEvent?.Invoke(Vector2.zero);
    }

    public void OnLook(InputAction.CallbackContext context)
    {
        //throw new System.NotImplementedException();
    }

    public void OnAttack(InputAction.CallbackContext context)
    {
        //throw new System.NotImplementedException();
    }

    public void OnInteract(InputAction.CallbackContext context)
    {
        //throw new System.NotImplementedException();
    }

    public void OnCrouch(InputAction.CallbackContext context)
    {
        //throw new System.NotImplementedException();
    }

    public void OnJump(InputAction.CallbackContext context)
    {
        OnJumpEvent?.Invoke(context.ReadValueAsButton());
    }

    public void OnPrevious(InputAction.CallbackContext context)
    {
        //throw new System.NotImplementedException();
    }

    public void OnNext(InputAction.CallbackContext context)
    {
        //throw new System.NotImplementedException();
    }

    public void OnSprint(InputAction.CallbackContext context)
    {
        //throw new System.NotImplementedException();
    }
}
