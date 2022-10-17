using UnityEngine;
using System.Collections.Generic;
using static MeshSimplifyTools;

public class MeshSimplify : MonoBehaviour
{
    public Mesh mesh;
    public List<Vector3> vertexes;
    public List<int> indexes;
    public MeshSimplify source;

    [HideInInspector]
    public TriangleType triangleType = TriangleType.Fan;
    public enum TriangleType
    {
        Fan,
        Strip,
        Polygon,
        PolygonClock,
        PolygonAnticlock,
    }

    public List<Vertex> vertices;
    public Dictionary<Edge, Edge> edges;
    public List<Triangle> triangles;


    public void SetMesh(Mesh m)
    {
        edges = null;
        mesh = m;
        var meshfilter = GetComponent<MeshFilter>();
        if (meshfilter != null)
        {
            meshfilter.mesh = m;
        }
        var meshRenderer = GetComponent<MeshRenderer>();
        //if(meshRenderer != null)
        //{
        //    meshRenderer.se
        //}
    }
}