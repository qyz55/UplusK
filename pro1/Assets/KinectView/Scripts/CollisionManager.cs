using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using HighlightingSystem;

public class CollisionManager : MonoBehaviour
{
    private float temptime = 0;
    // Use this for initialization
    void Start()
    {
        empty = GameObject.Find("ModelManager");
    }

    // Update is called once per frame
    void Update()
    {
    }

    public GameObject empty;
    void OnTriggerEnter(Collider col)
    {
        GameObject.Find("Crash").GetComponent<Text>().text = "撞车了";
        gameObject.GetComponent<Highlighter>().ConstantOn(Color.red);
        ModelManager mo = empty.GetComponent<ModelManager>();
        GameObject.Find("Operation").GetComponent<Operation>().LetGo();
        mo.ChangeCollisionCount();
        if (mo.inCollision == false)
        {
            mo.inCollision = true;
            mo.SavePos(mo.ShouldCatch);
            mo.inCollision = true;
            mo.SavePos(0);
        }
    }
    void OnTriggerStay(Collider col)
    {
        GameObject.Find("Crash").GetComponent<Text>().text = "撞车了";
        gameObject.GetComponent<Highlighter>().ConstantOn(Color.red);
        ModelManager mo = empty.GetComponent<ModelManager>();
        GameObject.Find("Operation").GetComponent<Operation>().LetGo();
        mo.ChangeCollisionCount();
        if (mo.inCollision == false)
        {
            mo.inCollision = true;
            mo.SavePos(mo.ShouldCatch);
            mo.inCollision = true;
            mo.SavePos(0);
        }
    }
    void OnTriggerExit(Collider col)
    {
        GameObject.Find("Crash").GetComponent<Text>().text = "撞车了";
        //gameObject.GetComponent<Highlighter>().ConstantOff();
        ModelManager mo = empty.GetComponent<ModelManager>();
        GameObject.Find("Operation").GetComponent<Operation>().LetGo();
        if (mo.inCollision == true)
            mo.inCollision = false;
    }
}
