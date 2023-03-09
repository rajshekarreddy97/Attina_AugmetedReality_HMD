using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ControllerManager : MonoBehaviour
{
    public static ControllerManager Instance;

    public Vector3 rotationOffset ;
    public float speedFactor = 15.0f;
    public string imuName = "r";        // You should ignore this if there is one IMU.
    public Transform referenceObject;
    public Transform head;
    public Transform arCameraRig;
    public Transform virtualContent;
    public bool isMoving;
    public GameObject laserBeam;
    public bool enableMotionDetection = false;

    private bool isCalibrationTimerRunning = false;
    private float calibrationTimer = 3.0f;
    private Vector3 oldEulerAngles;
    private Transform objectTransfom;
    private int controllerDormantCounter = 0;

    private float w = 0.1f;
    private float z = 0.1f;
    private float y = 0.1f;
    private float x = 0.1f;

    private void Awake() 
    {
        if (Instance == null)
        {
            Instance = this;
        }

        objectTransfom = this.transform;
    }

    private void Start() 
    {
        StartCoroutine(CalibrateSystems());   // For Debugging
        StartCoroutine(UpdateTransform());
        if(enableMotionDetection)
            StartCoroutine(MotionDetection());
    }

    private void Update() 
    {
        if(isCalibrationTimerRunning)
        {
            calibrationTimer -= Time.deltaTime;
            if(calibrationTimer <= 0)
            {
                StartCoroutine(CalibrateSystems());
                calibrationTimer = 3.0f;
            }
        }
        else
        {
            calibrationTimer = 3.0f;
        }
    }

    private IEnumerator UpdateTransform()
    {
        while(true)
        {
            this.transform.localRotation = Quaternion.Lerp(this.transform.localRotation,  new Quaternion(w, y, -x, z), Time.deltaTime * speedFactor);
            yield return new WaitForSeconds(0.001f);
        }
    }

    private IEnumerator CalibrateSystems()
    {
        yield return new WaitForSeconds(4.0f);    // Put zero when not debugging

        // Controller Calibration
        Vector3 direction = referenceObject.position - transform.position;
        Quaternion lookRotation = Quaternion.LookRotation(direction);
        lookRotation = lookRotation * Quaternion.Inverse(transform.rotation);
        lookRotation = lookRotation * transform.parent.rotation;
        Vector3 newRotation = lookRotation.eulerAngles;
        newRotation.y += 180;
        transform.parent.eulerAngles = newRotation;

        // AR View Calibration
        Vector3 direction1 = virtualContent.position - head.position;
        Quaternion lookRotation1 = Quaternion.LookRotation(direction1);
        lookRotation1 = lookRotation1 * Quaternion.Inverse(head.rotation);
        lookRotation1 = lookRotation1 * arCameraRig.rotation;
        Vector3 newRotation1 = lookRotation1.eulerAngles;
        arCameraRig.eulerAngles = newRotation1;

        Debug.Log("System Callibrated");
    }

    public void ReadIMU (string data) 
    {
        // Debug.Log(data);
        string[] values = data.Split('/');
        if (values.Length == 5 && values[0] == imuName) // Rotation of the first one 
        {
            LaserPointer.Instance.isButtonOnYourRemotePressed = false;

            // reset timer
            if(isCalibrationTimerRunning)
            {
                isCalibrationTimerRunning = false;
            }

            w = float.Parse(values[1]);
            z = float.Parse(values[2]);
            y = float.Parse(values[3]);
            x = float.Parse(values[4]);
        }
        else if(values[0] == "button")
        {
            LaserPointer.Instance.isButtonOnYourRemotePressed = true;
            controllerDormantCounter = 0;
            if(!isCalibrationTimerRunning)
            {
                isCalibrationTimerRunning = true;
            }
        } 
        else if (values.Length != 5)
        {
            Debug.LogWarning(data);
        }
    }

    private IEnumerator MotionDetection()
    {
        if (Mathf.Abs(oldEulerAngles.x - objectTransfom.rotation.eulerAngles.x) < 1.0f)
        {
            controllerDormantCounter++;
            if (controllerDormantCounter == 4)
            {
                laserBeam.SetActive(false);
            }
        } 
        else
        {
            oldEulerAngles = objectTransfom.rotation.eulerAngles;
            controllerDormantCounter = 0;
            laserBeam.SetActive(true);
        }

        yield return new WaitForSeconds(1.0f);
        StartCoroutine(MotionDetection());
    }
}