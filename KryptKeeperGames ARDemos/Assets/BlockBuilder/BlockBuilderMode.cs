using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR.ARFoundation;
using UnityEngine.XR.ARSubsystems;
using UnityEngine.UI;
using KryptKeeperGamesARDemo.BlockBuilder;
using KryptKeeperGamesARDemo.Enums;

public class BlockBuilderMode : GameMode
{
    public static BlockBuilderMode instance;

    public enum eBuildState
    {
        PLACE,
        PLACE_ENEMY,
        REMOVE
    }
    public eBuildState buildState = eBuildState.PLACE;

    public GameObject instructionsPanel;

    public GameObject[] block;
    public GameObject[] enemy;
    public GameObject[] flowers;

    public GameObject visualMarkerBlockPrefab;
    public Block lastHighlightedBlock;
    public Image blockImg;
    int index = 0;
    int enemyIndex = 0;
    bool canPlaceBlocks = false;

    UIManager uiManager;
    LinkedList<GameObject> spawnedBlocks = new LinkedList<GameObject>();
    GameObject worldZeroBlock;
    GameObject visualMarkerBlock;

    [HideInInspector] public float globalBlockSize = 0.1f;

    bool planesOn = true;

    private void Awake()
    {
        instance = this;
    }

    public override void Init(GameObject parentObject)
    {
        ARManager.SetPlaneManagerActive(true);
        ARManager.TogglePlanePrefab(true);


        visualMarkerBlock = Instantiate(visualMarkerBlockPrefab, Vector3.one * 100, Quaternion.identity);
    }

    
    private void Start()
    {
        uiManager = GetComponentInChildren<UIManager>();
        UIManager.onDrawModeButtonPressed += ChangeDrawState;
        UIManager.onBlockDisplayButtonPressed += ChangeBlock;
        UIManager.onEnemyDisplayButtonPressed += ChangeEnemy;
        globalBlockSize = block[0].GetComponent<MeshRenderer>().bounds.size.x;
    }

    //bool temp = false;
    void Update()
    {
        Vector2 pos;
        RaycastHit hit;
        RaycastHit markerHit;

        if (Physics.Raycast(Camera.main.ViewportPointToRay(new Vector2(0.5f, 0.5f)), out markerHit))
        {
            if (markerHit.collider.gameObject.CompareTag("BLOCK"))
            {
                if (buildState == eBuildState.PLACE || buildState == eBuildState.PLACE_ENEMY)
                {
                    if (visualMarkerBlock != null) visualMarkerBlock.transform.position = GetAimedPosition(markerHit);
                }
                else if (buildState == eBuildState.REMOVE)
                {
                    if (lastHighlightedBlock != null) lastHighlightedBlock.ToggleHighlight(false);
                    lastHighlightedBlock = markerHit.collider.gameObject.GetComponent<Block>();
                    lastHighlightedBlock.ToggleHighlight(true);

                }
            }
            else if (markerHit.collider.gameObject.CompareTag("ENEMY")) { visualMarkerBlock.transform.position = Vector3.one * 100; }
            else
            {
                if(buildState == eBuildState.PLACE || buildState == eBuildState.PLACE_ENEMY)
                    if(visualMarkerBlock != null)visualMarkerBlock.transform.position = markerHit.point;
                else
                {
                    if (lastHighlightedBlock != null) lastHighlightedBlock.ToggleHighlight(false);
                }
            }
            //else if(visualMarkerBlock != null) visualMarkerBlock.transform.position = Vector3.one * 100;
        }
        else
        {
            if (buildState == eBuildState.PLACE) { }
            //visualMarkerBlock.transform.position = Vector3.one * 100;
            else if (buildState == eBuildState.REMOVE)
            {
                if (lastHighlightedBlock != null)
                    lastHighlightedBlock.ToggleHighlight(false);

            }

        }

        if (Input.touchCount > 0)
        {
            if (Input.touches[0].phase != TouchPhase.Began) return;

            pos = Input.touches[0].position;

            if (pos.IsPointOverUIObject() || !canPlaceBlocks) return;

            List<ARRaycastHit> hits = new List<ARRaycastHit>();

            if (Physics.Raycast(Camera.main.ViewportPointToRay(new Vector2(0.5f, 0.5f)), out hit))
            {
                if (hit.collider.gameObject.CompareTag("BLOCK"))
                {
                    if (buildState == eBuildState.PLACE)
                        PlaceBlockOnBlock(hit);
                    else if (buildState == eBuildState.PLACE_ENEMY)
                        PlaceEnemyOnBlock(hit);
                    else if (buildState == eBuildState.REMOVE)
                        RemoveBlock(hit.collider.gameObject);
                }
                else if (hit.collider.gameObject.CompareTag("ENEMY"))
                {
                    if (buildState == eBuildState.REMOVE)
                        RemoveBlock(hit.collider.gameObject);
                }
            }

            bool doARRaycast = false;
            if (hit.collider == null)
                doARRaycast = true;
            else if (hit.collider.gameObject.name.Contains("Plane"))
                doARRaycast = true;

            if (doARRaycast)
            {
                if (ARManager.arRaycastManager.Raycast(new Vector2(Camera.main.pixelWidth / 2, Camera.main.pixelHeight / 2), hits, TrackableType.PlaneWithinPolygon))
                {
                    if (buildState == eBuildState.PLACE) PlaceBlockOnPlane(hits[0].pose);
                    //ARDebug.Log("Plane Raycast", 5);
                }
            }
        }
    }

    public void PlaceBlockOnBlock(RaycastHit hit)
    {
        //We can assume that the first block has been placed already if were placing a block on a block

        Vector3 spawnPos = GetAimedPosition(hit);

        GameObject go = Instantiate(block[index], spawnPos, Quaternion.identity);
        float blockSize = go.GetComponent<MeshRenderer>().bounds.size.x;

        go.transform.parent = worldZeroBlock.transform;

        //ARDebug.Log(index, 5);
        if(index == (int)eBlocks.GRASS)
        {
            RaycastHit topHit;
            Ray ray = new Ray(go.transform.position, transform.up);
            if(Physics.Raycast(ray, out topHit, blockSize / 4))
            {
               // ARDebug.Log(topHit.collider.gameObject.name, 5);
            }

            if (Random.Range(0,4) == 0)
            {
                int flowerIndex = Random.Range(0, flowers.Length);
                Vector3 pos = go.transform.position;
                pos.y += blockSize;

                GameObject flower = Instantiate(flowers[flowerIndex], pos, Quaternion.identity);
                go.GetComponent<Block>().foilage = flower;
                //ARDebug.Log("Spawned flower", 5);

                flower.transform.parent = go.transform;
            }
        }

        spawnedBlocks.AddLast(go);
    }

    public void PlaceEnemyOnBlock(RaycastHit hit)
    {
        //We can assume that the first block has been placed already if were placing a block on a block

        Vector3 spawnPos = GetAimedPosition(hit);

        GameObject go = Instantiate(enemy[enemyIndex], spawnPos, enemy[enemyIndex].transform.rotation);
        float blockSize = globalBlockSize;

        go.transform.parent = worldZeroBlock.transform;
    }

    Vector3 GetAimedPosition(RaycastHit hit)
    {
        //GameObject go = Instantiate(block[index], hit.point, Quaternion.identity);
        float blockSize = block[0].GetComponent<MeshRenderer>().bounds.size.x;

        float x = hit.point.x;
        float y = hit.point.y;
        float z = hit.point.z;

        Transform tappedObject = hit.collider.gameObject.transform;

        if (hit.normal.x != 0)
        {
            y = tappedObject.position.y;
            z = tappedObject.position.z;
            x += blockSize / 2 * hit.normal.x;
        }
        if (hit.normal.y != 0)
        {
            x = tappedObject.position.x;
            z = tappedObject.position.z;

            if (hit.normal.y < 0) y -= blockSize;
            //y += blockSize / 2 * hit.normal.y;
        }
        if (hit.normal.z != 0)
        {
            x = tappedObject.position.x;
            y = tappedObject.position.y;
            z += blockSize / 2 * hit.normal.z;
        }


        return new Vector3(x, y, z);
    }

    public void PlaceBlockOnPlane(Pose pose)
    {
        GameObject go = Instantiate(block[index], pose.position, Quaternion.identity);
        float blockSize = go.GetComponent<MeshRenderer>().bounds.size.x;
        if (worldZeroBlock == null)
        {
            worldZeroBlock = go;
            ARAnchor anchor = ARManager.arAnchorManager.AddAnchor(pose);
            PlaceableObject po = worldZeroBlock.AddComponent<PlaceableObject>();
            po.Init(anchor);
        }
        else
        {

              go.transform.parent = worldZeroBlock.transform;
        }

        if (index == (int)eBlocks.GRASS)
        {
            RaycastHit topHit;
            Ray ray = new Ray(go.transform.position, transform.up);
            if (Physics.Raycast(ray, out topHit, blockSize / 4))
            {
                //ARDebug.Log(topHit.collider.gameObject.name, 5);
            }

            if (Random.Range(0, 4) == 0)
            {
                int flowerIndex = Random.Range(0, flowers.Length);
                Vector3 pos = go.transform.position;
                pos.y += blockSize;

                GameObject flower = Instantiate(flowers[flowerIndex], pos, Quaternion.identity);
                go.GetComponent<Block>().foilage = flower;
                //ARDebug.Log("Spawned flower", 5);

                flower.transform.parent = go.transform;
            }
        }

        spawnedBlocks.AddLast(go);
    }

    float RoundToBlockSize(float numToRound)
    {
        float num = numToRound / globalBlockSize + 1;
        return num * globalBlockSize;
    }

    public void RemoveBlock(GameObject block)
    {
        if(block != worldZeroBlock)
            Destroy(block);
    }

    public void TogglePlanes()
    {
        planesOn = !planesOn;

        if (ARManager.arPlaneManager != null)
        {
            ARManager.SetPlanesInvisible(planesOn);
        }
    }

    void ChangeBlock(int blockIndex)
    {
        index = blockIndex;
        buildState = eBuildState.PLACE;
    }

    void ChangeEnemy(int p_enemyIndex)
    {
        enemyIndex = p_enemyIndex;
        buildState = eBuildState.PLACE_ENEMY;
    }

    public void ChangeDrawState()
    {
        if(buildState == eBuildState.PLACE || buildState == eBuildState.PLACE_ENEMY)
        {
            //Going into erase mode
            if(visualMarkerBlock != null) visualMarkerBlock.SetActive(false);
            buildState = eBuildState.REMOVE;
        }
        else
        {
            if (lastHighlightedBlock != null)
                lastHighlightedBlock.ToggleHighlight(false);
            if (visualMarkerBlock != null) visualMarkerBlock.SetActive(true);
            buildState = eBuildState.PLACE;
        }
    }

    public void DismissInstructionsAndStart()
    {
        canPlaceBlocks = true;
        instructionsPanel.SetActive(false);
    }

    public void GoToMainMenu()
    {
        UIManager.onDrawModeButtonPressed -= ChangeDrawState;
        UIManager.onBlockDisplayButtonPressed -= ChangeBlock;

        if (worldZeroBlock != null) Destroy(worldZeroBlock);

        ARManager.SetPlanesActive(false);
        ARManager.SetPlaneManagerActive(false);

        MenuManager.instance.SpawnMenu(eMenus.MAIN_MENU);
        GameModeManager.instance.DestroyActiveGameMode();
    }

    private void OnDisable()
    {
        UIManager.onDrawModeButtonPressed -= ChangeDrawState;
        UIManager.onBlockDisplayButtonPressed -= ChangeBlock;
        UIManager.onEnemyDisplayButtonPressed -= ChangeEnemy;

        if (worldZeroBlock != null) Destroy(worldZeroBlock);

        ARManager.SetPlanesInvisible(true);
        ARManager.SetPlanesActive(false);
        ARManager.SetPlaneManagerActive(false);
    }
}
