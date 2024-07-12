using UnityEngine;

[RequireComponent(typeof(MeshRenderer), typeof(MeshFilter))]
public class ProceduralTorus : MonoBehaviour
{
    public float outerRadius = 1.0f;
    public float innerRadius = 0.5f;
    public float height = 0.2f;
    public int segments = 36;
    public Vector3 offset = Vector3.zero;

    private Mesh mesh;
    private Vector3[] vertices;
    private int[] triangles;

    void OnValidate()
    {
        if (mesh == null) return;

        if (outerRadius > 0 || innerRadius > 0 || height > 0 || segments > 0)
        {
            SetMeshData(outerRadius, innerRadius, height, segments);
            CreateProceduralMesh();
        }
    }

    void Start()
    {
        mesh = GetComponent<MeshFilter>().mesh;

        SetMeshData(outerRadius, innerRadius, height, segments);
        CreateProceduralMesh();
    }

    void SetMeshData(float outerRadius, float innerRadius, float height, int segments)
    {
        int vertCount = (segments + 1) * 2 * 2; // Top and bottom vertices for both inner and outer circles
        vertices = new Vector3[vertCount];
        int triCount = segments * 4 * 6; // 4 triangles per segment, 6 vertices per quad
        triangles = new int[triCount];

        int vert = 0;
        int tri = 0;

        // Top outer circle
        for (int i = 0; i <= segments; i++)
        {
            float angle = (float)i / segments * Mathf.PI * 2.0f;
            vertices[vert++] = new Vector3(Mathf.Cos(angle) * outerRadius, height * 0.5f, Mathf.Sin(angle) * outerRadius) + offset;
        }

        // Top inner circle
        for (int i = 0; i <= segments; i++)
        {
            float angle = (float)i / segments * Mathf.PI * 2.0f;
            vertices[vert++] = new Vector3(Mathf.Cos(angle) * innerRadius, height * 0.5f, Mathf.Sin(angle) * innerRadius) + offset;
        }

        // Bottom outer circle
        for (int i = 0; i <= segments; i++)
        {
            float angle = (float)i / segments * Mathf.PI * 2.0f;
            vertices[vert++] = new Vector3(Mathf.Cos(angle) * outerRadius, -height * 0.5f, Mathf.Sin(angle) * outerRadius) + offset;
        }

        // Bottom inner circle
        for (int i = 0; i <= segments; i++)
        {
            float angle = (float)i / segments * Mathf.PI * 2.0f;
            vertices[vert++] = new Vector3(Mathf.Cos(angle) * innerRadius, -height * 0.5f, Mathf.Sin(angle) * innerRadius) + offset;
        }

        // Top triangles (outer and inner)
        for (int i = 0; i < segments; i++)
        {
            int currentOuter = i;
            int nextOuter = i + 1;
            int currentInner = segments + 1 + i;
            int nextInner = segments + 1 + i + 1;

            // Outer quad
            triangles[tri++] = currentOuter;
            triangles[tri++] = nextOuter;
            triangles[tri++] = currentInner;
            triangles[tri++] = nextOuter;
            triangles[tri++] = nextInner;
            triangles[tri++] = currentInner;
        }

        // Bottom triangles (outer and inner)
        for (int i = 0; i < segments; i++)
        {
            int currentOuter = (segments + 1) * 2 + i;
            int nextOuter = (segments + 1) * 2 + i + 1;
            int currentInner = (segments + 1) * 3 + i;
            int nextInner = (segments + 1) * 3 + i + 1;

            // Outer quad
            triangles[tri++] = currentOuter;
            triangles[tri++] = currentInner;
            triangles[tri++] = nextOuter;
            triangles[tri++] = nextOuter;
            triangles[tri++] = currentInner;
            triangles[tri++] = nextInner;
        }

        // Side triangles (inner and outer)
        for (int i = 0; i < segments; i++)
        {
            int topOuter = i;
            int topInner = segments + 1 + i;
            int bottomOuter = (segments + 1) * 2 + i;
            int bottomInner = (segments + 1) * 3 + i;

            // Outer quad
            triangles[tri++] = topOuter;
            triangles[tri++] = bottomOuter;
            triangles[tri++] = topOuter + 1;
            triangles[tri++] = topOuter + 1;
            triangles[tri++] = bottomOuter;
            triangles[tri++] = bottomOuter + 1;

            // Inner quad
            triangles[tri++] = topInner;
            triangles[tri++] = topInner + 1;
            triangles[tri++] = bottomInner;
            triangles[tri++] = topInner + 1;
            triangles[tri++] = bottomInner + 1;
            triangles[tri++] = bottomInner;
        }
    }

    void CreateProceduralMesh()
    {
        mesh.Clear();
        mesh.vertices = vertices;
        mesh.triangles = triangles;
        mesh.RecalculateNormals();

        Destroy(GetComponent<MeshCollider>());
        gameObject.AddComponent<MeshCollider>();
    }
}
