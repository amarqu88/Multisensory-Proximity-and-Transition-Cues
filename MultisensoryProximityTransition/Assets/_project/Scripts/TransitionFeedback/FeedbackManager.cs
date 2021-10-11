using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
//using Sirenix.OdinInspector;

public class FeedbackManager : MonoBehaviour
{
    public OuterFeedback outerFeedback;

    public TransitionFeedback transitionFeedback;

    public TargetInformation targetInformation;
    bool outerRunning = false;

    private void Update()
    {
        if(outerRunning)
            outerFeedback.doOuterfeedback(1);
    }
    public void setTargetInformation(TargetInformation ti)
    {
        targetInformation = ti;
        outerFeedback.setTargetInformation(ti);
        transitionFeedback.setTargetInformation(ti);
    }


    private void Start()
    {
        //outerFeedback.prepareFeedback(targetInformation.feedbackDirection);
        //transitionFeedback.prepareFeedback(targetInformation.feedbackDirection);
    }

    public void inTransition()
    {
        transitionFeedback.doTransitionalFeedback(1, FeedbackTransitionType.IN);
    }

    public void outTransition()
    {
        transitionFeedback.doTransitionalFeedback(1, FeedbackTransitionType.OUT);
     }

    public void stopInTransition()
    {
        transitionFeedback.stopFeedback();
    }

    public void stopOuterTransition()
    {
        outerRunning = false;
        outerFeedback.stopFeedback();
    }

    public void startOuterTransition()
    {
        outerRunning = true;
        outerFeedback.prepareFeedback(targetInformation.feedbackDirection);
        outerFeedback.doOuterfeedback(1);
    }

    public void preprareFeedback(FeedbackDirection direction)
    {
        outerRunning = false;
        outerFeedback.prepareFeedback(direction);
        transitionFeedback.prepareFeedback(direction);
    }

    public void disableFeedbacks()
    {
        outerRunning = false;
        outerFeedback.disableFeedback();
        transitionFeedback.disableFeedback();
    }
}
