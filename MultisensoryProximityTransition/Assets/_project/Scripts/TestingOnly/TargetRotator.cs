using System.Collections;
using System.Collections.Generic;
using UnityEngine;
//using Sirenix.OdinInspector;

public class TargetRotator : MonoBehaviour
{
    public enum RotationDirection
    {
        CLOCKWISE = 0,
        COUNTER_CLOCKWISE = 1,
    }

    public delegate void RotationDoneDelegate();
    public event RotationDoneDelegate RotationDoneEvent;

    public delegate void RotationStartDelegate();
    public event RotationStartDelegate RotationStartEvent;

    public GameObject targetToRotate;

    public float degreePerSecond;

    public RotationDirection rotationDirection;

    public float rotationYaw;

    private bool isRotating = false;
    private bool shouldStopRotate = false;

    private float targetDistanceMeter;

    private bool isEndlessRotation = true;

    public float degreeLeft { get; private set; }

    public float degreeToRotate { get; private set; }
    private void Update()
    {
        if (isRotating)
        {
            doFrameRotation();
            if (isRotating && shouldStopRotate)
            {
                stopRotation();
            }
        }
        //getMovingDirection();
    }

//    [Button]
    public void startRotation()
    {
        if(targetToRotate == null)
        {
            Debug.LogWarning("No Target to rotate available");
            isRotating = false;
            return;
        }

        // Get Distance to target
        targetDistanceMeter = Vector3.Distance(this.transform.position, targetToRotate.transform.position);
        shouldStopRotate = false;
        isRotating = true;
        RotationStartEvent?.Invoke();
    }

//    [Button]
    public void stopRotation()
    {
        if (isRotating)
        {
            isRotating = false;
            RotationDoneEvent?.Invoke();
        }
    }

    private void doFrameRotation()
    {
        float rotateByDegree = Time.deltaTime * degreePerSecond;
        degreeLeft -= rotateByDegree;

        if(!isEndlessRotation){
            if(degreeLeft < 0)
            {
                rotateByDegree += degreeLeft;
                shouldStopRotate = true;
            }
        }
        doRotation(rotateByDegree);
    }

    private void doRotation(float degree)
    {
        Vector3 rotationAxis = calculateRotationAxis();
        targetToRotate.transform.RotateAround(transform.position, rotationAxis, degree);
        if (Vector3.Distance(Vector3.zero, targetToRotate.transform.localPosition) != targetDistanceMeter)
            targetToRotate.transform.localPosition = targetToRotate.transform.localPosition.normalized * targetDistanceMeter;
    }

    private Vector3 calculateRotationAxis()
    {
        Vector3 rotationAxis = Vector3.zero;
        Vector3 tiltAxis = Vector3.zero;
        switch (rotationDirection)
        {
            case (RotationDirection.CLOCKWISE):
                rotationAxis = transform.up;
                tiltAxis = transform.right;
                break;
            case (RotationDirection.COUNTER_CLOCKWISE):
                rotationAxis = -transform.up;
                tiltAxis = -transform.right;
                break;
        }
        return Vector3.SlerpUnclamped(rotationAxis, tiltAxis, (1f / 90f) * (rotationYaw % 360f));
    }

//    [Button]
    public void setInitialPositionByDegree(float targetDistanceMeter, float degree)
    {
        this.targetDistanceMeter = targetDistanceMeter;
        targetToRotate.transform.position = transform.position + transform.forward * targetDistanceMeter;
        doRotation(degree);
    }

    public void setInitialPositionBySeconds(float targetDistanceMeter, float seconds)
    {
        setInitialPositionByDegree(targetDistanceMeter, seconds * degreePerSecond);
    }

//    [Button]
    public void setFiniteRotationByDegree(float degree)
    {
        isEndlessRotation = false;
        degreeToRotate = degree;
        degreeLeft = degree;
    }

    public Vector3 getMovingDirection()
    {
        Vector3 rotationAxis = calculateRotationAxis();
        Vector3 crossAxis = Vector3.Cross(rotationAxis, transform.forward);

        // Debug.Log(crossAxis);
        // Debug.DrawRay(transform.position + transform.forward, crossAxis);
        return crossAxis;
    }
}
