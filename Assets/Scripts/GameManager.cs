using System.Collections;
using System.Collections.Generic;
using System.Numerics;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using Vector3 = UnityEngine.Vector3;

//USAGE: GAME MANAGER- tracks player's mouse movements for Swipe and Tap Actions for minigames
public class GameManager : MonoBehaviour
{
    //tracks mouse position
    public GameObject mousePosition;

    public static GameManager instance;
    
    //progress bar Image
    public Image progressBar;

    //corn Ball Images
    public GameObject cornBall1;
    public GameObject cornBall2;
    public GameObject cornBall3;
    
    //firestarter gameobject

    public GameObject poleStringParent;
    
    
    //progress tracker
    public float progress;
    private float maxProgress = 1;

    //stores last hit object to avoid double hits
    private GameObject lastHitObject;

    //tracks the current open scene to progress to next level
    private Scene currentScene;

    //fire starter string scale START FIRE
    public bool scaling;

    //corn cake bob COOK CORN
    public bool bobbing;

    //corn cake formation
    public bool forming;
    
    //animation curves for lerping animations (corn rise, fire starter oscillation)
    public AnimationCurve animCurve;
    public AnimationCurve recursiveCurve;

    //water sprite for WASHING CORN step
    public GameObject water;
    //hand sprites for WASHING CORN step
    public GameObject handL;
    public GameObject handR;
    
    
    //fire sprites for START FIRE MINIGAME:
    public GameObject fire1, fire2;
    
    
    //finished corn cake COOKING
    public GameObject cornCake, fire;
    public GameObject clickIcon1,clickIcon2;
    
    //boolean for finishing corn Cake Cooking
    public bool cooking = false;
    
    
    // Start is called before the first frame update
    void Start()
    {
        //to reference the entire script on collisions
        instance = this;

        //chacking for current scene that is open
        currentScene = SceneManager.GetActiveScene();
    }

    // Update is called once per frame
    
    //establish mouse movement relative to screen
    void Update()
    {
        //mousePosition.transform.position = new Vector3(worldPosition.x, worldPosition.y, 0);
        //establish world position of the mouse
       
        if (currentScene.name == "CornWash" || currentScene.name == "StartFire")
        {
            Vector3 mousePos = Input.mousePosition;
            mousePos.z = 10;
            Vector3 worldPosition = Camera.main.ScreenToWorldPoint(mousePos);
            mousePosition.transform.position = worldPosition;
        }
        
        
        //lerps(makes the fill smooth for the progressBar)(if it needs to be faster multiply Time.deltaTime by multiplier of choice)
        progressBar.fillAmount = Mathf.Lerp(progressBar.fillAmount,progress,Time.deltaTime);
        
        //winstate:
        if (progress >= maxProgress && currentScene.name != "CookCorn")
        {
           
            SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex + 1);
            //move to next minigame OR end module and go back to map/results screen
        }
        

        //if the Cook Corn scene is open, run the corn tap coroutine
        if (Input.GetMouseButtonDown(0)&& currentScene.name == "CookCorn")
        {
            CookCornTap();
        }

        //PNGs Manager for CornWash Scene
        if (currentScene.name == "CornWash")
        {
            //changing PNGs based on completeness
            if (progress >= .3f)
            {
                cornBall1.SetActive(false);
                cornBall2.SetActive(true);
            }

            if (progress >= .7f)
            {
                cornBall2.SetActive(false);
                cornBall3.SetActive(true);
            }
        }
        

       
    }

    //corn washing minigame
    public void CornWash(GameObject hitObject)
    {
        if (lastHitObject != hitObject)
        {
            //increases the progress bar
            progress += .01f;
            
            //for the Start Fire Scene:
            if (currentScene.name == "StartFire")
            {
                //changing PNGs size based on completeness
                if (!scaling)
                {
                    StartCoroutine(ScaleStrings());
                }
            }
            //for Cornwash Scene:
            if (currentScene.name == "CornWash")
            {
                //changing PNGs size based on completeness
                if (!forming)
                {
                    StartCoroutine(CornWashing());
                }
            }

           
           
            //for testing purposes
            //progressBar.fillAmount = progress;
            
            //last hit Object, so the score does not continue to increase without touching the other collider
            lastHitObject = hitObject;

        }
    }

    //Tapping Action for 3rd Scene (cook corn)
    public void CookCornTap()
    {
        //raycast for mouse position
        RaycastHit hit;
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        
        if (Physics.Raycast(ray, out hit))
        {
            //if the gameobject/collier is tagged as "Click" then...
            if (hit.transform.gameObject.CompareTag("Click"))
            {
                //increases the progress bar
                progress += .04f;

                if (bobbing == false)
                {
                    StartCoroutine(CornBob());
                }
               

            }
            
            //if the player taps the corn after the last minigame it shows the end scene
            
            else if(hit.transform.gameObject.CompareTag("Corn"))
            {
                SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex + 1);
            }

            if (progress >= maxProgress && cooking == false)
            {
                StartCoroutine(CornRise());
            }


        }
    }
    
    //scale string size for Start Fire Minigame
    IEnumerator ScaleStrings()
    {
        if (!fire1.GetComponent<SpriteRenderer>().enabled)
        {
            fire1.GetComponent<SpriteRenderer>().enabled = true;
            fire2.GetComponent<SpriteRenderer>().enabled = false;
        }
        else

        {
            fire1.GetComponent<SpriteRenderer>().enabled = false;
            fire2.GetComponent<SpriteRenderer>().enabled = true;
        }
        scaling = true;
        float t = 0;
        Vector3 StartScale = poleStringParent.transform.localScale;
        Vector3 EndScale = poleStringParent.transform.localScale +new Vector3(-.2f,-.2f,-.2f);
        while (t < 1)
        {
            poleStringParent.transform.localScale = Vector3.LerpUnclamped(StartScale, EndScale, animCurve.Evaluate(t));
            t += Time.deltaTime * 6;
            yield return 0;
        }
        poleStringParent.transform.localScale = StartScale;
        scaling = false;
        
        
    }
    
    //enumerator for creating running water and hand movement
    IEnumerator CornWashing()
    {
       
       
            forming = true;
            float t = 0;
        //water moving up and down
            Vector3 StartPosition = water.transform.localPosition;
            Vector3 EndPosition = water.transform.localPosition +new Vector3(0,.15f,0);
            
        //left hand moving to the right
        Vector3 StartPositionL = handL.transform.localPosition;
        Vector3 EndPositionL = handL.transform.localPosition +new Vector3(-.15f,0,0);
        
        //right hand moving to the left
        Vector3 StartPositionR = handR.transform.localPosition;
        Vector3 EndPositionR = handR.transform.localPosition +new Vector3(.15f,0,0);
        
            while (t < 1)
            {
                water.transform.localPosition = Vector3.LerpUnclamped(StartPosition, EndPosition, recursiveCurve.Evaluate(t));
                handL.transform.localPosition = Vector3.LerpUnclamped(StartPositionL, EndPositionL, recursiveCurve.Evaluate(t));
                handR.transform.localPosition = Vector3.LerpUnclamped(StartPositionR, EndPositionR, recursiveCurve.Evaluate(t));
                t += Time.deltaTime * 6;
                yield return 0;
            }
            water.transform.localPosition = StartPosition;
        handL.transform.localPosition = StartPositionL;
        handR.transform.localPosition = StartPositionR;
            forming = false;
       
    }
    
    //enumerator for creating oscillating corncake bob
    IEnumerator CornBob()
    {
       
        bobbing = true;
        float t = 0;
        //corn cake bobs up and down
        Vector3 StartScale = cornCake.transform.localPosition;
        Vector3 EndScale = cornCake.transform.localPosition +new Vector3(0,.15f,0);
        
        //fire size oscillates
        Vector3 StartFireScale = fire.transform.localPosition;
        Vector3 EndFireScale = fire.transform.localPosition +new Vector3(0,-.2f,0);
        while (t < 1)
        {
            cornCake.transform.localPosition = Vector3.LerpUnclamped(StartScale, EndScale, recursiveCurve.Evaluate(t));
            fire.transform.localPosition = Vector3.LerpUnclamped(StartFireScale, EndFireScale, recursiveCurve.Evaluate(t));
            t += Time.deltaTime * 6;
            yield return 0;
        }
        cornCake.transform.localPosition = StartScale;
        fire.transform.localPosition = StartFireScale;
        bobbing = false;
        
        
    }
    
    //enumerator for the corn rising an rotation
    IEnumerator CornRise()
    {
        cooking = true;

       
        float t = 0;
        //start position and end position of the corn cakes
        Vector3 StartPosition = cornCake.transform.localPosition;
        Vector3 EndPosition = cornCake.transform.localPosition +new Vector3(0,1,0);
        
        //rotation of the corn cake to indicate it finished cooking
        Vector3 StartRotation = cornCake.transform.localEulerAngles;
        Vector3 EndRotation = cornCake.transform.localEulerAngles + new Vector3(0, 0, 90);
        while (t < 1)
        {
            cornCake.transform.localPosition = Vector3.LerpUnclamped(StartPosition, EndPosition, animCurve.Evaluate(t));
            cornCake.transform.localEulerAngles = Vector3.LerpUnclamped(StartRotation, EndRotation, animCurve.Evaluate(t));
            t += Time.deltaTime * 2;
            yield return 0;
        }
        cornCake.transform.localPosition = EndPosition;
        cornCake.transform.localEulerAngles = EndRotation;
        cornCake.GetComponent <BoxCollider>().enabled = true;




    }
    
    
}
