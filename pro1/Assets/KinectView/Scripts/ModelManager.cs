using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine.UI;
using HighlightingSystem;

//注意一下所有的位置都是世界坐标，而非相对于父物体的坐标;所有旋转默认是围绕自己的旋转，若有父物体则围绕父物体旋转
public class ModelManager : MonoBehaviour
{
    static public int NumOfPiece = 15;
    static public int TNumOfPiece = 2;
    public bool jointing = false;
    private int nowRotation = 0;
    private Vector3 rotationEuler;
    private int FinalRotation = 0;
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
    public Vector3  BirthPosition = new Vector3(30,15,30);
    public float RangeOfAngles = 80.0f;
    public float RangeOfDis = 5;
    private Vector3[] AllCenter = { Vector3.zero, new Vector3(-20, 0, 0), new Vector3(-15, 0, 0), new Vector3(-6, 0, 0), new Vector3(3,0,0),new Vector3(3,-4,0),new Vector3(3,-9,0),new Vector3(3,-13,0),
                                  new Vector3(6,4,0),new Vector3(6,4,25),new Vector3(6,4,-25),new Vector3(6,-2,30),new Vector3(6,-2,20),new Vector3(6,-2,-20),new Vector3(6,-2,-30)};
    private Vector3[] TAllCenter = { Vector3.zero, Vector3.zero, Vector3.zero };
    private Vector3[] AllJointPosition = { Vector3.zero, new Vector3(2, 0, 0), new Vector3(3, 0, 0), new Vector3(3,0,0), new Vector3(3,0,0),new Vector3(0,-3,0),new Vector3(0,-3,0),new Vector3(0,-3,0),
                                         new Vector3(0,3,0),new Vector3(0,0,5),new Vector3(0,0,-5),new Vector3(-3,0,0),new Vector3(-3,0,0),new Vector3(-3,0,0),new Vector3(-3,0,0),};
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
        if (jointing || rotating) return;
        _Data[num].state = state;
    }
    public void MoveOne(int num, Vector3 _v3) // 传入想要移动的物体的下标和想要其移动到的位置
    {
        if (jointing || rotating) return;
        if (_Data[num].state == StateOfBlock.caught && num == ShouldCatch)
            _Data[num].model.transform.position += _v3;//CalcCenterPosition(num, _v3);
        else
            return;
        SavePos(num);
        TryJoint();
    }
    public void Move0(int num, Vector3 _v3)
    {
        if (jointing || rotating) return;
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
        if (jointing || rotating) return;
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
    public void Rotate(int num,int mode) //对下标为num的物体进行6种旋转，0-5分别为x(顺逆)y(顺逆)z(顺逆)
    {
        if (jointing || rotating) return;
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
        if (d > RangeOfDis * 2)
        {
            _Data[ShouldCatch].model.GetComponent<Highlighter>().ConstantOff();
            _Data[0].model.GetComponent<Highlighter>().ConstantOff();

        }
        else if (d >= RangeOfDis && CheckRotation() == true)
        {
            _Data[ShouldCatch].model.GetComponent<Highlighter>().ConstantOn(Color.green);
        }
        else if (d < RangeOfDis)
        {
            if (CheckRotation() == true)
            {
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
            for(int i = 1; i < ShouldCatch; ++i)
                _Data[i].model.GetComponent<Highlighter>().ConstantOn(Color.grey);
        }
        else
           _Data[num].model.GetComponent<Highlighter>().ConstantOn(Color.grey);
    }
    public void FlashOffForOneFrame(int num)
    {
        if (num == 0)
        {
            for (int i = 1; i < ShouldCatch; ++i)
                _Data[i].model.GetComponent<Highlighter>().ConstantOff();
        }
        else
            _Data[num].model.GetComponent<Highlighter>().ConstantOff();
    }
    void Congratulations()
    {
        ShouldCatch--;
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
            Ta[i].model.transform.position = BirthPosition;
            Ta[i].initialQuaternion = Quaternion.identity;
            Ta[i].LastPosition = BirthPosition;
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
            a[i].model.transform.position = BirthPosition;
            a[i].initialQuaternion = _Data[0].model.transform.rotation;
            a[i].model.transform.rotation = _Data[0].model.transform.rotation;
            a[i].LastPosition = BirthPosition;
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
                FinalRotation = 80;
                nowRotation = 0;
                rotationEuler = Vector3.up;//new Vector3(1, 1, 1);
                break;
            default:
                break;
        }
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
        if (rotating == true)
        {
            ++nowRotation; 
            _Data[0].model.transform.RotateAround(CalcRealPosition(1, _Data[1].model.transform.position), rotationEuler, 50*Time.deltaTime);
            if (nowRotation >= FinalRotation)
            {
                rotating = false;
                SavePos(0);
                Creat(ShouldCatch);
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
                        i.model.transform.position += i.MoveVector / 2 * (float)( Math.Cos(Math.PI * i.LeftStep1 / MoveFrames) -Math.Cos(Math.PI * (i.LeftStep1 + 1) / MoveFrames));
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
                            i.state = StateOfBlock.caught;
                            GetJointed(ShouldCatch);
                            TryRotation();
                            if (rotating == false)
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
	   
	}


}
