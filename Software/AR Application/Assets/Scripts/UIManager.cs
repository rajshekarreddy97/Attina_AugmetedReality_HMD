using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System;
using System.Diagnostics;
using TMPro;

public class UIManager : MonoBehaviour
{
    public static UIManager Instance;

    private void Awake() 
    {
        if(Instance == null)
        {
            Instance = this;
        }
    }

    private void Update() 
    {

    }

    
    public void ControllerMotionDetected()
    {
        
    }

    public void ControllerDormant()
    {
        
    }
}
