using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HeadTracker : MonoBehaviour
{
    public static HeadTracker Instance;

    public bool camGyroActive = false;
    public bool isSystemBeingCalibrated = false;
    public Quaternion camGyroOffset = Quaternion.identity;

    private void Awake()
    {
        if(Instance == null)
        {
            Instance = this;
        }

        // Reference to main GyroController
        if (GyroController.Instance == null)
        {
            Debug.Log("There needs to be a gyroController!");
        }
        // initialize camGyroOffset
        camGyroOffset = Quaternion.identity;
    }

    private void Start()
    {
        camGyroActive = true;
    }

    private void LateUpdate()
    {
        if(!isSystemBeingCalibrated)
        {
            TrackHeadPose();
        }
    }

    private void TrackHeadPose()
    {
        #if UNITY_EDITOR
        if (camGyroActive)
        {
            transform.localRotation = camGyroOffset * GyroController.Instance.gyroRotation;
        }
        #else
        if (camGyroActive)
        {
            transform.localRotation = camGyroOffset * GyroController.Instance.gyroRotation;
        }
        #endif
    }
}
