using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Spawner : PlaceableObject
{
    public Transform[] emitters;

    // Start is called before the first frame update
    void Start()
    {
        StartCoroutine(Spawn());
    }


    IEnumerator Spawn()
    {
        int num = 0;
        while (num < 100)
        {
            yield return new WaitForSeconds(3f);

            int rand = Random.Range(0, 3);

            GameObject go = GameObject.CreatePrimitive(PrimitiveType.Cube);
            go.transform.localScale = Vector3.one * 0.05f;
            //go.transform.parent = emitters[rand].transform.parent;
            go.transform.position = emitters[rand].transform.position;
            go.AddComponent<SimpleMove>();

            num++;
        }
    }
}
