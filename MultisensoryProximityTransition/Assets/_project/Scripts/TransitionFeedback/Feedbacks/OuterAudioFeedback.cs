using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OuterAudioFeedback : OuterFeedback
{
    public AudioController audioController;
    public AudioClip audioClip;

    public override void doOuterfeedback(float f)
    {
        f = Mathf.Max(Mathf.Min(1f, f), 0f);
        audioController.emit(f);
    }

    public override void prepareFeedback(FeedbackDirection feedbackDirection)
    {
        audioController.setAudioClip(audioClip);
        audioController.prepareController();
    }

    public override void stopFeedback()
    {
        audioController.emit(0);
    }
    public override string printName()
    {
        return "OuterAudio";
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
