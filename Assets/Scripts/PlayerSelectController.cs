using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Multiplayer.Lobby.Inputs;
using Mirror;
public class PlayerSelectController : NetworkBehaviour
{
    private Vector2 prevInput;

    private Controls controls;

    private Controls Controls
    {
        get
        {
            if (controls != null) { return controls; }
            return controls = new Controls();
        }
    }

    public override void OnStartAuthority()
    {
        enabled = true;

        Controls.PlayerSelect.Move.performed += ctx => SetMovement(ctx.ReadValue<Vector2>());
        Controls.PlayerSelect.Move.canceled += ctx => ResetMovement();
    }

    [ClientCallback]

    private void OnEnable()
    {
        Controls.Enable();
    }
    [ClientCallback]
    private void OnDisable()
    {
        Controls.Disable();
    }

    [ClientCallback]

    private void Update()
    {
        Move();
    }

    [Client]
    private void SetMovement(Vector2 movement)
    {
        prevInput = movement;
    }

    [Client]

    private void ResetMovement()
    {
        prevInput = Vector2.zero;
    }

    [Client]

    private void Move()
    {
        if (prevInput.x > 0)
        {
            Debug.Log("Rifht");
        }
    }

}
