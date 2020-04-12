using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
//USAGE: GAME MANAGER- tracks player's mouse movements for Swipe and Tap Actions for minigames
public class GameManager : MonoBehaviour
{
    public GameObject mousePosition;

    public static GameManager instance;
    
    //progress bar Image
    public Image progressBar;

    //progress tracker
    public float progress;
    private float maxProgress = 1;

    private GameObject lastHitObject;
    
    // Start is called before the first frame update
    void Start()
    {
        //to reference the entire script on collisions
        instance = this;
    }

    // Update is called once per frame
    
    //establish mouse movement relative to screen
    void Update()
    {
        //mousePosition.transform.position = new Vector3(worldPosition.x, worldPosition.y, 0);
        //establish world position of the mouse
       // Vector3 worldPosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
       // mousePosition.transform.position = new Vector3(worldPosition.x, worldPosition.y, 0);
        Vector3 mousePos = Input.mousePosition;
        mousePos.z = 10;
       Vector3 worldPosition = Camera.main.ScreenToWorldPoint(mousePos);
        mousePosition.transform.position = worldPosition;
        
        //lerps(makes the fill smooth for the progressBar)(if it needs to be faster multiply Time.deltaTime by multiplier of choice)
        progressBar.fillAmount = Mathf.Lerp(progressBar.fillAmount,progress,Time.deltaTime);
        
        //winstate:
        if (progress >= maxProgress)
        {
            //move to next minigame OR end module and go back to map/results screen
        }


    }

    //corn washing minigame
    public void CornWash(GameObject hitObject)
    {
        if (lastHitObject != hitObject)
        {
            //increases the progress bar
            progress += .01f;
            
            
            //for testing purposes
            //progressBar.fillAmount = progress;
            
            //last hit Object, so the score does not continue to increase without touching the other collider
            lastHitObject = hitObject;

        }
    }
}
