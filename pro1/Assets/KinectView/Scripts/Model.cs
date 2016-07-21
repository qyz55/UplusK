using UnityEngine;
using System.Collections;
using System.Collections.Generic;
<<<<<<< HEAD
public enum StateOfBlock:int
{
    free       =0,//自由
    caught     =1, //被抓取
    /*aroundaxis,*/
    //轴对上但角度不对，有待商榷
    jointed    =2,//已被拼上，意味着不可动
=======

public enum StateOfBlock
{
    free,   //自由
    caught, //被抓取
    /*aroundaxis,*/
    //轴对上但角度不对，有待商榷
    jointed//已被拼上，意味着不可动
>>>>>>> 859783a0d9529979645e444a28443d6a21ab1961
};
public class Model
{
    public GameObject model{get;set;}
    public int num { get; set; }
    public StateOfBlock state { get; set; }
    public int father { get; set; }
    Model() { model = null; num = 0; state = StateOfBlock.free; father = 0; }
}