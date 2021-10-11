using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static PerformanceStudy;

public class XboControllerInput : MonoBehaviour
{
    public PerformanceStudy performanceStudy;
    public PrimaryTask primaryTask;

    float directoryBlocked = 0f;
    float blockDirectoryForSeconds = 0.2f;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {

        if (Input.GetButtonDown("xboxA"))
        {
            if (primaryTask.taskRunning && primaryTask.taskStarted)
                primaryTask.userSubmit();

            performanceStudy.startStudy();
            //print("Rotate " + handType);
        }

        if (Input.GetButtonDown("xboxA") &&
            performanceStudy.performanceStudyState == PerformanceStudyState.BEFORE_ROTATE &&
            (performanceStudy.isTraining || performanceStudy.isSecondTraining))
        {
            performanceStudy.startNextRun();
        }

        directoryBlocked -= Time.deltaTime;
        if(directoryBlocked <= 0)
        {
            if (Input.GetButtonDown("xboxLB"))
            {
                directoryBlocked = blockDirectoryForSeconds;
                performanceStudy.selectDirection(new Vector2(-1, 0));
            }
            else if (Input.GetButtonDown("xboxRB"))
            {
                directoryBlocked = blockDirectoryForSeconds;
                performanceStudy.selectDirection(new Vector2(1, 0));
            }
        }
    }
}
