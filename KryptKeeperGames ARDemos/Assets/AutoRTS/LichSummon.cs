using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LichSummon : MonoBehaviour
{
    Vector3 baseScale;
    float scaleSpeed = 0.07f;

    bool doOnce = false;

    private void Awake()
    {
        baseScale = transform.localScale;
        transform.localScale = Vector3.zero;
    }

    private void Update()
    {
        transform.localScale = (transform.localScale + (Vector3.one * (scaleSpeed * Time.deltaTime)));
        if (transform.localScale.x >= baseScale.x)
        {
            transform.localScale = baseScale;
            //if (!doOnce)
            //{
            //    GameObject root = transform.root.gameObject;
            //    transform.parent = null;
            //    //Destroy(root);
            //    doOnce = true;
            //}

        }
    }


}
