
using System.Linq;
using System;
using Unity.Mathematics;
using System.Collections.Generic;
using UnityEngine;
public class MeshSimplifyTools
{
    public class Vertex
    {
        public int id;
        public float3 p;

        public List<Edge> edges = new List<Edge>();
        public List<Triangle> triangles = new List<Triangle>();

        //public void CalcNormal()
        //{
        //    Vector3 n = triangles.Select(v=>v.normal).Aggregate((t, v)=>t + v);
        //    n /= triangles.Count;
        //    normal = n;
        //}

        //public Vector3 normal;

        public float4x4 quadric = float4x4.zero;

        public void CaclQuadric()
        {
            quadric = float4x4.zero;
            foreach (var triangle in triangles)
            {
                quadric += triangle.quadric;
            }
        }
    }

    public class Edge : IEqualityComparer<Edge>//, IComparer<Edge>, IComparable<Edge>
    {
        //public static bool Equals(Edge objA, Edge objB)
        //{
        //    return objA.IsEqual(objB);
        //}

        public static bool ReferenceEquals(Edge objA, Edge objB)
        {
            return objA == objB;
        }

        public class EqualityComparer : IEqualityComparer<Edge>
        {
            public int GetHashCode(Edge key)
            {
                return key.GetHashCode2();
            }

            public bool Equals(Edge v1, Edge v2)
            {
                return v1.Equals2(v1, v2);
            }
        }

        override public string ToString()
        {
            return $"ID=<color='#00FF00'>{id}</color>,quadric=<color='#00FFFF'>{quadric}</color>,{v1.p},{v2.p}";
        }

        public int id;

        public Vertex v1;
        public Vertex v2;

        public Vector3 center => (v1.p + v2.p) / 2;

        public float quadric;
        public List<Triangle> triangles = new List<Triangle>();
        //public int CompareTo(Edge e2)
        //{

        //    if(quadric != e2.quadric)
        //    {
        //        return quadric.CompareTo(e2.quadric);
        //    }

        //    return id.CompareTo(e2.id);
        //}
        //public int Compare(Edge e1, Edge e2)
        //{
        //    if (e1.quadric != e2.quadric)
        //    {
        //        return e1.quadric.CompareTo(e2.quadric);
        //    }

        //    return e1.id.CompareTo(e2.id);
        //}

        public static int __Compare(Edge e1, Edge e2)
        {
            if (e1.quadric != e2.quadric)
            {
                return e1.quadric.CompareTo(e2.quadric);
            }

            return e1.id.CompareTo(e2.id);
        }

        //public override int CompareTo(Edge other);
        //{
        //    return 0;
        //}
        public bool Equals(Edge x, Edge y)
        {
            return Equals2(x,y);
        }
        public bool Equals2(Edge x, Edge y)
        {
            return x.v1 == y.v1 && x.v2 == y.v2 || x.v1 == y.v2 && x.v2 == y.v1;
        }
        public int GetHashCode(Edge obj)
        {
            return obj.GetHashCode2();
        }

        public int GetHashCode2()
        {
            return v1.id + v2.id;
        }

        public void CalcQuadricValue()
        {
            float4x4 q = v1.quadric + v2.quadric;
            float3 v;
            if (math.determinant(q) != 0)
            {
                float3x3 q3 = (float3x3)q;
                float3x3 vq = math.inverse(q3);
                float3 tv = -q.c3.xyz;
                v = math.mul(vq, tv);
                float4 v4 = new float4(v, 1);
                quadric = math.dot(v4, math.mul(q, v4));
            }
            else
            {
                float4 vv1 = new float4(v1.p, 1);
                float quadric1 = math.dot(vv1, math.mul(q, vv1));
                float4 vv2 = new float4(v2.p, 1);
                float quadric2 = math.dot(vv2, math.mul(q, vv2));
                float4 vv12 = new float4((v1.p + v2.p) / 2, 1);
                float quadric3 = math.dot(vv12, math.mul(q, vv12));
                quadric = math.min(quadric1, math.min(quadric2, quadric3));
            }
        }
    }

    public class Triangle
    {
        public int id;
        public Vertex[] vertices = new Vertex[3];
        public Edge[] edges = new Edge[3];
        public float3 normal;
        public float4 abcd;

        public float4x4 quadric;
        public void CalcNormal()
        {
            float3 v1 = math.normalize((vertices[1].p - vertices[0].p));
            float3 v2 = math.normalize(vertices[2].p - vertices[1].p);
            var tv = math.cross(v1, v2);
            normal = math.normalize(tv);

            float d = -(math.dot(normal, vertices[0].p));
            abcd = new Vector4(normal.x, normal.y, normal.z, d);

            float ttv = math.dot(normal, vertices[0].p) + d;
            float ttv2 = math.dot(normal, vertices[1].p) + d;
            float ttv3 = math.dot(normal, vertices[2].p) + d;

            quadric = new float4x4(abcd * abcd.x, abcd * abcd.y, abcd * abcd.z, abcd * abcd.w);
        }
    }

    public static void Simplify(MeshSimplify target)
    {
        Mesh mesh = target.source.mesh;

        List<Vertex> vertices = mesh.vertices.Select((v, i) => new Vertex { id = i, p = v }).ToList();

        //List<Edge> edges = new List<Edge>();
        //HashSet<Edge> edges = new HashSet<Edge>();
        Dictionary<Edge, Edge> edges = new Dictionary<Edge, Edge>(new Edge.EqualityComparer());


        List<Triangle> triangles = new List<Triangle>();
        var indices = mesh.GetIndices(0);
        for (int i = 0; i < indices.Length; i += 3)
        {
            Triangle triangle = new Triangle { id = triangles.Count };
            triangles.Add(triangle);
            for (int j = 0; j < 3; j++)
            {
                int i1 = indices[i + j];
                Vertex v1 = vertices[i1];
                int i2 = indices[i + ((j + 1) % 3)];
                Vertex v2 = vertices[i2];
                Edge e = new Edge { id = edges.Count, v1 = v1, v2 = v2 };
                if (edges.TryGetValue(e, out var ee))
                {
                    e = ee;
                }
                else
                {
                    edges.Add(e,e);
                }
                v1.edges.Add(e);
                v2.edges.Add(e);
                //edges.Add(e,e);
                v1.triangles.Add(triangle);
                e.triangles.Add(triangle);
                triangle.vertices[j] = v1;
                triangle.edges[j] = e;
            }
            triangle.CalcNormal();
        }

        vertices.ForEach(v => v.CaclQuadric());

        target.source.vertices = vertices;
        target.source.edges = edges;
        target.source.triangles = triangles;


        List<Edge> list = new List<Edge>();


        foreach (var e in edges)
        {
            e.Value.CalcQuadricValue();
            list.Add(e.Value);
        }
        list.Sort((v1, v2) => Edge.__Compare(v1, v2));
        foreach(var e in list)
        {
            Debug.Log($"s={e}");
        }
    }
}