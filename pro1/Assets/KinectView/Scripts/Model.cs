using UnityEngine;
using System.Collections;
using System.Collections.Generic;
public enum StateOfBlock:int
{
    unactive   = 0,
    free       = 1, //自由
    caught     = 2, //被抓取
    jointing   = 3, //已被拼上，正在往该走的地方移动
    jointed    = 4, //已被拼上，意味着不可动
};
public class Model
{
    public GameObject model{get;set;}
    public int num { get; set; }
    public StateOfBlock state { get; set; }
    public int father { get; set; }
    public Vector3 center { get; set; }
    public int NumOfLast { get; set; }
    public Vector3 LastPosition { get; set;}
    public Quaternion LastQuaternion { get; set; }
    public Quaternion initialQuaternion { get; set; }
    public Vector3 MoveVector { get; set; }
    public Model() { 
        model = null; 
        num = 0; 
        state = StateOfBlock.unactive;
        father = 0; 
        center = Vector3.zero;
        NumOfLast = 0;
        LastPosition = Vector3.zero;
		initialQuaternion = Quaternion.identity;
        LastQuaternion = Quaternion.identity;
    }
}