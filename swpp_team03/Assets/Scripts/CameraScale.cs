using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraScale : MonoBehaviour
{
    // Start is called before the first frame update

    // Update is called once per frame
    void Update()
    {
        
    }

    void Start()
    {
        Camera camera = GetComponent<Camera>();

        float targetAspect = 16f / 9f;
        float screenAspect = (float)Screen.width / Screen.height;

        if (screenAspect >= targetAspect)
        {
            float inset = 1f - targetAspect / screenAspect;
            camera.rect = new Rect(inset / 2f, 0, 1f - inset, 1f);
        }
        else
        {
            float inset = 1f - screenAspect / targetAspect;
            camera.rect = new Rect(0, inset / 2f, 1f, 1f - inset);
        }
    }

}
