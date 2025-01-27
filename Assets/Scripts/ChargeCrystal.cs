using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ChargeCrystal : ChargeSource
{
    [Tooltip("The source this should charge when charged")]
    [SerializeField] ChargeSource nextSource;

    [Tooltip("Time it takes for the light to die out when uncharged")]
    [SerializeField] float dieTime;
    float timer;

    [Tooltip("Should this be on by default?")]
    [SerializeField] bool startOn;

    private void Start()
    {
        if (startOn)
        {
            isLit = true;
            timer = dieTime;
            beamObject.SetActive(true);
            if (darkObject != null)
                darkObject.SetActive(false);
            if (nextSource != null)
                nextSource.Charge();
        } else
        {
            isLit = false;
            beamObject.SetActive(false);
            if (darkObject != null)
                darkObject.SetActive(true);
        }
    }

    private void Update()
    {
        if (!isLit && timer > 0)
        {
            timer -= Time.deltaTime;
            //Add color changing
            if (timer <= 0)
            {
                beamObject.SetActive(false);
                if (darkObject != null)
                    darkObject.SetActive(true);
                if (nextSource != null)
                    nextSource.Uncharge();
                Debug.Log($"{name} is now unlit.");
            }
        }
    }

    public new void Charge()
    {
        if (!isLit)
        {
            timer = dieTime;
            isLit = true;
            beamObject.SetActive(true);
            if (darkObject != null)
                darkObject.SetActive(false);
            Debug.Log($"{name} is now lit up.");
            if (nextSource != null)
                nextSource.Charge();
        }
    }

    public new void Uncharge()
    {
        if (isLit)
        {
            isLit = false;
            Debug.Log($"{name} is dying.");
        }
    }
}
