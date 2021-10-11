using System.Collections.Generic;
using UnityEngine;
using UnityEngine.VR;
//using Sirenix.OdinInspector;
using System;
/// <summary>
/// Abgeleitete Klassen prüfen ob Targets im Sichtbereich bzw. in Definierten Regionen sind.
/// </summary>
public abstract class TargetChecker : MonoBehaviour
{
    /*  |<---------------------------------------------------------------------------------------------------------->|
     *        |<---------------------------------------------------------------------------------------------->|
     *            |<-------------------------------------------------------------------------------------->|     
     *                |<------------------------------------------------------------------------------>|        
     *                    |<--------------------------------------------------------------------->|              
     *   ____________________________________________________________________________________________________________
     *  |                                                                                                            |
     *  |                                                                                                            |
     *  |      _________________________________________________________________________________________________     |
     *  |     |                                                                                                |     |
     *  |     |                                                                                                |     |
     *  |     |    _________________________________________________________________________________________   |     |
     *  |     |   |                                                                                        |   |     |
     *  |     |   |                                                                                        |   |     |
     *  |     |   |    _________________________________________________________________________________   |   |     |
     *  |     |   |   |                                                                                |   |   |     |
     *  |     |   |   |   _________________________________________________________________________    |   |   |     | 
     *  |     |   |   |   |                                                                       |    |   |   |     |
     *  |     |   |   |   |                                                                       |    |   |   |     |
     *  |     |   |   |   |                                                                       |    |   |   |     |
     *  |     |   |   |   |                                                                       |    |   |   |     |
     *  |     |   |   |   |                                                                       |    |   |   |     |
     *  |     |   |   |   |                                                                       |    |   |   |     |
     *  |     |   |   |   |_______________________________________________________________________|    |   |   |     |
     *  |     |   |   |                                                                                |   |   |     |
     *  |     |   |   |________________________________________________________________________________|   |   |     |
     *  |     |   |                                                                                        |   |     |
     *  |     |   |                                                                                        |   |     |
     *  |     |   |________________________________________________________________________________________|   |     |
     *  |     |                                                                                                |     |
     *  |     |                                                                                                |     |
     *  |     |________________________________________________________________________________________________|     |
     *  |                                                                                                            |
     *  |                                                                                                            |
     *  |____________________________________________________________________________________________________________|
     *  
     */

    public enum FovType {
        SIMULATED = 0,
        TRANSITION_START_IN,
        TRANSITION_STOP_IN,
        OUTER_STOP,
        OUTER_START,
        TRANSITION_START_OUT,
        TRANSITION_STOP_OUT,
        OUTER_ALLOWED
    }
    /// <summary>
    /// Sollen Gizmos im Editor gezeichnet werden?
    /// </summary>
    [Tooltip("Sollen Gizmos im Editor gezeichnet werden?")]
    public bool drawGizmos = true;

    public bool keepAspect = true;

//    [TitleGroup("Simulated FOV")]
    public Vector2 simulatedFrustumDegree = new Vector2(30f, 17.5f);
    public Color simulatedColor = Color.red;
    public bool simulatedGizmoVisible;


//    [TitleGroup("Transition In")]
    public Vector2 transitionInStartFrustumDegree = new Vector2(31f, 18.5f);
    public Vector2 transitionInStopFrustumDegree = new Vector2(29f, 16.5f);
    
    public Color transitionInStartColor = Color.green;
    public Color transitionInStopColor = Color.green * .5f + new Color(0,0,0,1);

    public bool transitionInStartGizmoVisible;
    public bool transitionInStopGizmoVisible;

//    [TitleGroup("Transition Out")]
    public Vector2 transitionOutStartFrustumDegree = new Vector2(31f, 18.5f);
    public Vector2 transitionOutStopFrustumDegree = new Vector2(29f, 16.5f);

    public Color transitionOutStartColor = Color.magenta;
    public Color transitionOutStopColor = Color.magenta * .5f + new Color(0, 0, 0, 1);

    public bool transitionOutStartGizmoVisible;
    public bool transitionOutStopGizmoVisible;

//    [TitleGroup("Outer Feedback")]
    public Vector2 outerFeedbackStopFrustumDegree = new Vector2(30f, 17.5f);
    public Vector2 outerFeedbackStartFrustumDegree = new Vector2(30f, 17.5f);
    public Vector2 outerFeedbackAllowedFrustumDegree = new Vector2(60f, 17.5f);

    public Color outerStopColor = Color.blue;
    public Color outerStartColor = Color.blue * .5f + new Color(0, 0, 0, 1);
    public Color outerAllowedColor = Color.blue * 0.7f + new Color(0, 0, 0, 1);


    public bool outerStartGizmoVisible;
    public bool outerStopGizmoVisible;
    public bool outerAllowedGizmoVisible;
    

    protected FovTypeVector2Dictionary fovSizes = new FovTypeVector2Dictionary();

//    [ReadOnly, ShowInInspector]
    protected GameObjectVisibilityStatusDictionary gameObjectStatus = new GameObjectVisibilityStatusDictionary();

    

    public Vector3[] getFrustumCorners(Vector2 fovDegree, GameObject go, float nearClippingDistance, float farClippingDistance)
    {
        return getFrustumCorners(fovDegree, go.transform.position, go.transform.forward, go.transform.right, go.transform.up, nearClippingDistance, farClippingDistance);
    }

    public Vector3[] getFrustumCorners(Vector2 fovDegree, Camera c)
    {
        return getFrustumCorners(fovDegree, c.transform.position, c.transform.forward, c.transform.right, c.transform.up, c.nearClipPlane, c.farClipPlane);
    }

    public Vector3[] getFrustumCorners(Vector2 fovDegree, Vector3 worldPosition, Vector3 forwardDirection, Vector3 rightDirection, Vector3 upDirection, float nearClippingDistance, float farCLippingDistance)
    {
        Vector3 nearCenter = worldPosition + forwardDirection * nearClippingDistance;
        Vector3 farCenter = worldPosition + forwardDirection * farCLippingDistance;

        float aspectRatio = fovDegree.x / fovDegree.y;
        float e = Mathf.Tan((Mathf.Deg2Rad*fovDegree.y) * .5f);
        float nearExtendY = e * nearClippingDistance;
        float nearExtendX = nearExtendY * aspectRatio;
        float farExtendY = e * farCLippingDistance;
        float farExtendX = farExtendY * aspectRatio;

        Vector3[] corners = new Vector3[8];
        corners[0] = nearCenter + rightDirection * nearExtendX - upDirection * nearExtendY; // Near Links oben 
        corners[1] = nearCenter + rightDirection * nearExtendX + upDirection * nearExtendY; // Near Links unten
        corners[2] = nearCenter - rightDirection * nearExtendX + upDirection * nearExtendY; // Near Rechts unten
        corners[3] = nearCenter - rightDirection * nearExtendX - upDirection * nearExtendY; // Near Links unten

        corners[4] = farCenter + rightDirection * farExtendX - upDirection * farExtendY; // Near Links oben 
        corners[5] = farCenter + rightDirection * farExtendX + upDirection * farExtendY; // Near Links unten
        corners[6] = farCenter - rightDirection * farExtendX + upDirection * farExtendY; // Near Rechts unten
        corners[7] = farCenter - rightDirection * farExtendX - upDirection * farExtendY; // Near Links oben

        return corners;
    }

    // Reset Funktion für Editor. Prüft ob an Kamera.
    protected virtual void Reset()
    {
        Camera c = GetComponent<Camera>();
        
        if(c == null)
        {
            Debug.LogError("The Script TargetChecker must be attached to a camera!!!");
            DestroyImmediate(this);
        }
    }

    /// <summary>
    /// Prüft ob die Offset-Werte valid sind.
    /// </summary>
    protected virtual void validateValues()
    {
        float vertFOV = GetComponent<Camera>().fieldOfView;
        float horFOV = vertFOV * GetComponent<Camera>().aspect;
        simulatedFrustumDegree.x = Mathf.Min(simulatedFrustumDegree.x, horFOV);
        simulatedFrustumDegree.y = Mathf.Min(simulatedFrustumDegree.y, vertFOV);
        
        float simulatedAspect = simulatedFrustumDegree.x / simulatedFrustumDegree.y;

        //transitionInStartFrustumDegree.x = Mathf.Max(simulatedFrustumDegree.x, transitionInStartFrustumDegree.x);
        //transitionInStopFrustumDegree.x = Mathf.Min(simulatedFrustumDegree.x, Mathf.Max(transitionInStopFrustumDegree.x, 0));

        //transitionOutStopFrustumDegree.x = Mathf.Max(simulatedFrustumDegree.x, transitionOutStopFrustumDegree.x);
        //transitionOutStartFrustumDegree.x = Mathf.Min(simulatedFrustumDegree.x, Mathf.Max(transitionOutStartFrustumDegree.x, 0));

        outerFeedbackStopFrustumDegree.x = Mathf.Max(outerFeedbackStopFrustumDegree.x, 0);
        outerFeedbackStartFrustumDegree.x = Mathf.Max(outerFeedbackStartFrustumDegree.x, 0);

        if (keepAspect)
        {
            transitionInStartFrustumDegree.y = transitionInStartFrustumDegree.x / simulatedAspect;
            transitionInStopFrustumDegree.y = transitionInStopFrustumDegree.x / simulatedAspect;
            transitionOutStartFrustumDegree.y = transitionOutStartFrustumDegree.x / simulatedAspect;
            transitionOutStopFrustumDegree.y = transitionOutStopFrustumDegree.x / simulatedAspect;


            outerFeedbackStopFrustumDegree.y = outerFeedbackStopFrustumDegree.x / simulatedAspect;
            outerFeedbackStartFrustumDegree.y = outerFeedbackStartFrustumDegree.x / simulatedAspect;

        }
        else
        {
            //transitionInStartFrustumDegree.y = Mathf.Max(simulatedFrustumDegree.y, transitionInStartFrustumDegree.y);
            //transitionInStopFrustumDegree.y = Mathf.Min(simulatedFrustumDegree.y, transitionInStopFrustumDegree.y);
            //outerFeedbackStopFrustumDegree.y = Mathf.Max(outerFeedbackStopFrustumDegree.y, 0);
            //outerFeedbackStartFrustumDegree.y = Mathf.Max(outerFeedbackStartFrustumDegree.y, 0);
        }


        fovSizes.Clear();
        fovSizes.Add(FovType.OUTER_START, outerFeedbackStartFrustumDegree);
        fovSizes.Add(FovType.OUTER_STOP, outerFeedbackStopFrustumDegree);
        fovSizes.Add(FovType.TRANSITION_STOP_IN, transitionInStopFrustumDegree);
        fovSizes.Add(FovType.TRANSITION_START_IN, transitionInStartFrustumDegree);
        fovSizes.Add(FovType.SIMULATED, simulatedFrustumDegree);
        fovSizes.Add(FovType.TRANSITION_START_OUT, transitionOutStartFrustumDegree);
        fovSizes.Add(FovType.TRANSITION_STOP_OUT, transitionOutStopFrustumDegree);
        fovSizes.Add(FovType.OUTER_ALLOWED, outerFeedbackAllowedFrustumDegree);
    }

    // Bei Änderung von Werten im Editor.
    protected virtual void OnValidate()
    {
        validateValues();
    }

    // Draw Gizmos or not.
    protected virtual void OnDrawGizmos()
    {
        
        if (drawGizmos)
        {
//            UnityEditorInternal.InternalEditorUtility.SetShowGizmos(true);
            Camera c = GetComponent<Camera>();
            //Gizmos.matrix = c.transform.localToWorldMatrix;
            Color beforeColor = Gizmos.color;

            List<Tuple<Color, Vector3[], bool>> frustumCornersWithColor = new List<Tuple<Color, Vector3[], bool>>();
            frustumCornersWithColor.Add(new Tuple<Color, Vector3[], bool>(simulatedColor, getFrustumCorners(simulatedFrustumDegree, c), simulatedGizmoVisible));
            frustumCornersWithColor.Add(new Tuple<Color, Vector3[], bool>(transitionInStartColor, getFrustumCorners(transitionInStartFrustumDegree, c), transitionInStartGizmoVisible));
            frustumCornersWithColor.Add(new Tuple<Color, Vector3[], bool>(transitionInStopColor, getFrustumCorners(transitionInStopFrustumDegree, c), transitionInStopGizmoVisible));
            frustumCornersWithColor.Add(new Tuple<Color, Vector3[], bool>(outerStartColor, getFrustumCorners(outerFeedbackStartFrustumDegree, c), outerStartGizmoVisible));
            frustumCornersWithColor.Add(new Tuple<Color, Vector3[], bool>(outerStopColor, getFrustumCorners(outerFeedbackStopFrustumDegree, c), outerStopGizmoVisible));
            frustumCornersWithColor.Add(new Tuple<Color, Vector3[], bool>(transitionOutStartColor, getFrustumCorners(transitionOutStartFrustumDegree, c), transitionOutStartGizmoVisible));
            frustumCornersWithColor.Add(new Tuple<Color, Vector3[], bool>(transitionOutStopColor, getFrustumCorners(transitionOutStopFrustumDegree, c), transitionOutStopGizmoVisible));
            frustumCornersWithColor.Add(new Tuple<Color, Vector3[], bool>(outerAllowedColor, getFrustumCorners(outerFeedbackAllowedFrustumDegree, c), outerAllowedGizmoVisible));

            foreach (Tuple<Color, Vector3[], bool> item in frustumCornersWithColor)
            {
                if (item.Item3)
                {
                    for (int i = 0; i < 4; i++)
                    {
                        Gizmos.color = item.Item1;
                        Gizmos.DrawLine(item.Item2[i], item.Item2[(i + 1) % 4]);
                        Gizmos.DrawLine(item.Item2[i], item.Item2[i + 4]);
                        Gizmos.DrawLine(item.Item2[i + 4], item.Item2[((i + 1) % 4) + 4]);
                    }
                }
            }
            Gizmos.color = beforeColor;
            Gizmos.matrix = Matrix4x4.identity;
        }
    }

    /// <summary>
    /// Gibt das Visibility-Object oder null aus Dict <see cref="gameObjectStatus"/> zurück.
    /// </summary>
    /// <param name="go"></param>
    /// <returns></returns>
    protected VisibilityStatus GetVisibility(GameObject go)
    {
        VisibilityStatus vs = null;
        gameObjectStatus.TryGetValue(go, out vs);
        return vs;
    }

    public bool isTouching(GameObject go, FovType meshType)
    {
        VisibilityStatus vs = GetVisibility(go);
        if (vs == null)
            return false;
        return vs.IsTouching(meshType);
    }

    public float getHorizontalDistanceDegree(FovType meshType1, FovType meshType2)
    {
        Vector2 meshDegree1 = Vector2.zero, meshDegree2 = Vector2.zero;
        fovSizes.TryGetValue(meshType1, out meshDegree1);
        fovSizes.TryGetValue(meshType2, out meshDegree2);
        return Mathf.Abs(meshDegree1.x - meshDegree2.x) / 2f;
    }
}
