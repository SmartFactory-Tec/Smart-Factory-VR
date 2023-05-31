using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR;
using UnityEngine.XR.OpenXR;
//using Valve.VR;

/*public class ViveControllerScript : MonoBehaviour
{
    // Start is called before the first frame update
    public Vector3 pointedPosition;


    // Update is called once per frame
    void Update()
    {
        if (SteamVR_Input.GetStateDown("Trigger", SteamVR_Input_Sources.Any))
        {
            RaycastHit hit;

            //Get position and direciton of the controller
            var controller = GetComponent<SteamVR_Behaviour_Pose>();
            var position = controller.transform.position;
            var direction = controller.transform.forward;

            //Perform a raycast to find the object or point of intersection
            if (Physics.Raycast(position, direction, out hit))
            {
                //Save the pointed position
                pointedPosition = hit.point;
                Debug.Log(pointedPosition);
            }
        }
    }
} */


public class ViveControllerScript : MonoBehaviour {
    //Varibles
    private InputDevice controllerDevice;
    private bool isTriggerPressed = false;
    private bool errorMess = true;

    private void Start()
    {
        //Find controller input device
        InputDevices.deviceConnected += OnDeviceConnected;
    }

    private void OnDeviceConnected(InputDevice device) {
        if (device.characteristics.HasFlag(InputDeviceCharacteristics.Controller))
        {
            controllerDevice = device;
        }
    }

    private void Update()
    {
        if (controllerDevice.isValid)
        {
            //Get the position and rotation of the controller
            Vector3 controllerPosition;
            Quaternion controllerRotation;

            if (controllerDevice.TryGetFeatureValue(CommonUsages.devicePosition, out controllerPosition)
                && controllerDevice.TryGetFeatureValue(CommonUsages.deviceRotation, out controllerRotation))
            {
                //Convert the controller pose to world space
                Vector3 worldPosition = transform.TransformPoint(controllerPosition);
                Vector3 worldDiretion = transform.TransformDirection(controllerRotation * Vector3.forward);

                //Check if the trigger is pressed
                bool triggerValue;
                if (controllerDevice.TryGetFeatureValue(CommonUsages.triggerButton, out triggerValue))
                {
                    if (triggerValue && !isTriggerPressed)
                    {
                        isTriggerPressed = true;

                        //Perform a raycast from the controllers position and direction
                        RaycastHit hit;

                        if (Physics.Raycast(worldPosition, worldDiretion, out hit))
                        {
                            //Use the hit.point as the position where the controller is pointing
                            Vector3 pointedPosition = hit.point;

                            //Use the position of the position for desired purposes
                            Debug.Log(pointedPosition);
                        }
                    }
                    else if (!triggerValue && isTriggerPressed)
                    {
                        isTriggerPressed = false;
                    }
                }
            }
        }
        else 
        {
            if (errorMess)
            {
                errorMess = false;
                Debug.Log("No controller deteced");
            }
        }
    }
}