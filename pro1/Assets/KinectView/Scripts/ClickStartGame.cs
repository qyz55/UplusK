using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;

public class ClickStartGame : MonoBehaviour {

	// Use this for initialization
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
                GameStart();
                break;
            case "GameEnd":
                Application.Quit();
                break;
            default:
                Debug.Log("none");
                break;
        }
    }
    void GameStart()
    {
        GameObject.Find("ModelManager").GetComponent<ModelManager>().init();
    }
}
