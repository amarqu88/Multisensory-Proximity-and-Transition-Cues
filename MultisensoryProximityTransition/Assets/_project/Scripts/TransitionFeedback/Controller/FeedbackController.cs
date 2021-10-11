using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class FeedbackController : MonoBehaviour
{
    public abstract void prepareController();
    public abstract void disableController();
    public abstract void emit(float intensity);
}
