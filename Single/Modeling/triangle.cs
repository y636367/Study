using UnityEngine;

[RequireComponent(typeof(MeshRenderer), typeof(MeshFilter))]                                    // MeshRenderer과 MeshFilter가 반드시 필요하기에 강제
public class triangle : MonoBehaviour
{
    public float size = 1.0f;
    public Vector3 offset = new Vector3(0, 0, 0);

    Mesh mesh;
    Vector3[] vertices;
    int[] triangles;

    /// <summary>
    /// 도형의 크기 및 mesh 존재 시 무조건 생성
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
    /// 삼각형의 좌표 3개 설정, 시계방향 순으로 vertices의 index 삽입, 반대쪽 면도 보이게 반시계 방향 vertices도 동일하게 index 삽입
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
    /// 기존 mesh 데이터 초기화 후 삽입
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