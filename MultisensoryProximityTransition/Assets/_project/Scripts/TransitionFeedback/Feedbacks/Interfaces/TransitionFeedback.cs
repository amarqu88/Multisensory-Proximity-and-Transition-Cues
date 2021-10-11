using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class TransitionFeedback : MonoBehaviour, Feedback
{
    protected TargetInformation ti;
    public abstract void prepareFeedback(FeedbackDirection feedbackDirection);
    public abstract void disableFeedback();
    public abstract void stopFeedback();
    public virtual void setTargetInformation(TargetInformation ti)
    {
        this.ti = ti;
    }

    public abstract string printName();
    public abstract string getShortcut();
    public abstract void doTransitionalFeedback(float f, FeedbackTransitionType feedbackTransitionType);
}
