using UnityEngine;
using System.Collections;

public class MainCameraManager : MonoBehaviour {

	// Use this for initialization
	void Start () {
	
	}

    private bool moving = false;
    private int MoveFrames = 10;
    private Vector3 presentPos;
    private Vector3 presentRotation;
    private Vector3 deltaPos;
    private Vector3 deltaRotation;



    public void TransferTo(Vector3 pos, Quaternion rotation, Vector3 targetPos)
    {
        moving = true;
        presentPos = this.transform.position;
        presentRotation = this.transform.rotation.eulerAngles;
        deltaPos = pos - presentPos;
        deltaRotation = rotation.eulerAngles - presentRotation;
        //this.transform.
    }
	// Update is called once per frame
	void Update ()
    {
        if (moving)
        {

        }
	}

}
