using UnityEngine;
using UnityEngine.XR.OpenXR;
using UnityEngine.XR;

public class ViveControllerScript : MonoBehaviour
{

    private XRNode controllerNode;
    public GameObject floor;
    public ModBusSocket socket;
    private void Start()
    {
        controllerNode = XRNode.LeftHand;
    }

    private void Update()
    {

        InputDevices.GetDeviceAtXRNode(controllerNode).TryGetFeatureValue(CommonUsages.triggerButton, out bool triggerValue);

        if (triggerValue)
        {
            Vector3 controllerPosition;
            Quaternion controllerRotation;

            InputDevices.GetDeviceAtXRNode(controllerNode).TryGetFeatureValue(CommonUsages.devicePosition, out controllerPosition);
            InputDevices.GetDeviceAtXRNode(controllerNode).TryGetFeatureValue(CommonUsages.deviceRotation, out controllerRotation);

            RaycastHit hit;

            if (Physics.Raycast(controllerPosition, controllerRotation * Vector3.forward, out hit))
            {
                Vector3 hitPosition = hit.point;
                //hitPosition.x -= 3.45f;
                //hitPosition.z -= 2.13f;

                Vector3 hitPositionFloor = floor.transform.InverseTransformPoint(hitPosition);

                //Action
                Debug.Log("Hit Position: " + hitPosition);
                socket.SetRobotTarget(new Vector2(hitPosition.x, hitPosition.z));

            }
        }
    }
}