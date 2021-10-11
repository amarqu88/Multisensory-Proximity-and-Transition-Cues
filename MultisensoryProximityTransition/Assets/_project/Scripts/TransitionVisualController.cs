using UnityEngine;

public class TransitionVisualController : FeedbackController
{
    public GameObject borderTarget;
    public VisualFeedbackObject visualFeedbackObject;
    MeshRenderer mr;
    Color originalColor;

    public Vector2 distanceDegree;

    public override void disableController()
    {
        mr.material.color = originalColor;
        mr.enabled = false;
        visualFeedbackObject.setVisible(false);
    }

    public override void emit(float intensity)
    {
        if (intensity > 0)
        {
            mr.enabled = true;
            mr.material.color = Color.red;// originalColor - new Color(1, 0, 0, 0);
            Vector2 relativeDistance = new Vector2(
               distanceDegree.y,
               0.5f // Da aktuell vertikal ignoriert wird.
               );
            if (distanceDegree.y < 0f)
            {
                relativeDistance.x = 0.49f;

            }
            else if (distanceDegree.y >= 0)
            {
                relativeDistance.x = 0.51f;
            }
            visualFeedbackObject.updateBorderTargetPosition(relativeDistance, distanceDegree.y < 0 ? true : false, .002f);
        }
        else
        {
            mr.enabled = false;
        }
    }

    private void Update()
    {
        if(mr != null && !mr.enabled)
            visualFeedbackObject.updateBorderTargetPosition(distanceDegree, distanceDegree.y < 0 ? true : false, .002f);
    }

    public override void prepareController()
    {
        mr = borderTarget.GetComponent<MeshRenderer>();
        mr.enabled = false;
        visualFeedbackObject.setVisible(true);
        originalColor = mr.material.color;
    }
}
