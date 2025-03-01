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
    public string text;
    public bool dissapear;
    public bool forCreature;
    public float dissapearTimer;
    private float time;
    public GameObject box;
        // Start is called before the first frame update
    public void Start()
    {
        pause = GameObject.Find("Pause Menu").GetComponent<PauseMenuManager>();
    }

    public void Update()
    {
        if (time > 0)
        {
            time -= Time.deltaTime;
            if (time < 0)
            {
                popUp.text = null;
                this.gameObject.SetActive(false);
                box.SetActive(false);
            }
        }
    }

    public void OnTriggerEnter(Collider other)
    {
        if (pause.isPaused == false)
        {
            if (forCreature == false)
            {
                if (other.transform.tag == "Player")
                {
                    popUp.gameObject.SetActive(true);
                    box.SetActive(true);
                    popUp.text = text;
                    if (dissapear == true)
                    {
                        time = dissapearTimer;
                    }
                }
            }
            else if (forCreature == true)
            {
                if (other.CompareTag("Creature"))
                {
                    popUp.gameObject.SetActive(true);
                    box.SetActive(true);
                    popUp.text = text;
                    if (dissapear == true)
                    {
                        time = dissapearTimer;
                    }
                }
            }
        }
    }
    public void OnTriggerExit(Collider other)
    {
        if (forCreature == false)
        {
            if (other.transform.tag == "Player" && dissapear == false)
            {
                popUp.gameObject.SetActive(false);
                box.SetActive(false);
                popUp.text = null;
                time = 0;
            }
        }
        else if  (forCreature == true)
        {
            if (other.CompareTag("Creature") && dissapear == false)
            {
                popUp.gameObject.SetActive(false);
                box.SetActive(false);
                popUp.text = null;
                time = 0;
            }
        }
    }

    public void EnableTrigger()
    {
        this.gameObject.SetActive(true);
    }

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
}
