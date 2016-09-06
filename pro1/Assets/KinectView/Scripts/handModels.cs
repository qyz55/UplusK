using UnityEngine;
using System.Collections;
using Kinect = Windows.Kinect;

public class handModels : MonoBehaviour {

    string[] pre = { "open", "closed"/*, "lasso"*/ };
    //public GameObject[] status = new GameObject[3];

    public void setStatus(bool LorR, Kinect.HandState handState)
    {
        if (!LorR)
        {
            for (int i = 0; i < 2; ++i)
            {
                if (i + 2 == (int)handState)
                {
                    //status[i].GetComponent<Renderer>().enabled = true;
                    GameObject.Find("Root").transform.Find("leftHand").transform.Find("l_" + pre[i]).gameObject.SetActive(true);
                    //GameObject.Find("leftHand").transform.Find("l_" + pre[i]).gameObject.renderer.enabled = true;
                }
                else
                {
                    //status[i].GetComponent<Renderer>().enabled = false;
                    GameObject.Find("Root").transform.Find("leftHand").transform.Find("l_" + pre[i]).gameObject.SetActive(false);
                    //GameObject.Find("leftHand").transform.Find("l_" + pre[i]).gameObject.SetActive(false);
                }
            }
        }
        else
        {
            for (int i = 0; i < 2; ++i)
            {
                if (i + 2 == (int)handState)
                {
                    GameObject.Find("Root").transform.Find("rightHand").transform.Find("r_" + pre[i]).gameObject.SetActive(true);
                    //GameObject.Find("rightHand").transform.Find("r_" + pre[i]).gameObject.SetActive(true);
                }
                else
                {
                    GameObject.Find("Root").transform.Find("rightHand").transform.Find("r_" + pre[i]).gameObject.SetActive(false);
                    //GameObject.Find("rightHand").transform.Find("r_" + pre[i]).gameObject.SetActive(false);
                }
            }
        }
    }

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
	
	}
}
