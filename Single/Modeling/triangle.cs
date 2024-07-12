using UnityEngine;

[RequireComponent(typeof(MeshRenderer), typeof(MeshFilter))]                                    // MeshRenderer�� MeshFilter�� �ݵ�� �ʿ��ϱ⿡ ����
public class triangle : MonoBehaviour
{
    public float size = 1.0f;
    public Vector3 offset = new Vector3(0, 0, 0);

    Mesh mesh;
    Vector3[] vertices;
    int[] triangles;

    /// <summary>
    /// ������ ũ�� �� mesh ���� �� ������ ����
    /// </summary>
    void OnValidate()
    {
        if (mesh == null) return;

        if (size > 0 || offset.magnitude > 0)
        {
            setMeshData(size);
            createProceduralMesh();
        }
    }

    void Start()
    {
        mesh = GetComponent<MeshFilter>().mesh;

        setMeshData(size);
        createProceduralMesh();
    }
    /// <summary>
    /// �ﰢ���� ��ǥ 3�� ����, �ð���� ������ vertices�� index ����, �ݴ��� �鵵 ���̰� �ݽð� ���� vertices�� �����ϰ� index ����
    /// </summary>
    /// <param name="size"></param>
    void setMeshData(float size)
    {
        float g = Mathf.Sqrt(3.0f) / 6.0f * size;
        vertices = new Vector3[] {
            new Vector3(-0.5f * size, 0, -g) + offset,
            new Vector3(0, 0, Mathf.Sqrt(3.0f) / 2.0f * size - g) + offset,
            new Vector3(0.5f * size, 0, -g) + offset,

            new Vector3(-0.5f * size, 0, -g) + offset,
            new Vector3(0, 0, Mathf.Sqrt(3.0f) / 2.0f * size - g) + offset,
            new Vector3(0.5f * size, 0, -g) + offset};

        triangles = new int[] { 0, 1, 2, 5, 4, 3 };
    }
    /// <summary>
    /// ���� mesh ������ �ʱ�ȭ �� ����
    /// </summary>
    void createProceduralMesh()
    {
        mesh.Clear();
        mesh.vertices = vertices;
        mesh.triangles = triangles;
        mesh.RecalculateNormals();

        Destroy(this.GetComponent<MeshCollider>());
        this.gameObject.AddComponent<MeshCollider>();
    }
}