using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TestingScript : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        GameObject box = GameObject.CreatePrimitive(PrimitiveType.Cube);
        box.transform.localScale = new Vector3(2, 2, 2);
        box.name = "Testing Box";
        box.transform.position = new Vector3(0, 0, 100);
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
