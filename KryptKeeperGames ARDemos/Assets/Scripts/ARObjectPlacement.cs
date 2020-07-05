using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.XR.ARFoundation;
using UnityEngine.XR.ARSubsystems;

public class ARObjectPlacement : MonoBehaviour
{
    public AudioClip placementSFX;

    string[] helpTips = new string[]
    {
        "AR works much better in well lit environments.",
        "AR stands for Augmented Reality.",
        "AR is awesome but not perfect. At any point tap the Reset AR button to fix strange behaviours.",
        "Be patient, sometimes it can take up to 20 seconds to start detecting surfaces.",
        "More space, more fun.",
        "The more time spent extending your playable surface (if possible) the better.",
        "After a bit of AR gaming, your device can become quite warm. When this happens, AR tracking becomes less effective.",
        "Continuously move your phone over the same area to merge clusters of small planes."
    };

    public delegate void OnPlacementFinished(GameObject p_spawnedObject, float baseObjectScale);
    public OnPlacementFinished onPlacementFinished;

    enum ePlacementState
    {
        PLANE_DETECTION,
        //POINT_CLOUD_FALLBACK,
        //DISTANCE_ESTIMATION_FALLBACK,
        OBJECT_PLACEMENT,
        SCALE_OBJECT,
        FINISH
    }
    ePlacementState placementState = ePlacementState.PLANE_DETECTION;

    [Header("Effects")]
    public GameObject spawnCloud;

    [Header("UI")]
    public Animator UIAnimator;
    public TMP_Text instructionsText;
    public TMP_Text tipsText;
    public GameObject planeDetectionUI;
    public GameObject scaleUI;
    public Slider scaleSlider;
    float defaultSliderValue = 1;

    string gameObjectName = "";
    GameObject prefabToSpawn; //What to spawn, will be private and sent in in Init

    GameObject spawnedPrefab; //The spawned object from placement

    float tipRotateTime = 10;

    bool canClick = true;
    float clickCD = 0;
    float clickCDMax = 1;

    float timeSpentOnPlaneDetectionMin = 3;
    float curTimeSpentOnPlaneDetection = 0;

    private void Start()
    {
        ChangeState(ePlacementState.PLANE_DETECTION);
        StartCoroutine(RotateTipsText());
    }

    public void Init(string objectName, GameObject p_spawnPrefab)
    {
        gameObjectName = objectName;
        prefabToSpawn = p_spawnPrefab;
    }

    void ChangeState(ePlacementState newState)
    {
        //Exiting State
        if (placementState == ePlacementState.PLANE_DETECTION)
            planeDetectionUI.SetActive(false);
        else if (placementState == ePlacementState.SCALE_OBJECT)
        {
            if(newState != ePlacementState.FINISH)
                scaleUI.SetActive(false);
        }
        

        placementState = newState;

            
        if(placementState == ePlacementState.PLANE_DETECTION)
        {
            ARManager.SetPlaneManagerActive(true);
            planeDetectionUI.SetActive(true);
            curTimeSpentOnPlaneDetection = 0;
            instructionsText.text = "Slowly pan your phone around your play area to discover surfaces";
        }
        else if(placementState == ePlacementState.OBJECT_PLACEMENT)
        {
            instructionsText.text = "Tap anywhere within the surface to spawn the " + gameObjectName;
        }
        else if(placementState == ePlacementState.SCALE_OBJECT)
        {
            scaleUI.SetActive(true);
            instructionsText.text = "Use the slider to adjust the size to fit your environment if neccesary";
        }
        else if(placementState == ePlacementState.FINISH)
        {
            FinishObjectPlacement();
        }
    }

    private void Update()
    {
        if(placementState == ePlacementState.PLANE_DETECTION)
        {
            curTimeSpentOnPlaneDetection += Time.deltaTime;

            if(ARManager.arPlaneManager.trackables.count > 0 && curTimeSpentOnPlaneDetection > timeSpentOnPlaneDetectionMin)
            {
                ChangeState(ePlacementState.OBJECT_PLACEMENT);
            }
        }
        if(placementState == ePlacementState.OBJECT_PLACEMENT)
        {
            if (Input.touchCount > 0 && canClick)
                PlacementInput();
        }

        if(clickCD > 0)
        {
            clickCD -= Time.deltaTime;
            if(clickCD <= 0)
            {
                clickCD = 0;
                canClick = true;
            }
        }
    }

    void PlacementInput()
    {
        Vector3 pos = Input.touches[0].position;
        List<ARRaycastHit> hits = new List<ARRaycastHit>();
        if (ARManager.arRaycastManager.Raycast(pos, hits, TrackableType.PlaneWithinPolygon))
        {
            ARAnchor anchor = ARManager.arAnchorManager.AddAnchor(hits[0].pose);
            if (SpawnObject(anchor, hits[0].pose))
            {
                SoundManager.instance.PlaySound(placementSFX);
                ChangeState(ePlacementState.SCALE_OBJECT);
            }
        }
    }

    bool SpawnObject(ARAnchor anchor, Pose pose)
    {
        if (anchor == null || pose == null) return false;
        spawnedPrefab = Instantiate(prefabToSpawn, anchor.transform.localPosition, anchor.transform.rotation);
        spawnedPrefab.GetComponent<PlaceableObject>().Init(anchor);

        if(spawnCloud != null) Instantiate(spawnCloud, anchor.transform.localPosition, anchor.transform.rotation);

        return true;
    }

    void FinishObjectPlacement()
    {
        StartCoroutine(FadeAwayAndClose());
    }

    IEnumerator FadeAwayAndClose()
    {
        UIAnimator.Play("fadeout");

        yield return new WaitForSeconds(1);
        if (onPlacementFinished != null)
            onPlacementFinished(spawnedPrefab, scaleSlider.value);

        Destroy(gameObject);
    }

    IEnumerator RotateTipsText()
    {
        
        tipsText.text = helpTips[Random.Range(0, helpTips.Length)];

        while (true)
        {
            yield return new WaitForSeconds(tipRotateTime);
            bool found = false;
            int randomIndex = 0;
            while (!found)
            {
                randomIndex = Random.Range(0, helpTips.Length);
                if (tipsText.text != helpTips[randomIndex]) found = true;
            }

            tipsText.text = helpTips[randomIndex];
        }
    }

    public void UpdateScale()
    {
        spawnedPrefab.transform.localScale = Vector3.one * scaleSlider.value;
    }

    public void Reset()
    {
        ActivateClick();

        scaleSlider.value = defaultSliderValue;
        Destroy(spawnedPrefab);

        if (ARManager.arPlaneManager.trackables.count <= 0)
            ChangeState(ePlacementState.PLANE_DETECTION);
        else
            ChangeState(ePlacementState.OBJECT_PLACEMENT);
    }
    public void DoneScaling()
    {
        ChangeState(ePlacementState.FINISH);
    }

    public void ActivateClick()
    {
        canClick = false;
        clickCD = clickCDMax;
    }

    public void RestartButton() {
        StartCoroutine(RestartLocalARSession()); 
    }

    IEnumerator RestartLocalARSession()
    {
        yield return new WaitForSeconds(0.1f);

        StartCoroutine(GameManager.RestartARSession());
        ChangeState(ePlacementState.PLANE_DETECTION);
        yield return new WaitForSeconds(0.5f);

        ActivateClick();

        scaleSlider.value = defaultSliderValue;
        Destroy(spawnedPrefab);

        ARManager.SetPlaneManagerActive(true);
    }
}
