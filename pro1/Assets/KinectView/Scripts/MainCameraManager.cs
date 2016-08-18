using UnityEngine;
using System.Collections;

public class MainCameraManager : MonoBehaviour {

	// Use this for initialization
	void Start () {
	
	}

    private bool moving = false;
    private int MoveCount = 0;
    private const int MoveFrames = 10;
    private Vector3 presentPos;
    private Vector3 posToGo;
    private Vector3 deltaPos;

    private bool rotating = false;
    private int RotateCount = 0;
    private const int RotateFrames = 20;

    private bool goingBack = false;

    private Vector3 presentRotation;
    private Vector3 deltaRotation;
    private Vector3 rotateCenter;

    Random ran;

    public void TransferTo(Vector3 toGoPos, Vector3 targetPos)
    {
        moving = true;
        goingBack = false;
        MoveCount = MoveFrames;
        presentPos = this.transform.position;
        posToGo = toGoPos;
        deltaPos = posToGo - presentPos;
        rotateCenter = targetPos;
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
        if (moving)
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
                    rotating = true;
                    RotateCount = RotateFrames;
                }
            }
        }
        if (rotating)
        {
            if (RotateCount > (RotateFrames >> 1))
            {
                this.transform.RotateAround(rotateCenter, Vector3.up, 50 * Time.deltaTime);
            }
            else
            {
                if (RotateCount <= 0)
                {
                    rotating = false;
                    GoBack();
                    return;
                }
                this.transform.RotateAround(rotateCenter, Vector3.down, 50 * Time.deltaTime);
            }
            RotateCount--;
        }
    }
}
