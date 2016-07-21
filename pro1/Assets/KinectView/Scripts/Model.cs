using UnityEngine;
using System.Collections;
using System.Collections.Generic;
<<<<<<< HEAD
=======

>>>>>>> 0604060478e88b27cc6b68e93b7e7dfd2df607da
public enum StateOfBlock:int
{
    free       =0,//自由
    caught     =1, //被抓取
    /*aroundaxis,*/
    //轴对上但角度不对，有待商榷
    jointed    =2,//已被拼上，意味着不可动
<<<<<<< HEAD
};
=======
}

>>>>>>> 0604060478e88b27cc6b68e93b7e7dfd2df607da
public class Model
{
    public GameObject model{get;set;}
    public int num { get; set; }
    public StateOfBlock state { get; set; }
    public int father { get; set; }
    Model() { model = null; num = 0; state = StateOfBlock.free; father = 0; }
}