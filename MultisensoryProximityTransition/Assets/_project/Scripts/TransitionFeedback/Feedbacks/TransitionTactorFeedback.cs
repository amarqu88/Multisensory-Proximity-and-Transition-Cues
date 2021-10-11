using UnityEngine;

public class TransitionTactorFeedback : TransitionFeedback
{
    public TactorController tactorController;
    FeedbackDirection direction;

    public override void doTransitionalFeedback(float f, FeedbackTransitionType feedbackTransitionType)
    {
        tactorController.emit(1, feedbackTransitionType);
    }

    public override void prepareFeedback(FeedbackDirection feedbackDirection)
    {
        direction = feedbackDirection;
        tactorController.setNextFeedback(feedbackDirection);
        tactorController.prepareController();
    }

    public override void stopFeedback()
    {
        tactorController.emit(0);
    }
    public override string printName()
    {
        return "TransitionTactor";
    }
    public override string getShortcut()
    {
        return "T";
    }

    public override void disableFeedback()
    {
        tactorController.disableController();
    }

}

