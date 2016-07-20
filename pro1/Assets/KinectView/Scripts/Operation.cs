using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;
using Kinect = Windows.Kinect;

public class Operation : MonoBehaviour {

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame

    public GameObject BodySourceManager;
    private BodySourceManager _BodyManager;
    private float standardLeftX;
    private float standardLeftY;
    private float standardLeftZ;
    private float standardRightX;
    private float standardRightY;
    private float standardRightZ;
/*    private float deltaX = 1;
    private float deltaY = 1;
    private float deltaZ = 1;*/
    private float threshold = 0.5F;
    private float deltaX = 1;
    private float deltaY = 1;
    private float deltaZ = 1;
    //标准化双手位置，因人而异，可在最初设计流程校准

	void Update () {

        //获取模型位置
        List<Vector3> modelPos = ModelManager.GetAllPosition();

        //获取身体数据
        _BodyManager = BodySourceManager.GetComponent<BodySourceManager>();
        if (_BodyManager == null)
        {
            return;
        }

        Kinect.Body[] data = _BodyManager.GetData();
        if (data == null)
        {
            return;
        }
        foreach (Kinect.Body body in data)
        {
            float leftX = body.Joints[Kinect.JointType.HandLeft].Position.X;
            float leftY = body.Joints[Kinect.JointType.HandLeft].Position.Y;
            float leftZ = body.Joints[Kinect.JointType.HandLeft].Position.Z;
            float rightX = body.Joints[Kinect.JointType.HandRight].Position.X;
            float rightY = body.Joints[Kinect.JointType.HandRight].Position.Y;
            float rightZ = body.Joints[Kinect.JointType.HandRight].Position.Z;



            //双手均Lasso，进行视野变换
            if (body.HandLeftState == Kinect.HandState.Lasso && body.HandRightState == Kinect.HandState.Lasso)
            {
                /*************
                 * 都在上，摄像机和手同步向上移动
                 * 都在下，视野向下
                 * 都在左，视野向左
                 * 都在右，视野向右
                 * 左偏左右偏右，摄像机和手前进
                 * 左偏右右偏左，摄像机和手后退
                 * 左下右上，？摄像机和手？逆时针转动
                 * 左上右下，？摄像机和手？顺时针转动
                 * 左前右后，？各物体？俯视顺时针转动
                 * 左后右前，？各物体？俯视顺时针转动
                 * 
                 * 移动或转动速度与双手相关方向距离有关
                 * */


                
                if (body.HandLeftState == Kinect.HandState.Lasso && body.HandRightState == Kinect.HandState.Lasso)//双手均Lasso
                {
                    //计算在XY平面内手与标准位置的距离和连线与空间角，
                    float leftDist = (float)System.Math.Sqrt(System.Math.Pow(leftX - standardLeftX, 2)
                        + System.Math.Pow(leftY - standardLeftY, 2)
                        /*+ System.Math.Pow(leftZ - standardLeftZ, 2)*/);

                    float rightDist = (float)System.Math.Sqrt(System.Math.Pow(rightX - standardRightX, 2)
                        + System.Math.Pow(rightY - standardRightY, 2)
                        /*+ System.Math.Pow(rightZ - standardRightZ, 2)*/);
                    //若左右手距离标准位置的距离均大于XY平面内操作阈值
                    if (leftDist > threshold && rightDist > threshold)
                    {
                        //计算空间角
                        float leftXYangle = (float)System.Math.Atan2(leftY - standardLeftY, leftX - standardLeftX);
                        float rightXYangle = (float)System.Math.Atan2(rightY - standardRightY, rightX - standardRightX);
                        float leftZangle = (float)System.Math.Atan2(leftZ - standardLeftZ, System.Math.Sqrt(System.Math.Pow(leftX - standardLeftX, 2)
                                                                                                            + System.Math.Pow(leftY - standardLeftY, 2)));
                        float rightZangle = (float)System.Math.Atan2(rightZ - standardRightZ, System.Math.Sqrt(System.Math.Pow(rightX - standardRightX, 2)
                                                                                                            + System.Math.Pow(rightY - standardRightY, 2)));
                        if (leftXYangle > System.Math.PI * 0.75 || leftXYangle < -System.Math.PI * 0.75)
                        {//左手在左
                            if (rightXYangle > System.Math.PI * 0.75 || rightXYangle < -System.Math.PI * 0.75)
                            {//右手在左
                                //视野向左
                            }
                            else if (rightXYangle < System.Math.PI * 0.25 && rightXYangle > -System.Math.PI * 0.25)
                            {//右手在右
                                //摄像机前进
                            }
                            else if (rightXYangle > System.Math.PI * 0.25 && rightXYangle < System.Math.PI * 0.75)
                            {//右手在上
                            }
                            else
                            {//右手在下
                            }
                        }
                        else if (leftXYangle < System.Math.PI * 0.25 && leftXYangle > -System.Math.PI * 0.25)
                        {//左手在右
                            if (rightXYangle > System.Math.PI / 2 || rightXYangle < -System.Math.PI / 2)
                            {//右手在左
                                //摄像机后退
                            }
                            else if (rightXYangle < System.Math.PI * 0.25 && rightXYangle > -System.Math.PI * 0.25)
                            {//右手在右
                                //视野向右
                            }
                            else if (rightXYangle > System.Math.PI * 0.25 && rightXYangle < System.Math.PI * 0.75)
                            {//右手在上
                            }
                            else
                            {//右手在下
                            }
                        }
                        else if (leftXYangle > System.Math.PI * 0.25 && leftXYangle < System.Math.PI * 0.75)
                        {//左手在上
                            if (rightXYangle > System.Math.PI / 2 || rightXYangle < -System.Math.PI / 2)
                            {//右手在左
                            }
                            else if (rightXYangle < System.Math.PI * 0.25 && rightXYangle > -System.Math.PI * 0.25)
                            {//右手在右
                            }
                            else if (rightXYangle > System.Math.PI * 0.25 && rightXYangle < System.Math.PI * 0.75)
                            {//右手在上
                                //视野向上
                            }
                            else
                            {//右手在下
                                //视野右旋
                            }
                        }
                        else
                        {//左手在下
                            if (rightXYangle > System.Math.PI / 2 || rightXYangle < -System.Math.PI / 2)
                            {//右手在左
                            }
                            else if (rightXYangle < System.Math.PI * 0.25 && rightXYangle > -System.Math.PI * 0.25)
                            {//右手在右
                            }
                            else if (rightXYangle > System.Math.PI * 0.25 && rightXYangle < System.Math.PI * 0.75)
                            {//右手在上
                                //视野左旋
                            }
                            else
                            {//右手在下
                                //视野向下
                            }
                        }
                    }
                    /*else if(判断纵深)
                    {}*/

                    /*另一种写法，靠直角坐标系，感觉性能低于极坐标
                    //第一层进行左右判断
                    if(leftX - standardLeftX < -deltaX && rightX - standardRightX < -deltaX)//都左
                    {

                    }
                    else if (leftX - standardLeftX > deltaX && rightX - standardRightX > deltaX)//都右
                    {
                    }
                    else if (leftX - standardLeftX < -deltaX && rightX - standardRightX > deltaX)//左左右右 （摄像机前进）
                    {
                    }
                    else if (leftX - standardLeftX > deltaX && rightX - standardRightX < -deltaX)//左右右左 摄像机后退
                    {
                    }*/
                }
                else foreach (Vector3 pos in modelPos)
                    {
                    /* 各种自带手势
                           * if (body.HandRightState == Kinect.HandState.Closed)
                               print("rightClose");
                           if (body.HandRightState == Kinect.HandState.Open)
                               print("rightOpen");
                           if (body.HandRightState == Kinect.HandState.Lasso)
                               print("rightLesso");
                           if (body.HandLeftState == Kinect.HandState.Closed)
                               print("leftClose");
                           if (body.HandRightState == Kinect.HandState.Open)
                               print("leftOpen");
                           if (body.HandRightState == Kinect.HandState.Lasso)
                               print("leftLasso");*/
                    float leftDist = (float)System.Math.Sqrt(System.Math.Pow(leftX - pos.x, 2)
                        + System.Math.Pow(leftY - pos.y, 2)
                        + System.Math.Pow(leftZ - pos.z, 2));
                    float rightDist = (float)System.Math.Sqrt(System.Math.Pow(rightX - pos.x, 2)
                        + System.Math.Pow(rightY - pos.y, 2)
                        + System.Math.Pow(rightZ - pos.z, 2));
                    if (rightDist < 1 && leftDist < 1)//双手均在物体操作范围内
                    {
                        if (body.HandLeftState == Kinect.HandState.Closed && body.HandRightState == Kinect.HandState.Closed)//双手闭合
                        {
                            //模型旋转Rotate();
                            break;
                        }
                        else if (body.HandLeftState == Kinect.HandState.Closed && body.HandRightState == Kinect.HandState.Open)//左手闭合，右手张开
                        {
                            //模型随动
                            break;
                        }
                        else if (body.HandLeftState == Kinect.HandState.Open && body.HandRightState == Kinect.HandState.Closed)//右手闭合，左手张开
                        {
                            //模型随动
                            break;
                        }
                    }
                    else if (leftDist < 1 && rightDist > 1)//左手在操作范围内
                    {
                        if (body.HandLeftState == Kinect.HandState.Closed)//左手闭合
                        {
                            //模型随动
                            break;
                        }
                    }
                    else if (rightDist < 1 && leftDist > 1)//右手在操作范围内
                    {
                        if (body.HandRightState == Kinect.HandState.Closed)//右手闭合
                        {
                            //模型随动
                            break;
                        }
                    }
                }
            }
        }
        
	}
}
