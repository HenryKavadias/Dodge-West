using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Fusion;

public class OnlinePlayerController : NetworkBehaviour
{
    private OLPlayerInputHandler playerInputHandler = null;

    public override void Spawned()
    {
        playerInputHandler = GetComponent<OLPlayerInputHandler>();

        playerInputHandler.GetMovement().SetRigidbody();
    }

    //private void Awake()
    //{
    //    playerInputHandler = GetComponent<OLPlayerInputHandler>();
    //}

    public override void FixedUpdateNetwork()
    {
        if (!HasInputAuthority)
        {
            return;
        }

        playerInputHandler.UpdateInputs();

    }
}
