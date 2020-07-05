using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR.ARFoundation;

public class PlaceableObject : MonoBehaviour
{
    public ARAnchor refAnchor;
    Vector3 lastKnownAnchorPosition;

    public void Init(ARAnchor p_refAnchor)
    {
        refAnchor = p_refAnchor;
        lastKnownAnchorPosition = p_refAnchor.transform.localPosition;
    }

    protected virtual void Update()
    {
        if (refAnchor != null)
        {
            transform.position = refAnchor.transform.localPosition;
            lastKnownAnchorPosition = refAnchor.transform.localPosition;
        }
        else transform.position = lastKnownAnchorPosition;
    }
}
