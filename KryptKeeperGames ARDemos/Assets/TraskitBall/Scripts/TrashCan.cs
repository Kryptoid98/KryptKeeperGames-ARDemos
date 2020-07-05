using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TrashCan : PlaceableObject
{
    public Detector scoreDetector;
    public MeshCollider hallowCollider;
    public MeshCollider trashCanCollider;
    public GameObject scoreParticles;

    LinkedList<Trash> trashInScoreDetector = new LinkedList<Trash>();

    Vector3 spawnedPosition = Vector3.zero;

    private void Start()
    {
        scoreDetector.onTriggerEnter += OnScoreDetectorEnter;
        //scoreDetector.onTriggerStay += OnScoreDetectorStay;
        spawnedPosition = transform.position;
    }

    void OnScoreDetectorEnter(Collider other)
    {
        Trash trash = other.gameObject.GetComponentInParent<Trash>();

        if (trash != null)
        {
            if (trash.hasScored || !trash.isThrown || TrashkitBallMode.instance.toCloseUI.activeSelf) return;
            else
            {
                Instantiate(scoreParticles, other.transform.position, Quaternion.identity);
                float distance = Vector3.Distance(transform.position, Camera.main.transform.position);
                TrashkitBallMode.instance.UpdateScore(Mathf.RoundToInt(20 * distance));
                trash.hasScored = true;

                trash.gameObject.transform.parent = transform;
            }
        }
    }

    //void OnScoreDetectorStay(Collider other)
    //{
    //    Trash trash = other.gameObject.GetComponent<Trash>();

    //    if (trash != null)
    //    {
    //        trash.inScoreDetectorTime += Time.deltaTime;
    //        if (trash.inScoreDetectorTime > 20)
    //        {
    //            ARDebug.Log("Flush", 10);
    //            //StartCoroutine(Flush());
    //        }
    //    }
    //}



    void UnsubFromDetector()
    {
        if (scoreDetector != null) return;
        scoreDetector.onTriggerEnter -= OnScoreDetectorEnter;
        //scoreDetector.onTriggerStay -= OnScoreDetectorStay;
    }

    private void OnDestroy()
    {
        UnsubFromDetector();
    }

    private void OnDisable()
    {
        UnsubFromDetector();
    }
}
