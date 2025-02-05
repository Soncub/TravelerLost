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


    [Tooltip("Renderer to change material of when on and off")]
    [SerializeField] MeshRenderer crystalMesh;
    [Tooltip("Material to use while off")]
    [SerializeField] Material offMat;
    [Tooltip("Material to use while on")]
    [SerializeField] Material onMat;

    private Color color;

    private void Start()
    {
        //Set default state
        if (startOn)
        {
            isLit = true;
            timer = dieTime;
            beamObject.SetActive(true);
            if (darkObject != null)
                darkObject.SetActive(false);
            if (nextSource != null)
                nextSource.Charge();
            if (crystalMesh != null)
                crystalMesh.material = onMat;
        }
        else
        {
            isLit = false;
            beamObject.SetActive(false);
            if (darkObject != null)
                darkObject.SetActive(true);
            if (crystalMesh != null)
                crystalMesh.material = offMat;
        }
        LineRenderer line = beamObject.GetComponent<LineRenderer>();
        if (line != null)
            color = line.startColor;
    }

    private void Update()
    {
        //If its currently dying, update the timer
        if (!isLit && timer > 0)
        {
            timer -= Time.deltaTime;
            //Change the beam based on remaining time
            LineRenderer line = beamObject.GetComponent<LineRenderer>();
            if (line != null)
            {
                color.a = timer / dieTime;
                line.startColor = color;
                color.a = (timer / dieTime * .5f) + .5f;
                line.endColor = color;
            }
            //When it runs out, kill the light and re-enable the dark
            if (timer <= 0)
            {
                beamObject.SetActive(false);
                if (darkObject != null)
                    darkObject.SetActive(true);
                if (nextSource != null)
                    nextSource.Uncharge();
                Debug.Log($"{name} is now unlit.");
                if (crystalMesh != null)
                    crystalMesh.material = offMat;
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
            if (crystalMesh != null)
                crystalMesh.material = onMat;
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
