using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SwitchScenes : MonoBehaviour
{
    public string SceneName;

    // Update is called once per frame
    void Update()
    {
        if(Input.GetKeyDown("l"))
        {
            SceneManager.LoadScene(SceneName);
        }
    }
}
