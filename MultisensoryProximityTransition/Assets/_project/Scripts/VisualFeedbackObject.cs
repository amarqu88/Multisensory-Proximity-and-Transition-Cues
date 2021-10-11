using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class VisualFeedbackObject : MonoBehaviour
{
    public BorderManager bm;

    MeshRenderer mr;

    public Transform borderTarget;

    private void Start()
    {
        mr = borderTarget.GetComponent<MeshRenderer>();
        mr.enabled = false;
    }

    public void updateBorderTargetPosition(Vector2 relativePosition, bool left,float depth) {
        float minLeft = -.52f, maxLeft = -0.443f, minRight = 0.443f, maxRight = .52f;

        relativePosition.x = Mathf.Clamp(relativePosition.x, 0f, 1f);
        relativePosition.y = Mathf.Clamp(relativePosition.y, 0f, 1f);


        if (borderTarget == null)
            return;

        float start = minLeft;
        if (!left)
        {
            start = minRight;
            // 0.5 -1 => 0-1
            relativePosition.x = Mathf.InverseLerp(0.5f, 1, relativePosition.x);
        }
        else
        {
            relativePosition.x = Mathf.InverseLerp(0f, 0.5f, relativePosition.x);
        }
        float position = start + .077f * relativePosition.x;

        borderTarget.transform.localPosition = new Vector3(
            position,
            0,
            depth
            );
    }

    public void setVisible(bool isVisible)
    {
        bm.setVisible(isVisible);
    }

    public void setTargetVisible(bool isVisible)
    {
        mr.enabled = isVisible;
    }

    public void prepare()
    {
        setTargetVisible(false);
        setVisible(true);
    }
}




