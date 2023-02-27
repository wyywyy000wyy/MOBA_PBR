using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using System.Linq;


[CustomEditor(typeof(MeshSimplify))]
public class MeshSimplifyEditor : Editor
{
    static EditMode _editMode;
    List<Vector3> colliderVectexes = new List<Vector3>();
    static float pointRadius = 0.03f;
    static Color lineColor = Color.green;
    int curPot;

    string[] triangleTypeNames =
{
        "Fan",
        "Strip",
        "Polygon(自动识别)",
        "Polygon(逆时针)",
        "Polygon(顺时针)"
    };
    int[] triangleTypeValues =
{
        (int)MeshSimplify.TriangleType.Fan,
        (int)MeshSimplify.TriangleType.Strip,
        (int)MeshSimplify.TriangleType.Polygon,
        (int)MeshSimplify.TriangleType.PolygonClock,
        (int)MeshSimplify.TriangleType.PolygonAnticlock,
    };
    enum EditMode
    {
        EditNormal,
        EditColliderMesh,
        MoveColliderMesh,
    }
    public class PolygonBuilder
    {
        class Point
        {
            public Vector3 v;
            public int ID;
        }

        public List<int> Indexes = new List<int>();

        public static bool LineIntersection(Vector3 a, Vector3 b, Vector3 c, Vector3 d)
        {
            float minX = Mathf.Min(a.x, b.x);
            float maxX = Mathf.Max(a.x, b.x);

            float minZ = Mathf.Min(a.z, b.z);
            float maxZ = Mathf.Max(a.z, b.z);

            float minX2 = Mathf.Min(c.x, d.x);
            float maxX2 = Mathf.Max(c.x, d.x);

            float minZ2 = Mathf.Min(c.z, d.z);
            float maxZ2 = Mathf.Max(c.z, d.z);

            //if (Mathf.Min(a.x, b.x) <= Mathf.Max(c.x, d.x) && Mathf.Min(c.z, d.z) <= Mathf.Max(a.z, b.z) && Mathf.Min(c.x, d.x) <= Mathf.Max(a.x, b.x) && Mathf.Min(a.z, b.z) <= Mathf.Max(c.z, d.z))
            //    return false;
            if (maxX2 < minX || minX2 > maxX || maxZ2 < minZ || minZ2 > maxZ)
                return false;

            Vector3 ca = a - c;
            Vector3 cb = b - c;
            Vector3 cd = d - c;

            Vector3 caXcd = Vector3.Cross(ca, cd);
            Vector3 cbXcd = Vector3.Cross(cb, cd);

            if (Vector3.Dot(caXcd, cbXcd) <= 0)
                return true;

            return false;

        }

        public PolygonBuilder(List<Vector3> vertexes, MeshSimplify.TriangleType type = MeshSimplify.TriangleType.Polygon)
        {
            List<Point> points = new List<Point>(vertexes.Count);
            for (int i = 0; i < vertexes.Count; ++i)
            {
                points.Add(new Point
                {
                    ID = i,
                    v = vertexes[i]
                });
            }

            int anticlockwiseCount = 0;
            for (int i = 0; i < points.Count; ++i)
            {
                if (Check(i, points))
                {
                    anticlockwiseCount++;
                }
            }

            anticlockwise = anticlockwiseCount * 2 >= points.Count;
            if (type == MeshSimplify.TriangleType.PolygonClock)
            {
                anticlockwise = false;
            }
            else if (type == MeshSimplify.TriangleType.PolygonAnticlock)
            {
                anticlockwise = true;
            }

            List<List<Point>> polygons = BuildPolygons(points);

            foreach (var polygon in polygons)
            {
                for (int i = 0; i < polygon.Count - 2; ++i)
                {
                    if (anticlockwise)
                    {
                        Indexes.Add(polygon[0].ID);
                        Indexes.Add(polygon[i + 1].ID);
                        Indexes.Add(polygon[i + 2].ID);
                    }
                    else
                    {
                        Indexes.Add(polygon[0].ID);
                        Indexes.Add(polygon[i + 2].ID);
                        Indexes.Add(polygon[i + 1].ID);
                    }
                }
            }
        }

        bool anticlockwise;

        bool ValidLine(int i1, int i2, List<Point> points)
        {
            Point p1 = points[i1];
            Point p2 = points[i2];

            int pre1 = (i1 + points.Count - 1) % points.Count;

            int pre2 = (i2 + points.Count - 1) % points.Count;

            for (int i = 0; i < points.Count - 1; ++i)
            {
                if (i == pre1 || i == pre2 || i == i1 || i == i2)
                    continue;
                if (LineIntersection(p1.v, p2.v, points[i].v, points[i + 1].v))
                {
                    return false;
                }
            }

            return true;
        }

        List<List<Point>> BuildPolygons(List<Point> points)
        {
            List<List<Point>> list = new List<List<Point>>();
            if (points.Count < 3)
            {
                return list;
            }
            else if (points.Count == 3)
            {
                list.Add(points);
                return list;
            }

            for (int i = 0; i < points.Count; ++i)
            {
                bool t = Check(i, points);
                if (t != anticlockwise)
                {
                    Vector3 p = points[i].v;
                    int endI = -1;

                    float dis = float.MaxValue;

                    int preI = (i + points.Count - 1) % points.Count;
                    int nextI = (i + 1) % points.Count;

                    for (int j = 0; j < points.Count; j++)
                    {
                        if (j == i || j == nextI || j == preI)
                            continue;
                        var d = (points[i].v - points[j].v).magnitude;
                        if (d < dis && ValidLine(i, j, points))
                        {
                            Vector3 d_ac = (points[preI].v - p).normalized;
                            Vector3 d_bc = (points[nextI].v - p).normalized;
                            Vector3 d_ab = (d_ac + d_bc) / 2;
                            Vector3 d_cd = (points[j].v - p).normalized;

                            float dot1 = Vector3.Dot(d_ab, d_ac);
                            float dot2 = Vector3.Dot(d_ab, d_cd);

                            if (dot2 < dot1)
                            {
                                dis = d;
                                endI = j;
                            }
                        }
                    }

                    if (endI != -1)
                    {
                        int start = 0;
                        int range0 = Mathf.Min(i, endI);
                        int range1 = Mathf.Max(i, endI);
                        int last = points.Count - 1;

                        //Debug.Log($"切割 {points[i].ID}, {points[endI].ID}");

                        List<Point> p1 = new List<Point>();
                        p1.AddRange(points.GetRange(start, range0 + 1));
                        p1.AddRange(points.GetRange(range1, last - range1 + 1));


                        List<Point> p2 = new List<Point>();
                        p2.AddRange(points.GetRange(range0, range1 - range0 + 1));

                        if (p1.Count > 0)
                        {
                            if (p1.Count > 2)
                            {
                                list.AddRange(BuildPolygons(p1));
                            }
                            else
                            {
                                throw new System.Exception($"异常节点, {i}, {endI}");
                            }
                        }
                        if (p2.Count > 0)
                        {
                            if (p2.Count > 2)
                            {
                                list.AddRange(BuildPolygons(p2));
                            }
                            else
                            {
                                throw new System.Exception($"异常节点, {i}, {endI}");
                            }
                        }
                        return list;
                    }
                }
            }
            list.Add(points);
            return list;
        }

        bool Check(int i, List<Point> points)
        {
            return Check(
                points[(i + points.Count - 1) % points.Count].v,
                points[i].v,
                points[(i + 1) % points.Count].v
                );
        }

        bool Check(Vector3 v1, Vector3 v2, Vector3 v3)
        {
            if (v1 == v2 || v1 == v3)
            {
                return true;
            }

            Vector3 v = Vector3.Cross(v2 - v1, v3 - v2);

            return v.y >= 0;
        }
    }
    //public static Matrix4x4 Add(Matrix4x4 a, Matrix4x4 b)
    //{
    //    return new Matrix4x4(
    //        a.GetColumn(0) + b.GetColumn(0),
    //        a.GetColumn(1) + b.GetColumn(1),
    //        a.GetColumn(2) + b.GetColumn(2),
    //        a.GetColumn(3) + b.GetColumn(3));
    //}
    static void SaveTargetMesh(MeshSimplify _target, List<Vector3> colliderVectexes)
    {
        if (colliderVectexes.Count < 3)
        {
            UnityEditor.EditorUtility.DisplayDialog("错误", "顶点数不能少于3个", "确认");
            return;
        }
        if (_target.vertexes == null)
        {
            _target.vertexes = new List<Vector3>();
        }
        if (_target.indexes == null)
        {
            _target.indexes = new List<int>();
        }

        colliderVectexes = colliderVectexes.Select(
            (v) => (new Vector3(Mathf.Floor(v.x), Mathf.Floor(v.y), Mathf.Floor(v.z))
            )).ToList();

        _target.vertexes = new List<Vector3>();
        _target.vertexes.InsertRange(0, colliderVectexes);
        _target.indexes.Clear();
        List<int> cacheIList = new List<int>();

        if (_target.triangleType == MeshSimplify.TriangleType.Fan)
        {
            for (int i = 1; i < colliderVectexes.Count - 1; ++i)
            {
                cacheIList.Add(0);
                Vector3 dir1 = colliderVectexes[i] - colliderVectexes[0];
                Vector3 dir2 = colliderVectexes[i + 1] - colliderVectexes[i];

                Vector3 cd = Vector3.Cross(dir1, dir2);
                if (cd.y >= 0)
                {
                    cacheIList.Add(i);
                    cacheIList.Add(i + 1);
                }
                else
                {
                    cacheIList.Add(i + 1);
                    cacheIList.Add(i);
                }
            }
        }
        else if (_target.triangleType == MeshSimplify.TriangleType.Polygon || _target.triangleType == MeshSimplify.TriangleType.PolygonClock
            || _target.triangleType == MeshSimplify.TriangleType.PolygonAnticlock
            )
        {
            PolygonBuilder builder = new PolygonBuilder(colliderVectexes, _target.triangleType);
            cacheIList = builder.Indexes;
        }
        else
        {
            for (int i = 1; i < colliderVectexes.Count - 1; ++i)
            {
                cacheIList.Add(i - 1);
                Vector3 dir1 = colliderVectexes[i] - colliderVectexes[i - 1];
                Vector3 dir2 = colliderVectexes[i + 1] - colliderVectexes[i];

                Vector3 cd = Vector3.Cross(dir1, dir2);
                if (cd.y >= 0)
                {
                    cacheIList.Add(i);
                    cacheIList.Add(i + 1);
                }
                else
                {
                    cacheIList.Add(i + 1);
                    cacheIList.Add(i);
                }
            }
        }

        _target.indexes = cacheIList;

        Mesh mesh = new Mesh();
        mesh.vertices = _target.vertexes.ToArray();
        mesh.SetIndices(_target.indexes, MeshTopology.Triangles, 0);
        _target.SetMesh(mesh);
        EditorUtility.SetDirty(_target);
        AssetDatabase.Refresh();
    }
    public override void OnInspectorGUI()
    {
        base.OnInspectorGUI();

        MeshSimplify _target = (MeshSimplify)target;

        if (_target.source != null && _target.source.mesh != null)
        {
            if (GUILayout.Button("SimplifyFromSource"))
            {
                MeshSimplifyTools.Simplify(_target);
            }
        }

        GUILayout.BeginHorizontal();
        {


            int intType = EditorGUILayout.IntPopup(((int)_target.triangleType), triangleTypeNames, triangleTypeValues);

            MeshSimplify.TriangleType type = (MeshSimplify.TriangleType)intType;//(MeshSimplify.TriangleType)EditorGUILayout.EnumPopup(_target.triangleType, triangleTypeNames);
            if (type != _target.triangleType)
            {
                _target.triangleType = type;
                SaveTargetMesh(_target, colliderVectexes);
            }
        }
        GUILayout.EndHorizontal();

        

        if (_editMode == EditMode.EditNormal)
        {
            if (GUILayout.Button("Create Collider Mesh"))
            {
                colliderVectexes.Clear();
                _editMode = EditMode.EditColliderMesh;
                SceneView.RepaintAll();
            }
            if (_target.vertexes != null && _target.vertexes.Count > 0 && GUILayout.Button("Move Collider Mesh"))
            {
                colliderVectexes.Clear();
                if (_target.vertexes != null)
                    colliderVectexes.InsertRange(0, _target.vertexes);
                _editMode = EditMode.MoveColliderMesh;
                curPot = 0;
                SceneView.RepaintAll();
            }
        }
        else if (_editMode == EditMode.EditColliderMesh || _editMode == EditMode.MoveColliderMesh)
        {
            if (_editMode == EditMode.MoveColliderMesh)
            {
                if (GUILayout.Button("添加顶点"))
                {
                    colliderVectexes.Insert(curPot, colliderVectexes[curPot]);
                }
            }
            if (GUILayout.Button("Save Collider Mesh"))
            {
                _editMode = EditMode.EditNormal;
                SaveTargetMesh(_target, colliderVectexes);
            }

            if (GUILayout.Button("Cancel Collider Mesh"))
            {
                _editMode = EditMode.EditNormal;

            }

            SceneView.RepaintAll();
        }
    }

    private void OnSceneGUI()
    {
        Matrix4x4 preMat = Handles.matrix;
        var _target = (MeshSimplify)target;
        if (_editMode == EditMode.EditColliderMesh)
        {
            int controlID = GUIUtility.GetControlID(FocusType.Passive);
            HandleUtility.AddDefaultControl(controlID);
            var e = Event.current;

            if (e.type == EventType.MouseDown && e.button == 0)
            {

                Undo.RecordObject(_target, "Add Collider Vertex");

                Vector3 mousePos = e.mousePosition;
                float ppp = EditorGUIUtility.pixelsPerPoint;


                var camera = SceneView.GetAllSceneCameras()[0];
                mousePos.y = camera.pixelHeight - mousePos.y * ppp;
                mousePos.x *= ppp;


                //var pos = MapEditorUtility.GUIPointToGroundPosition(e.mousePosition);
                //Vector2 mousePosition = e.mousePosition;
                //mousePosition.y = (float)Screen.height - mousePosition.y - 40f;
                Ray ray = Camera.current.ScreenPointToRay(mousePos);

                Plane plane = new Plane(Vector3.up, 0);
                float dis;
                plane.Raycast(ray, out dis);
                Vector3 groundPos = ray.GetPoint(dis);
                colliderVectexes.Add(_target.transform.worldToLocalMatrix.MultiplyPoint(groundPos));
                SceneView.RepaintAll();
            }

            Handles.color = lineColor;
            if (_target.triangleType == MeshSimplify.TriangleType.Fan)
            {
                for (int i = 0; i < colliderVectexes.Count; ++i)
                {
                    Handles.DrawLine(colliderVectexes[i], colliderVectexes[(i + 1) % colliderVectexes.Count]);
                    Handles.DrawLine(colliderVectexes[0], colliderVectexes[(i + 1) % colliderVectexes.Count]);
                    Handles.DrawLine(colliderVectexes[0], colliderVectexes[i]);
                }
            }
            else if (_target.triangleType == MeshSimplify.TriangleType.Polygon || _target.triangleType == MeshSimplify.TriangleType.PolygonClock
            || _target.triangleType == MeshSimplify.TriangleType.PolygonAnticlock)
            {
                Handles.color = Color.red;
                if (colliderVectexes.Count > 1)
                {
                    Handles.DrawLine(colliderVectexes[colliderVectexes.Count - 1], colliderVectexes[0]);
                }
                Handles.color = lineColor;
                for (int i = 0; i < colliderVectexes.Count - 1; ++i)
                {
                    Handles.DrawLine(colliderVectexes[i], colliderVectexes[(i + 1) % colliderVectexes.Count]);
                }

            }
            else
            {
                for (int i = 1; i < colliderVectexes.Count - 1; ++i)
                {
                    Handles.DrawLine(colliderVectexes[i], colliderVectexes[(i + 1)]);
                    Handles.DrawLine(colliderVectexes[i - 1], colliderVectexes[(i + 1)]);
                    Handles.DrawLine(colliderVectexes[i - 1], colliderVectexes[i]);
                }
            }

            Handles.color = Color.red;
            foreach (var vertex in colliderVectexes)
            {
                Handles.DrawSolidDisc(vertex, Vector3.up, pointRadius);
            }
        }
        else if (_editMode == EditMode.MoveColliderMesh)
        {
            Handles.color = lineColor;

            if (_target.triangleType == MeshSimplify.TriangleType.Polygon || _target.triangleType == MeshSimplify.TriangleType.PolygonClock
            || _target.triangleType == MeshSimplify.TriangleType.PolygonAnticlock)
            {
                Handles.color = Color.red;
                if (colliderVectexes.Count > 1)
                {
                    Handles.DrawLine(colliderVectexes[colliderVectexes.Count - 1], colliderVectexes[0]);
                }
                Handles.color = lineColor;
                for (int i = 0; i < colliderVectexes.Count - 1; ++i)
                {
                    Handles.DrawLine(colliderVectexes[i], colliderVectexes[i + 1]);
                }
                //if(_target.amplifyVertexes.Count > 0)
                //{
                //    Handles.color = Color.blue;
                //    for (int i = 0; i < _target.amplifyVertexes.Count; ++i)
                //    {
                //        Handles.DrawLine(_target.amplifyVertexes[i], _target.amplifyVertexes[(i + 1)% _target.amplifyVertexes.Count]);
                //    }
                //}
            }
            else
            {
                if (_target.mesh != null)
                {
                    DrawMesh(colliderVectexes, _target.mesh.GetIndices(0).ToList());
                }
                else if (_target.indexes != null)
                {
                    DrawMesh(colliderVectexes, _target.indexes);
                }
            }

            Handles.color = Color.red;
            for (int i = 0; i < colliderVectexes.Count; ++i)
            {
                Vector3 vertex = colliderVectexes[i];
                EditorGUI.BeginChangeCheck();
                Vector3 newTargetPosition = Handles.PositionHandle(vertex, Quaternion.identity);
                if (EditorGUI.EndChangeCheck())
                {
                    vertex = newTargetPosition;
                    colliderVectexes[i] = new Vector3(newTargetPosition.x, newTargetPosition.y, newTargetPosition.z);
                    curPot = i;
                }
                Handles.DrawSolidDisc(vertex, Vector3.up, pointRadius);
            }
            if (curPot >= 0 && curPot < colliderVectexes.Count)
            {
                Handles.color = Color.yellow;
                Handles.DrawSolidDisc(colliderVectexes[curPot], Vector3.up, pointRadius);
            }
        }
        else
        {
            if(_target.task != null && _target.task.edges != null)
            {
                var pc = GUI.color;
                GUI.color = Color.blue;
                var pm = Handles.matrix;

                foreach (var e in _target.task.removedEdges)
                {
                    Handles.DrawLine(e.GetV1().p, e.GetV2().p, 1);
                    Handles.DrawLine(e.GetV2().p, e.GetV1().p, 1);
                    Handles.Label(e.GetV1().p, "????????");
                    //Handles.DrawLine(new Vector3(0, -1, 0), new Vector3(0, 1, 0), 1);
                }
                Handles.matrix = _target.source.transform.localToWorldMatrix;
                var pcc = Handles.color;
                Handles.color = Color.red;
                
                Handles.color = pcc;

                if (_target.drawEdge)
                {
                    foreach (var e in _target.task.oldEdges)
                    {
                        Vector3 p = e.center;
                        if (!e.first)
                        {
                            p += _target.offset;
                        }
                        Handles.Label(p, e.id.ToString());
                    }
                }

                GUI.color = Color.green;
                Dictionary<Vector3, int> drawDic = new Dictionary<Vector3, int>();
                if(_target.drawVec)
                {
                    foreach (var e in _target.task.oldVertices)
                    {
                        //if (e.version == 0)
                        {
                            Vector3 p = e.p;
                            if (drawDic.ContainsKey(p))
                            {
                                int v = drawDic[p];
                                drawDic[p]++;
                                p += _target.offset * v;
                            }
                            else
                            {
                                drawDic[p] = 1;

                            }
                            Handles.Label(p, e.id.ToString());
                        }
                    }
                }
                

                GUI.color = Color.blue;
                Handles.matrix = _target.transform.localToWorldMatrix;
                if (_target.drawEdge)
                {
                    foreach (var e in _target.task.oldEdges)
                    {
                        Vector3 p = e.center;
                        if (!e.first)
                        {
                            p += _target.offset;
                        }
                        Handles.Label(p, e.id.ToString());
                    }
                }
                if (_target.drawVec)
                {
                    drawDic.Clear();
                    foreach (var e in _target.task.oldVertices)
                    {
                        //if (e.version == 0)
                        {
                            Vector3 p = e.p;
                            if (drawDic.ContainsKey(p))
                            {
                                int v = drawDic[p];
                                drawDic[p]++;
                                p += _target.offset * v;
                            }
                            else
                            {
                                drawDic[p] = 1;

                            }
                            Handles.Label(p, e.id.ToString());
                        }
                    }
                }
                


                Handles.matrix = pm;
                GUI.color = pc;
            }
        }

        Handles.matrix = preMat;
    }

    void DrawMesh(List<Vector3> vertexes, List<int> indexes)
    {
        for (int i = 0; i < indexes.Count - 2; i += 3)
        {
            Handles.DrawLine(vertexes[indexes[i]], vertexes[indexes[i + 1]]);
            Handles.DrawLine(vertexes[indexes[i]], vertexes[indexes[i + 2]]);
            Handles.DrawLine(vertexes[indexes[i + 1]], vertexes[indexes[i + 2]]);
        }
    }

}
