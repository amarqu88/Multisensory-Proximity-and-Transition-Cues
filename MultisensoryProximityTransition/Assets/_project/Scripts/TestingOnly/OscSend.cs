using System.Collections;
using System.Collections.Generic;
using UnityEngine;
//using Sirenix.OdinInspector;
using extOSC;

public class OscSend : MonoBehaviour
{
    public OSCTransmitter transmitter;

    public int motors;

    OSCValue[] values;
    // Start is called before the first frame update
    void Start()
    {
        
    }
    
//    [Button]
    void SendValues()
    {
        var message = new OSCMessage("/actuators");
        //message.AddValue(OSCValue.Array(values));
        message.AddValue(OSCValue.Array(OSCValue.Int(4), OSCValue.Int(motors), OSCValue.Int(0), OSCValue.Int(motors), OSCValue.Int(1), OSCValue.Int(motors), OSCValue.Int(2), OSCValue.Int(motors), OSCValue.Int(3), OSCValue.Int(motors)));
        //transmitter.Connect();
        transmitter.Send(message);
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
