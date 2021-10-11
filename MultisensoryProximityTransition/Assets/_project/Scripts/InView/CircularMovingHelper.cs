using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class CircularMovingHelper
{
    /// <summary>
    /// Calculates the length of an arc of a circle given by his radius and the angle in degree.
    /// </summary>
    /// <param name="circleRadius">Radius of the circle.</param>
    /// <param name="degree">Part of the circle in degree.</param>
    /// <returns>Length of the arc.</returns>
    public static float CircleArcLength(float circleRadius, float degree)
    {
        return (degree / 360f) * 2f * Mathf.PI * circleRadius;
    }

    public static float degreePerDuration(float degree, float duration)
    {
        return degree / duration;
    }

    public static float degreeFromPerDuration(float duration, float degreePerDuration)
    {
        return degreePerDuration * duration;
    }

    public static float timeForDegreeAndSpeed(float degree, float velocityDegPerSecond)
    {
        return degree / velocityDegPerSecond;
    }


}
