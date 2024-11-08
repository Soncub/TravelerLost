using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.Playables;
using UnityEngine.UI;

public class PopUpUI : MonoBehaviour
{
    public TextMeshProUGUI popUp;
    // Start is called before the first frame update
    public void Start()
    {
        popUp.gameObject.SetActive(false);
    }

    public void Update()
    {

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
