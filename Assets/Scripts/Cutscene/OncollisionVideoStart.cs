using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OncollisionVideoStart : MonoBehaviour
{
public CutsceneManager cutsceneManager;
    private void Start()
    {
        cutsceneManager.PlayCutscene();
    }

    private void OnCollisionEnter(Collision collision)
    {

            cutsceneManager.PlayCutscene();
            Debug.Log("Cutscenetrigger");
    }
}
