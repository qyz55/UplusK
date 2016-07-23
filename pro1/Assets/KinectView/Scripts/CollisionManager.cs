using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using HighlightingSystem;

public class CollisionManager : MonoBehaviour
{

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
        if (mo.inCollision == false)
            mo.inCollision = true;
    }
    void OnTriggerStay(Collider col)
    {
        GameObject.Find("Crash").GetComponent<Text>().text = "撞车了";
        ModelManager mo = empty.GetComponent<ModelManager>();
        if (mo.inCollision == false)
            mo.inCollision = true;
    }
    void OnTriggerExit(Collider col)
    {
        GameObject.Find("Crash").GetComponent<Text>().text = "撞车了";
        gameObject.GetComponent<Highlighter>().ConstantOff();
        ModelManager mo = empty.GetComponent<ModelManager>();
        if (mo.inCollision == true)
            mo.inCollision = false;
    }
}
