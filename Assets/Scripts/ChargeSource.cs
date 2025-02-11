using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ChargeSource : MonoBehaviour
{
    [Tooltip("Lightbeam object used when charged")]
    [SerializeField] public GameObject beamObject;
    [Tooltip("Darkness removed when charged")]
    [SerializeField] public GameObject darkObject;
    [HideInInspector] public bool isLit = false;

    public virtual void Charge()
    {
        if (!isLit)
        {
            isLit = true;
            beamObject.SetActive(true);
            if(darkObject != null)
                darkObject.SetActive(false);
            Debug.Log($"{name} is now lit up.");
        }
    }

    public virtual void Uncharge()
    {
        if (isLit)
        {
            isLit = false;
            beamObject.SetActive(false);
            if (darkObject != null)
                darkObject.SetActive(true);
            Debug.Log($"{name} is now unlit.");
        }
    }
}
