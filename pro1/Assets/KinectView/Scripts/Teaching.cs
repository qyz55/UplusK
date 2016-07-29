using UnityEngine;
using System.Collections;
using Kinect = Windows.Kinect;
using UnityEngine.UI;

public class Teaching : MonoBehaviour {
    public enum State : int
    {
        tryHands = 0,
        tryRotate = 1,/*?????????????*/
        tryLasso = 2,
        tryMove = 3,
        tryJoint = 4,
        over = 5
    };

    public State teachingState = State.tryHands;






    private bool gotLeft = false;
    private bool gotRight = false;
    private int leftCnt = 0;
    private int rightCnt = 0;

    private bool l_done = false;
    private bool r_done = false;
    private int doneCnt = 0;
    private int handsProgress = 0;//0 try open 1 try closed 2 try lasso

    public int lassoProgress = 0;
    /*public int checkLasso()
    {
 
    }*/

    public bool checkHands(Kinect.HandState l_state, Kinect.HandState r_state)
    {
        if (handsProgress == 3)
        {
            gotLeft = false;
            gotRight = false;
            handsProgress = 0;
            return true;
        }

        switch (handsProgress)
        {
            case 0:
                if (l_done && r_done)
                {
                    print("好了好了好了好了好了");
                    //tip  done congratulations
                    ++doneCnt;
                    if (doneCnt > 60)
                    {
                        l_done = false;
                        r_done = false;
                        ++handsProgress;
                        doneCnt = 0;
                        break;
                    }
                }
                if (!l_done)
                {
                    if (!gotLeft)
                    {
                        GameObject.Find("LeftTeachingHints").GetComponent<Text>().text = "请将左手张开";
                        print("左左左左左左张开张开张开张开");
                        //tip  left hand lost
                        if (l_state == Kinect.HandState.Open)
                        {
                            gotLeft = true;
                            leftCnt = 0;
                        }
                    }
                    else
                    {
                        GameObject.Find("LeftTeachingHints").GetComponent<Text>().text = "左手请保持";
                        //tip  got left hand   please hold
                        if (l_state == Kinect.HandState.Open)
                        {
                            ++leftCnt;
                            if (leftCnt > 60)
                            {
                                l_done = true;
                                gotLeft = false;
                                leftCnt = 0;
                            }
                        }
                        else
                        {
                            gotLeft = false;
                            leftCnt = 0;
                        }
                    }
                }
                else
                {
                    GameObject.Find("LeftTeachingHints").GetComponent<Text>().text = "左手完毕";
                }
                if (!r_done)
                {
                    if (!gotRight)
                    {
                        GameObject.Find("RightTeachingHints").GetComponent<Text>().text = "请将右手张开";
                        print("右右右右右右张开张开张开张开");
                        //tip  right hand lost
                        if (r_state == Kinect.HandState.Open)
                        {
                            gotRight = true;
                            rightCnt = 0;
                        }
                    }
                    else
                    {
                        GameObject.Find("RightTeachingHints").GetComponent<Text>().text = "右手请保持";
                        print("右右右右右右保持保持保持");
                        //tip  got right hand   please hold
                        if (r_state == Kinect.HandState.Open)
                        {
                            ++rightCnt;
                            if (rightCnt > 60)
                            {
                                r_done = true;
                                gotRight = false;
                                rightCnt = 0;
                            }
                        }
                        else
                        {
                            gotRight = false;
                            rightCnt = 0;
                        }
                    }
                }
                else
                {
                    GameObject.Find("RightTeachingHints").GetComponent<Text>().text = "右手完毕";
                }
                break;
            case 1:
                if (l_done && r_done)
                {
                    print("好了好了好了好了好了");
                    //tip  done congratulations
                    ++doneCnt;
                    if (doneCnt > 60)
                    {
                        l_done = false;
                        r_done = false;
                        ++handsProgress;
                        doneCnt = 0;
                        break;
                    }
                }
                if (!l_done)
                {
                    if (!gotLeft)
                    {
                        GameObject.Find("LeftTeachingHints").GetComponent<Text>().text = "请将左手握拳";
                        print("左左左左左左关上关上关上");
                        //tip  left hand lost
                        if (l_state == Kinect.HandState.Closed)
                        {
                            gotLeft = true;
                            leftCnt = 0;
                        }
                    }
                    else
                    {
                        GameObject.Find("LeftTeachingHints").GetComponent<Text>().text = "左手请保持";
                        //tip  got left hand   please hold
                        if (l_state == Kinect.HandState.Closed)
                        {
                            ++leftCnt;
                            if (leftCnt > 60)
                            {
                                l_done = true;
                                gotLeft = false;
                                leftCnt = 0;
                            }
                        }
                        else
                        {
                            gotLeft = false;
                            leftCnt = 0;
                        }
                    }
                }
                else
                {
                    GameObject.Find("LeftTeachingHints").GetComponent<Text>().text = "左手完毕";
                }
                if (!r_done)
                {
                    if (!gotRight)
                    {
                        GameObject.Find("RightTeachingHints").GetComponent<Text>().text = "请将右手握拳";
                        print("右右右右右右关上关上关上");
                        //tip  right hand lost
                        if (r_state == Kinect.HandState.Closed)
                        {
                            gotRight = true;
                            rightCnt = 0;
                        }
                    }
                    else
                    {
                        GameObject.Find("RightTeachingHints").GetComponent<Text>().text = "右手请保持";
                        print("右右右右右右保持保持保持");
                        //tip  got right hand   please hold
                        if (r_state == Kinect.HandState.Closed)
                        {
                            ++rightCnt;
                            if (rightCnt > 60)
                            {
                                r_done = true;
                                gotRight = false;
                                rightCnt = 0;
                            }
                        }
                        else
                        {
                            gotRight = false;
                            rightCnt = 0;
                        }
                    }
                }
                else
                {
                    GameObject.Find("RightTeachingHints").GetComponent<Text>().text = "右手完毕";
                }
                break;
            case 2:
                if (l_done && r_done)
                {
                    print("好了好了好了好了好了");
                    //tip  lasso done congratulations
                    ++doneCnt;
                    if (doneCnt > 60)
                    {
                        l_done = false;
                        r_done = false;
                        ++handsProgress;
                        doneCnt = 0;
                        break;
                    }
                }
                if (!l_done)
                {
                    if (!gotLeft)
                    {
                        GameObject.Find("RightTeachingHints").GetComponent<Text>().text = "请将左手Lasso";
                        print("左左左左左左LassoLassoLasso");
                        //tip  left hand lost
                        if (l_state == Kinect.HandState.Lasso)
                        {
                            gotLeft = true;
                            leftCnt = 0;
                        }
                    }
                    else
                    {
                        GameObject.Find("LeftTeachingHints").GetComponent<Text>().text = "左手请保持";
                        //tip  got left hand   please hold
                        if (l_state == Kinect.HandState.Lasso)
                        {
                            ++leftCnt;
                            if (leftCnt > 60)
                            {
                                l_done = true;
                                gotLeft = false;
                                leftCnt = 0;
                            }
                        }
                        else
                        {
                            gotLeft = false;
                            leftCnt = 0;
                        }
                    }
                }
                else
                {
                    GameObject.Find("LeftTeachingHints").GetComponent<Text>().text = "左手完毕";
                }
                if (!r_done)
                {
                    if (!gotRight)
                    {
                        GameObject.Find("RightTeachingHints").GetComponent<Text>().text = "请将右手Lasso";
                        print("右右右右右右LassoLassoLasso");
                        //tip  right hand lost
                        if (r_state == Kinect.HandState.Lasso)
                        {
                            gotRight = true;
                            rightCnt = 0;
                        }
                    }
                    else
                    {
                        GameObject.Find("RightTeachingHints").GetComponent<Text>().text = "右手请保持";
                        print("右右右右右右保持保持保持");
                        //tip  got right hand   please hold
                        if (r_state == Kinect.HandState.Lasso)
                        {
                            ++rightCnt;
                            if (rightCnt > 60)
                            {
                                r_done = true;
                                gotRight = false;
                                rightCnt = 0;
                            }
                        }
                        else
                        {
                            gotRight = false;
                            rightCnt = 0;
                        }
                    }
                }
                else
                {
                    GameObject.Find("RightTeachingHints").GetComponent<Text>().text = "右手完毕";
                }
                break;
            default:
                break;
        }

        return false;
    }

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
	
	}
}
