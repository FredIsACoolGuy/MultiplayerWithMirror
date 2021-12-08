using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Multiplayer.Lobby.Inputs;
using Mirror;
public class PlayerSelectController : NetworkBehaviour
{
    //this is for the player movement in game

    //stores previous input
    private Vector2 prevInput;
    //controls for player
    private Controls controls;

    private Controls Controls
    {
        get
        {
            if (controls != null) { return controls; }
            return controls = new Controls();
        }
    }

    //called on start on anything that has authority
    public override void OnStartAuthority()
    {
        enabled = true;

        Controls.PlayerSelect.Move.performed += ctx => SetMovement(ctx.ReadValue<Vector2>());
        Controls.PlayerSelect.Move.canceled += ctx => ResetMovement();
    }

    //only run on client
    [ClientCallback]
    //enables controls
    private void OnEnable()
    {
        Controls.Enable();
    }
    [ClientCallback]
    //disables controls
    private void OnDisable()
    {
        Controls.Disable();
    }

    [ClientCallback]
    //calls move
    private void Update()
    {
        Move();
    }

    [Client]
    //updates prevuis movement
    private void SetMovement(Vector2 movement)
    {
        prevInput = movement;
    }

    [Client]
    //resets to zero
    private void ResetMovement()
    {
        prevInput = Vector2.zero;
    }

    [Client]
    //moves character
    private void Move()
    {
        if (prevInput.x > 0)
        {
            Debug.Log("Right");
        }
    }

}
