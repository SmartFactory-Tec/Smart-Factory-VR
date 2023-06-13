using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SetRobotTargetTest : MonoBehaviour
{
    public ModBusSocket socket;
    public void Test()
    {
        socket.SetRobotTarget(new Vector2(transform.localPosition.x, transform.localPosition.z));
    }
}

#if UNITY_EDITOR
[UnityEditor.CustomEditor(typeof(SetRobotTargetTest))]
public class SetRobotTargetTestEditor : UnityEditor.Editor
{
    public override void OnInspectorGUI()
    {
        base.OnInspectorGUI();

        if (GUILayout.Button("Test"))
        {
            ((SetRobotTargetTest)target).Test();
        }
    }
}
#endif
