using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class PlayerScoreData : MonoBehaviour
{
    public TextMeshProUGUI playerNumber;
    public TextMeshProUGUI winCount;
    
    public void SetScoreData(string num, string win)
    {
        playerNumber.text = "Player " + num;
        winCount.text = win;
    }
}
