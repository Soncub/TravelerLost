using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.Playables;
using UnityEngine.UI;

public class PopUpUI : MonoBehaviour
{
    public TextMeshProUGUI popUp;
    public PauseMenuManager pause;
    // Start is called before the first frame update
    public void Start()
    {
        popUp.gameObject.SetActive(false);
        pause = GameObject.Find("Pause Menu").GetComponent<PauseMenuManager>();
    }

    public void Update()
    {

    }

    public void OnTriggerEnter(Collider other)
    {
        if (pause.isPaused == false)
        { 
            if (other.transform.tag == "Player")
            {
                popUp.gameObject.SetActive(true);
            }
        }
    }
    public void OnTriggerExit(Collider other)
    {
        if (other.transform.tag == "Player")
        {
            popUp.gameObject.SetActive(false);
        }
    }
    /*
    public void PopUpOn(string notification)
    {
        popUp.gameObject.SetActive(true);
        popUp.text = notification;
    }

    public void PopUpOff()
    {
        popUp.gameObject.SetActive(false);
        popUp.text = null;
    }
    */
}
