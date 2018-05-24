using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//Base Class for objects the player can gaze at
public abstract class GazeObjectUI : MonoBehaviour
{
    public void Awake()
    {
        gameObject.layer = LayerMask.NameToLayer("UI");
    }

    public abstract void SetGazed();
    public abstract void SetNotGazed();
}
