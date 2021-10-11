using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface Feedback
{
    string printName();
    string getShortcut();
    void prepareFeedback(FeedbackDirection feedbackDirection);
    void stopFeedback();

    void disableFeedback();
}
