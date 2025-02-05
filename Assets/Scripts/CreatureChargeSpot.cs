using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class CreatureChargeSpot : ChargeSource
{
    [Tooltip("The source this should charge when charged")]
    [SerializeField] ChargeSource nextSource;

    private bool creatureInPos;

    [Tooltip("Should this be on by default? (If false, another source needs to lead into it)")]
    [SerializeField] bool startOn;

    [Tooltip("Crystal on creature's back to alter the material of")]
    [SerializeField] MeshRenderer creatureBack;
    [Tooltip("Material for the creature back when not charged")]
    [SerializeField] Material offMat;
    [Tooltip("Material for the creature back when charged")]
    [SerializeField] Material onMat;

    private void Start()
    {
        if (startOn)
        {
            isLit = true;
            beamObject.SetActive(true);
            if (darkObject != null)
                darkObject.SetActive(false);
            if(nextSource != null)
                nextSource.Charge();
        }
        else
        {
            isLit = false;
            beamObject.SetActive(false);
            if (darkObject != null)
                darkObject.SetActive(true);
        }
    }

    public new void Charge()
    {
        if (!isLit)
        {
            isLit = true;
            if (creatureInPos)
            {
                beamObject.SetActive(true);
                if (darkObject != null)
                    darkObject.SetActive(false);
                Debug.Log($"{name} is now lit up.");
                if (nextSource != null)
                    nextSource.Charge();
                creatureBack.material = onMat;
            }
        }
    }

    public new void Uncharge()
    {
        if (isLit)
        {
            isLit = false;
            if (creatureInPos)
            {
                beamObject.SetActive(false);
                if (darkObject != null)
                    darkObject.SetActive(true);
                Debug.Log($"{name} is now unlit.");
                if (nextSource != null)
                    nextSource.Uncharge();
                creatureBack.material = offMat;
            }
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Creature"))
        {
            creatureInPos = true;
            if (isLit)
            {
                beamObject.SetActive(true);
                if (darkObject != null)
                    darkObject.SetActive(false);
                Debug.Log($"{name} is now lit up.");
                nextSource.Charge();
                creatureBack.material = onMat;
            }
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Creature"))
        {
            creatureInPos = true;
            if (!isLit)
            {
                beamObject.SetActive(false);
                if (darkObject != null)
                    darkObject.SetActive(true);
                Debug.Log($"{name} is now unlit.");
                nextSource.Uncharge();
                creatureBack.material = offMat;
            }
        }
    }
}
