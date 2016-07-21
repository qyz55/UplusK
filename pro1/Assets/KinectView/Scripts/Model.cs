using UnityEngine;
using System.Collections;
using System.Collections.Generic;
public enum StateOfBlock:int
{
    unactive   = 0,
    free       = 1,//自由
    caught     = 2, //被抓取
    /*aroundaxis,*/
    //轴对上但角度不对，有待商榷
    jointed    = 3,//已被拼上，意味着不可动
};
public class Model
{
    public GameObject model{get;set;}
    public int num { get; set; }
    public StateOfBlock state { get; set; }
    public int father { get; set; }
    public Vector3 center { get; set; }
	public Quaternion initialQuaternion{get; set;}
    public Model() { model = null; num = 0; state = StateOfBlock.free; father = 0; center = new Vector3(0,0,0);}
}