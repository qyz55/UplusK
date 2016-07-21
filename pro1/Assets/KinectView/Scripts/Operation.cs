﻿using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;
using Kinect = Windows.Kinect;

public class Operation : MonoBehaviour {

    public ModelManager ModelManager;
    public GameObject leftHand;
    public GameObject rightHand;
    List<Vector3> modelPos;
	// Use this for initialization

	void Start () {
        //ModelManager = new ModelManager();
	}

	// Update is called once per frame

    public GameObject BodySourceManager;
    private BodySourceManager _BodyManager;
    private float standardLeftX = -0.5F;
    private float standardLeftY = 0;
    private float standardLeftZ;
    private float standardRightX = 0.5F;
    private float standardRightY = 0;
    private float standardRightZ;
/*    private float deltaX = 1;
    private float deltaY = 1;
    private float deltaZ = 1;*/
    private float threshold = 0.5F;
    private float deltaX = 1;
    private float deltaY = 1;
    private float deltaZ = 1;
    //标准化双手位置，因人而异，可在最初设计流程校准

    private bool rotating = false;
    private int rotatingNum;
    private int cntCancelRotate = 0;

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
            float leftX = body.Joints[Kinect.JointType.HandLeft].Position.X*40;
            float leftY = body.Joints[Kinect.JointType.HandLeft].Position.Y*40;
            float leftZ = body.Joints[Kinect.JointType.HandLeft].Position.Z*40;
            float rightX = body.Joints[Kinect.JointType.HandRight].Position.X*40;
            float rightY = body.Joints[Kinect.JointType.HandRight].Position.Y*40;
            float rightZ = body.Joints[Kinect.JointType.HandRight].Position.Z*40;

            print(leftX + " " + leftY + " " + leftZ);
            leftHand.transform.position = new Vector3(leftX, leftY, leftZ);
            rightHand.transform.position = new Vector3(rightX, rightY, rightZ);

            //正在旋转物体
            if (rotating)
            {
                if (body.HandLeftState != Kinect.HandState.Closed || body.HandRightState != Kinect.HandState.Closed)
                {
                    ++cntCancelRotate;
                    if (cntCancelRotate == 10/*阈值*/)
                    {
                        rotating = false;
                        ModelManager.ChangeState(rotatingNum, StateOfBlock.free);
                    }
                }
                else
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
                        //计算平面角
                        float leftXYangle = (float)System.Math.Atan2(leftY - standardLeftY, leftX - standardLeftX);
                        float rightXYangle = (float)System.Math.Atan2(rightY - standardRightY, rightX - standardRightX);
                        /*float leftZangle = (float)System.Math.Atan2(leftZ - standardLeftZ, System.Math.Sqrt(System.Math.Pow(leftX - standardLeftX, 2)
                                                                                                            + System.Math.Pow(leftY - standardLeftY, 2)));
                        float rightZangle = (float)System.Math.Atan2(rightZ - standardRightZ, System.Math.Sqrt(System.Math.Pow(rightX - standardRightX, 2)
                                                                                                            + System.Math.Pow(rightY - standardRightY, 2)));*/
                        if (leftXYangle > System.Math.PI * 0.75 || leftXYangle < -System.Math.PI * 0.75)
                        {//左手在左
                            if (rightXYangle > System.Math.PI * 0.75 || rightXYangle < -System.Math.PI * 0.75)
                            {//右手在左
                                //绕Y，俯视顺时针
                                ModelManager.Rotate(rotatingNum, 2);
                            }
                            else if (rightXYangle < System.Math.PI * 0.25 && rightXYangle > -System.Math.PI * 0.25)
                            {//右手在右
                                //绕X，左视逆时针
                                ModelManager.Rotate(rotatingNum, 0);
                            }
                            else if (rightXYangle > System.Math.PI * 0.25 && rightXYangle < System.Math.PI * 0.75)
                            {//右手在上
                                //不转
                            }
                            else
                            {//右手在下
                                //不转
                            }
                        }
                        else if (leftXYangle < System.Math.PI * 0.25 && leftXYangle > -System.Math.PI * 0.25)
                        {//左手在右
                            if (rightXYangle > System.Math.PI / 2 || rightXYangle < -System.Math.PI / 2)
                            {//右手在左
                                //绕X，左视顺时针
                                ModelManager.Rotate(rotatingNum, 1);
                            }
                            else if (rightXYangle < System.Math.PI * 0.25 && rightXYangle > -System.Math.PI * 0.25)
                            {//右手在右
                                //绕Y，俯视逆时针
                                ModelManager.Rotate(rotatingNum, 3);
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
                                //不转
                            }
                            else if (rightXYangle < System.Math.PI * 0.25 && rightXYangle > -System.Math.PI * 0.25)
                            {//右手在右
                                //不转
                            }
                            else if (rightXYangle > System.Math.PI * 0.25 && rightXYangle < System.Math.PI * 0.75)
                            {//右手在上
                                //不转
                            }
                            else
                            {//右手在下
                                //绕Z，正视顺时针
                                ModelManager.Rotate(rotatingNum, 4);
                            }
                        }
                        else
                        {//左手在下
                            if (rightXYangle > System.Math.PI / 2 || rightXYangle < -System.Math.PI / 2)
                            {//右手在左
                                //不转
                            }
                            else if (rightXYangle < System.Math.PI * 0.25 && rightXYangle > -System.Math.PI * 0.25)
                            {//右手在右
                                //不转
                            }
                            else if (rightXYangle > System.Math.PI * 0.25 && rightXYangle < System.Math.PI * 0.75)
                            {//右手在上
                                //绕Z，正视逆时针
                                ModelManager.Rotate(rotatingNum, 5);
                            }
                            else
                            {//右手在下
                                //不转
                            }
                        }
                    }
                }
            }
            //双手均Lasso，进行视野变换
            else if (body.HandLeftState == Kinect.HandState.Lasso && body.HandRightState == Kinect.HandState.Lasso)
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
                    /*float leftZangle = (float)System.Math.Atan2(leftZ - standardLeftZ, System.Math.Sqrt(System.Math.Pow(leftX - standardLeftX, 2)
                        + System.Math.Pow(leftY - standardLeftY, 2)));
                    float rightZangle = (float)System.Math.Atan2(rightZ - standardRightZ, System.Math.Sqrt(System.Math.Pow(rightX - standardRightX, 2)
                        + System.Math.Pow(rightY - standardRightY, 2)));*/
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
            }
            //非旋转、非视野，则可进行移动或开始旋转的判断
            else 
                for (int i = 0; i < modelPos.Count; ++i )
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
                    float leftDist = (float)System.Math.Sqrt(System.Math.Pow(leftX - modelPos[i].x, 2)
                        + System.Math.Pow(leftY - modelPos[i].y, 2)
                        + System.Math.Pow(leftZ - modelPos[i].z, 2));
                    float rightDist = (float)System.Math.Sqrt(System.Math.Pow(rightX - modelPos[i].x, 2)
                        + System.Math.Pow(rightY - modelPos[i].y, 2)
                        + System.Math.Pow(rightZ - modelPos[i].z, 2));
                    if (rightDist < 1 && leftDist < 1 && ModelManager.GetState(i) == StateOfBlock.free)//双手均在物体操作范围内 且 该物体free
                    {
                        if (body.HandLeftState == Kinect.HandState.Closed && body.HandRightState == Kinect.HandState.Closed)//双手闭合
                        {
                            //标记开始旋转
                            ModelManager.ChangeState(i, StateOfBlock.caught);
                            rotating = true;
                            rotatingNum = i;
                            cntCancelRotate = 0;
                        }
                        else if (body.HandLeftState == Kinect.HandState.Closed && body.HandRightState == Kinect.HandState.Open)//左手闭合，右手张开
                        {
                            //模型随动
                            ModelManager.ChangeState(i, StateOfBlock.caught);
                            ModelManager.MoveOne(i, new Vector3(leftX, leftY, leftZ));
                            ModelManager.ChangeState(i, StateOfBlock.free);
                        }
                        else if (body.HandLeftState == Kinect.HandState.Open && body.HandRightState == Kinect.HandState.Closed)//右手闭合，左手张开
                        {
                            //模型随动
                            ModelManager.ChangeState(i, StateOfBlock.caught);
                            ModelManager.MoveOne(i, new Vector3(rightX, rightY, rightZ));
                            ModelManager.ChangeState(i, StateOfBlock.free);
                        }
                    }
                    else if (leftDist < 1 && rightDist > 1)//左手在操作范围内
                    {
                        if (body.HandLeftState == Kinect.HandState.Closed)//左手闭合
                        {
                            //模型随动
                            ModelManager.ChangeState(i, StateOfBlock.caught);
                            ModelManager.MoveOne(i, new Vector3(leftX, leftY, leftZ));
                            ModelManager.ChangeState(i, StateOfBlock.free);
                        }
                    }
                    else if (rightDist < 1 && leftDist > 1)//右手在操作范围内
                    {
                        if (body.HandRightState == Kinect.HandState.Closed)//右手闭合
                        {
                            //模型随动
                            ModelManager.ChangeState(i, StateOfBlock.caught);
                            ModelManager.MoveOne(i, new Vector3(rightX, rightY, rightZ));
                            ModelManager.ChangeState(i, StateOfBlock.free);
                        }
                    }
                }
        }
    }
}
