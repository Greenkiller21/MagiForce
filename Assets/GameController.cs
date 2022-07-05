using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameController : MonoBehaviour
{
    public GameObject playerList;

    void Start()
    {
        
    }

    void Update()
    {
        if (Input.GetKeyDown(Constants.KeyBindings.PlayerList))
        {
            playerList.SetActive(true);
        }
        else if (Input.GetKeyUp(Constants.KeyBindings.PlayerList))
        {
            playerList.SetActive(false);
        }
    }
}
