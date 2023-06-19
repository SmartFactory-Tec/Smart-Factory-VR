using System;
using System.Collections;
using System.Collections.Generic;
using System.Security.Cryptography;
using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;

public class UR5e_Controller : MonoBehaviour
{
    [Header("Robot Joints")] [SerializeField]
    private RobotJoint[] robotJoints;

    [SerializeField] private GameObject jointIndicator;

    private Dictionary<int, JointIndicator> indicators = new Dictionary<int, JointIndicator>();

    [SerializeField] private Transform buttonParent;

    private void Start()
    {
        return;
        int i = 0;
        foreach (Transform child in buttonParent)
        {
            XRSimpleInteractable[] buttons = child.GetComponentsInChildren<XRSimpleInteractable>();

            foreach(var button in buttons)
            {
                if (button.transform.parent.name.StartsWith("Left"))
                {
                    button.selectEntered.AddListener(delegate { NegativeMoveJoint(i);});
                    button.selectExited.AddListener(delegate { StopJoint(i); });
                }
                else
                {
                    button.selectEntered.AddListener(delegate { PositiveMoveJoint(i); });
                    button.selectExited.AddListener(delegate { StopJoint(i); });
                }

            }

            i++;
        }
    }

    private void Update()
    {
        foreach (RobotJoint joint in robotJoints)
        {
            joint.JointUpdate();
        }
    }

    public void IndicateJointPositive(int joint)
    {
        IndicateJoint(joint, true);
    }

    public void IndicateJointNegative(int joint)
    {
        IndicateJoint(joint, false);
    }

    private void IndicateJoint(int joint, bool positive)
    {
        if (indicators.ContainsKey(joint)) return;

        Debug.Log("Indicating joint " + joint);

        JointIndicator indicator = Instantiate(jointIndicator).GetComponent<JointIndicator>();
        indicator.gameObject.SetActive(true);

        Transform indicatorTransform = indicator.transform;
        indicatorTransform.SetParent(robotJoints[joint].jointTransform);
        indicatorTransform.localPosition = Vector3.zero;
        indicatorTransform.localRotation = Quaternion.identity;

        switch (robotJoints[joint].axis)
        {
            case Axis.X:
                break;
            case Axis.Y:
                break;
            case Axis.Z:
                indicatorTransform.Rotate(Vector3.right, 90);
                break;
        }

        if (positive) indicatorTransform.Rotate(Vector3.right, 180f);

        indicator.turnSpeed = 30f;

        indicators.Add(joint, indicator);
    }

    public void StopIndicatingJoint(int joint)
    {
        if (!indicators.ContainsKey(joint)) return;

        indicators[joint].StopIndicating();
        indicators.Remove(joint);
    }


    public void PositiveMoveJoint(int joint)
    {
        robotJoints[joint].moveValue = 1f;
    }

    public void NegativeMoveJoint(int joint)
    {
        Debug.Log("Negative move joint " + joint);
        robotJoints[joint].moveValue = -1f;
    }

    public void StopJoint(int joint)
    {
        robotJoints[joint].moveValue = 0;
    }

    [System.Serializable]
    private class RobotJoint
    {
        public Joint joint = Joint.Base;
        public Axis axis = Axis.X;
        public Transform jointTransform;
        [Range(0f, 50f)] public float turnRate = 15f;
        [Range(0, 360)] public float minAngle;
        [Range(0, 360)] public float maxAngle;

        public float moveValue = 0f;

        public void JointUpdate()
        {
            if (moveValue == 0) return;

            Vector3 rotDirection = axises[(int) axis];
            jointTransform.Rotate(rotDirection, Time.deltaTime * moveValue * turnRate, Space.Self);

            Vector3 rotation = jointTransform.localEulerAngles;
            switch (axis)
            {
                case Axis.X:
                    rotation.x = Mathf.Clamp(rotation.x, minAngle, maxAngle);
                    break;
                case Axis.Y:
                    rotation.y = Mathf.Clamp(rotation.y, minAngle, maxAngle);
                    break;
                case Axis.Z:
                    rotation.z = Mathf.Clamp(rotation.z, minAngle, maxAngle);
                    break;
            }

            jointTransform.localEulerAngles = rotation;
        }
    }


    enum Axis
    {
        X,
        Y,
        Z
    }

    public static Vector3[] axises = {Vector3.right, Vector3.up, Vector3.forward};

    public enum Joint
    {
        Base,
        Shoulder,
        Elbow,
        Wrist1,
        Wrist2,
        Wrist3
    }
}