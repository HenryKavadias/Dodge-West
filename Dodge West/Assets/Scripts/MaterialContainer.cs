using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "MaterialContainer", menuName = "ScriptableObjects/MaterialContainer")]
public class MaterialContainer : ScriptableObject
{
    public Material[] materials;
}
