using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "New Item", menuName = "Item")]
public class Items_ScriptableObject : ScriptableObject {

    public new string name;

    public float wieght;

    public bool PickedUp;

}
