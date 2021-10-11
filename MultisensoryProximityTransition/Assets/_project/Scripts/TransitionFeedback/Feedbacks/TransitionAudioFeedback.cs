using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TransitionAudioFeedback : TransitionFeedback
{
    public AudioController audioController;
    public AudioClip audioClip;

    public override void doTransitionalFeedback(float f, FeedbackTransitionType feedbackTransitionType)
    {
        if (f > 0)
            audioController.emit(1);
        else
        {
            stopFeedback();
            audioController.prepareController();
        }
    }

    public override void prepareFeedback(FeedbackDirection feedbackdirection)
    {
        audioController.setAudioClip(audioClip);
        audioController.prepareController();
        audioController.setLoop(false);
    }
    
    public override void stopFeedback()
    {
        audioController.emit(0);
    }
    public override string printName()
    {
        return "TransitionAudio";
    }
    public override string getShortcut()
    {
        return "A";
    }

    public override void disableFeedback()
    {
        audioController.disableController();
    }
}
