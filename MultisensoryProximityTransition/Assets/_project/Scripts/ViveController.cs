using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Valve.VR;

public class ViveController : MonoBehaviour
{
    public SteamVR_Input_Sources handType; // 1
    public SteamVR_Action_Boolean positiveAction; // 2
    public SteamVR_Action_Boolean negativeAction; // 3

    public SteamVR_Action_Boolean trackPadAction; // 3


    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if (GetPositiveActionDown())
        {
            print("Positive " + handType);
        }

        if (GetNegativeActionDown())
        {
            print("Negative " + handType);
        }
    }

    public bool GetPositiveActionDown() // 1
    {
        return positiveAction.GetStateDown(handType);
    }

    public bool GetNegativeActionDown() // 2
    {
        return negativeAction.GetStateDown(handType);
    }

    public void printIt()
    {
        print("Clicked");
    }

    //public Vector2 GetTrackPadAction()
    //{

    //}

}
