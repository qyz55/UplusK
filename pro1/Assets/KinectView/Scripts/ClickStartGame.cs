using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class ClickStartGame : MonoBehaviour {

	// Use this for initialization
    private int state = 0;
    void Start()
    {
        List<string> btnsName = new List<string>();
        btnsName.Add("GameStart");
        btnsName.Add("GameEnd");


        foreach (string btnName in btnsName)
        {
            GameObject btnObj = GameObject.Find(btnName);
            Button btn = btnObj.GetComponent<Button>();
            btn.onClick.AddListener(delegate()
            {
                this.OnClick(btnObj);
            });
        }
    }

    public void OnClick(GameObject sender)
    {
        switch (sender.name)
        {
            case "GameStart":
                /*if (state == 0)
                {*/
                GameObject.Find("Root").transform.Find("SpaceTraveler").gameObject.SetActive(true);
                GameObject.Find("Main Camera").GetComponent<MainCameraManager>().DemoRotate();
                /*
                    state = 1;
                }
                else if (state == 1)
                {
                    if (GameObject.Find("Main Camera").GetComponent<MainCameraManager>().getDemoRotating())
                    {

                    }
                GameObject.Find("GameStart").GetComponent<Button>().GetComponentInChildren<Text>().text = "教学模式";
                GameObject.Find("ModelManager").GetComponent<ModelManager>().TeachInit();
                    state = 2;
                }
                else if (state == 2)
                {
                    //GameStart();
                    GameObject.Find("ModelManager").GetComponent<ModelManager>().ChangeTeachState(2);
                    Destroy(GameObject.Find("GameStart"));
                }*/
                break;
            case "GameEnd":
                SceneManager.LoadScene("MainScene");
                break;
            default:
                Debug.Log("none");
                break;
        }
    }
    void TeachingModelStart()
    {
        GameObject.Find("ModelManager").GetComponent<ModelManager>().TeachInit();
    }
    void GameStart()
    {
        GameObject.Find("ModelManager").GetComponent<ModelManager>().init();
    }
}
