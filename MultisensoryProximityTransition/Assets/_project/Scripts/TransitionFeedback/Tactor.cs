using UnityEngine;

[System.Serializable]
public class Tactor : ScriptableObject {
    /// <summary>
    /// Name of the tactor.
    /// </summary>
    public string TactorName { get; set; }

    /// <summary>
    /// Pin of the controlling device where this tactor is connected to.
    /// </summary>
    public int GPIOPin { get; set; }

    /// <summary>
    /// The current frequency of the tactor. The frequency will be clamped between <see cref="MinFrequency"/> and <see cref="MaxFrequency"/>.
    /// </summary>
    public int Frequency { get; set; }

    /// <summary>
    /// The minimal allowed frequency see <see cref="Frequency"/>.
    /// </summary>
    public int MinFrequency { get; set; }

    /// <summary>
    /// The maximal allowed frequency see <see cref="Frequency"/>.
    /// </summary>
    public int MaxFrequency { get; set; }

    /// <summary>
    /// Local position in degree
    /// </summary>
    public float DegreePosition { get; set; }

    /// <summary>
    /// If the tactor is not enabled or get disabled the <see cref="Frequency"/> will go to zero.
    /// </summary>
    public bool IsEnabled { get; set; }

    /// <summary>
    /// The position of the offset position of this tactor from the related body.
    /// </summary>
    public Vector3 Position { get; set; }
    
    /// <summary>
    /// Relative rotation.
    /// </summary>
    public Vector3 Rotation { get; set; }

    /// <summary>
    /// Minimal and maximal degree horizontally. Describes a frustum in which the tactor should response. 
    /// </summary>
    public Vector2 LatitudeResponseAreaDeg { get; set; }

    /// <summary>
    /// Minimal and maximal degree vertically. Describes a frustum in which the tactor should response.
    /// </summary>
    public Vector2 LongitudeResponseAreaDeg { get; set; }

    public float CurrentPulseDuration { get; set; }

    public int CurrentPulseState { get; set; }

    public Tactor(int gpioPin, string tactorName, Vector3 position, Vector3 rotation, float degreePosition)
    {
        TactorName = tactorName;
        Position = position;
        Rotation = rotation;
        GPIOPin = gpioPin;
        CurrentPulseState = 1;
        CurrentPulseDuration = 100f;
        DegreePosition = degreePosition;
    }

    public Tactor()
    {
        CurrentPulseState = 1;
        CurrentPulseDuration = 100f;
    }
}
