using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public static class Extensions 
{
    public static bool IsPointOverUIObject(this Vector2 pos)
    {
        //Check if is over a game object, if we are than we know were not over a UI object
        if (EventSystem.current.IsPointerOverGameObject())
        {
            return false;
        }

        PointerEventData eventPosition = new PointerEventData(EventSystem.current);
        eventPosition.position = new Vector2(pos.x, pos.y);

        List<RaycastResult> results = new List<RaycastResult>();
        EventSystem.current.RaycastAll(eventPosition, results);

        return results.Count > 0; //Aka if there was a hit than its greater than one aka true, if 0 than hit nothing false
    }

}
