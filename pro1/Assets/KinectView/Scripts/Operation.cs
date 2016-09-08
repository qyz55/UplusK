using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;
using Kinect = Windows.Kinect;
using UnityEngine.UI;

public class Operation : MonoBehaviour {

    public ModelManager ModelManager;
    public handModels leftHand;
    public handModels rightHand;
    public GameObject BodySourceManager;
    private BodySourceManager _BodyManager;

    private Vector3 prevLeftDisplayPos;
    private Vector3 prevRightDisplayPos;
    private Vector3 nowLeftDisplayPos;
    private Vector3 nowRightDisplayPos;
    private float catchThreshold = 5F;
    private bool leftOccupied = false;
    private int leftCatching = -1;
    private bool rightOccupied = false;
    private int rightCatching = -1;

    private int startViewCountDown;

    public Teaching Teaching;

    Kinect.HandState preLeft = Kinect.HandState.Unknown;
    Kinect.HandState preRight = Kinect.HandState.Unknown;

    Queue<Vector3> leftHandPos = new Queue<Vector3>();
    Queue<Vector3> rightHandPos = new Queue<Vector3>();
    Vector3 leftPosNow;
    Vector3 rightPosNow;
    Vector3 leftPosSum = new Vector3(0, 0, 0);
    Vector3 rightPosSum = new Vector3(0, 0, 0);

    Queue<Kinect.HandState> leftHandHist = new Queue<Kinect.HandState>();
    Queue<Kinect.HandState> rightHandHist = new Queue<Kinect.HandState>();
    Dictionary<Kinect.HandState, int> cntLeftHandHist = new Dictionary<Kinect.HandState, int> { { Kinect.HandState.Open, 0 }, { Kinect.HandState.Closed, 0 }, { Kinect.HandState.Lasso, 0 }, { Kinect.HandState.NotTracked, 0 }, { Kinect.HandState.Unknown, 0 } };
    Dictionary<Kinect.HandState, int> cntRightHandHist = new Dictionary<Kinect.HandState, int> { { Kinect.HandState.Open, 0 }, { Kinect.HandState.Closed, 0 }, { Kinect.HandState.Lasso, 0 }, { Kinect.HandState.NotTracked, 0 }, { Kinect.HandState.Unknown, 0 } };
    Kinect.HandState HandLeftState;
    Kinect.HandState HandRightState;

    List<Vector3> modelPos;
	// Use this for initialization

	void Start () {
        //ModelManager = new ModelManager();
        //Teaching = new Teaching();
        GameObject.Find("HandsHints").GetComponent<Text>().text = "";
        GameObject.Find("LeftTeachingHints").GetComponent<Text>().text = "";
        GameObject.Find("RightTeachingHints").GetComponent<Text>().text = "";
	}

	// Update is called once per frame

    public void LetGo()
    {
        leftOccupied = false;
        leftCatching = -1;
        rightOccupied = false;
        rightCatching = -1;
        HandLeftState = Kinect.HandState.Open;
        HandRightState = Kinect.HandState.Open;
        for (int i = 0; i < 5; ++i)
        {
            leftHandHist.Enqueue(HandLeftState);
            cntLeftHandHist[HandLeftState]++;
            cntLeftHandHist[leftHandHist.Dequeue()]--;
            rightHandHist.Enqueue(HandLeftState);
            cntRightHandHist[HandRightState]++;
            cntRightHandHist[rightHandHist.Dequeue()]--;
        }
    }

    void FixedUpdate()
    {

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

            //维护手位置队列
            leftPosNow = new Vector3(body.Joints[Kinect.JointType.HandLeft].Position.X, body.Joints[Kinect.JointType.HandLeft].Position.Y, body.Joints[Kinect.JointType.HandLeft].Position.Z);
            rightPosNow = new Vector3(body.Joints[Kinect.JointType.HandRight].Position.X, body.Joints[Kinect.JointType.HandRight].Position.Y, body.Joints[Kinect.JointType.HandRight].Position.Z);
            leftHandPos.Enqueue(leftPosNow);
            rightHandPos.Enqueue(rightPosNow);
            leftPosSum += leftPosNow;
            rightPosSum += rightPosNow;
            if (leftHandPos.Count > 20)
            {
                leftPosSum -= leftHandPos.Dequeue();
                rightPosSum -= rightHandPos.Dequeue();
            }

            //维护手状态队列
            if (body.HandLeftState == Kinect.HandState.Closed || body.HandLeftState == Kinect.HandState.Open)
            {
                leftHandHist.Enqueue(body.HandLeftState);
                ++cntLeftHandHist[body.HandLeftState];
            }
            if (leftHandHist.Count > 5)
            {
                --cntLeftHandHist[leftHandHist.Dequeue()];
            }
            if (body.HandRightState == Kinect.HandState.Closed || body.HandRightState == Kinect.HandState.Open)
            {
                rightHandHist.Enqueue(body.HandRightState);
                ++cntRightHandHist[body.HandRightState];
            }
            if (rightHandHist.Count > 5)
            {
                --cntRightHandHist[rightHandHist.Dequeue()];
            }
            HandLeftState = Kinect.HandState.Open;
            HandRightState = Kinect.HandState.Open;
            if (cntLeftHandHist[Kinect.HandState.Closed] > cntLeftHandHist[HandLeftState])
                HandLeftState = Kinect.HandState.Closed;
            /*if (cntLeftHandHist[Kinect.HandState.Lasso] >= cntLeftHandHist[HandLeftState])
                HandLeftState = Kinect.HandState.Lasso;*/
            if (cntRightHandHist[Kinect.HandState.Closed] > cntRightHandHist[HandRightState])
                HandRightState = Kinect.HandState.Closed;
            /*if (cntRightHandHist[Kinect.HandState.Lasso] >= cntRightHandHist[HandRightState])
                HandRightState = Kinect.HandState.Lasso;*/

            if (HandLeftState == Kinect.HandState.Open)
            {
                leftOccupied = false;
                leftCatching = -1;
            }
            if (HandRightState == Kinect.HandState.Open)
            {
                rightOccupied = false;
                rightCatching = -1;
            }
            //print((leftPosSum / leftHandPos.Count).z);

            prevLeftDisplayPos = nowLeftDisplayPos;
            prevRightDisplayPos = nowRightDisplayPos;

            nowLeftDisplayPos = new Vector3((leftPosSum / leftHandPos.Count).x * 70/* + offsetLeftX*/, (leftPosSum / leftHandPos.Count).y * 50/* + offsetLeftY*/, (2 - (leftPosSum / leftHandPos.Count).z) * 40/* + offsetLeftZ*/);
            nowRightDisplayPos = new Vector3((rightPosSum / rightHandPos.Count).x * 70/* + offsetRightX*/, (rightPosSum / rightHandPos.Count).y * 50/* + offsetRightY*/, (2 - (rightPosSum / rightHandPos.Count).z) * 40/* + offsetRightZ*/);

            //print("left: " + leftX + " " + nowLeftDisplayPos.y + " " + nowLeftDisplayPos.z);
            //print("right: " + nowRightDisplayPos.x + " " + nowRightDisplayPos.y + " " + nowRightDisplayPos.z);
            leftHand.transform.position = nowLeftDisplayPos;
            rightHand.transform.position = nowRightDisplayPos;
            if (HandLeftState != preLeft)
            {
                leftHand.setStatus(false, HandLeftState);
                preLeft = HandLeftState;
            }
            if (HandRightState != preRight)
            {
                rightHand.setStatus(true, HandRightState);
                preRight = HandRightState;
            }

            if (ModelManager.isInTeachMode)
            {
                //正在教手势 教视野变换、移动在后面流程中进行判断
                if (Teaching.teachingState == Teaching.State.tryHands)
                {
                    GameObject.Find("TeachingState").GetComponent<Text>().text = "试一下手势吧";
                    if (Teaching.checkHands(HandLeftState, HandRightState))
                    {
                        Teaching.teachingState = Teaching.State.tryLasso;
                        ModelManager.ChangeTeachState(1);
                        Teaching.teachingState = Teaching.State.tryMove;
                        ModelManager.ChangeTeachState(2);
                    }
                    return;
                }
                else if (Teaching.teachingState == Teaching.State.tryLasso)
                {
                    GameObject.Find("TeachingState").GetComponent<Text>().text = "试一下变换视角吧,双手保持Lasso";
                }
                else if (Teaching.teachingState == Teaching.State.tryMove)
                {
                    GameObject.Find("TeachingState").GetComponent<Text>().text = "试着把他们拼起来吧";
                }
                else
                {
                    GameObject.Find("TeachingState").GetComponent<Text>().text = " ";
                }
            }
            /*else
            {
                if (HandLeftState == Kinect.HandState.Closed)
                    GameObject.Find("Main Camera").GetComponent<MainCameraManager>().TransferTo(nowLeftDisplayPos + (new Vector3(0, 0, -10)), nowLeftDisplayPos);
            }*/

            if (leftOccupied && rightOccupied)
            {
                if (leftCatching < ModelManager.ShouldCatch && rightCatching < ModelManager.ShouldCatch)
                {
                    ModelManager.ChangeState(0, StateOfBlock.caught);
                    ModelManager.Move0(leftCatching, (nowLeftDisplayPos - prevLeftDisplayPos + nowRightDisplayPos - prevRightDisplayPos) / 2);
                    ModelManager.ChangeState(0, StateOfBlock.free);
                }
                else if (leftCatching == ModelManager.ShouldCatch && rightCatching == ModelManager.ShouldCatch)
                {
                    ModelManager.ChangeState(leftCatching, StateOfBlock.caught);
                    ModelManager.MoveOne(leftCatching, (nowLeftDisplayPos - prevLeftDisplayPos + nowRightDisplayPos - prevRightDisplayPos) / 2);
                    ModelManager.ChangeState(leftCatching, StateOfBlock.free);
                }
                else
                {
                    if (leftCatching == ModelManager.ShouldCatch)
                    {
                        ModelManager.ChangeState(leftCatching, StateOfBlock.caught);
                        ModelManager.MoveOne(leftCatching, nowLeftDisplayPos - prevLeftDisplayPos);
                        ModelManager.ChangeState(leftCatching, StateOfBlock.free);
                    }
                    else
                    {
                        ModelManager.ChangeState(0, StateOfBlock.caught);
                        ModelManager.Move0(leftCatching, nowLeftDisplayPos - prevLeftDisplayPos);
                        ModelManager.ChangeState(0, StateOfBlock.free);
                    }

                    if (rightCatching == ModelManager.ShouldCatch)
                    {
                        ModelManager.ChangeState(rightCatching, StateOfBlock.caught);
                        ModelManager.MoveOne(rightCatching, nowRightDisplayPos - prevRightDisplayPos);
                        ModelManager.ChangeState(rightCatching, StateOfBlock.free);
                    }
                    else
                    {
                        ModelManager.ChangeState(0, StateOfBlock.caught);
                        ModelManager.Move0(rightCatching, nowRightDisplayPos - prevRightDisplayPos);
                        ModelManager.ChangeState(0, StateOfBlock.free);
                    }
                }
            }
            else
            {
                GameObject.Find("HandsHints").GetComponent<Text>().text = " ";
                int operateLeftNum = 100000;
                float nearestLeftDist = 100000000;
                int operateRightNum = 100000;
                float nearestRightDist = 100000000;
                if (leftOccupied)
                {
                    if (leftCatching == ModelManager.ShouldCatch)
                    {
                        ModelManager.ChangeState(leftCatching, StateOfBlock.caught);
                        ModelManager.MoveOne(leftCatching, nowLeftDisplayPos - prevLeftDisplayPos);
                        ModelManager.ChangeState(leftCatching, StateOfBlock.free);
                    }
                    else
                    {
                        ModelManager.ChangeState(0, StateOfBlock.caught);
                        ModelManager.Move0(leftCatching, nowLeftDisplayPos - prevLeftDisplayPos);
                        ModelManager.ChangeState(0, StateOfBlock.free);
                    }
                }
                else
                {
                    for (int i = 1; i <= ModelManager.ShouldCatch; ++i)
                    {
                        //print("Model " + i + " X:" + modelPos[i].x + " Y:" + modelPos[i].y + " Z:" + modelPos[i].z);
                        float leftDist = (float)System.Math.Sqrt(System.Math.Pow(nowLeftDisplayPos.x - modelPos[i].x, 2)
                            + System.Math.Pow(nowLeftDisplayPos.y - modelPos[i].y, 2)
                            + System.Math.Pow(nowLeftDisplayPos.z - modelPos[i].z, 2));
                        if (leftDist < nearestLeftDist)
                        {
                            nearestLeftDist = leftDist;
                            if (nearestLeftDist < catchThreshold)
                            {
                                operateLeftNum = i;
                            }
                        }
                    }
                    if (operateLeftNum <= ModelManager.ShouldCatch)
                    {
                        if (HandLeftState == Kinect.HandState.Closed)//抓
                        {
                            leftOccupied = true;
                            leftCatching = operateLeftNum;
                        }
                    }
                }


                if (rightOccupied)
                {
                    print(rightCatching);
                    print(operateRightNum);
                    if (rightCatching == ModelManager.ShouldCatch)
                    {
                        ModelManager.ChangeState(rightCatching, StateOfBlock.caught);
                        ModelManager.MoveOne(rightCatching, nowRightDisplayPos - prevRightDisplayPos);
                        ModelManager.ChangeState(rightCatching, StateOfBlock.free);
                    }
                    else
                    {
                        ModelManager.ChangeState(0, StateOfBlock.caught);
                        ModelManager.Move0(rightCatching, nowRightDisplayPos - prevRightDisplayPos);
                        ModelManager.ChangeState(0, StateOfBlock.free);
                    }
                }
                else
                {
                    for (int i = 1; i <= ModelManager.ShouldCatch; ++i)
                    {
                        //print("Model " + i + " X:" + modelPos[i].x + " Y:" + modelPos[i].y + " Z:" + modelPos[i].z);
                        float rightDist = (float)System.Math.Sqrt(System.Math.Pow(nowRightDisplayPos.x - modelPos[i].x, 2)
                            + System.Math.Pow(nowRightDisplayPos.y - modelPos[i].y, 2)
                            + System.Math.Pow(nowRightDisplayPos.z - modelPos[i].z, 2));
                        if (rightDist < nearestRightDist)
                        {
                            nearestRightDist = rightDist;
                            if (nearestRightDist < catchThreshold)
                            {
                                operateRightNum = i;
                            }
                        }
                    }
                    if (operateRightNum <= ModelManager.ShouldCatch)
                    {
                        if (HandRightState == Kinect.HandState.Closed)//抓
                        {
                            rightOccupied = true;
                            rightCatching = operateRightNum;
                        }
                    }
                }

                //shouldCatch灰
                if ( (operateLeftNum == ModelManager.ShouldCatch && !leftOccupied) || (operateRightNum == ModelManager.ShouldCatch && !rightOccupied) )
                    ModelManager.FlashOnGreyForOneFrame(ModelManager.ShouldCatch);
                /*else
                    ModelManager.FlashOffForOneFrame(ModelManager.ShouldCatch);*/

                //0灰
                if ( (operateLeftNum < ModelManager.ShouldCatch && !leftOccupied) || (operateRightNum < ModelManager.ShouldCatch && !rightOccupied) )
                    ModelManager.FlashOnGreyForOneFrame(0);
                /*else
                    ModelManager.FlashOffForOneFrame(0);*/
            }
            //print("Model " + 0 + " X:" + modelPos[0].x + " Y:" + modelPos[0].y + " Z:" + modelPos[0].z);
        }
    }
}
