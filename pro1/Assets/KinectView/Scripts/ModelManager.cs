using UnityEngine;
using System.Collections;
using System.Collections.Generic;
//注意一下所有的位置都是世界坐标，而非相对于父物体的坐标;所有旋转默认是围绕自己的旋转，若有父物体则围绕父物体旋转
public class ModelManager : MonoBehaviour
{
    List<Model> _Data = new List<Model>();
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
        if (_Data[num].state == StateOfBlock.caught)
            _Data[num].model.transform.position = CalcCenterPosition(num, _v3);
    }
    public void MoveOneByVector3(int num, Vector3 _v3)
    {
        _Data[num].model.transform.position += _v3;
    }
    public void MoveAllByVector3(Vector3 _V3)
    {
        foreach (Model i in _Data)
            i.model.transform.position += _V3;
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
    private Model[] a = new Model[3];
    public GameObject[] b = new GameObject[3];
    public GameObject e;
    
	void Start () {
        a[0] = new Model();
        a[0].model = GameObject.Find("Sphere");
        _Data.Add(a[0]);
        for (int i = 1; i <= 2; ++i)
        {
            a[i] = new Model();
            a[i].model = GameObject.Instantiate(b[i]);
            a[i].num = i;
            a[i].state = StateOfBlock.free;
            a[i].model.transform.localPosition = new Vector3(0, 0, 0) + a[0].model.transform.position;
            a[i].initialQuaternion = a[i].model.transform.rotation;
            a[i].model.AddComponent<Rigidbody>();
            a[i].model.GetComponent<Rigidbody>().isKinematic = true;
            a[i].model.GetComponent<Rigidbody>().useGravity = false;
            a[i].model.GetComponent<Rigidbody>().drag = 2000;
            _Data.Add(a[i]);
        }
        a[1].model.transform.parent = a[0].model.transform;
        a[1].father = 0;
        a[1].center = new Vector3(-20, 0, 0);
        a[2].center = new Vector3(-15, 0, 0);
        a[2].model.transform.position += new Vector3(20, 10, 0);
	}
    
	
	// Update is called once per frame
	void Update () {
	   
	}


}
