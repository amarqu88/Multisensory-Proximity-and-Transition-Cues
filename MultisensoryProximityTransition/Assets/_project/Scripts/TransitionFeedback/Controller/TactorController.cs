using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using extOSC;
using UnityEditor;

public class TactorController : FeedbackController
{
    public string ip = "10.20.105.19";
    public int port = 5005;
    public string topic = "tactors";

    OSCTransmitter transmitter;

    public Tactor left;
    public Tactor right;

    FeedbackDirection direction;


    public override void emit(float intensity)
    {
        OSCMessage m = new OSCMessage(topic);
        m.AddValue(OSCValue.Array(OSCValue.Int(1), OSCValue.Int(0), OSCValue.Int(3), OSCValue.Int(0)));
        transmitter.Send(m);
    }

    public void emit(float intensity, FeedbackTransitionType transitionType)
    {
        int tactor = 1;
        Tactor current = left;
        Debug.Log(transitionType + " " + direction);
        if (transitionType == FeedbackTransitionType.IN && direction == FeedbackDirection.R2L)
        {
            current = right;
            tactor = 3;
        }
        else if(transitionType == FeedbackTransitionType.OUT && direction == FeedbackDirection.L2R)
        {
            current = right;
            tactor = 3;
        }
        current.Frequency = Mathf.RoundToInt(intensity * 100);

        OSCMessage m = new OSCMessage(topic);
        m.AddValue(OSCValue.Array(OSCValue.Int(tactor), OSCValue.Int(Mathf.RoundToInt(intensity * 100))));
        transmitter.Send(m);
    }

    public void setNextFeedback(FeedbackDirection direction)
    {
        this.direction = direction;
    }

    public override void prepareController()
    {
        transmitter = GetComponent<OSCTransmitter>();
        if (transmitter == null)
        {
            transmitter = gameObject.AddComponent<OSCTransmitter>();
        }
        transmitter.RemotePort = port;
        transmitter.RemoteHost = ip;
        transmitter.AutoConnect = true;
        transmitter.Connect();
    }

//    [Sirenix.OdinInspector.Button]
    void logTactors()
    {
        Debug.Log(left.TactorName + "  GPIO: " + left.GPIOPin);
        Debug.Log(right.TactorName + "  GPIO: " + right.GPIOPin);
    }

    public override void disableController()
    {
        emit(0);
    }

}
