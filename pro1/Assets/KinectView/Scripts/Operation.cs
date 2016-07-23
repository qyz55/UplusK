using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;
using Kinect = Windows.Kinect;
using UnityEngine.UI;

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

    private float leftX;
    private float leftY;
    private float leftZ;
    private float rightX;
    private float rightY;
    private float rightZ;

    private float offsetLeftX = 0;
    private float offsetLeftY = 0;
    private float offsetLeftZ = 0;
    private float offsetRightX = 0;
    private float offsetRightY = 0;
    private float offsetRightZ = 0;

    private float standardLeftX;
    private float standardLeftY;
    private float standardLeftZ;
    private float standardRightX;
    private float standardRightY;
    private float standardRightZ;

    private float standardRotateLeftX;
    private float standardRotateLeftY;
    private float standardRotateRightX;
    private float standardRotateRightY;

    private float catchThreshold = 10F;
    private float rotateThreshold = 5F;
    //标准化双手位置，因人而异，可在最初设计流程校准

    private int startViewCountDown;

    private bool rotating = false;
    private int rotatingNum;
    private int startRotateCountDown;
    private int cntCancelRotate = 0;
    private const int cancelRotateThreshold = 120;
    
    private bool lasso = false;
    private int cntCancelLasso = 0;
    private const int cancelLassoThreshold = 120;

	void FixedUpdate () {

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
        int chosenBody = -1;
        float nearestZ=1000000;
        for (int i = 0; i < data.Length; ++i)
        {
            if (data[i] == null)
                continue;
            if (data[i].IsTracked && data[i].Joints[Kinect.JointType.HandLeft].Position.Z + data[i].Joints[Kinect.JointType.HandRight].Position.Z < nearestZ)
            {
                chosenBody = i;
                nearestZ = data[i].Joints[Kinect.JointType.HandLeft].Position.Z + data[i].Joints[Kinect.JointType.HandRight].Position.Z;
            }
        }
        if (chosenBody != -1)
        {
            Kinect.Body body = data[chosenBody];
            leftX = body.Joints[Kinect.JointType.HandLeft].Position.X * 40 + offsetLeftX;
            leftY = body.Joints[Kinect.JointType.HandLeft].Position.Y * 40 + offsetLeftY;
            leftZ = body.Joints[Kinect.JointType.HandLeft].Position.Z * 30 + offsetLeftZ;
            rightX = body.Joints[Kinect.JointType.HandRight].Position.X * 40 + offsetRightX;
            rightY = body.Joints[Kinect.JointType.HandRight].Position.Y * 40 + offsetRightY;
            rightZ = body.Joints[Kinect.JointType.HandRight].Position.Z * 30 + offsetRightZ;

            print("left: " + leftX + " " + leftY + " " + leftZ);
            print("right: " + rightX + " " + rightY + " " + rightZ);
            leftHand.transform.position = new Vector3(leftX, leftY, leftZ);
            rightHand.transform.position = new Vector3(rightX, rightY, rightZ);

            //正在旋转物体
            if (rotating)
            {
                if (startRotateCountDown > 0)
                {
                    GameObject.Find("HandsHints").GetComponent<Text>().text = "把手放到在正常位置并保持一会儿";
                    if (startRotateCountDown == 20)
                    {
                        standardRotateLeftX = leftX;
                        standardRotateLeftY = leftY;
                        standardRotateRightX = rightX;
                        standardRotateRightY = rightY;
                    }
                    startRotateCountDown--;
                    return;
                }

                GameObject.Find("HandsHints").GetComponent<Text>().text = "旋转中";
                if (body.HandLeftState != Kinect.HandState.Closed || body.HandRightState != Kinect.HandState.Closed)
                {
                    GameObject.Find("HandsHints").GetComponent<Text>().text = "退出旋转中";
                    ++cntCancelRotate;
                    if (cntCancelRotate == cancelRotateThreshold)
                    {
                        rotating = false;
                        ModelManager.ChangeState(rotatingNum, StateOfBlock.free);
                    }
                }
                else
                {
                    cntCancelRotate = 0;
                    //计算在XY平面内手与标准位置的距离和连线与空间角，
                    float leftDist = (float)System.Math.Sqrt(System.Math.Pow(leftX - standardRotateLeftX, 2)
                        + System.Math.Pow(leftY - standardRotateLeftY, 2)
                        /*+ System.Math.Pow(leftZ - standardLeftZ, 2)*/);
                    float rightDist = (float)System.Math.Sqrt(System.Math.Pow(rightX - standardRotateRightX, 2)
                        + System.Math.Pow(rightY - standardRotateRightY, 2)
                        /*+ System.Math.Pow(rightZ - standardRightZ, 2)*/);
                    //若左右手距离标准位置的距离均大于XY平面内操作阈值
                    if (leftDist > rotateThreshold && rightDist > rotateThreshold)
                    {
                        //计算平面角
                        float leftXYangle = (float)System.Math.Atan2(leftY - standardRotateLeftY, leftX - standardRotateLeftX);
                        float rightXYangle = (float)System.Math.Atan2(rightY - standardRotateRightY, rightX - standardRotateRightX);
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
                                //绕X，左视逆时针
                                ModelManager.Rotate(rotatingNum, 0);
                            }
                            else
                            {//右手在下
                                //绕Z，正视顺时针
                                ModelManager.Rotate(rotatingNum, 5);
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
                                ModelManager.Rotate(rotatingNum, 4);
                            }
                            else
                            {//右手在下
                                //绕X，左视顺时针
                                ModelManager.Rotate(rotatingNum, 1);
                            }
                        }
                    }
                }
            }
            //双手均Lasso，进行视野变换
            else if (lasso)
            {
                if (startViewCountDown > 0)
                {
                    GameObject.Find("HandsHints").GetComponent<Text>().text = "把手放到正常位置并保持一会儿";
                    print("把手保持在正常位置");
                    if (startViewCountDown == 20)
                    {
                        standardLeftX = leftX;
                        standardLeftY = leftY;
                        standardRightX = rightX;
                        standardRightY = rightY;
                    }
                    startViewCountDown--;
                    return;
                }
                GameObject.Find("HandsHints").GetComponent<Text>().text = "视野变换中";
                print("two hands Lasso");
                if (body.HandLeftState != Kinect.HandState.Lasso || body.HandRightState != Kinect.HandState.Lasso)
                {
                    GameObject.Find("HandsHints").GetComponent<Text>().text = "退出视野变换中";
                    ++cntCancelLasso;
                    if (cntCancelLasso > cancelLassoThreshold)
                    {
                        cntCancelLasso = 0;
                        lasso = false;
                    }
                }
                else
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
                    if (leftDist > rotateThreshold && rightDist > rotateThreshold)
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
                                //视野向左 即物体x+
                                ModelManager.MoveAllByVector3(new Vector3(0.1F, 0, 0));
                                offsetLeftX += 0.1F;
                                offsetRightX += 0.1F;
                                standardLeftX += 0.1F;
                                standardRightX += 0.1F;
                            }
                            else if (rightXYangle < System.Math.PI * 0.25 && rightXYangle > -System.Math.PI * 0.25)
                            {//右手在右
                                //摄像机前进 即物体z+
                                ModelManager.MoveAllByVector3(new Vector3(0, 0, 0.1F));
                                offsetLeftZ += 0.1F;
                                offsetRightZ += 0.1F;
                                standardLeftZ += 0.1F;
                                standardRightZ += 0.1F;
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
                                //摄像机后退 即物体z-
                                ModelManager.MoveAllByVector3(new Vector3(0, 0, -0.1F));
                                offsetLeftZ -= 0.1F;
                                offsetRightZ -= 0.1F;
                                standardLeftZ -= 0.1F;
                                standardRightZ -= 0.1F;
                            }
                            else if (rightXYangle < System.Math.PI * 0.25 && rightXYangle > -System.Math.PI * 0.25)
                            {//右手在右
                                //视野向右
                                ModelManager.MoveAllByVector3(new Vector3(-0.1F, 0, 0));
                                offsetLeftX -= 0.1F;
                                offsetRightX -= 0.1F;
                                standardLeftX -= 0.1F;
                                standardRightX -= 0.1F;
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
                                ModelManager.MoveAllByVector3(new Vector3(0, -0.1F, 0));
                                offsetLeftY -= 0.1F;
                                offsetRightY -= 0.1F;
                                standardLeftY -= 0.1F;
                                standardRightY -= 0.1F;
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
                                ModelManager.MoveAllByVector3(new Vector3(0, 0.1F, 0));
                                offsetLeftY += 0.1F;
                                offsetRightY += 0.1F;
                                standardLeftY += 0.1F;
                                standardRightY += 0.1F;
                            }
                        }
                    }
                }
            }
            else if (body.HandLeftState == Kinect.HandState.Lasso && body.HandRightState == Kinect.HandState.Lasso)
            {
                lasso = true;
                cntCancelLasso = 0;
                startViewCountDown = 120;
            }
            //非Rotating、非Lasso，则可进行移动或开始旋转的判断
            else
            {
                GameObject.Find("HandsHints").GetComponent<Text>().text = "";
                int operateLeftNum = -1;
                int operateRightNum = -1;
                float nearestLeftDist = (float)System.Math.Sqrt(System.Math.Pow(leftX - modelPos[0].x, 2)
                        + System.Math.Pow(leftY - modelPos[0].y, 2)
                        + System.Math.Pow(leftZ - modelPos[0].z, 2));
                float nearestRightDist = (float)System.Math.Sqrt(System.Math.Pow(rightX - modelPos[0].x, 2)
                        + System.Math.Pow(rightY - modelPos[0].y, 2)
                        + System.Math.Pow(rightZ - modelPos[0].z, 2));
                for (int i = 1; i <= ModelManager.ShouldCatch; ++i)
                {
                    print("Model " + i + " X:" + modelPos[i].x + " Y:" + modelPos[i].y + " Z:" + modelPos[i].z);
                    float leftDist = (float)System.Math.Sqrt(System.Math.Pow(leftX - modelPos[i].x, 2)
                        + System.Math.Pow(leftY - modelPos[i].y, 2)
                        + System.Math.Pow(leftZ - modelPos[i].z, 2));
                    float rightDist = (float)System.Math.Sqrt(System.Math.Pow(rightX - modelPos[i].x, 2)
                        + System.Math.Pow(rightY - modelPos[i].y, 2)
                        + System.Math.Pow(rightZ - modelPos[i].z, 2));
                    if (leftDist < nearestLeftDist)
                    {
                        nearestLeftDist = leftDist;
                        if (nearestLeftDist < catchThreshold)
                        {
                            operateLeftNum = i;
                        }
                    }
                    if (rightDist < nearestRightDist)
                    {
                        nearestRightDist = rightDist;
                        if (nearestRightDist < catchThreshold)
                        {
                            operateRightNum = i;
                        }
                    }
                }
                if(operateLeftNum != -1)
                    print("LeftHandOperating " + operateLeftNum + " X:" + modelPos[operateLeftNum].x + " Y:" + modelPos[operateLeftNum].y + " Z:" + modelPos[operateLeftNum].z);
                if(operateRightNum != -1)
                    print("RightHandOperating " + operateRightNum + " X:" + modelPos[operateRightNum].x + " Y:" + modelPos[operateRightNum].y + " Z:" + modelPos[operateRightNum].z);
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
                if (operateLeftNum != -1 && operateRightNum != -1)
                {
                    if (operateLeftNum < ModelManager.ShouldCatch && operateRightNum < ModelManager.ShouldCatch)
                    {
                        print("two hands in zero");
                        if (body.HandLeftState == Kinect.HandState.Closed)
                        {
                            if (body.HandRightState == Kinect.HandState.Open)
                            {
                                print("Moving zero");
                                //模型随动
                                ModelManager.ChangeState(0, StateOfBlock.caught);
                                ModelManager.Move0(operateLeftNum, new Vector3(leftX, leftY, leftZ));
                                ModelManager.ChangeState(0, StateOfBlock.free);
                            }
                            else
                            {
                                //标记开始旋转
                                startRotateCountDown = 120;
                                rotatingNum = 0;
                                ModelManager.ChangeState(rotatingNum, StateOfBlock.caught);
                                rotating = true;
                                cntCancelRotate = 0;
                            }
                        }
                        else if (body.HandRightState == Kinect.HandState.Closed)
                        {
                            if (body.HandLeftState == Kinect.HandState.Open)
                            {
                                print("Moving zero");
                                //模型随动
                                ModelManager.ChangeState(0, StateOfBlock.caught);
                                ModelManager.Move0(operateLeftNum, new Vector3(rightX, rightY, rightZ));
                                ModelManager.ChangeState(0, StateOfBlock.free);
                            }
                            else
                            {
                                //标记开始旋转
                                startRotateCountDown = 120;
                                rotatingNum = 0;
                                ModelManager.ChangeState(rotatingNum, StateOfBlock.caught);
                                rotating = true;
                                cntCancelRotate = 0;
                            }
                        }
                    }
                    else if (operateLeftNum == operateRightNum)
                    {
                        print("two hands in");
                        if (body.HandLeftState == Kinect.HandState.Closed)
                        {
                            if (body.HandRightState == Kinect.HandState.Open)
                            {
                                print("two hands in && left hand closed");
                                print("Moving");
                                //模型随动
                                ModelManager.ChangeState(operateLeftNum, StateOfBlock.caught);
                                ModelManager.MoveOne(operateLeftNum, new Vector3(leftX, leftY, leftZ));
                                ModelManager.ChangeState(operateLeftNum, StateOfBlock.free);
                            }
                            else
                            {
                                print("two hands in && two hands closed");
                                //标记开始旋转
                                startRotateCountDown = 200;
                                rotatingNum = operateLeftNum;
                                ModelManager.ChangeState(rotatingNum, StateOfBlock.caught);
                                rotating = true;
                                cntCancelRotate = 0;
                            }
                        }
                        else if (body.HandRightState == Kinect.HandState.Closed)
                        {
                            if (body.HandLeftState == Kinect.HandState.Open)
                            {
                                print("two hands in && right hand closed");
                                print("Moving");
                                //模型随动
                                ModelManager.ChangeState(operateLeftNum, StateOfBlock.caught);
                                ModelManager.MoveOne(operateLeftNum, new Vector3(rightX, rightY, rightZ));
                                ModelManager.ChangeState(operateLeftNum, StateOfBlock.free);
                            }
                            else
                            {
                                print("two hands in && two hands closed");
                                //标记开始旋转
                                startRotateCountDown = 200;
                                rotatingNum = operateLeftNum;
                                ModelManager.ChangeState(rotatingNum, StateOfBlock.caught);
                                rotating = true;
                                cntCancelRotate = 0;
                            }
                        }
                        /*if (body.HandLeftState == Kinect.HandState.Closed && body.HandRightState == Kinect.HandState.Closed)//双手闭合
                        {
                            print("two hands in && two hands closed");
                            //标记开始旋转
                            startRotateCountDown = 200;
                            rotatingNum = operateLeftNum;
                            ModelManager.ChangeState(rotatingNum, StateOfBlock.caught);
                            rotating = true;
                            cntCancelRotate = 0;
                        }
                        else if (body.HandLeftState == Kinect.HandState.Closed && body.HandRightState == Kinect.HandState.Open)//左手闭合，右手张开
                        {
                            print("two hands in && left hand closed");
                            print("Moving");
                            //模型随动
                            ModelManager.ChangeState(operateLeftNum, StateOfBlock.caught);
                            ModelManager.MoveOne(operateLeftNum, new Vector3(leftX, leftY, leftZ));
                            ModelManager.ChangeState(operateLeftNum, StateOfBlock.free);
                        }
                        else if (body.HandLeftState == Kinect.HandState.Open && body.HandRightState == Kinect.HandState.Closed)//右手闭合，左手张开
                        {
                            print("two hands in && right hand closed");
                            print("Moving");
                            //模型随动
                            ModelManager.ChangeState(operateLeftNum, StateOfBlock.caught);
                            ModelManager.MoveOne(operateLeftNum, new Vector3(rightX, rightY, rightZ));
                            ModelManager.ChangeState(operateLeftNum, StateOfBlock.free);
                        }*/
                    }
                    else
                    {
                        print("left hand in & right hand in");
                        if (body.HandLeftState == Kinect.HandState.Closed)//左手闭合
                        {
                            //模型随动
                            print("Moving");
                            if (operateLeftNum == ModelManager.ShouldCatch)
                            {
                                ModelManager.ChangeState(operateLeftNum, StateOfBlock.caught);
                                ModelManager.MoveOne(operateLeftNum, new Vector3(leftX, leftY, leftZ));
                                ModelManager.ChangeState(operateLeftNum, StateOfBlock.free);
                            }
                            else
                            {
                                ModelManager.ChangeState(0, StateOfBlock.caught);
                                ModelManager.Move0(operateLeftNum, new Vector3(leftX, leftY, leftZ));
                                ModelManager.ChangeState(0, StateOfBlock.free);
                            }
                        }
                        if (body.HandRightState == Kinect.HandState.Closed)//右手闭合
                        {
                            //模型随动
                            print("Moving");
                            if (operateRightNum == ModelManager.ShouldCatch)
                            {
                                ModelManager.ChangeState(operateRightNum, StateOfBlock.caught);
                                ModelManager.MoveOne(operateRightNum, new Vector3(rightX, rightY, rightZ));
                                ModelManager.ChangeState(operateRightNum, StateOfBlock.free);
                            }
                            else
                            {
                                ModelManager.ChangeState(0, StateOfBlock.caught);
                                ModelManager.Move0(operateRightNum, new Vector3(rightX, rightY, rightZ));
                                ModelManager.ChangeState(0, StateOfBlock.free);
                            }
                        }
                    }
                }
                else if (operateLeftNum != -1)
                {
                    print("left hand in");
                    if (operateLeftNum == ModelManager.ShouldCatch)
                    {
                        if (body.HandLeftState == Kinect.HandState.Closed)//左手闭合
                        {
                            //模型随动
                            print("Moving");
                            ModelManager.ChangeState(operateLeftNum, StateOfBlock.caught);
                            ModelManager.MoveOne(operateLeftNum, new Vector3(leftX, leftY, leftZ));
                            ModelManager.ChangeState(operateLeftNum, StateOfBlock.free);
                        }
                    }
                    else
                    {
                        if (body.HandLeftState == Kinect.HandState.Closed)//左手闭合
                        {
                            //模型随动
                            print("Moving");
                            ModelManager.ChangeState(0, StateOfBlock.caught);
                            ModelManager.Move0(operateLeftNum, new Vector3(leftX, leftY, leftZ));
                            ModelManager.ChangeState(0, StateOfBlock.free);
                        }
                    }
                }
                else if (operateRightNum != -1)
                {
                    print("right hand in");
                    if (operateRightNum == ModelManager.ShouldCatch)
                    {
                        if (body.HandRightState == Kinect.HandState.Closed)//右手闭合
                        {
                            //模型随动
                            print("Moving");
                            ModelManager.ChangeState(operateRightNum, StateOfBlock.caught);
                            ModelManager.MoveOne(operateRightNum, new Vector3(rightX, rightY, rightZ));
                            ModelManager.ChangeState(operateRightNum, StateOfBlock.free);
                        }
                    }
                    else
                    {
                        if (body.HandRightState == Kinect.HandState.Closed)//左手闭合
                        {
                            //模型随动
                            print("Moving");
                            ModelManager.ChangeState(0, StateOfBlock.caught);
                            ModelManager.Move0(operateRightNum, new Vector3(rightX, rightY, rightZ));
                            ModelManager.ChangeState(0, StateOfBlock.free);
                        }
                    }
                }
                else
                { }
            }
        }
    }
}
