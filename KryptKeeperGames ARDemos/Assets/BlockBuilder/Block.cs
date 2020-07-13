using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Block : MonoBehaviour
{
    MeshRenderer mr;
    Color originalColor;
    //bool isHighlighted = false;

    public GameObject foilage;

    private void Awake()
    {
        mr = GetComponent<MeshRenderer>();
    }

    private void Start()
    {
        originalColor = mr.material.color;
    }

    public void ToggleHighlight(bool highlightOn)
    {
        //isHighlighted = !isHighlighted;
        if (highlightOn)
        {
            //mr.material.EnableKeyword("_EMISSION");
            mr.material.color = Color.red;
            Color temp = mr.material.color;
            temp.a = 0.1f;
            mr.material.color = temp;
        }
        else mr.material.color = originalColor; //mr.material.DisableKeyword("_EMISSION")//
    }

}
