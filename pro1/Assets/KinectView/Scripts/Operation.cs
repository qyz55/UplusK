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

            if (body.HandLeftState == Kinect.HandState.Lasso && body.HandRightState == Kinect.HandState.Lasso)//双手均Lasso
            {
                /*************
                 * 都在上，视野向上移动
                 * 都在下，视野向下
                 * 都在左，视野向左
                 * 都在右，视野向右
                 * 左偏左右偏右，摄像机前进
                 * 左偏右右偏左，摄像机后退
                 * 左下右上，视野逆时针转动
                 * 左上右下，视野顺时针转动
                 * 左前右后，视野俯视顺时针转动
                 * 左后右前，视野俯视顺时针转动
                 * 
                 * 移动或转动速度与双手相关方向距离有关
                 * */
                if(leftX - standardLeftX < -deltaX && rightX - standardRightX < -deltaX)
                {

                }
                else if (leftX - standardLeftX < -deltaX && rightX - standardRightX > deltaX)
                {
                }
                break;
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
