using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Photon.Pun;
using TMPro;

public class CreateAndJoinRooms : MonoBehaviourPunCallbacks
{
    public TMP_InputField createInput;
    public TMP_InputField joinInput;

    //public InputField createInput;
    //public InputField joinInput;

    public string roomName = "Alpha_MediumLevel";

    public void CreateRoom()
    {
        if (createInput.text != "")
        {
            PhotonNetwork.CreateRoom(createInput.text);
        }
        //PhotonNetwork.CreateRoom(createInput.text);
    }

    public void JoinRoom()
    {
        if (joinInput.text != "")
        {
            PhotonNetwork.JoinRoom(joinInput.text);
        }
    }

    public override void OnJoinedRoom()
    {
        PhotonNetwork.LoadLevel(roomName);
    }
}
