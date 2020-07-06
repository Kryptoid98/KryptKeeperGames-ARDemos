using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DodgeGameMode : GameMode
{
    public GameObject spawnerPrefab;
    GameObject spawner;
    ARObjectPlacement spawnedObjectPlacement;

    public override void Init(GameObject parentObject)
    {
        ARManager.SetPlaneManagerActive(true);
        ARManager.SetPlanesInvisible(false);
        SpawnARObjectPlacement();
    }

    void SpawnARObjectPlacement()
    {
        GameObject arPlacement = Instantiate(GameManager.instance.ARObjectPlacementPrefab, Vector3.zero, Quaternion.identity);
        spawnedObjectPlacement = arPlacement.GetComponent<ARObjectPlacement>();
        spawnedObjectPlacement.Init("Spawner", spawnerPrefab);
        spawnedObjectPlacement.onPlacementFinished += StartGameMode;
    }

    private void StartGameMode(GameObject p_spawnedObject, float baseObjectScale)
    {
        if (spawnedObjectPlacement != null) spawnedObjectPlacement.onPlacementFinished -= StartGameMode;
        spawner = p_spawnedObject;
        p_spawnedObject.transform.position = new Vector3(p_spawnedObject.transform.position.x, Camera.main.transform.position.y, p_spawnedObject.transform.position.z);
    }

    private void OnDisable()
    {
        if (spawnedObjectPlacement != null) spawnedObjectPlacement.onPlacementFinished -= StartGameMode;

    }
}
