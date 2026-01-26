using UnityEngine;
using UnityEngine.InputSystem;

public class InputManager : Singleton<InputManager>, InputSystem_Actions.IPlayerActions
{
    private InputSystem_Actions input;
    public event System.Action<Vector2> OnMoveEvent;
    public event System.Action OnJumpEvent;

    // Called when the script instance is being loaded
    void Awake()
    {
        input = new InputSystem_Actions();
        input.Player.SetCallbacks(this);
    }

    // Called when the MonoBehaviour will be destroyed
    void OnDestroy()
    {
        input.Dispose();
    }

    // Called when the object becomes enabled and active
    void OnEnable()
    {
        input.Enable();
    }

    // Called when the behaviour becomes disabled or inactive
    void OnDisable()
    {
        input.Disable();
        input.Dispose();
    }




    // Attack input handling
    public void OnAttack(InputAction.CallbackContext context)
    {
        //throw new System.NotImplementedException();
    }


    // Crouch input handling
    public void OnCrouch(InputAction.CallbackContext context)
    {
        //throw new System.NotImplementedException();
    }


    // Interact input handling
    public void OnInteract(InputAction.CallbackContext context)
    {
        //throw new System.NotImplementedException();
    }


    // Jump input handling
    public void OnJump(InputAction.CallbackContext context)
    {
        jumpPressed = context.ReadValueAsButton();
    }


    // Camera look input handling
    public void OnLook(InputAction.CallbackContext context)
    {
        //throw new System.NotImplementedException();
    }


    // Movement input handling
    public void OnMove(InputAction.CallbackContext context)
    {
        if (context.started || context.performed)
        {
            OnMoveEvent? .Invoke(context.ReadValue<Vector2>());
            return;
        }

        OnMoveEvent?.Invoke(Vector2.zero);
    }


    // Inventory navigation input handling
    public void OnNext(InputAction.CallbackContext context)
    {
        //throw new System.NotImplementedException();
    }


    // Previous inventory navigation input handling
    public void OnPrevious(InputAction.CallbackContext context)
    {
        //throw new System.NotImplementedException();
    }


    // Sprint input handling
    public void OnSprint(InputAction.CallbackContext context)
    {
        //throw new System.NotImplementedException();
    }
}
