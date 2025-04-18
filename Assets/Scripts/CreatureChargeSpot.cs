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

    [Tooltip("Crystals on creature's back to alter the material of")]
    [SerializeField] SkinnedMeshRenderer creatureMats;
    [Tooltip("Material for the creature back when not charged")]
    [SerializeField] Material[] offMat;
    [Tooltip("Material for the creature back when charged")]
    [SerializeField] Material[] onMat;

    private void Start()
    {
        if (startOn)
            isLit = true;
        else
            isLit = false;
    }

    public override void Charge()
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
                for (int i = 0; i < 5; i++)
                    creatureMats.materials[i+2] = onMat[i];
            }
        }
    }

    public override void Uncharge()
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
                for (int i = 0; i < 5; i++)
                    creatureMats.materials[i+2] = offMat[i];
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
                for (int i = 0; i < 5; i++)
                    creatureMats.materials[i+2] = onMat[i];
            }
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Creature"))
        {
            creatureInPos = true;
            if (isLit)
            {
                beamObject.SetActive(false);
                if (darkObject != null)
                    darkObject.SetActive(true);
                Debug.Log($"{name} is now unlit.");
                nextSource.Uncharge();
                for (int i = 0; i < 5; i++)
                    creatureMats.materials[i+2] = offMat[i];
            }
        }
    }
}
