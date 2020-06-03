using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 扇形统计图
/// </summary>
public class SectorChart : ChartBase
{
    List<float> tempPointY = new List<float>();
    /// <summary>
    /// 展示动画
    /// </summary>
    public override void ShowAnim()
    {
        for (int i = 0; i < pointsInfos.Count; i++)
        {
            for (int j = 0; j < pointsInfos[i].itemSectorPercent.Length; j++)
            {
                int tempI = i;
                int tempJ = j;

                StartCoroutine(DoFloatValue(0, pointsInfos[i].itemSectorPercent[j], 1, (value) =>
                {
                    pointsInfos[tempI].itemSectorPercent[tempJ] = value;
                }));
            }
        }
    }


    public override void CreateMesh()
    {

        for (int i = 0; i < pointsInfos.Count; i++)
        {
            if (pointsInfos[i].itemSectorPercent.Length != meshs.Count)
            {
                DeleteAllItemObjs();
                GameObject tempParent = new GameObject();
                tempParent.name = meshName;
                tempParent.transform.position = Vector3.zero;
                allChartParentObj.Add(tempParent);
                for (int j = 0; j < pointsInfos[i].itemSectorPercent.Length; j++)
                {
                    GameObject itemObj = new GameObject();
                    itemObj.name = meshName + "_Child_" + j.ToString();
                    itemObj.transform.parent = tempParent.transform;
                    Mesh mesh = new Mesh();
                    meshs.Add(mesh);
                    itemObj.AddComponent<MeshFilter>().mesh = mesh;
                    itemObj.AddComponent<MeshRenderer>();
                    itemObj.GetComponent<MeshRenderer>().material = mat;
                    allChartItemObj.Add(itemObj);
                }
            }
        }
    }
    /// <summary>
    /// 当前这个扇形的起始角度
    /// </summary>
    float pieceAngle = 0;
    /// <summary>
    /// 当前这个扇形的结束角度
    /// </summary>
    float nextAngle = 0;

    public override void SetMeshInfo()
    {
        triangles.Clear();
        verticesAddOffset.Clear();

        pieceAngle = 0;
        nextAngle = 0;

        for (int j = 0; j < pointsInfos[0].itemSectorPercent.Length; j++)
        {
            Vector3[] vertices = GetPointsVector3s(pointsInfos[0], j);
            verticesAddOffset.Add(new PointsInfo(SetVerticesOffset(vertices, pointsInfos[0]), Vector3.one));
            triangles.Add(GetTrianglesVector3s(vertices));
        }
    }


    /// <summary>
    /// 根据顶点和位移差获取最终位置
    /// </summary>
    /// <param name="vertices"></param>
    /// <param name="index"></param>
    /// <returns></returns>
    public override Vector3[] SetVerticesOffset(Vector3[] vertices, PointsInfo _points)
    {

        Vector3[] tempV3 = new Vector3[vertices.Length];

        for (int i = 0; i < vertices.Length; i++)
        {
            tempV3[i] = vertices[i];

            tempV3[i] += _points.points[0];

        }

        return tempV3;
    }


    /// <summary>
    /// 获取正方体的每个面顶点坐标   （先绘出顶面第一个长方形，再绘制底面第一个长方形，依次连接长方体的面）
    /// </summary>
    /// <param name="count"></param>
    /// <returns></returns>
    protected Vector3[] GetPointsVector3s(PointsInfo _points, int index)
    {

        List<Vector3> tempPoints = new List<Vector3>();

        //立方体初始的点，取内圆心   上表面的一个点
        Vector3 firstUpPoint = new Vector3(_points.size.x * _points.inCirclePercent, -_points.size.y / 2f, 0);
        ///上表面的中心点
        Vector3 centerUp = center + new Vector3(0, _points.size.y / 2f, 0);

        if (index == 0)
        {
            pieceAngle = 0;
        }
        else
        {
            pieceAngle += _points.itemSectorPercent[index - 1] * 360f;
        }

        nextAngle += _points.itemSectorPercent[index] * 360f;
        float offsetAngle = nextAngle - pieceAngle;
        for (int i = 0; i < _points.sertorCount; i++)
        {
            //立方体上表面多边形点
            List<Vector3> tempUpPoints = new List<Vector3>();
            //立方体下表面多边形点
            List<Vector3> tempDownPoints = new List<Vector3>();

            float tempAngle = pieceAngle + (float)(i) / (float)_points.sertorCount * (nextAngle - pieceAngle);
            float tempNextAngle = pieceAngle + (float)(i + 1) / (float)_points.sertorCount * (nextAngle - pieceAngle);
            if (i == 0)
            {
                tempUpPoints.Add(RotateAroundByPointByDistance(firstUpPoint, centerUp, Vector3.up, _points.size.x * _points.inCirclePercent, tempAngle, offsetAngle * _points.insertValue));
                tempUpPoints.Add(RotateAroundByPointByDistance(firstUpPoint, centerUp, Vector3.up, _points.size.x, tempAngle, offsetAngle * _points.insertValue));
            }
            else
            {
                tempUpPoints.Add(RotateAroundByPointByDistance(firstUpPoint, centerUp, Vector3.up, _points.size.x * _points.inCirclePercent, tempAngle));
                tempUpPoints.Add(RotateAroundByPointByDistance(firstUpPoint, centerUp, Vector3.up, _points.size.x, tempAngle));
            }
            if (i == _points.sertorCount - 1)
            {
                tempUpPoints.Add(RotateAroundByPointByDistance(firstUpPoint, centerUp, Vector3.up, _points.size.x, tempNextAngle, -offsetAngle * _points.insertValue));
                tempUpPoints.Add(RotateAroundByPointByDistance(firstUpPoint, centerUp, Vector3.up, _points.size.x * _points.inCirclePercent, tempNextAngle, -offsetAngle * _points.insertValue));
            }
            else
            {
                tempUpPoints.Add(RotateAroundByPointByDistance(firstUpPoint, centerUp, Vector3.up, _points.size.x, tempNextAngle));
                tempUpPoints.Add(RotateAroundByPointByDistance(firstUpPoint, centerUp, Vector3.up, _points.size.x * _points.inCirclePercent, tempNextAngle));
            }


            for (int j = 0; j < tempUpPoints.Count; j++)
            {
                tempDownPoints.Add(tempUpPoints[j] - new Vector3(0, _points.size.y, 0));
            }
            tempPoints.AddRange(GetAllTrianglesPoint(tempUpPoints, tempDownPoints));

        }

        return tempPoints.ToArray();
    }
    /// <summary>
    /// 根据上表面下表面的八个顶点确定立方体的所有点位（顺时针排序）
    /// </summary>
    /// <param name="inputUpV3s">上表面的四个点</param>
    /// <param name="inputDownV3s">下表面的四个点</param>
    /// <returns></returns>
    Vector3[] GetAllTrianglesPoint(List<Vector3> inputUpV3s, List<Vector3> inputDownV3s, string ceMian = "Both")
    {
        Vector3[] outPutPoints = new Vector3[24];

        //顶面
        for (int i = 0; i < inputUpV3s.Count; i++)
        {
            outPutPoints[i] = inputUpV3s[i];
        }
        //内侧
        outPutPoints[4] = inputUpV3s[0];
        outPutPoints[5] = inputUpV3s[3];
        outPutPoints[6] = inputDownV3s[3];
        outPutPoints[7] = inputDownV3s[0];

        //右面
        if (ceMian == "Both" || ceMian == "Right")
        {
            outPutPoints[8] = inputUpV3s[3];
            outPutPoints[9] = inputUpV3s[2];
            outPutPoints[10] = inputDownV3s[2];
            outPutPoints[11] = inputDownV3s[3];
        }
        //外侧
        outPutPoints[12] = inputUpV3s[2];
        outPutPoints[13] = inputUpV3s[1];
        outPutPoints[14] = inputDownV3s[1];
        outPutPoints[15] = inputDownV3s[2];

        //左面
        if (ceMian == "Both" || ceMian == "Left")
        {
            outPutPoints[16] = inputUpV3s[1];
            outPutPoints[17] = inputUpV3s[0];
            outPutPoints[18] = inputDownV3s[0];
            outPutPoints[19] = inputDownV3s[1];
        }

        //底面
        outPutPoints[20] = inputDownV3s[3];
        outPutPoints[21] = inputDownV3s[2];
        outPutPoints[22] = inputDownV3s[1];
        outPutPoints[23] = inputDownV3s[0];

        return outPutPoints;
    }


    public Vector3 RotateAroundByPointByDistance(Vector3 selfPoint, Vector3 rotatePoint, Vector3 axis, float distance, float angle, float offsetAngle = 0)
    {
        Quaternion q = Quaternion.AngleAxis(angle + offsetAngle, axis);
        Vector3 distanceOffset = Vector3.ProjectOnPlane(Vector3.Normalize(new Vector3((1 - axis.x) * selfPoint.x, (1 - axis.y) * selfPoint.y, (1 - axis.z) * selfPoint.z) - new Vector3((1 - axis.x) * rotatePoint.x, (1 - axis.y) * rotatePoint.y, (1 - axis.z) * rotatePoint.z)), axis) * distance;
        return q * distanceOffset;
    }

}



public partial class PointsInfo
{
    /// <summary>
    /// 内圈比例
    /// </summary>
    public float inCirclePercent;
    /// <summary>
    /// 扇形之间的间隔
    /// </summary>
    public float insertValue;
    /// <summary>
    /// 多边形边数
    /// </summary>
    public int sertorCount;

    /// <summary>
    /// 每个扇形的百分比
    /// </summary>
    public float[] itemSectorPercent;
    public PointsInfo(Vector3[] _points, Vector3 _size, float _percent, float _insertValue, int _sertorCount, float[] _itemSectorPercent)
    {
        points = _points;
        size = _size;
        inCirclePercent = _percent;
        insertValue = _insertValue;
        sertorCount = _sertorCount;
        itemSectorPercent = _itemSectorPercent;
    }
}

