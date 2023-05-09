using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DisableOnDie : MonoBehaviour
{
    private GameController gameController;
    
    // Start is called before the first frame update
    //void Start()
    //{
    //    GameObject gm = GameObject.FindGameObjectWithTag("GameController");
    //    if (gm && gm.GetComponent<GameController>())
    //    {
    //        gameController = gm.GetComponent<GameController>();
    //    }
    //    else
    //    {
    //        Debug.Log("Can't find game manager or it script!");
    //    }
    //}

    public void DisableThis()
    {
        GameObject gm = GameObject.FindGameObjectWithTag("GameController");
        if (gm && gm.GetComponent<GameController>())
        {
            gameController = gm.GetComponent<GameController>();

            // Disable the player, (maybe their camera), and make them drop the object they are holding
            gameObject.GetComponent<PhysicsPickup>().DropObject();
            gameObject.SetActive(false);

            // Message the Game Manager that this player is dead
            gameController.PlayerDies(gameObject);
        }
        else
        {
            Debug.Log("Can't find game manager or it script!");
        }
    }
}
