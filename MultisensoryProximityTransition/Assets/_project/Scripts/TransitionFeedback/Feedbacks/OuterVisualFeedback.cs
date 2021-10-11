using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class OuterVisualFeedback : OuterFeedback
{
    public VisualController visualController;
    bool isEnabled = false;
    bool innerDo = false;
    public override void doOuterfeedback(float f)
    {
        if (ti == null || !isEnabled)
            return;

        visualController.distanceDegree = ti.distanceDegree;
        visualController.emit(f);
        innerDo = false;
    }

    private void Update()
    {
        if (ti == null || !isEnabled)
            return;

        if (Mathf.Abs(ti.distanceDegree.y) < 15f && innerDo && isEnabled)
        {
            visualController.emit(0);
            visualController.distanceDegree = ti.distanceDegree;
        }
        innerDo = true;
    }

    public override void setTargetInformation(TargetInformation ti)
    {
        base.setTargetInformation(ti);
        if(ti != null)
            visualController.distanceDegree = ti.distanceDegree;
    }

    public override void prepareFeedback(FeedbackDirection feedbackdirection)
    {
        isEnabled = true;
        visualController.prepareController();
    }

    public override void stopFeedback()
    {
        visualController.emit(0);
        ti = null;
        isEnabled = false;

    }
    public override string printName()
    {
        return "OuterVisual";
    }
    public override string getShortcut()
    {
        return "V";
    }

    public override void disableFeedback()
    {
        isEnabled = false;
        ti = null;
        visualController.disableController();
    }
}
