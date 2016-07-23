using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;

//注意一下所有的位置都是世界坐标，而非相对于父物体的坐标;所有旋转默认是围绕自己的旋转，若有父物体则围绕父物体旋转
public class ModelManager : MonoBehaviour
{
    static public int NumOfPiece = 3;
    private List<Model> _Data = new List<Model>();
    private Model[] a = new Model[NumOfPiece + 1];
    public GameObject[] b = new GameObject[NumOfPiece + 1];
    public GameObject e;
    public int ShouldCatch = 1;
    public Vector3  BirthPosition = new Vector3(20,20,30);
    public float RangeOfAngles = 80.0f;
    public float RangeOfDis = 5;
    private Vector3[] AllCenter = { Vector3.zero, new Vector3(-20, 0, 0), new Vector3(-15, 0, 0), new Vector3(-6, 0, 0) };
    public bool inCollision = false;
    //以下是写了一个大概的
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
        _Data[num].state = state;
    }
    public void MoveOne(int num, Vector3 _v3) // 传入想要移动的物体的下标和想要其移动到的位置
    {
        if (_Data[num].state == StateOfBlock.caught && num == ShouldCatch)
            _Data[num].model.transform.position = CalcCenterPosition(num, _v3);
        else 
            return;
        SavePos(num);
        TryJoint();
    }
    public void Move0(int num, Vector3 _v3)
    {
        if (_Data[0].state != StateOfBlock.caught)
            return;
        _Data[0].model.transform.position = _v3 - _Data[0].model.transform.rotation * _Data[num].center;
        SavePos(0);
        TryJoint();
    }
    /*public void MoveOneByVector3(int num, Vector3 _v3)
    {
        _Data[num].model.transform.position += _v3;
    }*/
    public void MoveAllByVector3(Vector3 _V3)
    {
        _Data[0].model.transform.position += _V3;
        _Data[ShouldCatch].model.transform.position += _V3;
        SavePos(0);
        SavePos(ShouldCatch);
    }
    private void SavePos(int num)
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
    public bool IsJointed(int num1, int num2) // 输入两个物体的下标，判断物体可否被拼接
    {
        return true;
    }
    public Vector3 GetPosition(int num)  //获得输入下标物体的位置
    {
        return _Data[num].model.transform.position;
    }
    public Vector3 GetLocalPosition(int num)  //获得输入下标物体相对于父物体的位置
    {
        return _Data[num].model.transform.localPosition;
    }
    public Quaternion GetRotation(int num) //获得输入下标物体的旋转角度，用一个四元数表示
    {
        return _Data[num].model.transform.rotation;
    }
    public Quaternion GetLocalRotation(int num) //获得输入下标物体相对于父物体的旋转角度，用一个四元数表示
    {
        return _Data[num].model.transform.localRotation;
    } 
    public void Rotate(int num,int mode) //对下标为num的物体进行6种旋转，0-5分别为x(顺逆)y(顺逆)z(顺逆)
    {
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
    }
    private Vector3 CalcCenterPosition(int num, Vector3 pos)
    {
        return pos - _Data[num].model.transform.rotation * _Data[num].center;
    }
    public StateOfBlock GetState(int num) // 获得某个物体的状态
    {
        return _Data[num].state;
    }
	// Use this for initialization
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
        
        if (Vector3.Distance(CalcRealPosition(0, _Data[0].model.transform.position) - _Data[0].center, CalcRealPosition(ShouldCatch, _Data[ShouldCatch].model.transform.position) - _Data[ShouldCatch].center) < RangeOfDis)
        {
            if (CheckRotation() == true)
            {
                print("OK");
                GetJointed(ShouldCatch);
                Creat(ShouldCatch);
            }
        }
    }
    void GetJointed(int i)
    {
        _Data[i].father = 0;
        _Data[i].model.transform.parent = _Data[0].model.transform;
        _Data[i].model.transform.localPosition = Vector3.zero;
        _Data[i].model.transform.localRotation = _Data[ShouldCatch].initialQuaternion;
        _Data[i].state = StateOfBlock.jointed;
        ShouldCatch++;
    }
    void Congratulations()
    {
        return;
    }
    void Creat(int i)
    {
        if (i > NumOfPiece) { Congratulations(); return; };
        a[i] = new Model();
        a[i].model = GameObject.Instantiate(b[i]);
        a[i].num = i;
        a[i].state = StateOfBlock.free;
        a[i].father = i;
        a[i].model.transform.position = BirthPosition;
        a[i].initialQuaternion = Quaternion.identity;
        if (!a[i].model.GetComponent<Rigidbody>())
            a[i].model.AddComponent<Rigidbody>();
        a[i].model.GetComponent<Rigidbody>().useGravity = false;
        a[i].model.GetComponent<Rigidbody>().drag = 2000;
        a[i].center = AllCenter[i];
        _Data.Add(a[i]);
    }
    public void init()
    {
        GameObject.Find("Root").transform.Find("Sphere").gameObject.SetActive(true);
        Creat(ShouldCatch);
        GetJointed(ShouldCatch);
        Creat(ShouldCatch);
    }
	void Start () {
        a[0] = new Model();
        a[0].model = GameObject.Find("Sphere");
        _Data.Add(a[0]);
        a[0].model.SetActive(false);
        ShouldCatch = 1;
	}
    
	
	// Update is called once per frame
	void Update () {
	   
	}


}
