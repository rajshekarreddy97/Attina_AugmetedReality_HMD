using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class LaserPointer : MonoBehaviour
{
    public static LaserPointer Instance;

    public Transform yourRemoteTransform;
    public bool isButtonOnYourRemotePressed;

    private void Awake() 
    {
        if (Instance == null)
        {
            Instance = this;
        }
    }

    void Update()
    {
        CurvedUIInputModule.CustomControllerRay = new Ray(yourRemoteTransform.position, yourRemoteTransform.forward);
        CurvedUIInputModule.CustomControllerButtonState = isButtonOnYourRemotePressed;
    }
}
