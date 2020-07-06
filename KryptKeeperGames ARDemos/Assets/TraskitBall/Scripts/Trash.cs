using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Trash : MonoBehaviour
{
    Rigidbody rb = null;
    AudioSource aSource;
    private void Start()
    {
        aSource = GetComponentInChildren<AudioSource>();
    }
    protected void Update()
    {
        if(rotate)transform.Rotate(new Vector3(0,0,1));
        //transform.rotation += Quaternion.Euler(0, 0, 1);
    }

    public bool hasScored = false;
    public bool rotate = true;
    public bool isThrown = false;
    public float inScoreDetectorTime = 0;

    public void Throw(float distance, Vector3 dir)
    {
        transform.parent = null;
        ActivatePhysics();
        rb.AddForce((Camera.main.transform.forward + Camera.main.transform.up * dir.y * distance) * (distance * 5), ForceMode.Impulse);
    }

    public void ActivatePhysics()
    {
        rb = gameObject.AddComponent<Rigidbody>();
        rb.collisionDetectionMode = CollisionDetectionMode.Continuous;
        //rb.AddForce(Camera.main.transform.forward * 2, ForceMode.Impulse);
        isThrown = true;
    }

    public void Destroy()
    {
        SoundManager.instance.PlaySound(SoundManager.instance.poof_sfx);
        Instantiate(ContentLoader.LoadPrefab(ContentLoader.PrefabID.DESTROY_PUFF_FX), transform.position, Quaternion.identity);
        Destroy(gameObject);
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (SoundManager.instance.canClink_sfx != null) {
            int rand = Random.Range(0, SoundManager.instance.canClink_sfx.Length);
            //aSource.clip = SoundManager.instance.canClink_sfx[rand];
            //aSource.Play();
            SoundManager.instance.PlaySound(SoundManager.instance.canClink_sfx[rand]);
        }
    }
}
