using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// Holds the model data for the player character

// Used to switch between the games player character models,
// and given them player colour to a respective part of their model
public class CorpseData : MonoBehaviour
{
    public GameObject dogModel;
    public GameObject catModel;

    public MeshRenderer dogRender;
    public MeshRenderer catRender;
}
