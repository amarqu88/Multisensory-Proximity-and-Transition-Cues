using System.Collections.Generic;
using UnityEngine;
//using Sirenix.OdinInspector;

/// <summary>
/// Prüft ob ein GameObject mit MeshRenderer in vordefinierten Bereichen ist. Basierend auf Collidern! Die Targets müssen im Layer TargetMeshCollisionLayer liegen.
/// </summary>
public class TargetMeshChecker : TargetChecker
{
    protected FovTypeGameObjectDictionary meshObjects = new FovTypeGameObjectDictionary();

//    [ReadOnly, ShowInInspector]
    protected FovTypeStringDictionary meshNames = new FovTypeStringDictionary()
    {
        { FovType.SIMULATED, "SimulatedFOV" },
        { FovType.TRANSITION_START_IN, "TransitionStartIn" },
        { FovType.TRANSITION_STOP_IN, "TransitionStopIn" },
        { FovType.TRANSITION_START_OUT, "TransitionStartOut" },
        { FovType.TRANSITION_STOP_OUT, "TransitionStopOut" },
        { FovType.OUTER_STOP, "OuterStop" },
        { FovType.OUTER_START, "OuterStart" },
        { FovType.OUTER_ALLOWED, "OuterAllowed" }
    };

    protected void MeshTouchingCallback(FovType meshType, GameObject go, bool isTouching)
    {
        VisibilityStatus visibility;
        if (!gameObjectStatus.TryGetValue(go, out visibility))
        {
            visibility = new VisibilityStatus();
            if (isTouching)
            {
                gameObjectStatus.Add(go, visibility);
            }else
            {
                return;
            }
        }
        visibility.IsTouching(meshType, isTouching);
    }

    protected override void Reset()
    {
        base.Reset();

        if (this == null || this.gameObject == null)
            return;

        removeMeshes();
    }

    private void Start()
    {
        GenerateMeshes();
    }

    void removeMeshes()
    {
        foreach (var keyVal in meshObjects)
        {
            if (keyVal.Value != null)
                DestroyImmediate(keyVal.Value);
        }

        meshObjects.Clear();

        Camera c = GetComponent<Camera>();
        foreach (Transform t in c.transform)
        {
            if (meshNames.ContainsValue(t.gameObject.name))
            {
                DestroyImmediate(t.gameObject);
            }
        }
        foreach (KeyValuePair<FovType, string> item in meshNames)
        {
            GameObject go = new GameObject(item.Value);
            go.transform.parent = c.transform;
            validateCamChild(go);
            go.GetComponent<MeshTouchingPublisher>().meshFovType = item.Key;
            meshObjects.Add(item.Key, go);
        }
    }

//    [Button]
    public void GenerateMeshes()
    {
        removeMeshes();
        base.validateValues();
        meshObjects.Clear();
        foreach (Transform t in this.transform)
        {
            MeshTouchingPublisher m = t.gameObject.GetComponent<MeshTouchingPublisher>();
            if (m != null)
            {
                meshObjects.Add(m.meshFovType, t.gameObject);
                t.gameObject.GetComponent<MeshTouchingPublisher>().MeshTouchingEvent += MeshTouchingCallback;
            }
        }
        createMeshes();
    }

    protected void OnDestroy()
    {
        foreach (var keyval in meshObjects)
        {
            keyval.Value.GetComponent<MeshTouchingPublisher>().MeshTouchingEvent -= MeshTouchingCallback;
        }
        meshObjects.Clear();
    }

    /// <summary>
    /// Prüft ob gegebenes GameObject Meshcollider sowie Publisher für Events enthält. Erstellt diese, wenn nötig.
    /// </summary>
    /// <param name="go">Zu prüfendes Gameobject.</param>
    protected void validateCamChild(GameObject go)
    {
        go.transform.localPosition = Vector3.zero;
        go.transform.localScale = Vector3.one;
        if (go.GetComponent<MeshTouchingPublisher>() == null)
        {
            go.AddComponent<MeshTouchingPublisher>();
        }

        MeshCollider mc = go.GetComponent<MeshCollider>();
        if (mc == null)
        {
            mc = go.AddComponent<MeshCollider>();
        }
        mc.convex = false;
    }

    private void createMesh(GameObject go, Vector2 fovDegree)
    {
        Camera camera = GetComponent<Camera>();
        Vector3[] frustumCorners = getFrustumCorners(fovDegree, camera);

        MeshCollider mc = go.GetComponent<MeshCollider>();
        Mesh m = new Mesh();

        Vector3[] vertices = new Vector3[5];
        vertices[0] = Vector3.zero;
        vertices[1] = frustumCorners[5];
        vertices[2] = frustumCorners[6];
        vertices[3] = frustumCorners[7];
        vertices[4] = frustumCorners[4];

        int[] triangles = new int[] {
            0,2,3,
            0,1,2,
            0,4,1,
            0,3,4,
            1,4,3,
            1,3,2,
            0,3,2,
            0,2,1,
            0,1,4,
            0,4,3,
            1,3,4,
            1,2,3
        };

        m.vertices = vertices;
        m.triangles = triangles;

        m.RecalculateNormals();

        mc.sharedMesh = m;
    }

    /// <summary>
    /// Erzeugt Meshes für alle MeshCollider der vorhandenen Offsets.
    /// </summary>
    void createMeshes()
    {
        validateValues();
        foreach (var item in meshObjects)
        {
            Vector2 tmp;
            fovSizes.TryGetValue(item.Key, out tmp);
            createMesh(item.Value, tmp);
        }
    }
}
