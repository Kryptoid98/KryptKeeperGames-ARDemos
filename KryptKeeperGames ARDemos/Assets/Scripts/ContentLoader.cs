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
        DESTROY_PUFF_FX
    }

    public enum MaterialID
    {
        PLANE_VISBLE,
        PLANE_LINE_VISIBLE,
        PLANE_INVISIBLE
    }

    public static GameObject LoadPrefab(PrefabID prefabID)
    {
        if (prefabID == PrefabID.AR_OBJECT_PLACEMENT) return (GameObject)Resources.Load("ARObjectPlacement");
        else if (prefabID == PrefabID.TRASH_CAN) return (GameObject)Resources.Load("Trashcan");
        else if (prefabID == PrefabID.DESTROY_PUFF_FX) return (GameObject)Resources.Load("DestroyPuff_FX");
        else return null;
    }

    public static Material LoadMaterial(MaterialID matID)
    {
        if (matID == MaterialID.PLANE_INVISIBLE) return (Material)Resources.Load("Materials/UnlitInvisible");
        else if(matID == MaterialID.PLANE_VISBLE) return (Material)Resources.Load("Materials/PlaneDefault");
        else if (matID == MaterialID.PLANE_LINE_VISIBLE) return (Material)Resources.Load("Materials/PlaneLineDefault");
        else return (Material)Resources.Load("Materials/PlaneDefault");
    }

    public static XRReferenceImageLibrary LoadReferenceLibrary()
    {
        return (XRReferenceImageLibrary)Resources.Load("ReferenceImageLibrary");
    }
}
