using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SimpleMove : MonoBehaviour
{



    // Update is called once per frame
    void Update()
    {
        transform.position += transform.forward * 0.15f * Time.deltaTime;
    }
}
