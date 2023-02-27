
using System.Linq;
using System;
using Unity.Mathematics;
using System.Collections.Generic;
using UnityEngine;
public class MeshSimplifyTools
{

    public static float BounderEdgePenalty = 1e6f;
    public static float ErrorTranglePenalty = 1e7f;

    UInt32 Murmur32(List<UInt32> InitList)
    {
        UInt32 Hash = 0;
        foreach (var i in InitList)
        {
            var Element = i;
            Element *= 0xcc9e2d51;
            Element = (Element << 15) | (Element >> (32 - 15));
            Element *= 0x1b873593;

            Hash ^= Element;
            Hash = (Hash << 13) | (Hash >> (32 - 13));
            Hash = Hash * 5 + 0xe6546b64;
        }

        return MurmurFinalize32(Hash);
    }
    UInt32 MurmurFinalize32(UInt32 Hash)
    {
        Hash ^= Hash >> 16;
        Hash *= 0x85ebca6b;
        Hash ^= Hash >> 13;
        Hash *= 0xc2b2ae35;
        Hash ^= Hash >> 16;
        return Hash;
    }

    public class Vertex
    {
        public int version;
        public int id;
        public float3 p;
        public float2 uv;
        public Vertex next;

        static HashSet<Vertex> _CollectVertex_tmp_set = new HashSet<Vertex>();
        public void CollectVertex(List<Vertex> vertexes)
        {
            if(this == this.next)
            {
                int a = 1;
                a = 2;
            }
            for(var v = this; v.next != this; v = v.next)
            {
                vertexes.Add(v);
            }
        }

        public float CalcVertCost(float3 newPos)
        {
            float Cost = 0;
            float disCost = math.length(newPos - p);
            foreach(var t in triangles)
            {
                if (t.Removed)
                    continue;
                Cost += disCost;
                if(!t.ValidNewPos(this, newPos))
                {
                    Cost += ErrorTranglePenalty;
                }
            }
            return Cost;
        }
        public float CalcCost(float3 newPos)
        {
            float Cost = 0;
            for(var ip = this; this != ip.next; ip = ip.next)
            {
                if (ip.Removed)
                    continue;
                Cost += CalcVertCost(newPos);
            }

            return Cost;
        }

        public bool PosAndUvEqual(Vertex v)
        {
            return PosAndUvEqual(v.p, v.uv);
        }
        public bool PosAndUvEqual(float3 pos,float2 _uv)
        {
            return p.Equals(pos) && uv.Equals(_uv);
        }

        public bool PosEqual(Vertex v)
        {
            return p.Equals(v.p);
        }

        public Vertex()
        {
            next = this;
        }

        public Vertex AddNext(Vertex _next)
        {
            if(_next.next != _next)
            {
                Debug.LogError("Vertex AddNext _next.next != null");
                return null;
            }
            _next.next = this.next;
            this.next = _next;
            return _next;
        }

        public List<Edge> edges = new List<Edge>();

        public void AddEdge(Edge e, bool check = true)
        {
            if (!edges.Contains(e))
                edges.Add(e);
            if(check)
            {
                CheckValid();
            }
        }

        void CheckValid()
        {
            foreach(var e in edges)
            {
                if(e.GetV1()!=this && e.GetV2() != this)
                {
                    Debug.LogError("error");
                }
            }
        }

        public class EqualityComparer : IEqualityComparer<Vertex>
        {
            public int GetHashCode(Vertex key)
            {
                return key.p.GetHashCode();
            }

            public bool Equals(Vertex v1, Vertex v2)
            {
                return (Vector3)v1.p == (Vector3)v2.p;
            }
        }

        List<Triangle> triangles = new List<Triangle>();

        public List<Triangle> _triangles => triangles;

        public void AddTriangle(Triangle t)
        {
            if(triangles.Contains(t))
            {
                //Debug.LogError("has triangle");
                return;
            }
            triangles.Add(t);
        }

        public Triangle GetTriangle(int i)
        {
            return triangles[i];
        }

        //public void CalcNormal()
        //{
        //    Vector3 n = triangles.Select(v=>v.normal).Aggregate((t, v)=>t + v);
        //    n /= triangles.Count;
        //    normal = n;
        //}

        //public Vector3 normal;

        public bool Removed => version < 0;

        public float4x4 quadric = float4x4.zero;

        public void CaclQuadric()
        {
            quadric = float4x4.zero;
            foreach (var triangle in triangles)
            {
                if(triangle.version == 0)
                quadric += triangle.quadric;
            }
        }
    }

    public class Edge : IEqualityComparer<Edge>//, IComparer<Edge>, IComparable<Edge>
    {
        public int version;
        //public static bool Equals(Edge objA, Edge objB)
        //{
        //    return objA.IsEqual(objB);
        //}
        public bool first;
        public Edge next;
        public Edge()
        {
            next = this;
        }

        public Edge AddNext(Edge _next)
        {
            if (_next.next != _next)
            {
                Debug.LogError("Edge AddNext _next.next != null");
                return null;
            }
            _next.next = this.next;
            this.next = _next;
            return _next;
        }

        public Edge(Vertex _v1, Vertex _v2)
        {
            //if (!_v1.edges.Contains(this) || !_v2.edges.Contains(this))
            //{
            //    Debug.LogError("error v");
            //    return;
            //}
            next = this;
            v1 = _v1;
            v1.AddEdge(this);
            v2 = _v2;
            v2.AddEdge(this);

        }

        public static bool ReferenceEquals(Edge objA, Edge objB)
        {
            return objA == objB;
        }

        public class EdgePosComparer : IEqualityComparer<Edge>
        {
            public int GetHashCode(Edge key)
            {
                return key.v1.p.GetHashCode() + key.v2.p.GetHashCode();
            }

            public bool Equals(Edge v1, Edge v2)
            {
                return v1.PosEquals(v1, v2);
            }
        }

        public class EqualityComparer : IEqualityComparer<Edge>
        {
            public int GetHashCode(Edge key)
            {
                return key.GetHashCode2();
            }

            public bool Equals(Edge v1, Edge v2)
            {
                return v1.VertEquals(v1, v2);
            }
        }

        override public string ToString()
        {
            return $"ID=<color='#00FF00'>{id}</color>,quadric=<color='#00FFFF'>{quadric}</color>,{v1.p},{v2.p}";
        }

        public int id;

        Vertex v1;

        public void SetV1(Vertex v)
        {
            if(!v.edges.Contains(this))
            {
                Debug.LogError("error v");
                return;
            }
            v1 = v;
        }

        public void ReplaceVert(Vertex oldVert, Vertex newVert)
        {
            Vertex v = null;
            if(v1 == oldVert)
            {
                v = v1;
            }
            else if(v2 == oldVert)
            {
                v = v2;
            }
            else
            {
                throw new Exception($"错误的替换边{this}上的顶点{oldVert}");
            }

        }

        public Vertex GetV1()
        {
            return v1;
        }

        Vertex v2;

        public void SetV2(Vertex v)
        {
            if (!v.edges.Contains(this))
            {
                Debug.LogError("error v");
                return;
            }
            v2 = v;
        }
        public Vertex GetV2()
        {
            return v2;
        }

        public float3 center => (v1.p + v2.p) / 2;

        public float3 newVec;
        public float quadric;
        public float cost;
        public List<Triangle> triangles = new List<Triangle>();
        public static int __Compare(Edge e1, Edge e2)
        {
            if(e1.version != e2.version)
            {
                return -e1.version.CompareTo(e2.version);
            }
            if (e1.cost != e2.cost)
            {
                return e1.cost.CompareTo(e2.cost);
            }

            return e1.id.CompareTo(e2.id);
        }

        public bool Equals(Edge x, Edge y)
        {
            return x.VertEquals(x,y);
        }

        public bool PosEquals(Edge x, Edge y)
        {
            bool b1 = x.v1.PosEqual(y.v1);
            bool b2 = x.v2.PosEqual(y.v2);
            bool b3 = x.v1.PosEqual(y.v2);
            bool b4 = x.v2.PosEqual(y.v1);

            return b1 && b2 || b3 && b4;

        }
        public bool VertEquals(Edge x, Edge y)
        {
            bool b1 = x.v1.PosAndUvEqual(y.v1);
            bool b2 = x.v2.PosAndUvEqual(y.v2);
            bool b3 = x.v1.PosAndUvEqual(y.v2);
            bool b4 = x.v2.PosAndUvEqual(y.v1);

            return b1 && b2 || b3 && b4;
        }
        public int GetHashCode(Edge obj)
        {
            return obj.GetHashCode2();
        }

        public int GetHashCode2()
        {
            return v1.p.GetHashCode() + v2.p.GetHashCode();
        }

        public bool isBounder()
        {
            int c = 0;
            foreach(var t in triangles)
            {
                if (!t.Removed)
                    c++;
            }
            return c == 1;
        }
        public bool Removed => version < 0;

        public float CalcVertCost()
        {
            float Cost = 0;
            if (Removed)
                return Cost;
            if (isBounder())
                Cost += BounderEdgePenalty;
            float v1Cost = v1.CalcCost(newVec);
            float v2Cost = v2.CalcCost(newVec);
            Cost += v1Cost + v2Cost;
            return Cost;
        }

        public float CalcEdgeCost()
        {
            float tCost = CalcVertCost();
            for (var e = this.next; e.next != this; e = e.next)
            {
                tCost += CalcVertCost();
            }
            cost = tCost;
            return tCost;
        }

        public void CalcQuadricValue()
        {
            float4x4 q = v1.quadric + v2.quadric;
            //q[3][0] = 0;
            //q[3][1] = 0
            //q[3][2] = 0;
            //q[3][3] = 1.0f;
            float3 v;
            float3x3 q3 = (float3x3)q;
            float dt = math.determinant(q3);
            if (math.abs(dt) > math.EPSILON)
            {
                float3x3 vq = math.inverse(q3);
                float3 tv = -q.c3.xyz;
                v = math.mul(tv, vq); 
                float4 v4 = new float4(v, 1);
                newVec = v;

                float nq = 0;
                for(int i = 0; i < 4; ++i)
                {
                    for(int j = 0; j < 4; ++j)
                    {
                        nq += v4[i] * q[i][j] * v4[j];
                    }
                }

                float4 dv4 = new float4(
                    q.c0.x * v4.x + q.c0.y * v4.y + q.c0.z * v4.z + q.c0.w * v4.w,
                    q.c1.x * v4.x + q.c1.y * v4.y + q.c1.z * v4.z + q.c1.w * v4.w,
                    q.c2.x * v4.x + q.c2.y * v4.y + q.c2.z * v4.z + q.c2.w * v4.w,
                    q.c3.x * v4.x + q.c3.y * v4.y + q.c3.z * v4.z + q.c3.w * v4.w
                    );

                float tf1 = q.c1.z;
                float tf2 = q[1][2];

                float tf21 = q.c2.y;
                float tf22 = q[2][1];


                float dq = math.dot(dv4, v4);

                float dq3 =
                    (q.c0.x * v4.x + q.c0.y * v4.y + q.c0.z * v4.z + q.c0.w * v4.w) * v4.x +
                    (q.c1.x * v4.x + q.c1.y * v4.y + q.c1.z * v4.z + q.c1.w * v4.w) * v4.y +
                    (q.c2.x * v4.x + q.c2.y * v4.y + q.c2.z * v4.z + q.c2.w * v4.w) * v4.z +
                    (q.c3.x * v4.x + q.c3.y * v4.y + q.c3.z * v4.z + q.c3.w * v4.w) * v4.w;

                var dv = math.mul(v4, q);
                quadric = math.dot(dv, v4);
                int a = 5;
                a++;
            }
            else
            {
                float4 vv12 = new float4((v1.p + v2.p) / 2, 1);
                float quadric3 = math.dot(math.mul(q, vv12), vv12);
                float4 vv1 = new float4(v1.p, 1);
                float quadric1 = math.dot(math.mul(q, vv1), vv1);
                float4 vv2 = new float4(v2.p, 1);
                float quadric2 = math.dot(math.mul(q, vv2), vv2);
                newVec = vv12.xyz;
                quadric = quadric3;

                if (quadric < quadric1)
                {
                    newVec = vv1.xyz;
                    quadric = quadric1;
                }
                if (quadric < quadric2)
                {
                    newVec = vv2.xyz;
                    quadric = quadric2;
                }
            }
        }
    }

    public class Triangle
    {
        public int version;
        public int id;
        public bool Removed => version < 0;

        public Vertex GetVert(int i)
        {
            return vertices[i];
        }

        public void ReplaceVert(Vertex oldVert, Vertex newVert)
        {

        }

        public bool ValidNewPos(Vertex v, float3 newPos)
        {
            float3 v0 = vertices[0].p;
            float3 v1 = vertices[1].p;
            float3 v2 = vertices[2].p;

            float3 o1 = v1 - v0;
            float3 o2 = v2 - v1;
            float3 ou = math.cross(o1, o2);


            float3 nv0 = vertices[0].p.Equals(v) ? newPos: vertices[0].p;
            float3 nv1 = vertices[1].p.Equals(v) ? newPos : vertices[1].p;
            float3 nv2 = vertices[2].p.Equals(v) ? newPos : vertices[2].p;

            float3 n1 = nv1 - nv0;
            float3 n2 = nv2 - nv1;
            float3 nu = math.cross(n1, n2);

            return math.dot(ou, nu) > 0;
        }

        public void SetVert(int ii, Vertex v)
        {
            int idx = -1;
            for(int i = 0; i < v._triangles.Count; ++i)
            {
                if(v._triangles[i] == this)
                {
                    idx = i;
                    break;
                }
            }
            if(idx == -1)
            {
                Debug.LogError($"SetVert Error v={v.id}");
            }
            var preVert = vertices[ii];
            vertices[ii] = v;
        }

        Vertex[] vertices = new Vertex[3];
        public Edge[] edges = new Edge[3];
        public float3 normal;
        public float4 abcd;

        public float4x4 quadric;
        public void CalcNormal()
        {
            if( vertices[0].p.Equals(vertices[1].p) || vertices[0].p.Equals(vertices[2].p) || vertices[1].p.Equals(vertices[2].p))
            {
                if(version >=0)
                    version--;
                return;
            }
            float3 v1 = math.normalize((vertices[1].p - vertices[0].p));
            float3 v2 = math.normalize(vertices[2].p - vertices[1].p);
            var tv = math.cross(v1, v2);
            normal = math.normalize(tv);

            float d = -(math.dot(normal, vertices[0].p));
            abcd = new Vector4(normal.x, normal.y, normal.z, d);

            float ttv  = math.dot(normal, vertices[0].p) + d;
            float ttv2 = math.dot(normal, vertices[1].p) + d;
            float ttv3 = math.dot(normal, vertices[2].p) + d;

            quadric = new float4x4(abcd * abcd.x, abcd * abcd.y, abcd * abcd.z, abcd * abcd.w);
        }
    }

    public class SimplifyTask
    {
        public List<Vertex> vertices;
        public Dictionary<Edge, Edge> edges;
        public List<Triangle> triangles;
        public int SimplifyEdgeCount;

        public List<Edge> sortedEdges;

        public List<Vertex> oldVertices;
        public List<Edge> oldEdges;
        public List<Edge> removedEdges = new List<Edge>();

        public void Execute()
        {
            sortedEdges = new List<Edge>();

            foreach (var e in edges)
            {
                e.Value.CalcQuadricValue();
                //sortedEdges.Add(e.Value);
            }
            foreach (var e in edges)
            {
                e.Value.CalcEdgeCost();
                sortedEdges.Add(e.Value);
            }

            sortedEdges.Sort((v1, v2) => Edge.__Compare(v1, v2));

            oldVertices = vertices.Select(v => new Vertex { id = v.id, p = v.p, quadric = v.quadric }).ToList();
            oldEdges = sortedEdges.Select(v => new Edge(oldVertices[v.GetV1().id], oldVertices[v.GetV2().id]) { id = v.id, quadric = v.quadric }).ToList();
            Dictionary<int, Edge> em = oldEdges.ToDictionary((Edge e) => { return e.id; });
            for(int i = 0; i < sortedEdges.Count; ++i)
            {
                oldEdges[i].first = sortedEdges[i].first;
                oldEdges[i].next = em[sortedEdges[i].next.id];
            }



            for (int i = 0; i < SimplifyEdgeCount; ++i)
            {
                RemoveEdge();
            }
        }

        Edge GetMinEdge()
        {
            //Edge minEdge = null;

            //foreach()
            

            return sortedEdges[0];
        }

        public Mesh GenMesh()
        {
            Mesh mesh = new Mesh();

            mesh.vertices = vertices.Select(v=>(Vector3)v.p).ToArray();

            List<int> indices = new List<int>();
            foreach(var t in triangles)
            {
                if(t.version == 0)
                {
                    indices.Add(t.GetVert(0).id);
                    indices.Add(t.GetVert(1).id);
                    indices.Add(t.GetVert(2).id);
                }
            }
            mesh.SetIndices(indices.ToArray(), MeshTopology.Triangles, 0);

            return mesh;
        }

        void RemoveEdge()
        {
            foreach (var e in sortedEdges)
            {
                Debug.Log($"edge={e.id} cost={e.cost} q={e.quadric} newVec={e.newVec} version={e.version}");
            }
            HashSet<Vertex> dirVert = new HashSet<Vertex>();
            HashSet<Edge> dirEdges = new HashSet<Edge>();
            HashSet<Triangle> dirTriangles = new HashSet<Triangle>();

            Edge minEdge = GetMinEdge();
            minEdge.version--;

            removedEdges.Add(new Edge(vertices[minEdge.GetV1().id], vertices[minEdge.GetV2().id]) { id = minEdge.id });

            Debug.LogWarning($"RemoveEdge {minEdge.id} {minEdge}");

            Vertex v1 = minEdge.GetV1();
            v1.version--;
            Vertex v2 = minEdge.GetV2();
            v2.version--;

            Vertex newVert = new Vertex
            {
                id = vertices.Count,
                p = minEdge.newVec
            };
            vertices.Add(newVert);
            foreach(var e in v1.edges)
            {
                if (e.version < 0)
                    continue;
                if(e.GetV1().PosAndUvEqual(v1))
                {
                    newVert.AddEdge(e, false);
                    e.SetV1(newVert);
                }
                else if(e.GetV2().PosAndUvEqual(v1))
                {
                    newVert.AddEdge(e, false);
                    e.SetV2(newVert);
                }
                else
                {
                    if (e.GetV1() != newVert && e.GetV2() != newVert)
                        Debug.LogError($"AddEdge Error {e.id}");
                }
            }

            foreach (var v in v1._triangles)
            {
                for(int i = 0; i < 3; ++i)
                {
                    if(v.GetVert(i) == v1)
                    {
                        newVert.AddTriangle(v);
                        v.SetVert(i, newVert);
                    }
                }

                dirTriangles.Add(v);
            }

            foreach (var e in v2.edges)
            {
                if (e.version < 0)
                    continue;
                if (e.GetV1() == v2)
                {
                    newVert.AddEdge(e, false);
                    e.SetV1(newVert);
                }
                else if (e.GetV2() == v2)
                {
                    newVert.AddEdge(e, false);
                    e.SetV2(newVert);
                }
                else
                {
                    if(e.GetV1() != newVert && e.GetV2() != newVert)
                    Debug.LogError($"AddEdge Error {e.id}");
                }
            }
            foreach (var v in v2._triangles)
            {
                for (int i = 0; i < 3; ++i)
                {
                    if (v.GetVert(i) == v2)
                    {
                        newVert.AddTriangle(v);
                        v.SetVert(i, newVert);
                    }
                }
                dirTriangles.Add(v);
            }

            foreach (var t in minEdge.triangles)
            {
                t.version--;
            }

            foreach (var t in dirTriangles)
            {
                dirVert.Add(t.GetVert(0));
                dirVert.Add(t.GetVert(1));
                dirVert.Add(t.GetVert(2));
                t.CalcNormal();
            }

            foreach (var v in dirVert)
            {
                foreach(var ie in v.edges)
                {
                    dirEdges.Add(ie);
                    if (ie.GetV1() == v)
                    {
                    }
                    else if (ie.GetV2() == v)
                    {
                    }
                    else
                    {
                        Debug.LogError($"Vert Error {v.id}");
                    }
                }

                v.CaclQuadric();
            }

            
            foreach (var e in dirEdges)
            {
                e.CalcQuadricValue();
            }
            foreach (var e in dirEdges)
            {
                e.CalcEdgeCost();
            }

            {
                sortedEdges.Sort((v1, v2) => Edge.__Compare(v1, v2));
            }

            //sortedEdges = null;
            //triangleSet.Remove(tri2);
        }
    }

    public static void Simplify(MeshSimplify target)
    {
        Mesh mesh = target.source.mesh;

        List<Vertex> vertices = new List<Vertex>();

        

        //List<Edge> edges = new List<Edge>();
        //HashSet<Edge> edges = new HashSet<Edge>();
        Dictionary<Edge, Edge> edges = new Dictionary<Edge, Edge>(new Edge.EqualityComparer());
        Dictionary<Edge, Edge> posEdges = new Dictionary<Edge, Edge>(new Edge.EdgePosComparer());
        
        Dictionary<float3, Vertex> vertMap = new Dictionary<float3, Vertex>();
        Dictionary<int, Vertex> vertIdMap = new Dictionary<int, Vertex>();
        //foreach (float3 v in mesh.vertices)
        for(int i = 0; i < mesh.vertices.Length; ++i)
        {
            var vert = mesh.vertices[i];
            if(!vertMap.TryGetValue(vert, out var vt))
            {
                vt = new Vertex
                {
                    id = vertices.Count,
                    p = vert,
                    uv = mesh.uv[i],
                };
                vertIdMap[i] = vt;
                vertMap[vert] = vt;
                vertices.Add(vt);
            }
            else
            {
                if(!vt.PosAndUvEqual(vert, mesh.uv[i]))
                {
                    var vt_new = new Vertex
                    {
                        id = vertices.Count,
                        p = vert,
                        uv = mesh.uv[i],
                    };
                    vertIdMap[i] = vt_new;
                    vertices.Add(vt_new);
                    vt.AddNext(vt_new);
                }
                else
                {
                    vertIdMap[i] = vt;
                }
            }
        }

        List<Triangle> triangles = new List<Triangle>();
        var indices = mesh.GetIndices(0);
        for (int i = 0; i < indices.Length; i += 3)
        {
            Triangle triangle = new Triangle { id = triangles.Count };
            triangles.Add(triangle);
            for (int j = 0; j < 3; j++)
            {
                int i1 = indices[i + j];
                Vertex v1 = vertIdMap[i1];
                int i2 = indices[i + ((j + 1) % 3)];
                Vertex v2 = vertIdMap[i2];

                Edge e = new Edge(v1,v2) { id = edges.Count };
                if (edges.TryGetValue(e, out var ee))
                {
                    e = ee;
                }
                else
                {
                    edges.Add(e,e);
                    if (posEdges.TryGetValue(e, out var preEdge))
                    {
                        preEdge.AddNext(e);
                    }
                    else
                    {
                        e.first = true;
                        posEdges[e] = e;
                    }
                }

                v1.AddEdge(e);
                v2.AddEdge(e);
                //v1.triangles.Add(triangle);
                v1.AddTriangle(triangle);
                e.triangles.Add(triangle);
                triangle.SetVert(j ,v1);
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
            list.Add(e.Value);
        }
        list.Sort((v1, v2) => v1.id.CompareTo(v2.id));
       //bool equal = list[11].Equals2(list[11], list[19]);


        SimplifyTask task = new SimplifyTask
        {
            edges = edges,
            vertices = vertices,
            triangles = triangles,
            SimplifyEdgeCount = target.SimplifyEdgeCount
        };

        task.Execute();
        //foreach (var e in edges)
        //{
        //    e.Value.CalcQuadricValue();
        //    list.Add(e.Value);
        //}
        //list.Sort((v1, v2) => Edge.__Compare(v1, v2));

        target.SetMesh(task.GenMesh());
        target.task = task;
    }

    static void SimplifyEdge()
    {

    }
}