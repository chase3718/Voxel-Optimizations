using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShipMesh : MonoBehaviour
{
    public MeshRenderer MeshRenderer;
    public MeshFilter MeshFilter;
    public MeshCollider MeshCollider;
    public List<GameObject> blocks = new List<GameObject>();
    public List<Vector3> vertices = new List<Vector3>();
    public List<Vector3> normals = new List<Vector3>();
    public List<int> triangles = new List<int>();
    public List<Vector2> uvs = new List<Vector2>();
    
    private void Awake() {
        MeshRenderer = GetComponent<MeshRenderer>();
        MeshFilter = GetComponent<MeshFilter>();
        MeshCollider = GetComponent<MeshCollider>();
        MeshCollider.convex = true;
        GenerateMesh();
    }

    private void GenerateMesh() {
        Mesh mesh = new Mesh();
        mesh.vertices = vertices.ToArray();
        mesh.normals = normals.ToArray();
        mesh.triangles = triangles.ToArray();
        mesh.uv = uvs.ToArray();
        mesh.name = "Ship";
        MeshFilter.mesh = mesh;
        MeshCollider.sharedMesh = MeshFilter.mesh;
    }

    private void UpdateMesh() {
        MeshFilter.mesh.vertices = vertices.ToArray();
        MeshFilter.mesh.normals = normals.ToArray();
        MeshFilter.mesh.triangles = triangles.ToArray();
        MeshFilter.mesh.uv = uvs.ToArray();
        MeshCollider.sharedMesh = MeshFilter.mesh;
    }

    public void AddBlock(GameObject _obj) {
        blocks.Add(_obj);
        Mesh blockMesh = _obj.transform.GetComponent<MeshFilter>().sharedMesh;
        int offset = vertices.Count;
        foreach (Vector3 vert in blockMesh.vertices) {
            vertices.Add(vert + _obj.transform.position);
        }
        foreach (Vector3 normal in blockMesh.normals) {
            normals.Add(normal);
        }
        foreach (int tri in blockMesh.triangles) {
            triangles.Add(tri + offset);
        }
        foreach (Vector2 uv in blockMesh.uv) {
            uvs.Add(uv);
        }
        UpdateMesh();
    }

    public void RemoveBlock(GameObject _obj) {
        blocks.Remove(_obj);
        
        UpdateMesh();
    }
}
