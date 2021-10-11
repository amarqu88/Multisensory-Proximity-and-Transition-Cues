using System;
/// <summary>
/// Container Object für Daten zur Sichtbarkeit.
/// </summary>
public class VisibilityStatus
{
    private int touchTransitionInStart = 0;
    private int touchTransitionInStop = 0;
    private int touchTransitionOutStart = 0;
    private int touchTransitionOutStop = 0;
    private int touchOuterStop = 0;
    private int touchOuterStart = 0;
    private int touchSimulatedFOV = 0;
    private int touchOuterAllowed = 0;

    public bool IsTouchingTransitionInStart
    {
        get { return (touchTransitionInStart & 0x1) == 1; }
        set
        {
            touchTransitionInStart <<= 1;
            touchTransitionInStart |= (value == true ? 1 : 0);
        }
    }

    public bool IsTouchingTransitionInStop
    {
        get { return (touchTransitionInStop & 0x1) == 1; }
        set { touchTransitionInStop <<= 1; touchTransitionInStop |= (value == true ? 1 : 0); }
    }

    public bool IsTouchingTransitionOutStart
    {
        get { return (touchTransitionOutStart & 0x1) == 1; }
        set
        {
            touchTransitionOutStart <<= 1;
            touchTransitionOutStart |= (value == true ? 1 : 0);
        }
    }

    public bool IsTouchingTransitionOutStop
    {
        get { return (touchTransitionOutStop & 0x1) == 1; }
        set { touchTransitionOutStop <<= 1; touchTransitionOutStop |= (value == true ? 1 : 0); }
    }

    public bool IsTouchingOuterStop
    {
        get { return (touchOuterStop & 0x1) == 1; }
        set { touchOuterStop <<= 1; touchOuterStop |= (value == true ? 1 : 0); }
    }

    public bool IsTouchingOuterStart
    {
        get { return (touchOuterStart & 0x1) == 1; }
        set { touchOuterStart <<= 1; touchOuterStart |= (value == true ? 1 : 0); }
    }

    public bool IsTouchingSimulatedFOV
    {
        get { return (touchSimulatedFOV & 0x1) == 1; }
        set { touchSimulatedFOV <<= 1; touchSimulatedFOV |= (value == true ? 1 : 0); }
    }

    public bool IsTouchingOuterAllowed
    {
        get { return (touchOuterAllowed & 0x1) == 1; }
        set { touchOuterAllowed <<= 1; touchOuterAllowed |= (value == true ? 1 : 0); }

    }

    public bool IsTouching(TargetChecker.FovType meshType)
    {
        return WasTouching(meshType, 0);
    }

    public void IsTouching(TargetChecker.FovType meshType, bool b)
    {
        switch (meshType)
        {
            case (TargetChecker.FovType.OUTER_START):
                IsTouchingOuterStart = b;
                break;
            case (TargetChecker.FovType.OUTER_STOP):
                IsTouchingOuterStop = b;
                break;
            case (TargetChecker.FovType.TRANSITION_STOP_IN):
                IsTouchingTransitionInStop = b;
                break;
            case (TargetChecker.FovType.TRANSITION_START_IN):
                IsTouchingTransitionInStart = b;
                break;
            case (TargetChecker.FovType.SIMULATED):
                IsTouchingSimulatedFOV = b;
                break;
            case (TargetChecker.FovType.TRANSITION_START_OUT):
                IsTouchingTransitionOutStart = b;
                break;
            case (TargetChecker.FovType.TRANSITION_STOP_OUT):
                IsTouchingTransitionOutStop = b;
                break;
            case (TargetChecker.FovType.OUTER_ALLOWED):
                IsTouchingOuterAllowed = b;
                break;
        }
    }

    public bool WasTouching(TargetChecker.FovType meshType, int i)
    {
        if (i < 0)
            i *= -1;

        int interest = 0;
        switch (meshType)
        {
            case (TargetChecker.FovType.OUTER_START):
                interest = touchOuterStart;
                break;
            case (TargetChecker.FovType.OUTER_STOP):
                interest = touchOuterStop;
                break;
            case (TargetChecker.FovType.TRANSITION_STOP_IN):
                interest = touchTransitionInStop;
                break;
            case (TargetChecker.FovType.TRANSITION_START_IN):
                interest = touchTransitionInStart;
                break;
            case (TargetChecker.FovType.SIMULATED):
                interest = touchSimulatedFOV;
                break;
            case (TargetChecker.FovType.TRANSITION_START_OUT):
                interest = touchTransitionOutStart;
                break;
            case (TargetChecker.FovType.TRANSITION_STOP_OUT):
                interest = touchTransitionOutStop;
                break;
            case (TargetChecker.FovType.OUTER_ALLOWED):
                interest = touchOuterAllowed;
                break;
        }
        return ((interest >> i) & 1) == 1;
    }

    


}
