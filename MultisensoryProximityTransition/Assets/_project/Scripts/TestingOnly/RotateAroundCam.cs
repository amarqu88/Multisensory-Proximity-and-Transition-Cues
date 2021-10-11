using System.Collections;
using System.Collections.Generic;
using UnityEngine;
//using Sirenix.OdinInspector;
using System;

public class RotateAroundCam : MonoBehaviour
{
    public event TrialCompleteDelegate TrialCompleteEvent;
    public delegate void TrialCompleteDelegate();

    public TargetMeshChecker targetMeshChecker;
    public FeedbackManager fm;
    public GameObject targetObject;

    public float radius = 5f;
    public float timeInView = 3;
    public float timeOutOfView = 3;
    public float maxWaitingDegree = 90;

//    [ReadOnly]
    public float velocity;
//    [ReadOnly]
    public float timeInTransition;
//    [ReadOnly]
    public float startEndDegree;
//    [ReadOnly]
    public float waitBeforeRotate;

    bool firstRun = true;

    public TargetMeshChecker.FovType fov00;
    public TargetMeshChecker.FovType fov01;

//    [Sirenix.OdinInspector.ReadOnly]
    public float secondsBetween0;

    public TargetMeshChecker.FovType fov10;
    public TargetMeshChecker.FovType fov11;

//   [Sirenix.OdinInspector.ReadOnly]
    public float secondsBetween1;

    bool shouldRotate = false;
    bool inTransition = false;
    bool inTransitionStarted = false;
    bool inTransitionFinished = false;
    bool outTransitionStarted = false;
    bool outTransitionFinished = false;
    bool canStartOutgoingTransition = false;
    bool transitionsDone = false;
    bool outerFeedbackAllowed = false;


    bool outerFeedbackRunning = false;
    bool canStartOutgoingOuterFeedback = false;

    Vector3 origin;
    FeedbackDirection direction = FeedbackDirection.L2R;
    List<TargetMeshChecker.FovType> touchingOrder;
    float degree;

    public PerformanceStudy pc;

    private void OnDrawGizmos()
    {
        checkValues();
    }

    private void OnValidate()
    {
        checkValues();
    }

    void checkValues()
    {
        degree = targetMeshChecker.simulatedFrustumDegree.x;
        velocity = CircularMovingHelper.degreePerDuration(degree, timeInView);

        startEndDegree = degree / 2f + CircularMovingHelper.degreeFromPerDuration(timeOutOfView, velocity);

        waitBeforeRotate = (startEndDegree - maxWaitingDegree) / velocity;

        timeInTransition = targetMeshChecker.getHorizontalDistanceDegree(TargetChecker.FovType.TRANSITION_START_IN, TargetChecker.FovType.TRANSITION_STOP_IN);
        timeInTransition = CircularMovingHelper.timeForDegreeAndSpeed(
            timeInTransition,
            velocity);

        if (targetMeshChecker)
        {
            secondsBetween0 = CircularMovingHelper.timeForDegreeAndSpeed(targetMeshChecker.getHorizontalDistanceDegree(fov00, fov01), velocity);
            secondsBetween1 = CircularMovingHelper.timeForDegreeAndSpeed(targetMeshChecker.getHorizontalDistanceDegree(fov10, fov11), velocity);
        }
    }
    
    // Update is called once per frame
    void Update()
    {
        if (shouldRotate)
        {
            doTransitionFeedback();
            calcTargetInformation();
            doOuterFeedback();
            rotateTarget();
        }
    }

    void calcTargetInformation()
    {
        TargetInformation ti = new TargetInformation();

        if(Camera.main.WorldToViewportPoint(targetObject.transform.position).x > 0.5)
            ti.isLeft = false;
        else
            ti.isLeft = true;

        float degree = 0;
        
        degree = Quaternion.LookRotation(targetObject.transform.localPosition, Vector3.up).eulerAngles.y;
        if (degree > 180)
            degree -= 360f;

    
        ti.distanceDegree = new Vector3(0, degree, 0);
        fm.setTargetInformation(ti);
    }

    void doOuterFeedback()
    {
        if(targetMeshChecker.isTouching(targetObject, TargetChecker.FovType.OUTER_ALLOWED)){
            if (!inTransitionStarted && !outerFeedbackRunning)
            {
                outerFeedbackAllowed = true;
                outerFeedbackRunning = true;
                startOuter();
            }
            if (outTransitionStarted && outerFeedbackRunning)
            {
                outerFeedbackAllowed = false;
                outerFeedbackRunning = false;
                stopOuter();
            }
        }

        if (
            (direction == FeedbackDirection.L2R && Camera.main.WorldToViewportPoint(targetObject.transform.position).x > 0.5) ||
            (direction == FeedbackDirection.R2L && Camera.main.WorldToViewportPoint(targetObject.transform.position).x < 0.5)
            )
        {
            if (!outerFeedbackRunning && inTransitionStarted)
            {
                canStartOutgoingOuterFeedback = true;
            }
        }

        if (targetMeshChecker.isTouching(targetObject, TargetChecker.FovType.OUTER_STOP) && outerFeedbackRunning && !canStartOutgoingOuterFeedback)
        {
            stopOuter();
            outerFeedbackRunning = false;
        }
        if(targetMeshChecker.isTouching(targetObject, TargetChecker.FovType.OUTER_START) && !outerFeedbackRunning && canStartOutgoingOuterFeedback && outerFeedbackAllowed)
        {
            startOuter();
            
            outerFeedbackRunning = true;
            StartCoroutine(stopAfterOut(timeOutOfView));
        }
    }

    void doTransitionFeedback()
    {
        Vector3 viewportCoordinates = Camera.main.WorldToViewportPoint(targetObject.transform.position);
        if ( inTransitionFinished && (
            (direction == FeedbackDirection.L2R && viewportCoordinates.x > 0.5) ||
            (direction == FeedbackDirection.R2L && viewportCoordinates.x < 0.5))
            )
        {
            canStartOutgoingTransition = true;
        }
        if (!transitionsDone && targetMeshChecker.isTouching(targetObject, touchingOrder[0]))
        {
            TargetChecker.FovType fovType = touchingOrder[0];
            switch (fovType)
            {
                case (TargetChecker.FovType.TRANSITION_START_IN):
                    if (!inTransition)
                    {
                        if (!inTransitionStarted && !inTransitionFinished)
                        {
                            startInTransition();
                            inTransition = true;
                            inTransitionStarted = true;
                            touchingOrder.RemoveAt(0);
                        }
                    }
                    break;
                case (TargetChecker.FovType.TRANSITION_STOP_IN):
                    if (inTransition)
                    {
                        if (inTransitionStarted && !inTransitionFinished)
                        {
                            stopTransition();
                            inTransition = false;
                            inTransitionFinished = true;
                            touchingOrder.RemoveAt(0);
                        }
                    }
                    break;
                case (TargetChecker.FovType.TRANSITION_START_OUT):
                    if (!outTransitionStarted && !outTransitionFinished && inTransitionFinished && canStartOutgoingTransition)
                    {
                        startOutTransition();
                        outTransitionStarted = true;
                        inTransition = true;
                        touchingOrder.RemoveAt(0);
                    }
                    break;
                case (TargetChecker.FovType.TRANSITION_STOP_OUT):
                    if (outTransitionStarted)
                    {
                        stopTransition();
                        inTransition = false;
                        outTransitionFinished = true;
                        transitionsDone = true;
                        touchingOrder.RemoveAt(0);
                    }
                    break;
            }
        }
    }

    void startOutTransition()
    {
        fm.outTransition();
    }

    void startInTransition()
    {
        fm.inTransition();
    }

    void stopTransition()
    {
        fm.stopInTransition();
    }

    void stopOuter()
    {
        Debug.Log("Stopouter");
        fm.stopOuterTransition();
    }

    void startOuter()
    {
        Debug.Log("StartOuter");
        fm.startOuterTransition();
    }

    void rotateTarget()
    {
        targetObject.transform.RotateAround(origin, Camera.main.transform.up, Time.deltaTime * velocity * ((int)direction > 0 ? -1 : 1) );
        if (Vector3.Distance(Vector3.zero, targetObject.transform.localPosition) != radius)
        {
            targetObject.transform.localPosition = targetObject.transform.localPosition.normalized * radius;
        }
    }

    void setUpRotation()
    {
        touchingOrder = new List<TargetChecker.FovType>();
        touchingOrder.Add(TargetChecker.FovType.TRANSITION_START_IN);
        touchingOrder.Add(TargetChecker.FovType.TRANSITION_STOP_IN);
        touchingOrder.Add(TargetChecker.FovType.TRANSITION_START_OUT);
        touchingOrder.Add(TargetChecker.FovType.TRANSITION_STOP_OUT);
    }

    void setStartPosition()
    {

        origin = Camera.main.transform.position;
        transform.position = origin;
        targetObject.transform.position = origin + Camera.main.transform.forward * radius;

        degree = targetMeshChecker.simulatedFrustumDegree.x;
        velocity = CircularMovingHelper.degreePerDuration(degree, timeInView);

        startEndDegree = degree / 2f + CircularMovingHelper.degreeFromPerDuration(timeOutOfView, velocity);


        targetObject.transform.RotateAround(origin, Camera.main.transform.up, ((int)direction > 0 ? 1 : -1)*startEndDegree);
        timeInTransition = targetMeshChecker.getHorizontalDistanceDegree(TargetChecker.FovType.TRANSITION_START_IN, TargetChecker.FovType.TRANSITION_STOP_IN);
        timeInTransition = CircularMovingHelper.timeForDegreeAndSpeed(
            timeInTransition,
            velocity);
    }

    public void setUpTrial()
    {
        direction = (FeedbackDirection)UnityEngine.Random.Range(0, 2);
        fm.preprareFeedback(direction);
        resetFlags();
        setStartPosition();
        setUpRotation();
    }

    public void startTrial()
    {
        shouldRotate = true;
        calcTargetInformation();
    }

    void resetFlags()
    {
        inTransition = false;
        inTransitionStarted = false;
        inTransitionFinished = false;
        outTransitionStarted = false;
        outTransitionFinished = false;
        canStartOutgoingTransition = false;
        transitionsDone = false;
        outerFeedbackRunning = false;
        canStartOutgoingOuterFeedback = false;
        outerFeedbackAllowed = false;
    }

    public void stopTrial()
    {
        shouldRotate = false;
        stopOuter();
        fm.disableFeedbacks();
        resetFlags();
    }

    IEnumerator stopAfterOut(float seconds)
    {
        yield return new WaitForSeconds(seconds);
        if (firstRun)
        {
            shouldRotate = false;
            direction = (FeedbackDirection)(((int)direction + 1) % 2);
            fm.preprareFeedback(direction);
            resetFlags();
            setStartPosition();
            setUpRotation();
            startTrial();
            firstRun = !firstRun;
        }
        else
        {
            stopTrial();
            fm.disableFeedbacks();
            firstRun = !firstRun;
            TrialCompleteEvent?.Invoke();
        }
    }

}
