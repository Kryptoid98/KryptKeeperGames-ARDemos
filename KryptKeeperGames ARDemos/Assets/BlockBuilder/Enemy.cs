using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy : MonoBehaviour
{
    float speed = 1.5f;
    float sizeX;

    bool giveUpOnMoving = false;
    int timesTurned = 0;
    float elaspedTime = 0;

    // Start is called before the first frame update
    void Start()
    {
        sizeX = BlockBuilderMode.instance.globalBlockSize;
        speed = speed * sizeX;
    }

    // Update is called once per frame
    void Update()
    {
        if (giveUpOnMoving) return;

        transform.position += transform.forward * speed * Time.deltaTime;

        //Ray tempAngleRay = new Ray();
        //tempAngleRay.origin = transform.position + transform.forward * sizeX / 2;
        //tempAngleRay.direction = transform.forward + -transform.up / 2;

        //Debug.DrawRay(tempAngleRay.origin, tempAngleRay.direction * sizeX / 2, Color.red);

        if (Physics.Raycast(transform.position + transform.up * sizeX / 2, transform.forward, sizeX) || !Physics.Raycast(transform.position + transform.forward * sizeX / 2, transform.forward + -transform.up / 2, sizeX))
        {
            if ((Physics.Raycast(transform.position + transform.up * sizeX / 2, transform.forward, sizeX) || !Physics.Raycast(transform.position + transform.forward * sizeX / 2, transform.forward + -transform.up / 2, sizeX))
                && (Physics.Raycast(transform.position + transform.up * sizeX / 2, -transform.forward, sizeX) || !Physics.Raycast(transform.position + -transform.forward * sizeX / 2, -transform.forward + -transform.up / 2, sizeX)))
            {
                giveUpOnMoving = true;
                GetComponent<Animator>().SetTrigger("Idle");
            }
            else
            {
                transform.Rotate(0, 180, 0);
                timesTurned++;
                //ARDebug.Log(timesTurned, 2);
            }
        }

        elaspedTime += Time.deltaTime;
        if(elaspedTime > 1)
        {
            elaspedTime = 0;
            if(timesTurned > 5)
            {
                giveUpOnMoving = true;
                GetComponent<Animator>().SetTrigger("Idle");
            }
            timesTurned = 0;
        }
    }

}
