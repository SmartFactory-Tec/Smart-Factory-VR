using UnityEngine;
using UnityEngine.XR.OpenXR;
using UnityEngine.XR;
using System;

public class ViveControllerScript : MonoBehaviour
{

    private XRNode controllerNode = XRNode.RightHand;
    public Transform robotOrigin;
    public ModBusSocket socket;

    public Transform debug;
    public Transform rightController;

    [NonSerialized] public bool useFreemovement;

    bool lastValue;

    public void Update()
    {
        if (!useFreemovement) return;

        InputDevices.GetDeviceAtXRNode(controllerNode).TryGetFeatureValue(CommonUsages.triggerButton, out bool triggerValue);

        if (triggerValue && !lastValue)
        {
            if (Physics.Raycast(rightController.position, rightController.rotation* Vector3.forward, out var hit))
            {
                Vector3 hitPosition = hit.point;
                //hitPosition.x -= 3.45f;
                //hitPosition.z -= 2.13f;

                debug.position = hitPosition;

                Vector3 hitPositionFloor = robotOrigin.transform.InverseTransformPoint(hitPosition);

                //Action
                Debug.Log("Hit Position: " + hitPosition);
                //socket.SetRobotTarget(new Vector2(hitPositionFloor.x, hitPositionFloor.z));
                Vector2 targetPos = new Vector2(hitPosition.x, hitPosition.z);
                var modbusPos = socket.UnityVector2ToModbus(targetPos);

                socket.missions((ushort)modbusPos.x, (ushort)modbusPos.y, 0);
            }
        }

        lastValue = triggerValue;
    }
}