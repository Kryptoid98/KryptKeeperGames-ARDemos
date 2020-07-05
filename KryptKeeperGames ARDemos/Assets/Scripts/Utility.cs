using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace KryptKeeperGamesARDemo.Utility
{
    public class Utility 
    {
        public static Vector3 FindPointInFrustrum(Camera cam, float xPercent, float yPercent, float zDepth)
        {
            //float x = cam.pixelWidth * xPercent;
            //float y = cam.pixelHeight * yPercent;
            //Ray ray = cam.ScreenPointToRay(new Vector3(x, y, zDepth));

            //return ray.origin + ray.direction * zDepth;

            return cam.ViewportToWorldPoint(new Vector3(xPercent, yPercent, zDepth));
        }
    }

}