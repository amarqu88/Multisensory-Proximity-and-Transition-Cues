using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static TargetRotator.RotationDirection;

public class TargetFeedback : MonoBehaviour
{
    public TargetMeshChecker targetMeshChecker;
    public FeedbackManager fm;
    public GameObject targetObject;
    public PerformanceStudy pc;
    bool shouldDoFeedback = false;
    bool inTransition = false;
    public bool inTransitionStarted { get; private set; }
    bool inTransitionFinished = false;
    bool outTransitionStarted = false;
    bool outTransitionFinished = false;
    bool canStartOutgoingTransition = false;
    bool transitionsDone = false;
    bool outerFeedbackAllowed = false;


    bool outerFeedbackRunning = false;
    bool canStartOutgoingOuterFeedback = false;
    TargetRotator.RotationDirection direction;
    List<TargetChecker.FovType> touchingOrder;

    private void Update()
    {
        if (shouldDoFeedback)
        {
            doOuterFeedback();
            calcTargetInformation();
            doTransitionFeedback();
        }
    }

    void calcTargetInformation()
    {
        TargetInformation ti = new TargetInformation();

        if (Camera.main.WorldToViewportPoint(targetObject.transform.position).x > 0.5)
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
        if (targetMeshChecker.isTouching(targetObject, TargetChecker.FovType.OUTER_ALLOWED))
        {
            if (!inTransitionStarted && !outerFeedbackRunning)
            {
                outerFeedbackAllowed = true;
                outerFeedbackRunning = true;

                fm.startOuterTransition();
            }
            if (outTransitionStarted && outerFeedbackRunning)
            {
                outerFeedbackAllowed = false;
                outerFeedbackRunning = false;

                fm.stopOuterTransition();
            }
        }

        if (
            (direction == CLOCKWISE && Camera.main.WorldToViewportPoint(targetObject.transform.position).x > 0.5) ||
            (direction == COUNTER_CLOCKWISE && Camera.main.WorldToViewportPoint(targetObject.transform.position).x < 0.5)
            )
        {
            if (!outerFeedbackRunning && inTransitionStarted)
            {
                canStartOutgoingOuterFeedback = true;
            }
        }

        if (targetMeshChecker.isTouching(targetObject, TargetChecker.FovType.OUTER_STOP) && outerFeedbackRunning && !canStartOutgoingOuterFeedback)
        {

            fm.stopOuterTransition();
            outerFeedbackRunning = false;
        }
        if (targetMeshChecker.isTouching(targetObject, TargetChecker.FovType.OUTER_START) && !outerFeedbackRunning && canStartOutgoingOuterFeedback && outerFeedbackAllowed)
        {

            fm.startOuterTransition();

            outerFeedbackRunning = true;
            //StartCoroutine(stopAfterOut(timeOutOfView));
        }
    }

    void doTransitionFeedback()
    {
        Vector3 viewportCoordinates = Camera.main.WorldToViewportPoint(targetObject.transform.position);
        if (inTransitionFinished && (
            (direction == CLOCKWISE && viewportCoordinates.x > 0.5) ||
            (direction == COUNTER_CLOCKWISE && viewportCoordinates.x < 0.5))
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
                            pc.logEntryTime = Time.realtimeSinceStartup;
                            fm.inTransition();
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
                            fm.stopInTransition();
                            inTransition = false;
                            inTransitionFinished = true;
                            touchingOrder.RemoveAt(0);
                        }
                    }
                    break;
                case (TargetChecker.FovType.TRANSITION_START_OUT):
                    if (!outTransitionStarted && !outTransitionFinished && inTransitionFinished && canStartOutgoingTransition)
                    {
                        fm.outTransition();
                        outTransitionStarted = true;
                        inTransition = true;
                        touchingOrder.RemoveAt(0);
                    }
                    break;
                case (TargetChecker.FovType.TRANSITION_STOP_OUT):
                    if (outTransitionStarted)
                    {
                        fm.stopInTransition();
                        inTransition = false;
                        outTransitionFinished = true;
                        transitionsDone = true;
                        touchingOrder.RemoveAt(0);
                    }
                    break;
            }
        }
    }

    public void setFeedback(OuterFeedback of, TransitionFeedback tf, TargetRotator.RotationDirection direction)
    {
        fm.disableFeedbacks();
        fm.outerFeedback = of;
        fm.transitionFeedback = tf;

        if(direction == TargetRotator.RotationDirection.CLOCKWISE)
        {
            fm.preprareFeedback(FeedbackDirection.L2R); 
        }else
        {
            fm.preprareFeedback(FeedbackDirection.R2L);
        }
        this.direction = direction;
    }

    void setUpFeedback()
    {
        resetFlags();
        touchingOrder = new List<TargetChecker.FovType>();
        touchingOrder.Add(TargetChecker.FovType.TRANSITION_START_IN);
        touchingOrder.Add(TargetChecker.FovType.TRANSITION_STOP_IN);
        touchingOrder.Add(TargetChecker.FovType.TRANSITION_START_OUT);
        touchingOrder.Add(TargetChecker.FovType.TRANSITION_STOP_OUT);
    }

    private void Start()
    {
        resetFlags();   
    }

    public void startFeedback()
    {
        setUpFeedback();
        shouldDoFeedback = true;
    }

    public void stopFeedback()
    {
        shouldDoFeedback = false;
        fm.disableFeedbacks();
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
}
