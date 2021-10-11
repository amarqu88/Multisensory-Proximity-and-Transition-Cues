using System.Collections;
using UnityEngine;
using UnityEngine.XR;

public class InViewChecker : MonoBehaviour
{
    public TargetChecker tc;
    public GameObject target;

    void Update()
    {
        /*
        MeshRenderer mr = target.GetComponent<MeshRenderer>();
        if (tc.insideInnerRegion(target))
        {
            mr.material.color = Color.green;
            //
        }else if(tc.insideOuterRegion(target))
        {
            mr.material.color = Color.yellow;
        }
        else if (tc.insideCamView(target))
        {
            mr.material.color = new Color(.92f, .64f, .2f);
        }
        else
        {
            mr.material.color = Color.red;
        }
        */
    }

}
