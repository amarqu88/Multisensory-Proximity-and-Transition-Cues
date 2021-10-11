using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class VisualController : FeedbackController
{
    public VisualFeedbackObject visualObject;
    public float maxDegree;
    public Vector2 distanceDegree;
    bool isEnabled = false;

    public override void emit(float intensity)
    {
        if (!isEnabled)
            return;

        if (intensity > 0)
        {
            visualObject.setTargetVisible(true);

            // distanceDegree enthält die Angabe dazu wie weit das Ziel in Grad vom Nutzer entfernt ist.            
            // Wert kann zwischen -maxDegree und maxDegree sein.
            // Bei werten zwischen (-maxDegree und -Fov/2) und (Fov/2 und maxDegree) soll es zwischen 0 und 1 interpoliert werden
            // Ist der Wert negativ -> linke Seite sonst rechte
            // Ist es beim stopcollider angekommen wird die intensity auf 0 gesetzt.
            // Ist es beim startcollider angekommen wird die intensity auf 1 gesetzt
            Vector2 relativeDistance = new Vector2(
               distanceDegree.y,
               0.5f // Da aktuell vertikal ignoriert wird.
               );
            if (distanceDegree.y <= 15f)
            {
                relativeDistance.x = 0f + ((0.5f - 0f) / (-15f - -maxDegree)) * (distanceDegree.y - -maxDegree);

            }
            else if(distanceDegree.y >= 15f)
            {
                relativeDistance.x = 0.5f + ((1f - 0.5f) / (maxDegree - 15f)) * (distanceDegree.y - 15f);
            }
            visualObject.updateBorderTargetPosition(relativeDistance, distanceDegree.y < 0 ? true : false, .0025f);

            
        }else
        {
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
            visualObject.updateBorderTargetPosition(relativeDistance, distanceDegree.y < 0 ? true : false, .0025f);

            visualObject.setTargetVisible(false);
        }
    }

    public override void prepareController()
    {
        visualObject.prepare();
        isEnabled = true;
    }

    public override void disableController()
    {
        isEnabled = false;
        visualObject.setTargetVisible(false);
        visualObject.setVisible(false);
    }
}
