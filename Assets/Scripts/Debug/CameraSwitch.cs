using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraSwitch : MonoBehaviour
{
    // first camera should always be main camera
    // other cameras should be inactive
    public GameObject[] cameras;

    // Update is called once per frame
    void Update()
    {

        // Cycles to next camera in scene
        if(Input.GetKeyDown("c"))
        {
            for(int i = 0; i < cameras.Length; ++i)
            {
                if(cameras[i].activeSelf)
                {
                    cameras[i].SetActive(false);
                    if((i + 1) >= cameras.Length)
                    {
                        cameras[0].SetActive(true);
                    }
                    else
                    {
                        cameras[i + 1].SetActive(true);
                    }
                    return;
                }
            }
        }
    }
}
