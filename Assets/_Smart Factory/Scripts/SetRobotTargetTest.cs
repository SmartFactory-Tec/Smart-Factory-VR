using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SetRobotTargetTest : MonoBehaviour
{
    public void Trolling()
    {
        FindObjectOfType<ModBusSocket>().SetRobotTarget(new Vector2(transform.localPosition.x, transform.localPosition.z));
    }
}

#if UNITY_EDITOR
[UnityEditor.CustomEditor(typeof(SetRobotTargetTest))]
public class SetRobotTargetTestEditor : UnityEditor.Editor
{
    public override void OnInspectorGUI()
    {
        base.OnInspectorGUI();

        if (GUILayout.Button("Trolling"))
        {
            ((SetRobotTargetTest)target).Trolling();
        }
    }
}
#endif
