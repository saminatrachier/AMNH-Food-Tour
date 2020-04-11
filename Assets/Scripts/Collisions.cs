using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Collisions : MonoBehaviour
{
    void OnTriggerEnter(Collider col)
    {
        Debug.Log("Hit");
        //passing gameObject that player collided with in Cornwash Function
        GameManager.instance.CornWash(col.gameObject);
    }
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
