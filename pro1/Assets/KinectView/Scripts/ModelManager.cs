using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine.UI;
using HighlightingSystem;

//注意一下所有的位置都是世界坐标，而非相对于父物体的坐标;所有旋转默认是围绕自己的旋转，若有父物体则围绕父物体旋转
public class ModelManager : MonoBehaviour
{
    static public int NumOfPiece = 32;
    static public int TNumOfPiece = 2;
    private int tiaoshi = 0;
    public int CollisionCount = 30;
    public bool jointing = false;
    private int nowRotation = 0;
    private Vector3 rotationEuler;
    private int nowMoving = 0;
    private int FinalMoving = 0;
    private bool inBirth = false;
    private bool needMoving = false;
    private int FinalRotation = 0;
    private Vector3 MoveVector = new Vector3(0, 0, 0);
    public bool rotating = false;
    private List<Model> _Data = new List<Model>();
    public int MoveFrames = 20;
    private Model[] a = new Model[NumOfPiece + 2];
    private Model[] Ta = new Model[TNumOfPiece + 1];
    public GameObject[] b = new GameObject[NumOfPiece + 2];
    public GameObject[] Tb = new GameObject[TNumOfPiece + 1];
    public int ShouldCatch = 1;
    public bool isInTeachMode = false;
    private int TeachState = 0;
    private Vector3[] FocusPosition = { new Vector3(0, 0, 0),   /*1*/    new Vector3(0, 0, 0),       new Vector3(-25, 0, 0),     new Vector3(-10, 0, 0),     new Vector3(-5, 10, 0),     new Vector3(-5, -2, 0),
                                                                /*6*/    new Vector3(-5, -2, 0),       new Vector3(-5, -5, 0),       new Vector3(0, 0, 0),       new Vector3(0, -10, 0),       new Vector3(0, -10, 0),
                                                                /*11*/   new Vector3(0, -10, 0),       new Vector3(0, 0, 0),       new Vector3(0, 0, 0),       new Vector3(0, 0, 0),       new Vector3(0, 0, 0),
                                                                /*16*/   new Vector3(0, 0, 0),       new Vector3(0, 0, 0),       new Vector3(0, 0, 0),       new Vector3(0, 0, 0),       new Vector3(-3, 5, 0),
                                                                /*21*/   new Vector3(-15, 5, 0),       new Vector3(-15, 8, 0),       new Vector3(3, 5, 0),       new Vector3(15, 5, 0),       new Vector3(18, 8, 0),
                                                                /*26*/   new Vector3(0, 5, 0),       new Vector3(0, 8, 0),       new Vector3(0, 8, 0),       new Vector3(-5, 5, 0),       new Vector3(0, 0, 0),
                                                                /*31*/   new Vector3(0, 0, 0),       new Vector3(0, 0, 0)};
    private Vector3[] BirthPosition = { new Vector3(30, 15, 30),   /*1*/     new Vector3(30, 15, 30),        new Vector3(30, 15, 30),        new Vector3(30, 10, 30),        new Vector3(20, 0, 30),         new Vector3(30, 15, 30),
                                                                   /*6*/     new Vector3(30, 15, 30),        new Vector3(30, 15, 30),        new Vector3(30, 15, 30),        new Vector3(30, 15, 20),        new Vector3(-15, 15, 20),
                                                                   /*11*/    new Vector3(20, 15, 30),        new Vector3(-15, 15, 30),        new Vector3(30, 10, 30),        new Vector3(30, 15, 30),        new Vector3(30, 15, 30),
                                                                   /*16*/    new Vector3(30, 15, 30),        new Vector3(30, 15, 30),        new Vector3(30, 15, 30),        new Vector3(30, 15, 30),        new Vector3(-15, -5, 30),
                                                                   /*21*/    new Vector3(-15, -5, 40),        new Vector3(-10, 0, 40),        new Vector3(25, -5, 40),        new Vector3(20, -5, 40),        new Vector3(10, 5, 40),
                                                                   /*26*/    new Vector3(0, 7, 30),        new Vector3(0, 7, 30),        new Vector3(0, 7, 30),        new Vector3(30, 7, 30),        new Vector3(0, -10, 30),
                                                                   /*31*/    new Vector3(0, -10, 30),        new Vector3(0, -10, 30)};
    private Vector3[] BirthFromPosition = { new Vector3(80, 15, 30),   /*1*/     new Vector3(80, 15, 30),        new Vector3(80, 15, 30),        new Vector3(80, 15, 30),        new Vector3(80, 15, 30),        new Vector3(80, 15, 30),
                                                                       /*6*/     new Vector3(80, 15, 30),        new Vector3(80, 15, 30),        new Vector3(80, 15, 30),        new Vector3(80, 15, 20),        new Vector3(-40, 15, 20),
                                                                       /*11*/    new Vector3(80, 15, 30),        new Vector3(-40, 15, 30),        new Vector3(80, 15, 30),        new Vector3(80, 15, 30),        new Vector3(80, 15, 30),
                                                                       /*16*/    new Vector3(80, 15, 30),        new Vector3(80, 15, 30),        new Vector3(80, 15, 30),        new Vector3(80, 15, 30),        new Vector3(-40, -5, 30),
                                                                       /*21*/    new Vector3(-40, -5, 40),        new Vector3(-40, 15, 30),        new Vector3(80, -5, 30),        new Vector3(80, -5, 30),        new Vector3(80, 15, 30),
                                                                       /*26*/    new Vector3(0, 40, 30),        new Vector3(0, 40, 30),        new Vector3(0, 40, 30),        new Vector3(80, 7, 30),        new Vector3(0, -50, 30),
                                                                       /*31*/    new Vector3(0, -50, 30),        new Vector3(0, -50, 30)};
    private Vector3[] AllJointPosition = { Vector3.zero,     /*1*/           new Vector3(2, 0, 0),          new Vector3(3, 0, 0),           new Vector3(0,3,0),             new Vector3(0,4,0),             new Vector3(0,-3,0),
                                                             /*6*/           new Vector3(0,-3,0),           new Vector3(0,-3,0),            new Vector3(0,-3,0),             new Vector3(0,-3,0),             new Vector3(-3,0,-3),
                                                             /*11*/          new Vector3(3,0,3),           new Vector3(-3,0,0),            new Vector3(3,0,0),            new Vector3(3,0,0),            new Vector3(0,-3,0),
                                                             /*16*/          new Vector3(0,-3,0),           new Vector3(0,-3,0),            new Vector3(0,3,0),            new Vector3(0,3,0),            new Vector3(0,-3,0),
                                                             /*21*/          new Vector3(0,-3,0),           new Vector3(0,3,0),            new Vector3(0,-3,0),            new Vector3(0,-3,0),            new Vector3(0,3,0),
                                                             /*26*/          new Vector3(0,3,0),           new Vector3(-3,0,0),            new Vector3(3,0,0),            new Vector3(0,3,0),            new Vector3(0,-3,0),
                                                             /*31*/          new Vector3(0,-3,0),           new Vector3(0,-3,0)};
    private Vector3[] AllMoveToPosition = { new Vector3(0, 0, 30), /*1*/        new Vector3(0, 0, 30),          new Vector3(0, 0, 30),      new Vector3(0, 0, 30),      new Vector3(0, 0, 30),      new Vector3(0, 0, 30),     
                                                                   /*6*/        new Vector3(0, 0, 30),          new Vector3(0, 0, 30),      new Vector3(0, 0, 30),      new Vector3(0, 0, 30),      new Vector3(0, 0, 30),     
                                                                   /*11*/       new Vector3(0, 0, 30),          new Vector3(0, 0, 30),      new Vector3(0, 0, 30),      new Vector3(0, 0, 30),      new Vector3(0, 0, 30),     
                                                                   /*16*/       new Vector3(0, 0, 30),          new Vector3(0, 0, 30),      new Vector3(0, 0, 30),      new Vector3(0, 0, 30),      new Vector3(0, 0, 30),      
                                                                   /*21*/       new Vector3(10, 0, 40),          new Vector3(10, -10, 40),      new Vector3(-10, 0, 40),      new Vector3(-10, 0, 40),      new Vector3(-10, -10, 40),    
                                                                   /*26*/       new Vector3(0, -15, 30),          new Vector3(0, -15, 30),      new Vector3(0, -15, 30),      new Vector3(-10, -15, 30),      new Vector3(0, 5, 30),    
                                                                   /*31*/       new Vector3(0, 5, 30),          new Vector3(0, 5, 30),          new Vector3(0,0,30)};
    

    public float RangeOfAngles = 80.0f;
    public float RangeOfDis = 5;
    private Vector3[] AllCenter = { Vector3.zero,   /*1*/    new Vector3(-20f, 0f, 0f),          new Vector3(-15f, 0f, 0f),          new Vector3(-11f,8f,0f),            new Vector3(-11f,15f,0f),           new Vector3(-11f,-2f,0f),
                                                    /*6*/    new Vector3(-11f,-7.6f,0f),         new Vector3(-11f,-11f,0f),          new Vector3(-9f,-7f,2f),            new Vector3(-6f,-7.5f,5f),          new Vector3(-13f,-7.6f,-2f),
                                                    /*11*/   new Vector3(-12f,-7.5f,1f),         new Vector3(-9.5f,-7.5f,-1.5f),     new Vector3(-6f, 0f, 0f),           new Vector3(3f,0f,0f),              new Vector3(-2f,-2f,0f),
                                                    /*16*/   new Vector3(-2.03f,-4.99f,0.04f),   new Vector3(-1.98f,-9.21f,0.03f),   new Vector3(-1.95f,4.66f,0.04f),    new Vector3(6f,4f,0f),              new Vector3(-1.66f,4.2f,9.9f),
                                                    /*21*/   new Vector3(-7,4,30) ,              new Vector3(20,4,28) ,              new Vector3(-1.61f,4.44f,-9.64f),   new Vector3(-8,3.5f,-28),           new Vector3(20,5,-28),                 
                                                    /*26*/   new Vector3(15.71f,0.54f,0f),       new Vector3(13.21f,1.03f,5.6f),     new Vector3(13.3f,0.98f,-3.8f),     new Vector3(13.2f,5.4f,0f),         new Vector3(3f,-4f,0f),             
                                                    /*31*/   new Vector3(3f,-9f,0f),             new Vector3(3f,-13f,0f)};
    private Vector3[] TAllCenter = { Vector3.zero, Vector3.zero, Vector3.zero };
    private Vector3[] TAllJpintPosition = { Vector3.zero, new Vector3(0, 0, 0), new Vector3(0, 2, 0) };
    public bool inCollision = false;
    public List<Vector3> GetAllPosition() // 获得所有物体的位置,返回一个内容为Vector3的list
    {
        List<Vector3> _position = new List<Vector3>();
        foreach (Model i in _Data)
        {
            _position.Add(CalcRealPosition(i.num,i.model.transform.position));
        }
        return _position;
    }
    public void ChangeState(int num, StateOfBlock state) // 传入一个物体的下标,和想要其变成的状态(包括自由，和被抓取)
    {
        if (jointing || rotating || needMoving || inBirth) return;
        _Data[num].state = state;
    }
    public void MoveOne(int num, Vector3 _v3) // 传入想要移动的物体的下标和想要其移动到的位置
    {
        if (jointing || rotating || needMoving || inBirth) return;
        if (_Data[num].state == StateOfBlock.caught && num == ShouldCatch)
            _Data[num].model.transform.position += _v3;//CalcCenterPosition(num, _v3);
        else
            return;
        SavePos(num);
        TryJoint();
    }
    public void Move0(int num, Vector3 _v3)
    {
        if (jointing || rotating || needMoving || inBirth) return;
        if (_Data[0].state != StateOfBlock.caught)
            return;
        _Data[0].model.transform.position += _v3;// -_Data[0].model.transform.rotation * _Data[num].center;
        SavePos(0);
        TryJoint();
    }
    /*public void MoveOneByVector3(int num, Vector3 _v3)
    {
        _Data[num].model.transform.position += _v3;
    }*/
    public void MoveAllByVector3(Vector3 _V3)
    {
        if (jointing || rotating || needMoving || inBirth) return;
        _Data[0].model.transform.position += _V3;
        _Data[ShouldCatch].model.transform.position += _V3;
        SavePos(0);
        SavePos(ShouldCatch);
    }
    public void SavePos(int num)
    {
        if (inCollision == true)
        {
            _Data[num].model.transform.position = _Data[num].LastPosition;
            _Data[num].model.transform.rotation = _Data[num].LastQuaternion;
            inCollision = false;
            return;
        }
        else
        {
            ++_Data[num].NumOfLast;
            if (_Data[num].NumOfLast >= 100)
            {
                _Data[num].NumOfLast = 0;
                _Data[num].LastPosition = _Data[num].model.transform.position;
                _Data[num].LastQuaternion = _Data[num].model.transform.rotation;
            }
        }
    }
    public void Rotate(int num,int mode) //对下标为num的物体进行6种旋转，0-5分别为x(顺逆)y(顺逆)z(顺逆)
    {
        if (jointing || rotating || needMoving || inBirth) return;
        if (num != ShouldCatch && num != 0)
                return;
        switch (mode)
        {
            case 0:
                _Data[num].model.transform.RotateAround(CalcRealPosition(num, _Data[num].model.transform.position), Vector3.right, 50 * Time.deltaTime);
                break;
            case 1:
                _Data[num].model.transform.RotateAround(CalcRealPosition(num, _Data[num].model.transform.position), Vector3.left, 50 * Time.deltaTime);
                break;
            case 2:
                _Data[num].model.transform.RotateAround(CalcRealPosition(num, _Data[num].model.transform.position), Vector3.up, 50 * Time.deltaTime);
                break;
            case 3:
                _Data[num].model.transform.RotateAround(CalcRealPosition(num, _Data[num].model.transform.position), Vector3.down, 50 * Time.deltaTime);
                break;
            case 4:
                _Data[num].model.transform.RotateAround(CalcRealPosition(num, _Data[num].model.transform.position), Vector3.forward, 50 * Time.deltaTime);
                break;
            case 5:
                _Data[num].model.transform.RotateAround(CalcRealPosition(num, _Data[num].model.transform.position), Vector3.back, 50 * Time.deltaTime);
                break;
            
        }
        SavePos(num);
        TryJoint();
    }
    private Vector3 CalcRealPosition(int num, Vector3 pos)
    {
        return _Data[num].model.transform.rotation * _Data[num].center + pos;
    }//根据物体存储的坐标来计算物体真正的坐标
    private Vector3 CalcCenterPosition(int num, Vector3 pos)
    {
        return pos - _Data[num].model.transform.rotation * _Data[num].center;
    }//根据物体真正的坐标来计算物体用于拼接的存储坐标
    public StateOfBlock GetState(int num) // 获得某个物体的状态
    {
        return _Data[num].state;
    }
    private float min(float x, float y)
    {
        return x < y ? x : y;
    }
    private bool CheckRotation()
    {
        
        Quaternion q = _Data[ShouldCatch].model.transform.rotation * Quaternion.Inverse(_Data[0].model.transform.rotation);
        if (min(q.eulerAngles.x, 360 - q.eulerAngles.x) + min(q.eulerAngles.y, 360 - q.eulerAngles.y) + min(q.eulerAngles.z, 360 - q.eulerAngles.z) < RangeOfAngles)
        {
            Debug.Log("CheckRotation" + q.eulerAngles.x + " " + q.eulerAngles.y + " " + q.eulerAngles.z);
            return true;
            
            
        }
        Debug.Log("AngleFalse");
        return false;
    }
    private void TryJoint()
    {
        if (ShouldCatch == 1 || ShouldCatch == NumOfPiece + 1) return;
        Vector3 d1 = _Data[0].model.transform.position;//CalcRealPosition(1, _Data[1].model.transform.position) - _Data[1].center;
        Vector3 d2 = _Data[ShouldCatch].model.transform.position;//CalcRealPosition(ShouldCatch, _Data[ShouldCatch].model.transform.position) - _Data[ShouldCatch].center;
        float d = (float)(Math.Sqrt((d1.x - d2.x) * (d1.x - d2.x) + (d1.y - d2.y) * (d1.y - d2.y) + (d1.z - d2.z) * (d1.z - d2.z) / 10));
        if (ShouldCatch > tiaoshi)
        {
            if (d > RangeOfDis * 2)
            {
                if (_Data[ShouldCatch].CollisionCount <= 0)
                    _Data[ShouldCatch].model.GetComponent<Highlighter>().ConstantOff();
                if (_Data[0].CollisionCount <= 0)
                    _Data[0].model.GetComponent<Highlighter>().ConstantOff();

            }
            else if (d >= RangeOfDis && CheckRotation() == true)
            {
                if (_Data[ShouldCatch].CollisionCount <= 0)
                    _Data[ShouldCatch].model.GetComponent<Highlighter>().ConstantOn(Color.green);
            }
            else if (d < RangeOfDis)
            {
                if (CheckRotation() == true)
                {
                    GameObject.Find("Operation").GetComponent<Operation>().LetGo();
                    GameObject.Find("Root").transform.Find("leftHand").gameObject.SetActive(false);
                    GameObject.Find("Root").transform.Find("rightHand").gameObject.SetActive(false);
                    GameObject.Find("Main Camera").GetComponent<MainCameraManager>().TransferTo(_Data[ShouldCatch].model.transform.position + new Vector3(0, 0, -20) + FocusPosition[ShouldCatch], _Data[ShouldCatch].model.transform.position + FocusPosition[ShouldCatch]);
                    jointing = true;

                    _Data[ShouldCatch].state = StateOfBlock.jointing;
                    _Data[ShouldCatch].LeftStep1 = MoveFrames;
                    _Data[ShouldCatch].LeftStep2 = MoveFrames;
                    _Data[ShouldCatch].MoveVector = _Data[0].model.transform.position + _Data[ShouldCatch].JointPosition - _Data[ShouldCatch].model.transform.position;
                }
                else
                    _Data[ShouldCatch].model.GetComponent<Highlighter>().ConstantOn(Color.yellow);
            }
        }
        else
        {
            if (CheckRotation() == true)
            {
                GameObject.Find("Operation").GetComponent<Operation>().LetGo();
                GameObject.Find("Root").transform.Find("leftHand").gameObject.SetActive(false);
                GameObject.Find("Root").transform.Find("rightHand").gameObject.SetActive(false);
                GameObject.Find("Main Camera").GetComponent<MainCameraManager>().TransferTo(_Data[ShouldCatch].model.transform.position + new Vector3(0, 0, -20) + FocusPosition[ShouldCatch], _Data[ShouldCatch].model.transform.position + FocusPosition[ShouldCatch]);
                jointing = true;

                _Data[ShouldCatch].state = StateOfBlock.jointing;
                _Data[ShouldCatch].LeftStep1 = MoveFrames;
                _Data[ShouldCatch].LeftStep2 = MoveFrames;
                _Data[ShouldCatch].MoveVector = _Data[0].model.transform.position + _Data[ShouldCatch].JointPosition - _Data[ShouldCatch].model.transform.position;
            }
            else
                _Data[ShouldCatch].model.GetComponent<Highlighter>().ConstantOn(Color.yellow);
        }
    }
    void GetJointed(int i)
    {
        _Data[ShouldCatch].model.GetComponent<Highlighter>().ConstantOff();
        _Data[0].model.GetComponent<Highlighter>().ConstantOff();
        _Data[i].father = 0;
        _Data[i].CollisionCount = 0;
        _Data[i].model.transform.parent = _Data[0].model.transform;
        _Data[i].model.transform.localPosition = Vector3.zero;
        _Data[i].model.transform.localRotation = Quaternion.identity;
        _Data[i].state = StateOfBlock.jointed;
        Vector3 t = Vector3.zero;
        for (int j = 1; j <= i; ++j)
            t += _Data[j].center;
        t /= i;
        _Data[0].center = t;
        ShouldCatch++;
    }
    public void FlashOnGreyForOneFrame(int num)
    {
        if (num == 0)
        {
            if (_Data[0].CollisionCount <= 0)
                for (int i = 1; i < ShouldCatch; ++i)
                    _Data[i].model.GetComponent<Highlighter>().On(Color.grey);
        }
        else
        {
            if (_Data[num].CollisionCount <= 0)
                _Data[num].model.GetComponent<Highlighter>().On(Color.grey);
        }
    }
    public void FlashOffForOneFrame(int num)
    {
        if (num == 0)
        {
            if (_Data[0].CollisionCount <= 0)
                for (int i = 1; i < ShouldCatch; ++i)
                    _Data[i].model.GetComponent<Highlighter>().ConstantOff();
        }
        else
        {
            if (_Data[num].CollisionCount <= 0)
                _Data[num].model.GetComponent<Highlighter>().ConstantOff();
        }
    }
    void Congratulations()
    {
        GameObject.Find("Root").transform.Find("leftHand").gameObject.SetActive(false);
        GameObject.Find("Root").transform.Find("rightHand").gameObject.SetActive(false);
        GameObject.Find("Main Camera").GetComponent<MainCameraManager>().ggRotate(AllMoveToPosition[ShouldCatch] + new Vector3(0, 0, -20), AllMoveToPosition[ShouldCatch]);
        return;
    }
    void Creat(int i)
    {
        if (isInTeachMode)
        {
            if (i > TNumOfPiece) 
            {
                GameObject.Find("HandsHints").GetComponent<Text>().text = "教学模式完成，现已启动主程序。";
                Destroy(GameObject.Find("GameStart")); 
                init(); 
                return; 
            }
            Ta[i] = new Model();
            Ta[i].model = GameObject.Instantiate(Tb[i]);
            Ta[i].num = i;
            Ta[i].state = StateOfBlock.free;
            Ta[i].father = i;
            Ta[i].model.transform.position = BirthPosition[0];
            Ta[i].initialQuaternion = Quaternion.identity;
            Ta[i].LastPosition = BirthPosition[0];
            if (!Ta[i].model.GetComponent<Rigidbody>())
                Ta[i].model.AddComponent<Rigidbody>();
            Ta[i].model.GetComponent<Rigidbody>().useGravity = false;
            Ta[i].model.GetComponent<Rigidbody>().drag = 2000;
            Ta[i].center = TAllCenter[i];
            Ta[i].JointPosition = TAllJpintPosition[i];
            _Data.Add(Ta[i]);
            Ta[i].model.GetComponent<Highlighter>().ReinitMaterials();
            _Data[0].model.GetComponent<Highlighter>().ReinitMaterials();
        }
        else
        {
            if (i > NumOfPiece) { Congratulations(); return; };
            a[i] = new Model();
            a[i].model = GameObject.Instantiate(b[i]);
            a[i].num = i;
            a[i].state = StateOfBlock.free;
            a[i].father = i;
            a[i].model.transform.position = BirthFromPosition[ShouldCatch];
            nowMoving = 0;
            FinalMoving = 80;
            MoveVector = (BirthPosition[ShouldCatch] - BirthFromPosition[ShouldCatch])/FinalMoving;
            inBirth = true;
            a[i].initialQuaternion = _Data[0].model.transform.rotation;
            a[i].model.transform.rotation = _Data[0].model.transform.rotation;
            a[i].LastPosition = BirthPosition[ShouldCatch];
            if (!a[i].model.GetComponent<Rigidbody>())
                a[i].model.AddComponent<Rigidbody>();
            a[i].model.GetComponent<Rigidbody>().useGravity = false;
            a[i].model.GetComponent<Rigidbody>().drag = 2000;
            a[i].center = AllCenter[i];
            a[i].LastQuaternion = a[i].initialQuaternion;
            a[i].JointPosition = AllJointPosition[i];
            _Data.Add(a[i]);
            a[i].model.GetComponent<Highlighter>().ReinitMaterials();
            _Data[0].model.GetComponent<Highlighter>().ReinitMaterials();
            a[i].model.GetComponent<Highlighter>().ConstantOn(Color.green);
        }
    }
    public void init()
    {
        foreach (Model i in _Data)
        {
            Destroy(i.model);
        }
        _Data.Clear();
        _Data.Add(a[0]);
        isInTeachMode = false;
        inCollision = false;
        GameObject.Find("Root").transform.Find("Sphere").gameObject.SetActive(true);
        ShouldCatch = 1;
        Creat(ShouldCatch);
        GetJointed(ShouldCatch);
        Creat(ShouldCatch);
    }
    public void TeachInit()
    {
        isInTeachMode = true;
        GameObject.Find("Root").transform.Find("Cube").gameObject.SetActive(true);
        //ChangeTeachState(1);
        //ChangeTeachState(2);
    }
    public void ChangeTeachState(int num)
    {
        if (num == 1)
        {
            ShouldCatch = 1;
            Creat(ShouldCatch);
            GetJointed(ShouldCatch);
            ShouldCatch--;
        }
        else if (num ==2)
        {
            GameObject.Find("TeachingState").GetComponent<Text>().text = "";
            GameObject.Find("LeftTeachingHints").GetComponent<Text>().text = "";
            GameObject.Find("RightTeachingHints").GetComponent<Text>().text = "";
            _Data[1].model.transform.localPosition = Vector3.zero;
            _Data[1].model.transform.localRotation = Quaternion.identity;
            _Data[0].model.transform.position = new Vector3(0, 0, 30);
            _Data[0].model.transform.rotation = Quaternion.identity;

            ShouldCatch++;
            Creat(ShouldCatch);
        }
    }
    private void TryRotation()
    {
        switch (ShouldCatch)
        {

            case 8:
                rotating = true;
                nowMoving = 0;
                FinalMoving = 40;
                FinalRotation = 80;
                nowRotation = 0;
                rotationEuler = Vector3.up;//new Vector3(1, 1, 1);
                break;
            case 11:
                rotating = true;
                nowMoving = 0;
                FinalMoving = 40;
                FinalRotation = 80;
                nowRotation = 0;
                rotationEuler = Vector3.up;//new Vector3(1, 1, 1);
                break;
            case 13:
                rotating = true;
                nowMoving = 0;
                FinalMoving = 40;
                FinalRotation = 160;
                nowRotation = 0;
                rotationEuler = Vector3.down;//new Vector3(1, 1, 1);
                break;
            case 20:
                rotating = true;
                nowMoving = 0;
                FinalMoving = 40;
                FinalRotation = 150;
                nowRotation = 0;
                rotationEuler = new Vector3(-1,-1,1);//new Vector3(1, 1, 1);
                break;
            case 29:
                rotating = true;
                nowMoving = 0;
                FinalMoving = 40;
                FinalRotation = 150;
                nowRotation = 0;
                rotationEuler = new Vector3(1, 1, -1);//new Vector3(1, 1, 1);
                break;
            default:
                needMoving = true;
                nowMoving = 0;
                FinalMoving = 80;
                MoveVector = (AllMoveToPosition[ShouldCatch] - _Data[0].model.transform.position) / FinalMoving;
                break;
        }
    }
    public void ChangeCollisionCount()
    {
        _Data[0].CollisionCount = CollisionCount;
        _Data[ShouldCatch].CollisionCount = CollisionCount;
    }
	void Start () {
        a[0] = new Model();
        a[0].model = GameObject.Find("Sphere");
        a[0].model.SetActive(false);

        Ta[0] = new Model();
        Ta[0].model = GameObject.Find("Cube");
        _Data.Add(Ta[0]);
        Ta[0].model.SetActive(false);
        ShouldCatch = 1;
	}
    
	
	// Update is called once per frame
	void Update () 
    {
        print(ShouldCatch);
        if (rotating || needMoving || inBirth)
        {
            if (rotating == true)
            {
                ++nowRotation;
                _Data[0].model.transform.RotateAround(CalcRealPosition(1, _Data[1].model.transform.position), rotationEuler, 50 * Time.deltaTime);
                if (nowRotation >= FinalRotation)
                {
                    rotating = false;
                    if (needMoving == false)
                    {
                        MoveVector = (AllMoveToPosition[ShouldCatch] - _Data[0].model.transform.position) / FinalMoving;
                        needMoving = true;
                    }
                }
            }
            if (needMoving == true)
            {
                ++nowMoving;
                _Data[0].model.transform.position += MoveVector;
                if (nowMoving >= FinalMoving)
                {
                    needMoving = false;
                    if (rotating == false)
                    {
                        SavePos(0);
                        Creat(ShouldCatch);
                        GameObject.Find("Operation").GetComponent<Operation>().LetGo();
                    }
                }
            }
            if (inBirth == true)
            {
                ++nowMoving;
                _Data[ShouldCatch].model.transform.position += MoveVector;
                if (nowMoving >= FinalMoving)
                {
                    inBirth = false;
                    GameObject.Find("Root").transform.Find("leftHand").gameObject.SetActive(true);
                    GameObject.Find("Root").transform.Find("rightHand").gameObject.SetActive(true);
                    _Data[ShouldCatch].model.GetComponent<Highlighter>().ConstantOff();
                }
            }
        }
        else if (jointing == true)
        {
            foreach (Model i in _Data)
            {
                if (i.state == StateOfBlock.jointing)
                {
                    //i.model.transform.position += i.MoveVector / MoveFrames;
                    if (i.LeftStep1 > 0)
                    {
                        --i.LeftStep1;
                        i.model.transform.position += i.MoveVector / 2 * (float)(Math.Cos(Math.PI * i.LeftStep1 / MoveFrames) - Math.Cos(Math.PI * (i.LeftStep1 + 1) / MoveFrames));
                        if (i.LeftStep1 <= 0)
                        {
                            i.MoveVector = -i.JointPosition;
                        }
                    }
                    else if (i.LeftStep2 > 0 && i.LeftStep1 == 0)
                    {
                        --i.LeftStep2;
                        i.model.transform.position += i.MoveVector / 2 * (float)(Math.Cos(Math.PI * i.LeftStep2 / MoveFrames) - Math.Cos(Math.PI * (i.LeftStep2 + 1) / MoveFrames));
                        if (i.LeftStep2 <= 0)
                        {
                            jointing = false;
                            GameObject.Find("Operation").GetComponent<Operation>().LetGo();
                            i.state = StateOfBlock.caught;
                            GetJointed(ShouldCatch);
                            TryRotation();
                            if (rotating == false && needMoving == false)
                                Creat(ShouldCatch);
                        }
                    }
                    /*if (Math.Abs(i.model.transform.position.x - _Data[0].model.transform.position.x) < 0.001f &&
                        Math.Abs(i.model.transform.position.y - _Data[0].model.transform.position.y) < 0.001f &&
                        Math.Abs(i.model.transform.position.z - _Data[0].model.transform.position.z) < 0.001f)
                    {
                        jointing = false;
                        i.state = StateOfBlock.caught;
                        GetJointed(ShouldCatch);
                        Creat(ShouldCatch);
                    }
                    else if (Math.Abs(i.model.transform.position.x - _Data[0].model.transform.position.x - i.JointPosition.x) < 0.001f &&
                        Math.Abs(i.model.transform.position.x - _Data[0].model.transform.position.x - i.JointPosition.x) < 0.001f &&
                        Math.Abs(i.model.transform.position.x - _Data[0].model.transform.position.x - i.JointPosition.x) < 0.001f)
                    {
                        i.MoveVector = -i.JointPosition;
                    }*/
                }
            }
        }
        if (inCollision == false)
        {
            if (_Data.Count >= 1)
                if (_Data[0] != null)
                {
                    SavePos(0);
                    if (_Data[0].CollisionCount > 0)
                    {
                        --_Data[0].CollisionCount;
                        if (_Data[0].CollisionCount <= 0)
                        {
                            _Data[0].model.GetComponent<Highlighter>().ConstantOff();
                            FlashOffForOneFrame(0);
                        }
                    }
                }
            if (_Data.Count >= ShouldCatch + 1)
                if (_Data[ShouldCatch] != null)
                {
                    SavePos(ShouldCatch);
                    if (_Data[ShouldCatch].CollisionCount > 0)
                    {
                        --_Data[ShouldCatch].CollisionCount;
                        if (_Data[ShouldCatch].CollisionCount <= 0)
                            _Data[ShouldCatch].model.GetComponent<Highlighter>().ConstantOff();
                    }
                }
        }
	}


}
