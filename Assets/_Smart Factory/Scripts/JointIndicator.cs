using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class JointIndicator : MonoBehaviour
{
    private Vector3 _targetScale;

    public float turnSpeed = 50f;
    
    private void Awake()
    {
        _targetScale = transform.localScale;
    }

    private void Start()
    {
        transform.localScale = Vector3.zero;
        LeanTween.scale(gameObject, _targetScale, 0.5f);
    }

    private void Update()
    {
        transform.Rotate(-Vector3.up * Time.deltaTime * turnSpeed);
    }

    public void StopIndicating()
    {
        LeanTween.scale(gameObject, Vector3.zero, 0.5f).destroyOnComplete = true;
    }
    
}
