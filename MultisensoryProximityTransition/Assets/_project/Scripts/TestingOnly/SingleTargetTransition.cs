using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SingleTargetTransition : MonoBehaviour
{
    public GameObject target;
    public TargetChecker targetChecker;
    public FeedbackManager feedbackManager;

    public bool wasInside = false;


    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        /*
        if(targetChecker.insideCamView(target))
        {
            feedbackManager.stopOuterTransition();
            
            if (!targetChecker.insideOuterRegion(target) && !wasInside)
                feedbackManager.inTransition();
            wasInside = true;

        }
        else
        {
            feedbackManager.startOuterTransition();
            wasInside = false;
        }
        */
    }
}
