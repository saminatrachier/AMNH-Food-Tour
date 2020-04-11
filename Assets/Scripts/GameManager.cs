using System.Collections;
using System.Collections.Generic;
using UnityEngine;
//USAGE: GAME MANAGER- tracks player's mouse movements for Swipe and Tap Actions for minigames
public class GameManager : MonoBehaviour
{
    public GameObject mousePosition;
    
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        //establish world position of the mouse
        Vector3 worldPosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        mousePosition.transform.position = worldPosition;

    }
}
