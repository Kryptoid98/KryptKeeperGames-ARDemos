using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR.ARFoundation;
using UnityEngine.XR.ARSubsystems;

public class AutoRTSMode : GameMode
{
    enum eAutoBattleState
    {
        INIT,
        WAITING_FOR_SUMMONS,
        IN_BATTLE
    }
    eAutoBattleState autoBattleState = eAutoBattleState.INIT;


    ARTrackedImageManager imageManager;
    //public IReferenceImageLibrary imageLibrary;
    //public GameObject spawnObject;

    public GameObject instructionUI;
    public GameObject mainMenuButton;

    public Minion[] minionPrefabs;
    Dictionary<string, GameObject> minionDictionary = new Dictionary<string, GameObject>();

    LinkedList<string> spawnedMinions = new LinkedList<string>();

    Minion minion1;
    Minion minion2;

    GameObject temp;
    public override void Init(GameObject parentObject)
    {
        ARManager.SetPlanesActive(false); //Incase theres existing planes
        ARManager.SetPlaneManagerActive(false);

        instructionUI.SetActive(true);
        mainMenuButton.SetActive(false);

        if(minionPrefabs != null)
        {
            foreach(Minion m in minionPrefabs)
            {
                minionDictionary.Add(m.gameObject.name, m.gameObject);
            }
        }

        //ARDebug.Log("Loaded prefabs", 5);

        imageManager = FindObjectOfType<ARTrackedImageManager>();
        if (imageManager == null)
        {
            imageManager = ARManager.arSessionOrigin.gameObject.AddComponent<ARTrackedImageManager>();
            imageManager.referenceLibrary = ContentLoader.LoadReferenceLibrary();
            imageManager.maxNumberOfMovingImages = 2;
            //imageManager.trackedImagePrefab = spawnObject;
        }

        imageManager.enabled = true;
        imageManager.trackedImagesChanged += UpdateImage;

        //ARDebug.Log("Finish Init", 5);
    }

    void ChangeState(eAutoBattleState newState)
    {
        autoBattleState = newState;

        if(autoBattleState == eAutoBattleState.IN_BATTLE)
        {
            minion1.Activate(minion2);
            minion2.Activate(minion1);
        }
    }

    void UpdateImage(ARTrackedImagesChangedEventArgs args)
    {
        //ARDebug.Log("Update", 5);
        foreach (ARTrackedImage i in args.added)
        {
            //ARDebug.Log(i.referenceImage.name, 10);
            if (!spawnedMinions.Contains(i.referenceImage.name))
            {
                GameObject minionToSpawn;
                minionDictionary.TryGetValue(i.referenceImage.name, out minionToSpawn);
                if (minionToSpawn == null) return;

                GameObject newMinionGO = Instantiate(minionToSpawn.gameObject, i.transform.position, Quaternion.identity);

                Minion newMinion = newMinionGO.GetComponent<Minion>();
                    
                newMinion.MinionInit(i.referenceImage.name);
                temp = newMinionGO;

                if (minion1 == null) minion1 = newMinion;
                else if (minion2 == null) minion2 = newMinion;
                spawnedMinions.AddLast(i.referenceImage.name);

                if (minion1 != null && minion2 != null) ChangeState(eAutoBattleState.IN_BATTLE);
            }
        }

        foreach (ARTrackedImage i in args.updated)
        {
            if (minion1 != null) {
                //ARDebug.Log(minion1.minionName + "   " + i.referenceImage.name, 5);
                if (minion1.minionName == i.referenceImage.name)
                    minion1.gameObject.transform.position = i.transform.position;
            }
            if (minion2 != null)
            {
                //ARDebug.Log(minion1.minionName + "   " + i.referenceImage.name, 5);
                if (minion2.minionName == i.referenceImage.name)
                    minion2.gameObject.transform.position = i.transform.position;
            }
        }
    }

    public void DismissInstruction()
    {
        instructionUI.SetActive(false);
        mainMenuButton.SetActive(true);
    }

    public void GoToMainMenu()
    {
        DestroySelf();
        MenuManager.instance.SpawnMenu(KryptKeeperGamesARDemo.Enums.eMenus.MAIN_MENU);
        GameModeManager.instance.DestroyActiveGameMode();
    }

    private void OnDisable()
    {
        DestroySelf();
    }

    private void OnDestroy()
    {
        DestroySelf();
    }

    void DestroySelf()
    {
        imageManager.trackedImagesChanged -= UpdateImage;
        if (minion1 != null) Destroy(minion1.gameObject);
        if (minion2 != null) Destroy(minion2.gameObject);
        Destroy(imageManager);

       
    }
}
