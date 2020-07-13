using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR.ARSubsystems;

public class ContentLoader
{
    public enum PrefabID
    {
        AR_OBJECT_PLACEMENT,
        TRASH_CAN,
        DESTROY_PUFF_FX,
        BLOCKBUILDER_UIMANAGER,
        AR_DEFAULT_PLANE,
        AR_WATER_PLANE
    }

    public enum MaterialID
    {
        PLANE_VISBLE,
        PLANE_LINE_VISIBLE,
        PLANE_INVISIBLE,
        WATER
    }

    public static GameObject LoadPrefab(PrefabID prefabID)
    {
        if (prefabID == PrefabID.AR_OBJECT_PLACEMENT) return (GameObject)Resources.Load("ARObjectPlacement");
        else if (prefabID == PrefabID.TRASH_CAN) return (GameObject)Resources.Load("Trashcan");
        else if (prefabID == PrefabID.DESTROY_PUFF_FX) return (GameObject)Resources.Load("DestroyPuff_FX");
        else if (prefabID == PrefabID.BLOCKBUILDER_UIMANAGER) return (GameObject)Resources.Load("BB_UIManager");
        else if (prefabID == PrefabID.AR_DEFAULT_PLANE) return (GameObject)Resources.Load("AR Default Plane");
        else if (prefabID == PrefabID.AR_WATER_PLANE) return (GameObject)Resources.Load("AR Water Plane");

        else return null;
    }

    public static Material LoadMaterial(MaterialID matID)
    {
        if (matID == MaterialID.PLANE_INVISIBLE) return (Material)Resources.Load("Materials/UnlitInvisible");
        else if(matID == MaterialID.PLANE_VISBLE) return (Material)Resources.Load("Materials/PlaneDefault");
        else if (matID == MaterialID.PLANE_LINE_VISIBLE) return (Material)Resources.Load("Materials/PlaneLineDefault");
        else if (matID == MaterialID.WATER) return (Material)Resources.Load("Materials/Water");
        else return (Material)Resources.Load("Materials/PlaneDefault");
    }

    public static XRReferenceImageLibrary LoadReferenceLibrary()
    {
        return (XRReferenceImageLibrary)Resources.Load("ReferenceImageLibrary");
    }
}
