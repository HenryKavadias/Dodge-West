using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Fusion;

public class NetworkPlayerSettings : NetworkBehaviour
{
    //[Networked(OnChanged = nameof(AvatarUrlChanged))]
    public NetworkString<_128> avatarurl {  get; set; }

    //[Networked(OnChanged = nameof(PlayerNameChanged))]
    public NetworkString<_128> playerName { get; set; }

    //[SerializeField]
    //private ReadyPlayerMe

    public static void AvatarUrlChanged(Changed<NetworkPlayerSettings> change)
    {
        //change.Behaviour.avatarLoader
    }

    public static void PlayerNameChanged(Changed<NetworkPlayerSettings> change)
    {
        
    }
}
