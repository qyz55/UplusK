using UnityEngine;
using System.Collections;
using System.Collections.Generic;
//注意一下所有的位置都是世界坐标，而非相对于父物体的坐标;所有旋转默认是围绕自己的旋转，若有父物体则围绕父物体旋转
public class ModelManager : MonoBehaviour
{
    private static List<Model> _Data = new List<Model>();
    //以下是写了一个大概的
    public static List<Vector3> GetAllPosition() // 获得所有物体的位置,返回一个内容为Vector3的list
    {
        List<Vector3> _position = new List<Vector3>();
        foreach (Model i in _Data)
        {
            _position.Add(i.model.transform.position);
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
        _Data[num].model.transform.position = _v3;
    }
    
    //以下是才定义出来没写的
    public bool IsJointed(int num1, int num2) // 输入两个物体的下标，判断物体可否被拼接
    {
        return true;
    }
    public Vector3 GetPosition(int num)  //获得输入下标物体的位置
    {
        return new Vector3(0, 0, 0);
    }
    public Quaternion GetRotation(int num) //获得输入下标物体的旋转角度，用一个四元数表示
    {
        return Quaternion.identity;
    }
    public void Rotate(int num,Vector3 i) //对下标为num的物体进行Vector3的旋转，旋转顺序为ZXY
    {

    }
    public StateOfBlock GetState(int num) // 获得某个物体的状态
    {
        return _Data[num].state;
    }


	// Use this for initialization
	void Start () {
	    
	}
	
	// Update is called once per frame
	void Update () {
	
	}


}
