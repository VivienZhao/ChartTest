﻿using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class ChartBase : MonoBehaviour
{

    public Material mat;//mesh材质

    /// <summary>
    /// 所有的顶点添加偏移
    /// </summary>
    protected List<PointsInfo> verticesAddOffset = new List<PointsInfo>();

    /// <summary>
    /// 索引
    /// </summary>
    protected List<int[]> triangles = new List<int[]>();

    /// <summary>
    /// 单个物体的点位信息
    /// </summary>
    [HideInInspector]
    public List<PointsInfo> pointsInfos = new List<PointsInfo>();

    protected List<Mesh> meshs = new List<Mesh>();

    /// <summary>
    /// 所有的生成的父物体
    /// </summary>
    protected List<GameObject> allChartParentObj = new List<GameObject>();
    /// <summary>
    /// 所有的生成的子物体
    /// </summary>
    protected List<GameObject> allChartItemObj = new List<GameObject>();

    /// <summary>
    /// 生成点位中心
    /// </summary>
    [Header("生成点位中心")]
    public Vector3 center = Vector3.zero;

    protected string meshName;

    /// <summary>
    /// 单位网格线的粗细
    /// </summary>
    public Vector3 unitLineThickness = new Vector3(0.1f, 0.1f, 0.1f);

    /// <summary>
    /// 辅助线线的粗细
    /// </summary>
    public Vector3 guideLineThickness = new Vector3(0.1f, 0.1f, 0.1f);

    /// <summary>
    /// 辅助点的大小
    /// </summary>
    public float guidePointsSize = 1;



    public virtual void Awake()
    {
        SetMeshName();
    }
    public virtual void SetMeshName()
    {
        meshName = gameObject.name;
    }
    private void OnEnable()
    {
        CreatGuidePointObj();
        CreateUnitObjs();
        CreateMesh();
        SetMeshInfo();
        ApplyValue();
        ShowAnim();
        CreatGuideLines();
    }

    private void OnDisable()
    {
        DestroyImmediate(lineObj);
        DeleteAllItemObjs();
    }


    /// <summary>
    /// 展示动画
    /// </summary>
    public virtual void ShowAnim()
    {
        for (int i = 0; i < pointsInfos.Count; i++)
        {
            for (int j = 0; j < pointsInfos[i].points.Length; j++)
            {
                int tempI = i;
                int tempJ = j;

                StartCoroutine(DoFloatValue(0, pointsInfos[i].points[j].y, 1, (value) =>
                {
                    pointsInfos[tempI].points[tempJ].y = value;
                }));
            }
        }
    }


    protected System.Collections.IEnumerator DoFloatValue(float startValue, float endValue, float time, System.Action<float> ChangeAction)
    {
        float tempF = startValue;
        float offsetValue = (endValue - startValue) / (time / 0.02f);
        bool addOrReduce = offsetValue >= 0;
        while (true)
        {
            yield return new WaitForFixedUpdate();
            tempF += offsetValue;
            ChangeAction(tempF);
            if ((addOrReduce && tempF >= endValue) || (!addOrReduce && tempF <= endValue))
            {
                ChangeAction(endValue);
                break;
            }
        }

    }

    public virtual void Update()
    {
        RefreshLineMesh();
        CreateMesh();
        SetMeshInfo();
        ApplyValue();
        ResetSetGuidePoints();
        RefreshGuideLineMehs();
    }

    #region 生成辅助点点
    GameObject guidePointParentObj;
    protected List<GameObject> guidePointObjs = new List<GameObject>();
    protected virtual void CreatGuidePointObj()
    {
        int guideCount = 0;
        if (IsCanInstanceGuidePointObj(out guideCount))
        {
            ClearGuidePoints();
            guidePointParentObj = new GameObject(gameObject.name + "_GuidePoints");
            for (int i = 0; i < guideCount; i++)
            {
                GameObject tempGuidePointObj = GameObject.CreatePrimitive(PrimitiveType.Sphere);
                tempGuidePointObj.name = gameObject.name + "_GuidePoints_" + i.ToString();
                tempGuidePointObj.transform.parent = guidePointParentObj.transform;
                guidePointObjs.Add(tempGuidePointObj);
            }
        }
    }

    void ClearGuidePoints()
    {
        if (guidePointParentObj != null)
        {
            DestroyImmediate(guidePointParentObj);
        }
        for (int i = 0; i < guidePointObjs.Count; i++)
        {
            DestroyImmediate(guidePointObjs[i]);
        }
        guidePointObjs.Clear();
    }
    /// <summary>
    /// 是否重新生成辅助点的条件
    /// </summary>
    /// <param name="count"></param>
    /// <returns></returns>
    protected virtual bool IsCanInstanceGuidePointObj(out int count)
    {

        count = pointsInfos.Count;
        return guidePointObjs.Count != count;
    }

    /// <summary>
    /// 刷新辅助点位置
    /// </summary>
    protected virtual void ResetSetGuidePoints()
    {
        ResetSetGuidePointsTrans(pointsInfos[0].points);
    }
    /// <summary>
    /// 设置辅助点的位置，缩放等信息
    /// </summary>
    protected void ResetSetGuidePointsTrans(Vector3[] poss)
    {
        for (int i = 0; i < guidePointObjs.Count; i++)
        {
            guidePointObjs[i].transform.position = poss[i];
            guidePointObjs[i].transform.localScale = Vector3.one * guidePointsSize;
        }

    }



    #endregion


    #region 辅助线部分


    Mesh guideLineObjMesh;


    /// <summary>
    /// 创建辅助线
    /// </summary>
    protected virtual void CreatGuideLines()
    {
        GameObject guideLineObj = new GameObject(gameObject.name + "_GuideLine");
        guideLineObj.transform.position = Vector3.zero;
        guideLineObjMesh = new Mesh();

        guideLineObj.AddComponent<MeshFilter>().mesh = guideLineObjMesh;
        guideLineObj.AddComponent<MeshRenderer>().material = mat;
    }

    protected virtual void RefreshGuideLineMehs()
    {
        guideLineObjMesh.Clear();
        guideLineObjMesh.vertices = GetGuideLinePoints();
        guideLineObjMesh.triangles = GetTrianglesVector3s(guideLineObjMesh.vertices);
        guideLineObjMesh.RecalculateNormals();//重置法线
        guideLineObjMesh.RecalculateBounds();   //重置范围
    }

    protected virtual Vector3[] GetGuideLinePoints()
    {
        List<Vector3> tempPoints = new List<Vector3>();
        for (int i = 0; i < guidePointObjs.Count; i++)
        {
            Vector3 tempPos = guidePointObjs[i].transform.position;

            tempPoints.AddRange(GetCubePointFromPoint(tempPos - new Vector3(0, 0, tempPos.z / 2f), new Vector3(0.1f, 0.1f, -tempPos.z)));

            tempPoints.AddRange(GetCubePointFromPoint(tempPos - new Vector3(tempPos.x / 2f, 0, 0), new Vector3(-tempPos.x, 0.1f, 0.1f)));

            tempPoints.AddRange(GetCubePointFromPoint(tempPos - new Vector3(0, tempPos.y / 2f, 0), new Vector3(0.1f, -tempPos.y, 0.1f)));
        }

        return tempPoints.ToArray();
    }



    #endregion


    #region 单位网格线部分

    /// <summary>
    /// 单位网格线物体
    /// </summary>
    GameObject lineObj;
    /// <summary>
    /// 单位网格线的mesh
    /// </summary>
    Mesh lineMesh;

    /// <summary>
    /// 创建计量单位物体
    /// </summary>
    public virtual void CreateUnitObjs()
    {
        lineObj = new GameObject(gameObject.name + "Line");
        lineObj.transform.position = Vector3.zero;
        lineMesh = new Mesh();
        lineObj.AddComponent<MeshFilter>().mesh = lineMesh;
        lineObj.AddComponent<MeshRenderer>();
        lineObj.GetComponent<MeshRenderer>().material = mat;
    }
    /// <summary>
    /// 刷新单位网格线的mesh
    /// </summary>
    void RefreshLineMesh()
    {
        if (lineMesh != null)
        {
            lineMesh.Clear();
            SetXyzValueRange();
            lineMesh.vertices = GetLinePoints();
            lineMesh.triangles = GetTrianglesVector3s(lineMesh.vertices);
            lineMesh.RecalculateNormals();//重置法线
            lineMesh.RecalculateBounds();   //重置范围
        }
    }

    protected float minX, maxX, minY, maxY, minZ, maxZ;

    protected int xCount, yCount, zCount;

    /// <summary>
    /// 设置xyz数值的范围
    /// </summary>
    protected virtual void SetXyzValueRange()
    {
        List<Vector3> tempSortV3 = pointsInfos[0].points.ToList();
        tempSortV3.Sort((x, y) => { if (x.y > y.y) return 1; else if (x == y) return 0; else return -1; });


        minX = 0;
        maxX = pointsInfos[pointsInfos.Count - 1].points[0].x;

        maxY = tempSortV3[tempSortV3.Count - 1].y;
        minY = 0;

        minZ = 0;
        maxZ = pointsInfos[0].points[0].z;

        xCount = Mathf.Abs((int)(maxX - minX));
        yCount = Mathf.Abs((int)(maxY - minY));
        zCount = Mathf.Abs((int)(maxZ - minZ));

    }

    protected virtual Vector3[] GetLinePoints()
    {

        List<Vector3> tempLinePoints = new List<Vector3>();
        for (int i = 0; i < xCount + 1; i++)
        {
            AddLinePoint(tempLinePoints, i, i + unitLineThickness.x, minY, minY + unitLineThickness.y, maxZ > 0 ? minZ : maxZ, maxZ > 0 ? maxZ : minZ);
        }
        for (int i = 0; i < xCount + 1; i++)
        {
            AddLinePoint(tempLinePoints, i, i + unitLineThickness.x, minY, maxY, minZ, minZ + unitLineThickness.z);
        }

        for (int i = 0; i < yCount + 1; i++)
        {
            AddLinePoint(tempLinePoints, minX, minX + unitLineThickness.x, i, i + unitLineThickness.y, maxZ > 0 ? minZ : maxZ, maxZ > 0 ? maxZ : minZ);
        }
        for (int i = 0; i < yCount + 1; i++)
        {
            AddLinePoint(tempLinePoints, minX, maxX, i, i + unitLineThickness.y, minZ, minZ + unitLineThickness.z);
        }

        for (int i = 0; i < zCount + 1; i++)
        {
            AddLinePoint(tempLinePoints, minX, maxX, minY, minY + unitLineThickness.y, maxZ > 0 ? i : i * -1, (maxZ > 0 ? i : i * -1) + unitLineThickness.z);
        }

        for (int i = 0; i < zCount + 1; i++)
        {
            AddLinePoint(tempLinePoints, minX, minX + unitLineThickness.x, minY, maxY, maxZ > 0 ? i : i * -1, (maxZ > 0 ? i : i * -1) + unitLineThickness.z);
        }


        return tempLinePoints.ToArray();
    }


    /// <summary>
    /// 通过3个轴的最大值于最小值确定立方体
    /// </summary>
    /// <param name="tempLinePoints"></param>
    /// <param name="tempMinX"></param>
    /// <param name="tempMaxX"></param>
    /// <param name="tempMinY"></param>
    /// <param name="tempMaxY"></param>
    /// <param name="tempMinZ"></param>
    /// <param name="tempMaxZ"></param>
    protected void AddLinePoint(List<Vector3> tempLinePoints, float tempMinX, float tempMaxX, float tempMinY, float tempMaxY, float tempMinZ, float tempMaxZ)
    {
        tempLinePoints.Add(new Vector3(tempMinX, tempMinY, tempMinZ));
        tempLinePoints.Add(new Vector3(tempMinX, tempMinY, tempMaxZ));
        tempLinePoints.Add(new Vector3(tempMinX, tempMaxY, tempMaxZ));
        tempLinePoints.Add(new Vector3(tempMinX, tempMaxY, tempMinZ));

        tempLinePoints.Add(new Vector3(tempMinX, tempMinY, tempMinZ));
        tempLinePoints.Add(new Vector3(tempMinX, tempMaxY, tempMinZ));
        tempLinePoints.Add(new Vector3(tempMaxX, tempMaxY, tempMinZ));
        tempLinePoints.Add(new Vector3(tempMaxX, tempMinY, tempMinZ));

        tempLinePoints.Add(new Vector3(tempMinX, tempMaxY, tempMinZ));
        tempLinePoints.Add(new Vector3(tempMinX, tempMaxY, tempMaxZ));
        tempLinePoints.Add(new Vector3(tempMaxX, tempMaxY, tempMaxZ));
        tempLinePoints.Add(new Vector3(tempMaxX, tempMaxY, tempMinZ));

        tempLinePoints.Add(new Vector3(tempMaxX, tempMinY, tempMaxZ));
        tempLinePoints.Add(new Vector3(tempMaxX, tempMaxY, tempMaxZ));
        tempLinePoints.Add(new Vector3(tempMinX, tempMaxY, tempMaxZ));
        tempLinePoints.Add(new Vector3(tempMinX, tempMinY, tempMaxZ));

        tempLinePoints.Add(new Vector3(tempMaxX, tempMinY, tempMinZ));
        tempLinePoints.Add(new Vector3(tempMaxX, tempMinY, tempMaxZ));
        tempLinePoints.Add(new Vector3(tempMinX, tempMinY, tempMaxZ));
        tempLinePoints.Add(new Vector3(tempMinX, tempMinY, tempMinZ));

        tempLinePoints.Add(new Vector3(tempMaxX, tempMaxY, tempMinZ));
        tempLinePoints.Add(new Vector3(tempMaxX, tempMaxY, tempMaxZ));
        tempLinePoints.Add(new Vector3(tempMaxX, tempMinY, tempMaxZ));
        tempLinePoints.Add(new Vector3(tempMaxX, tempMinY, tempMinZ));
    }
    #endregion


    /// <summary>
    /// 创建mesh
    /// </summary>
    public virtual void CreateMesh()
    {
        if (pointsInfos.Count != meshs.Count)
        {
            DeleteAllItemObjs();
            GameObject objParent = new GameObject();
            objParent.name = meshName;
            objParent.transform.position = Vector3.zero;
            allChartParentObj.Add(objParent);
            for (int i = 0; i < pointsInfos.Count; i++)
            {
                GameObject itemObj = new GameObject();
                itemObj.transform.parent = objParent.transform;
                itemObj.name = meshName + "_Child_" + i.ToString();
                Mesh mesh = new Mesh();
                meshs.Add(mesh);
                itemObj.AddComponent<MeshFilter>().mesh = mesh;
                itemObj.AddComponent<MeshRenderer>();
                itemObj.GetComponent<MeshRenderer>().material = mat;
                allChartItemObj.Add(itemObj);
            }
        }
    }

    protected void DeleteAllItemObjs()
    {
        for (int i = 0; i < allChartParentObj.Count; i++)
        {
            DestroyImmediate(allChartParentObj[i]);
        }
        allChartParentObj.Clear();
        allChartItemObj.Clear();
        meshs.Clear();
    }

    /// <summary>
    /// 设置顶点信息
    /// </summary>
    public virtual void SetMeshInfo()
    {
        triangles.Clear();
        verticesAddOffset.Clear();

        for (int i = 0; i < pointsInfos.Count; i++)
        {
            Vector3[] vertices = GetPointsVector3s(pointsInfos[i]);
            verticesAddOffset.Add(new PointsInfo(SetVerticesOffset(vertices, pointsInfos[i]), Vector3.one));
            triangles.Add(GetTrianglesVector3s(vertices));
        }

    }


    public virtual void ApplyValue()
    {
        for (int i = 0; i < meshs.Count; i++)
        {
            meshs[i].Clear();
            meshs[i].vertices = verticesAddOffset[i].points;
            meshs[i].triangles = triangles[i];
            meshs[i].RecalculateNormals();//重置法线
                                          //  meshs[i].normals = ResetNormals(meshs[i].vertices, meshs[i].normals);
            meshs[i].RecalculateBounds();   //重置范围
        }

    }

    //Vector3[] ResetNormals(Vector3[] vertices, Vector3[] normals)
    //{
    //    for (int i = 0; i < vertices.Length; i++)
    //    {
    //        List<int> indexes = new List<int>();
    //        for (int j = 0; j < vertices.Length; j++)
    //        {
    //            if (vertices[i] == vertices[j] && i != j)
    //            {
    //                indexes.Add(i);
    //            }
    //        }
    //        Vector3 tempMiddle = Vector3.zero;
    //        for (int j = 0; j < indexes.Count; j++)
    //        {
    //            tempMiddle += normals[indexes[j]];
    //        }
    //        for (int j = 0; j < indexes.Count; j++)
    //        {
    //            normals[indexes[j]] = tempMiddle;
    //        }
    //    }

    //    return normals;
    //}



    /// <summary>
    /// 根据顶点和位移差获取最终位置
    /// </summary>
    /// <param name="vertices"></param>
    /// <param name="index"></param>
    /// <returns></returns>
    public virtual Vector3[] SetVerticesOffset(Vector3[] vertices, PointsInfo _points)
    {
        return vertices;
    }



    /// <summary>
    /// 来自大佬的方法，求直线上的投影点
    /// </summary>
    /// <param name="P"> 直线外的点</param>
    /// <param name="A">直线上点</param>
    /// <param name="B">直线上点</param>
    /// <returns></returns>
    protected Vector3 LinePointProjection(Vector3 P, Vector3 A, Vector3 B)
    {
        Vector3 v = B - A;
        return A + v * (Vector3.Dot(v, P - A) / Vector3.Dot(v, v));
    }

    /// <summary>
    /// 获取正方体的每个面顶点坐标
    /// </summary>
    /// <param name="count"></param>
    /// <returns></returns>
    protected virtual Vector3[] GetPointsVector3s(PointsInfo _points)
    {

        List<Vector3> tempPoints = new List<Vector3>();


        return tempPoints.ToArray();
    }

    /// <summary>
    /// 根据中心点与尺寸范围生成cube
    /// </summary>
    /// <param name="centerPoint"></param>
    /// <param name="size"></param>
    /// <returns></returns>
    protected List<Vector3> GetCubePointFromPoint(Vector3 centerPoint, Vector3 size, bool haveLeft = true, bool haveRight = true, bool haveFoward = true, bool haveBack = true, bool haveUp = true, bool haveDown = true)
    {
        List<Vector3> tempPoints = new List<Vector3>();
        if (haveDown)
        {
            tempPoints.Add(centerPoint + new Vector3(-size.x / 2f, -size.y / 2f, -size.z / 2f));
            tempPoints.Add(centerPoint + new Vector3(size.x / 2f, -size.y / 2f, -size.z / 2f));
            tempPoints.Add(centerPoint + new Vector3(size.x / 2f, -size.y / 2f, size.z / 2f));
            tempPoints.Add(centerPoint + new Vector3(-size.x / 2f, -size.y / 2f, size.z / 2f));
        }

        if (haveBack)
        {
            tempPoints.Add(centerPoint + new Vector3(-size.x / 2f, -size.y / 2f, -size.z / 2f));
            tempPoints.Add(centerPoint + new Vector3(-size.x / 2f, size.y / 2f, -size.z / 2f));
            tempPoints.Add(centerPoint + new Vector3(size.x / 2f, size.y / 2f, -size.z / 2f));
            tempPoints.Add(centerPoint + new Vector3(size.x / 2f, -size.y / 2f, -size.z / 2f));
        }

        if (haveLeft)
        {
            tempPoints.Add(centerPoint + new Vector3(-size.x / 2f, -size.y / 2f, -size.z / 2f));
            tempPoints.Add(centerPoint + new Vector3(-size.x / 2f, -size.y / 2f, size.z / 2f));
            tempPoints.Add(centerPoint + new Vector3(-size.x / 2f, size.y / 2f, size.z / 2f));
            tempPoints.Add(centerPoint + new Vector3(-size.x / 2f, size.y / 2f, -size.z / 2f));
        }

        if (haveFoward)
        {
            tempPoints.Add(centerPoint + new Vector3(-size.x / 2f, -size.y / 2f, size.z / 2f));
            tempPoints.Add(centerPoint + new Vector3(size.x / 2f, -size.y / 2f, size.z / 2f));
            tempPoints.Add(centerPoint + new Vector3(size.x / 2f, size.y / 2f, size.z / 2f));
            tempPoints.Add(centerPoint + new Vector3(-size.x / 2f, size.y / 2f, size.z / 2f));
        }

        if (haveRight)
        {
            tempPoints.Add(centerPoint + new Vector3(size.x / 2f, -size.y / 2f, -size.z / 2f));
            tempPoints.Add(centerPoint + new Vector3(size.x / 2f, size.y / 2f, -size.z / 2f));
            tempPoints.Add(centerPoint + new Vector3(size.x / 2f, size.y / 2f, size.z / 2f));
            tempPoints.Add(centerPoint + new Vector3(size.x / 2f, -size.y / 2f, size.z / 2f));
        }

        if (haveUp)
        {
            tempPoints.Add(centerPoint + new Vector3(-size.x / 2f, size.y / 2f, -size.z / 2f));
            tempPoints.Add(centerPoint + new Vector3(-size.x / 2f, size.y / 2f, size.z / 2f));
            tempPoints.Add(centerPoint + new Vector3(size.x / 2f, size.y / 2f, size.z / 2f));
            tempPoints.Add(centerPoint + new Vector3(size.x / 2f, size.y / 2f, -size.z / 2f));
        }


        if (size.x < 0 || size.y < 0 || size.z < 0)
        {
            tempPoints.Reverse();
        }

        return tempPoints;
    }


    /// <summary>
    /// 设置三角面索引
    /// </summary>
    /// <param name="vertices"></param>
    /// <returns></returns>
    protected int[] GetTrianglesVector3s(Vector3[] vertices)
    {
        List<int> all = new List<int>();
        for (int i = 0; i < vertices.Length; i++)
        {
            if (i % 4 == 0) //每个面四个顶点单独设置
            {
                SetIndex(all, i);
            }
        }
        return all.ToArray();
    }

    /// <summary>
    /// 通过每个面设置三角形索引
    /// </summary>
    /// <param name="ls"></param>
    /// <param name="i"></param>
    protected void SetIndex(List<int> ls, int i)
    {
        ls.Add(i);
        ls.Add(i + 1);
        ls.Add(i + 2);
        ls.Add(i);
        ls.Add(i + 2);
        ls.Add(i + 3);
    }
}



[System.Serializable]
public partial class PointsInfo
{
    public Vector3[] points;
    /// <summary>
    /// 三个轴向的尺寸
    /// </summary>
    public Vector3 size;
    public PointsInfo(Vector3[] _points, Vector3 _size)
    {
        points = _points;
        size = _size;
    }
}

[System.Serializable]
public class Indexes
{
    public int[] index;
    public Indexes(int[] _index)
    {
        index = _index;
    }
}