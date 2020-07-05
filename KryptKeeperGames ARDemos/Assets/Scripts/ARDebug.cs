using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ARDebug : MonoBehaviour
{
    static LinkedList<ObjectTimeTracker> objects = new LinkedList<ObjectTimeTracker>();
    static LinkedList<SystemObjectTimeTracker> systemObjects = new LinkedList<SystemObjectTimeTracker>();

    static int fontSize = 30;
    static Color fontColor = Color.red;

    public static void Log(Object obj, float displayTime)
    {
        objects.AddLast(new ObjectTimeTracker(obj, displayTime));
    }

    public static void Log(System.Object obj, float displayTime)
    {
        systemObjects.AddLast(new SystemObjectTimeTracker(obj, displayTime));
    }

    void OnGUI()
    {
        int guiInc = 80;
        GUI.skin.label.fontSize = fontSize;
        GUI.color = fontColor;

        LinkedList<ObjectTimeTracker> removeMes = null;
        int count = 0;

        foreach (ObjectTimeTracker ott in objects)
        {

            if (ott.obj is GameObject)
            {
                GUI.Label(new Rect(0, guiInc * count, Screen.width, 90), ((GameObject)ott.obj).name);
            }


            count++;
            ott.removeMeTime -= Time.deltaTime;
            if (ott.removeMeTime <= 0)
            {
                if (removeMes == null)
                    removeMes = new LinkedList<ObjectTimeTracker>();
                removeMes.AddLast(ott);
            }
        }

        if (removeMes != null)
        {
            foreach (ObjectTimeTracker ott in removeMes)
                objects.Remove(ott);
        }



        LinkedList<SystemObjectTimeTracker> sysRemoveMes = null;

        foreach (SystemObjectTimeTracker ott in systemObjects)
        {
            GUI.Label(new Rect(0, guiInc * count, Screen.width, fontSize), ott.obj.ToString());

            count++;
            ott.removeMeTime -= Time.deltaTime;
            if (ott.removeMeTime <= 0)
            {
                if (sysRemoveMes == null)
                    sysRemoveMes = new LinkedList<SystemObjectTimeTracker>();
                sysRemoveMes.AddLast(ott);
            }
        }

        if (sysRemoveMes != null)
        {
            foreach (SystemObjectTimeTracker ott in sysRemoveMes)
                systemObjects.Remove(ott);
        }
    }
}

class ObjectTimeTracker
{
    public Object obj;
    public float removeMeTime;

    public ObjectTimeTracker(Object p_obj, float p_removeTime)
    {
        obj = p_obj;
        removeMeTime = p_removeTime;
    }
}

class SystemObjectTimeTracker
{
    public System.Object obj;
    public float removeMeTime;

    public SystemObjectTimeTracker(System.Object p_obj, float p_removeTime)
    {
        obj = p_obj;
        removeMeTime = p_removeTime;
    }
}
