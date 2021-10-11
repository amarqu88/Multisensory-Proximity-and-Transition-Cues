using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TransitionVisualFeedback : TransitionFeedback
{
    public TransitionVisualController visualController;


    public override void doTransitionalFeedback(float f, FeedbackTransitionType feedbackTransitionType)
    {
        if (f > 0)
            visualController.emit(0.5f);
        else
            visualController.disableController();
    }

    public override void disableFeedback()
    {
        visualController.disableController();
    }

    public override void prepareFeedback(FeedbackDirection feedbackdirection)
    {
        visualController.prepareController();
    }

    public override void stopFeedback()
    {
        visualController.emit(0);
    }

    public override void setTargetInformation(TargetInformation ti)
    {
        base.setTargetInformation(ti);
        if (ti != null)
            visualController.distanceDegree = ti.distanceDegree;
    }

    public override string printName()
    {
        return "TransitionVisual";
    }

    public override string getShortcut()
    {
        return "V";
    }
}
