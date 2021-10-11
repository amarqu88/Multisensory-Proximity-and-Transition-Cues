using UnityEngine;

/// <summary>
/// Benachrichtigt Subscriber über Kollisionen in CollisionLayer.
/// </summary>
public class MeshTouchingPublisher : MonoBehaviour
{
    public delegate void MeshTouchingEventHandler(TargetChecker.FovType meshType, GameObject go, bool touching);
    public event MeshTouchingEventHandler MeshTouchingEvent;

    /// <summary>
    /// Name dieses Publishers für den Callback.
    /// </summary>
    public TargetChecker.FovType meshFovType;

    /// <summary>
    /// Name der CollisionLayerMask.
    /// </summary>
    public string CollisionLayerMaskName = "TargetMeshCollisionLayer";

    private void Start()
    {
        this.gameObject.layer = LayerMask.NameToLayer(CollisionLayerMaskName);
    }

    public void setMeshFovType(TargetChecker.FovType meshtype)
    {
        this.meshFovType = meshtype;
    }
    
    private void OnCollisionEnter(Collision collision)
    {
        MeshTouchingEvent?.Invoke(meshFovType, collision.gameObject, true);
    }

    private void OnCollisionExit(Collision collision)
    {
        MeshTouchingEvent?.Invoke(meshFovType, collision.gameObject, false);
    }

    private void OnCollisionStay(Collision collision)
    {
        MeshTouchingEvent?.Invoke(meshFovType, collision.gameObject, true);
    }
}
