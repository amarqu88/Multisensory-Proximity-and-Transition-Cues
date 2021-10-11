using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public abstract class OuterFeedback : MonoBehaviour, Feedback
{
    protected TargetInformation ti;

    public abstract void disableFeedback();
    public abstract void prepareFeedback(FeedbackDirection feedbackDirection);

    public virtual void setTargetInformation(TargetInformation ti)
    {
        this.ti = ti;
    }

    public abstract void stopFeedback();

    public abstract void doOuterfeedback(float f);

    public abstract string printName();
    public abstract string getShortcut();
}
