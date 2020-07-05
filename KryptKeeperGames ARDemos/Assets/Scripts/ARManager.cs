using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR.ARFoundation;
using UnityEngine.XR.ARSubsystems;
[RequireComponent(typeof(ARRaycastManager))]
[RequireComponent(typeof(ARAnchorManager))]
[RequireComponent(typeof(ARPointCloudManager))]
[RequireComponent(typeof(ARPlaneManager))]

public class ARManager : MonoBehaviour
{
    public static ARSessionOrigin arSessionOrigin
    {
        get
        {
            if (sessionOrigin == null)
            {
                ARDebug.Log("Unable to find Session Origin", 30);
                return null;
            }

            return sessionOrigin;
        }
    }
    static ARSessionOrigin sessionOrigin;
    public static ARPlaneManager arPlaneManager
    {
        get
        {
            if (planeManager == null)
            {
                ARDebug.Log("Unable to find Plane Manager", 30);
                return null;
            }
            return planeManager;
        }
    }
    static ARPlaneManager planeManager;
    public static ARPointCloudManager arPointCloudManager
    {
        get
        {
            if (pointCloudManager == null)
            {
                ARDebug.Log("Unable to find Point Cloud Manager", 30);
                return null;
            }
            return pointCloudManager;
        }
    }
    static ARPointCloudManager pointCloudManager;
    public static ARRaycastManager arRaycastManager
    {
        get
        {
            if (rayManager == null)
            {
                ARDebug.Log("Unable to find AR Raycast Manager, making default", 30);
                return null;
            }
            return rayManager;
        }
    }
    static ARRaycastManager rayManager;
    public static ARAnchorManager arAnchorManager
    {
        get
        {
            if (anchorManager == null)
            {
                ARDebug.Log("Unable to find Anchor Manager", 30);
                return null;
            }
            return anchorManager;
        }
    }
    static ARAnchorManager anchorManager;

    public static ARSession session;
    public static GameObject sessionGameObject;
    void Awake()
    {
        session = FindObjectOfType<ARSession>();
        sessionGameObject = session.gameObject;

        sessionOrigin = FindObjectOfType<ARSessionOrigin>();
        planeManager = GetComponent<ARPlaneManager>();
        pointCloudManager = GetComponent<ARPointCloudManager>();
        rayManager = GetComponent<ARRaycastManager>();
        anchorManager = GetComponent<ARAnchorManager>();
    }

    public static void AssignNewARSession(GameObject p_sessionGameObject)
    {
        sessionGameObject = p_sessionGameObject;
        session = sessionGameObject.GetComponent<ARSession>();
    }

    public static void DestroyCurrentARSession()
    {
        Destroy(session);
        Destroy(sessionGameObject);
    }

    public static void SetPointCloudManagerActive(bool setActive)
    {
        if (setActive)
        {
            pointCloudManager.enabled = true;
        }
        else
        {
            var points = pointCloudManager.trackables;
            foreach (var p in points)
            {
                p.gameObject.SetActive(false);
            }
            pointCloudManager.enabled = false;
        }
    }
    public static void SetPlaneManagerActive(bool setActive, bool affectTrackables = false)
    {
        if (setActive)
        {
            planeManager.enabled = true;

            if (affectTrackables)
            {
                var planes = planeManager.trackables;
                foreach (var p in planes)
                    p.gameObject.SetActive(true);
            }
        }
        else
        {
            if (affectTrackables)
            {
                var planes = planeManager.trackables;
                foreach (var p in planes)
                    p.gameObject.SetActive(false);
            }

            planeManager.enabled = false;
        }
    }

    public static void SetPlanesActive(bool setActive)
    {
        var planes = planeManager.trackables;
        foreach (var p in planes)
            p.gameObject.SetActive(setActive);
    }

    public static void SetPlanesInvisible(bool setInvisible)
    {
        if (setInvisible)
        {
            Material invisMatPlane = ContentLoader.LoadMaterial(ContentLoader.MaterialID.PLANE_INVISIBLE);
            var planes = planeManager.trackables;
            foreach (var p in planes)
            {
                p.GetComponent<MeshRenderer>().sharedMaterial = invisMatPlane;
                p.GetComponent<LineRenderer>().sharedMaterial = invisMatPlane;
            }
        }
        else
        {
            Material planeMat = ContentLoader.LoadMaterial(ContentLoader.MaterialID.PLANE_VISBLE);
            Material lineMat = ContentLoader.LoadMaterial(ContentLoader.MaterialID.PLANE_LINE_VISIBLE);
            var planes = planeManager.trackables;
            foreach (var p in planes)
            {
                p.GetComponent<MeshRenderer>().sharedMaterial = planeMat;
                p.GetComponent<LineRenderer>().sharedMaterial = lineMat;
            }
        }
    }

    public float GetDistanceFromSessionOrigin()
    {
        return Vector3.Distance(sessionOrigin.transform.position, Camera.main.transform.position);
    }
}
