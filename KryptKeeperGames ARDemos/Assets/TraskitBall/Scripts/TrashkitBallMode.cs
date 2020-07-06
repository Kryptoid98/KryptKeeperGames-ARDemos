using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using KryptKeeperGamesARDemo.Utility;
using KryptKeeperGamesARDemo.Enums;
using TMPro;

public class TrashkitBallMode : GameMode
{
    public static TrashkitBallMode instance;
    enum eTrashkitballState
    {
        INIT,
        IN_PLAY,
        RESETTING_AR,
        GAME_OVER
    }
    eTrashkitballState trashkitballState = eTrashkitballState.INIT;

    [Header("Trash Prefabs")]
    public GameObject[] trashRefs;

    [Header("Sound_FX")]
    public AudioClip gameOver_sfx;
    public AudioClip[] throw_sfx;

    [Header("UI")]
    public Animator canvasAnimator;
    public GameObject generalUI;
    public TMP_Text scoreText;
    public TMP_Text trashCountText;
    public GameObject gameOverUI;
    public TMP_Text goScoreText;
    public GameObject toCloseUI;
    public GameObject gameInstructionsUI;
    bool firstPlay = true;

    GameObject spawnedTrashcan;
    ARObjectPlacement spawnedObjectPlacement;

    LinkedList<Trash> spawnedTrash = new LinkedList<Trash>();
    GameObject activeTrash;
    bool holdingTrash = false;
    bool spawningTrash = false;
    float maxYDistanceFromTrashcan = 3;
    float zDistanceFromCamera = 0.5f;
    float baseScale = 1;

    int score = 0;
    int trashCount = 10;

    int currentDisplayedScore = 0;
    bool animatingScore = false;

    private void Awake()
    {
        instance = this;
    }


    private void Update()
    {
        if (trashkitballState == eTrashkitballState.IN_PLAY)
        {
            //Input
            if (Input.touchCount > 0)
            {
                GrabTrash();
            }

            CheckPlayerDistanceFromTrashCan();
            DestroyTrashBelowTrashCan();
        }
    }
    public override void Init(GameObject parentObject)
    {
        ChangeState(eTrashkitballState.INIT);
    }

    void ChangeState(eTrashkitballState newState)
    {
        //Exiting State
        if(trashkitballState == eTrashkitballState.IN_PLAY)
        {
            if(newState != eTrashkitballState.GAME_OVER) generalUI.SetActive(false);
        }
        else if(trashkitballState == eTrashkitballState.GAME_OVER)
        {
            gameOverUI.SetActive(false);
        }

        trashkitballState = newState;

        //Entering State
        if(trashkitballState == eTrashkitballState.INIT)
        {
            ARManager.SetPlaneManagerActive(true);
            ARManager.SetPlanesInvisible(false);
            SpawnARObjectPlacement();
            trashCount = 10;
            score = 0;
            currentDisplayedScore = 0;
            animatingScore = false;
            scoreText.text = "0";
            trashCountText.text = trashCount.ToString();

        }
        else if(trashkitballState == eTrashkitballState.IN_PLAY)
        {
            //Deactivate any planes that may be above the trashcan
            var planes = ARManager.arPlaneManager.trackables;
            foreach(var p in planes)
            {
                if (p.transform.position.y > (spawnedTrashcan.transform.position.y + spawnedTrashcan.GetComponentInChildren<MeshRenderer>().bounds.size.y / 2))
                {
                    //ARDebug.Log("Turned off plane above trashcan", 5);
                    p.gameObject.SetActive(false);
                }
            }

            ARManager.SetPlaneManagerActive(false);
            ARManager.SetPlanesInvisible(true);

            generalUI.SetActive(true);

            //This is a stupid way to do this, use triggers in the animator and have them be able to call themselves of the Any state
            //canvasAnimator.Play("New State");
            canvasAnimator.SetTrigger("Play");

            StartCoroutine(SpawnTrashTimed(1.5f));
        }
        else if (trashkitballState == eTrashkitballState.RESETTING_AR)
        {
            DestroyGameSessionObjects();
            StartCoroutine(RestartLocalARSession());
        }
        else if(trashkitballState == eTrashkitballState.GAME_OVER)
        {
            gameOverUI.SetActive(true);
            toCloseUI.SetActive(false);
            goScoreText.text = score.ToString();

            canvasAnimator.SetTrigger("GameOver");
            SoundManager.instance.PlaySound(gameOver_sfx);
        }
    }

    //Called from event listener when AR Object Placement has finished
    public void StartGameMode(GameObject p_spawnedObject, float baseObjectScale)
    {
        if (spawnedObjectPlacement != null) spawnedObjectPlacement.onPlacementFinished -= StartGameMode;
        spawnedTrashcan = p_spawnedObject;
        baseScale = baseObjectScale;

        ChangeState(eTrashkitballState.IN_PLAY);
    }

    void SpawnARObjectPlacement()
    {
        GameObject arPlacement = Instantiate(GameManager.instance.ARObjectPlacementPrefab, Vector3.zero, Quaternion.identity);
        spawnedObjectPlacement = arPlacement.GetComponent<ARObjectPlacement>();
        spawnedObjectPlacement.Init("Trash Can", ContentLoader.LoadPrefab(ContentLoader.PrefabID.TRASH_CAN));
        spawnedObjectPlacement.onPlacementFinished += StartGameMode;
    }

    public void GrabTrash()
    {
        if (activeTrash == null) return;

        if (Input.touches[0].phase == TouchPhase.Began && !spawningTrash)
        {
            Vector3 touchPos = Input.touches[0].position;
            RaycastHit hit;
            if (Physics.Raycast(Camera.main.ScreenPointToRay(touchPos), out hit))
            {
                Trash t = hit.collider.gameObject.GetComponentInParent<Trash>();
                if (t != null)
                {
                    if (t.isThrown) return;
                    if (firstPlay) {
                        firstPlay = false;
                        gameInstructionsUI.SetActive(false);
                    }

                    t.rotate = false;
                    holdingTrash = true;

                }
            }
        }
        else if(Input.touches[0].phase == TouchPhase.Moved && holdingTrash == true)
        {
            Vector3 touchViewPoint = Camera.main.ScreenToViewportPoint(Input.touches[0].position);
            if (touchViewPoint.y > 0.4f) touchViewPoint.y = 0.4f;

            activeTrash.transform.position = Utility.FindPointInFrustrum(Camera.main, touchViewPoint.x, touchViewPoint.y, zDistanceFromCamera);
        }
        else if(Input.touches[0].phase == TouchPhase.Ended || Input.touches[0].phase == TouchPhase.Canceled)
        {
            holdingTrash = false;

            Vector3 touchViewPoint = Camera.main.ScreenToViewportPoint(Input.touches[0].position);
            if (touchViewPoint.y > 0.4f) touchViewPoint.y = 0.4f;

            float distance = Mathf.Abs(0.08f - touchViewPoint.y) * 2.2f;
            Vector3 dir = (touchViewPoint - new Vector3(0.5f, 0.08f, 0)).normalized;

            activeTrash.GetComponent<Trash>().Throw(distance, dir);
            if(throw_sfx != null)
            {
                int randSFX = Random.Range(0, throw_sfx.Length);
                SoundManager.instance.PlaySound(throw_sfx[randSFX]);
            }

            if (trashCount > 0)
                StartCoroutine(SpawnTrashTimed());
            else
                StartCoroutine(GameOver());
        }
    }


    public void SpawnTrash()
    {
        if (trashkitballState != eTrashkitballState.IN_PLAY) return;

        int randomNum = Random.Range(0, trashRefs.Length);

        Vector3 spawnLocation = Utility.FindPointInFrustrum(Camera.main, 1, 0, zDistanceFromCamera);
        GameObject newGarbage = Instantiate(trashRefs[randomNum], spawnLocation, Camera.main.transform.rotation);
        newGarbage.transform.localScale = Vector3.one * baseScale;

        newGarbage.transform.parent = Camera.main.transform;

        activeTrash = newGarbage;
        spawnedTrash.AddLast(newGarbage.GetComponent<Trash>());

        trashCount--;
        if (trashCount < 0) trashCount = 0;
        trashCountText.text = "Trash Count: " + trashCount;
    }

    IEnumerator SpawnTrashTimed(float waitTime = 1)
    {
        spawningTrash = true;
        yield return new WaitForSeconds(waitTime);

        SpawnTrash();

        while (Vector3.Distance(Utility.FindPointInFrustrum(Camera.main, 0.5f, 0.08f, zDistanceFromCamera), activeTrash.transform.position) > 0.02f)
        {
            activeTrash.transform.position = Vector3.Lerp(activeTrash.transform.position, Utility.FindPointInFrustrum(Camera.main, 0.5f, 0.08f, zDistanceFromCamera), 0.1f);
            yield return new WaitForFixedUpdate();
        }

        spawningTrash = false;
        if (firstPlay) gameInstructionsUI.SetActive(true);
    }

    void CheckPlayerDistanceFromTrashCan()
    {
        Vector3 playerPos = Camera.main.transform.position;
        playerPos.y = 0;

        Vector3 trashCanPos = spawnedTrashcan.transform.position;
        trashCanPos.y = 0;

        float distance = Vector3.Distance(playerPos, trashCanPos);

        //ARDebug.Log(distance, 0.05f);

        if (distance < 0.3f)
        {
            if (!toCloseUI.activeSelf) toCloseUI.SetActive(true);
        }
        else if (toCloseUI.activeSelf) toCloseUI.SetActive(false);
    }

    IEnumerator GameOver()
    {
        yield return new WaitForSeconds(1);
        ChangeState(eTrashkitballState.GAME_OVER);
    }

    public void DestroyTrashBelowTrashCan()
    {
        //Check to make sure trash isnt below the trash can by max Y distance
        LinkedList<Trash> trashBeingDestroyed = null;
        foreach (Trash t in spawnedTrash)
        {
            if (Mathf.Abs(t.transform.position.y - spawnedTrashcan.transform.position.y) > maxYDistanceFromTrashcan)
            {
                if (trashBeingDestroyed == null) trashBeingDestroyed = new LinkedList<Trash>();
                trashBeingDestroyed.AddLast(t);
            }
        }

        if (trashBeingDestroyed != null)
        {
            foreach (Trash t in trashBeingDestroyed)
            {
                t.Destroy();
                spawnedTrash.Remove(t);
            }
        }
    }

    void DestroyGameSessionObjects()
    {
        if (spawnedTrashcan != null)
        {
            ARManager.arAnchorManager.RemoveAnchor(spawnedTrashcan.GetComponent<PlaceableObject>().refAnchor);
            Destroy(spawnedTrashcan);
            spawnedTrashcan = null;

            foreach (Trash t in spawnedTrash)
                t.Destroy();

            spawnedTrash = new LinkedList<Trash>();
        }
    }

    public void NewGameButton()
    {
        DestroyGameSessionObjects();
        ChangeState(eTrashkitballState.INIT);
    }

    public void BackToMainMenu()
    {
        DestroyGameSessionObjects();

        MenuManager.instance.SpawnMenu(eMenus.MAIN_MENU);
        GameModeManager.instance.DestroyActiveGameMode();
    }

    public void RestartARSessionButton()
    {
        ChangeState(eTrashkitballState.RESETTING_AR);
    }

    IEnumerator RestartLocalARSession()
    {
        yield return new WaitForSeconds(0.2f);
        StartCoroutine(GameManager.RestartARSession());
        yield return new WaitForSeconds(0.5f);

        ARManager.SetPlaneManagerActive(true);
        SpawnARObjectPlacement();
    }

    public void UpdateScore(int addedScore)
    {
        if (toCloseUI.activeSelf) return;

        score += addedScore;
        if (!animatingScore) StartCoroutine(AnimateScore());
    }

    IEnumerator AnimateScore()
    {
        while(currentDisplayedScore < score)
        {
            if (trashkitballState == eTrashkitballState.GAME_OVER) yield break;
            currentDisplayedScore++;
            scoreText.text = currentDisplayedScore.ToString();

            yield return new WaitForSeconds(0.1f * Time.deltaTime);
        }

        animatingScore = false;
        scoreText.text = score.ToString();
    }

    private void OnDisable()
    {
        if (spawnedObjectPlacement != null) spawnedObjectPlacement.onPlacementFinished -= StartGameMode;
    }
}
