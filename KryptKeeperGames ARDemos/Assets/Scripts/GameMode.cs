using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class GameMode : MonoBehaviour
{
    //public GameObject arObjectPlacementPrefab;
    public abstract void Init(GameObject parentObject);
}
