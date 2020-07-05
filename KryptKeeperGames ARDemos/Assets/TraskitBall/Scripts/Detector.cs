using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Detector : MonoBehaviour
{
    public delegate void CustomOnTriggerEnter(Collider other);
    public CustomOnTriggerEnter onTriggerEnter;
    public delegate void CustomOnTriggerStay(Collider other);
    public CustomOnTriggerEnter onTriggerStay;
    public delegate void CustomOnTriggerExit(Collider other);
    public CustomOnTriggerEnter onTriggerExit;

    private void OnTriggerEnter(Collider other)
    {
        onTriggerEnter(other);
    }

    private void OnTriggerStay(Collider other)
    {
        onTriggerStay(other);
    }

    private void OnTriggerExit(Collider other)
    {
        onTriggerExit(other);
    }
}
