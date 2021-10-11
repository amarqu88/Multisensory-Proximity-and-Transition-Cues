using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Valve.VR;
using static PerformanceStudy;

public class ViveControlPrimary : MonoBehaviour
{
    public SteamVR_Input_Sources handType;
    public SteamVR_Action_Boolean rotationKey;
    public SteamVR_Action_Boolean selectLeftKey;
    public SteamVR_Action_Boolean selectRightKey;
    public SteamVR_Action_Boolean primarySelectKey;

    public PerformanceStudy performanceStudy;
    public PrimaryTask primaryTask;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    public bool GetRotationKey() // 1
    {
        return rotationKey.GetStateDown(handType);
    }

    public bool GetSelectRightKey() // 1
    { 
        return selectRightKey.GetStateDown(handType);
    }

    public bool GetSelectLeftKey() // 2
    {
        return selectLeftKey.GetStateDown(handType);
    }

    public bool GetPrimarySelectKey() // 2
    {
        return primarySelectKey.GetStateDown(handType);
    }


    // Update is called once per frame
    void Update()
    {
        if (GetRotationKey())
        {
            if (primaryTask.taskRunning && primaryTask.taskStarted)
                primaryTask.userSubmit();

            performanceStudy.startStudy();
            //print("Rotate " + handType);
        }

        if (GetRotationKey() && 
            performanceStudy.performanceStudyState == PerformanceStudyState.BEFORE_ROTATE && 
            (performanceStudy.isTraining || performanceStudy.isSecondTraining))
        {
            performanceStudy.startNextRun();
        }

        if (GetSelectRightKey())
        {
            performanceStudy.selectDirection(new Vector2(1, 0));
            //print("Right " + handType);
        }
        if (GetSelectLeftKey())
        {
            performanceStudy.selectDirection(new Vector2(-1, 0));
            //print("Left " + handType);
        }
        if (GetPrimarySelectKey())
        {
            
            //print("Select " + handType);
        }


    }
}
