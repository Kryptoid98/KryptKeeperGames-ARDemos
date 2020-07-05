using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public abstract class Menu : MonoBehaviour
{
    protected float fadeOutWaitPerFrame = 0.8f;
    protected bool fadingOut = false;

    //protected void ChangeSubMenu<T>(T e) {
    //    Debug.Log(e);
    //}

    public abstract void Init(GameObject parentObject);

    protected IEnumerator FadeOut()
    {
        fadingOut = true;
        //Wait a very small amount to allow button anim to play
        yield return new WaitForSeconds(0.05f);

        //Fade out components
        float dec = 0.05f;
        Image[] images = GetComponentsInChildren<Image>();
        Text[] text = GetComponentsInChildren<Text>();
        TMP_Text[] tmpText = GetComponentsInChildren<TMP_Text>();

        for (float i = 1; i > 0; i -= dec)
        {
            foreach (Image im in images)
            {
                if (im.tag == "UI/FADEABLE")
                {
                    Color temp = im.color;
                    temp.a -= dec;
                    im.color = temp;
                }
            }
            foreach (Text t in text)
            {
                if (t.CompareTag("UI/FADEABLE"))
                {
                    Color temp = t.color;
                    temp.a -= dec;
                    t.color = temp;
                }
            }
            foreach (TMP_Text tmp in tmpText)
            {
                if (tmp.CompareTag("UI/FADEABLE"))
                {
                    Color temp = tmp.color;
                    temp.a -= dec;
                    tmp.color = temp;
                }
            }
            yield return new WaitForSeconds(fadeOutWaitPerFrame * Time.deltaTime);
        }

        yield return new WaitForSeconds(0.1f);

        fadingOut = false;
    }
}
