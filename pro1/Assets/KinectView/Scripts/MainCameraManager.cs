using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class MainCameraManager : MonoBehaviour {

    private bool moving = false;
    private int MoveCount = 0;
    private const int MoveFrames = 10;
    private Vector3 presentPos;
    private Vector3 posToGo;
    private Vector3 deltaPos;

    private bool rotating = false;
    private int RotateCount = 0;
    private const int RotateFrames = 60;

    private bool demoRotating = false;
    private int demoRotateCount = 0;
    private const int demoRotatingFrames = 600;

    public bool getDemoRotating() { return demoRotating; }

    private bool goingBack = false;

    private Vector3 presentRotation;
    private Vector3 deltaRotation;
    private Vector3 rotateCenter;

    int rotateDirection;

	// Use this for initialization
	void Start () {
	}

    public void DemoRotate()
    {
        demoRotating = true;
        demoRotateCount = demoRotatingFrames;
    }

    public void TransferTo(Vector3 toGoPos, Vector3 targetPos)
    {
        moving = true;
        goingBack = false;
        MoveCount = MoveFrames;
        presentPos = this.transform.position;
        posToGo = toGoPos;
        deltaPos = posToGo - presentPos;
        rotateCenter = targetPos;
        rotateDirection = Random.Range((int)0, (int)2);
    }
    public void TransferTo(Vector3 toGoPos, Vector3 targetPos, int dir/*旋转方式*/)
    {
        moving = true;
        goingBack = false;
        MoveCount = MoveFrames;
        presentPos = this.transform.position;
        posToGo = toGoPos;
        deltaPos = posToGo - presentPos;
        rotateCenter = targetPos;
        rotateDirection = dir;
    }
    private void startRotate()
    {
        rotating = true;
        RotateCount = RotateFrames;
    }
    private void GoBack()
    {
        moving = true;
        goingBack = true;
        MoveCount = MoveFrames;
        presentPos = this.transform.position;
        posToGo = new Vector3(0, 1, -17);
        deltaPos = posToGo - presentPos;
    }

	// Update is called once per frame
	void Update ()
    {
        if (demoRotating)
        {
            demoRotateCount--;
            this.transform.RotateAround(new Vector3(0, 0, 40), Vector3.up, (float)360/(float)demoRotatingFrames);
            if (demoRotateCount <= 0)
            {
                demoRotating = false;
                demoRotateCount = 0;
                GameObject.Find("Root").transform.Find("SpaceTraveler").gameObject.SetActive(false);
                GameObject.Find("Root").transform.Find("leftHand").gameObject.SetActive(true);
                GameObject.Find("Root").transform.Find("rightHand").gameObject.SetActive(true);
                GameObject.Find("GameStart").GetComponent<Button>().GetComponentInChildren<Text>().text = "教学模式";
                GameObject.Find("ModelManager").GetComponent<ModelManager>().TeachInit();
            }
        }
        else if (moving)
        {
            MoveCount--;
            if (MoveCount > 0)
            {
                this.transform.position += deltaPos / MoveFrames;
            }
            else
            {
                moving = false;
                this.transform.position = posToGo;
                if (goingBack)
                {
                    goingBack = false;
                }
                else
                {
                    if (!demoRotating)
                    {
                        startRotate();
                    }
                }
            }
        }
        else if (rotating)
        {
            if (RotateCount > (RotateFrames >> 1))
            {
                switch(rotateDirection)
                {
                    case 0:
                        this.transform.RotateAround(rotateCenter, Vector3.up, 50 * Time.deltaTime);
                        break;
                    case 1:
                        this.transform.RotateAround(rotateCenter, Vector3.down, 50 * Time.deltaTime);
                        break;
                }
            }
            else
            {
                if (RotateCount <= 0)
                {
                    rotating = false;
                    GoBack();
                    return;
                }
                switch(rotateDirection)
                {
                    case 0:
                        this.transform.RotateAround(rotateCenter, Vector3.down, 50 * Time.deltaTime);
                        break;
                    case 1:
                        this.transform.RotateAround(rotateCenter, Vector3.up, 50 * Time.deltaTime);
                        break;
                }
            }
            RotateCount--;
        }
    }
}
