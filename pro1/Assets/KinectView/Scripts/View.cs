using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine;
using Kinect = Windows.Kinect;

public class View : MonoBehaviour 
{
    public Material BoneMaterial;
    public GameObject BodySourceManager;
    
    private Dictionary<ulong, GameObject> _Bodies = new Dictionary<ulong, GameObject>();
    private BodySourceManager _BodyManager;
    
    public GameObject a;

    private Dictionary<Kinect.JointType, Kinect.JointType> _BoneMap = new Dictionary<Kinect.JointType, Kinect.JointType>()
    {
        { Kinect.JointType.FootLeft, Kinect.JointType.AnkleLeft },
        { Kinect.JointType.AnkleLeft, Kinect.JointType.KneeLeft },
        { Kinect.JointType.KneeLeft, Kinect.JointType.HipLeft },
        { Kinect.JointType.HipLeft, Kinect.JointType.SpineBase },
        
        { Kinect.JointType.FootRight, Kinect.JointType.AnkleRight },
        { Kinect.JointType.AnkleRight, Kinect.JointType.KneeRight },
        { Kinect.JointType.KneeRight, Kinect.JointType.HipRight },
        { Kinect.JointType.HipRight, Kinect.JointType.SpineBase },
        
        { Kinect.JointType.HandTipLeft, Kinect.JointType.HandLeft },
        { Kinect.JointType.ThumbLeft, Kinect.JointType.HandLeft },
        { Kinect.JointType.HandLeft, Kinect.JointType.WristLeft },
        { Kinect.JointType.WristLeft, Kinect.JointType.ElbowLeft },
        { Kinect.JointType.ElbowLeft, Kinect.JointType.ShoulderLeft },
        { Kinect.JointType.ShoulderLeft, Kinect.JointType.SpineShoulder },
        
        { Kinect.JointType.HandTipRight, Kinect.JointType.HandRight },
        { Kinect.JointType.ThumbRight, Kinect.JointType.HandRight },
        { Kinect.JointType.HandRight, Kinect.JointType.WristRight },
        { Kinect.JointType.WristRight, Kinect.JointType.ElbowRight },
        { Kinect.JointType.ElbowRight, Kinect.JointType.ShoulderRight },
        { Kinect.JointType.ShoulderRight, Kinect.JointType.SpineShoulder },
        
        { Kinect.JointType.SpineBase, Kinect.JointType.SpineMid },
        { Kinect.JointType.SpineMid, Kinect.JointType.SpineShoulder },
        { Kinect.JointType.SpineShoulder, Kinect.JointType.Neck },
        { Kinect.JointType.Neck, Kinect.JointType.Head },
    };

    void Start()
    {
        a.transform.rotation = Quaternion.identity;
        a.transform.position = new Vector3(0, 0, 8);
    }

    void Update () 
    {
        if (BodySourceManager == null)
        {
            return;
        }
        
        _BodyManager = BodySourceManager.GetComponent<BodySourceManager>();
        if (_BodyManager == null)
        {
            return;
        }
        
        Kinect.Body[] data = _BodyManager.GetData();
        if (data == null)
        {
            return;
        }
        
        List<ulong> trackedIds = new List<ulong>();
        foreach(var body in data)
        {
            if (body == null)
            {
                continue;
              }
                
            if(body.IsTracked)
            {
                trackedIds.Add (body.TrackingId);
            }
        }
        
        List<ulong> knownIds = new List<ulong>(_Bodies.Keys);
        
        // First delete untracked bodies
        foreach(ulong trackingId in knownIds)
        {
            if(!trackedIds.Contains(trackingId))
            {
                Destroy(_Bodies[trackingId]);
                _Bodies.Remove(trackingId);
            }
        }

        foreach(var body in data)
        {
            if (body == null)
            {
                continue;
            }
            
            if(body.IsTracked)
            {
                if(!_Bodies.ContainsKey(body.TrackingId))
                {
                    _Bodies[body.TrackingId] = CreateBodyObject(body.TrackingId);
                }
                
                RefreshBodyObject(body, _Bodies[body.TrackingId]);
            }
        }
        
    }
    
    private GameObject CreateBodyObject(ulong id)
    {
        GameObject body = new GameObject("Body:" + id);
        
        for (Kinect.JointType jt = Kinect.JointType.SpineBase; jt <= Kinect.JointType.ThumbRight; jt++)
        {
            GameObject jointObj = GameObject.CreatePrimitive(PrimitiveType.Cube);
            
            LineRenderer lr = jointObj.AddComponent<LineRenderer>();
            lr.SetVertexCount(2);
            lr.material = BoneMaterial;
            lr.SetWidth(0.05f, 0.05f);
            
            jointObj.transform.localScale = new Vector3(0.3f, 0.3f, 0.3f);
            jointObj.name = jt.ToString();
            jointObj.transform.parent = body.transform;
        }
        
        return body;
    }
    
    private void RefreshBodyObject(Kinect.Body body, GameObject bodyObject)
    {
        for (Kinect.JointType jt = Kinect.JointType.SpineBase; jt <= Kinect.JointType.ThumbRight; jt++)
        {
            Kinect.Joint sourceJoint = body.Joints[jt];
            Kinect.Joint? targetJoint = null;
            
            if(_BoneMap.ContainsKey(jt))
            {
                targetJoint = body.Joints[_BoneMap[jt]];
            }
            
            Transform jointObj = bodyObject.transform.FindChild(jt.ToString());
            jointObj.localPosition = GetVector3FromJoint(sourceJoint);
            
            LineRenderer lr = jointObj.GetComponent<LineRenderer>();
            if(targetJoint.HasValue)
            {
                lr.SetPosition(0, jointObj.localPosition);
                lr.SetPosition(1, GetVector3FromJoint(targetJoint.Value));
                lr.SetColors(GetColorForState (sourceJoint.TrackingState), GetColorForState(targetJoint.Value.TrackingState));
            }
            else
            {
                lr.enabled = false;
            }
        }
        print(body.Joints[Kinect.JointType.HandLeft].Position.X + "  " + body.Joints[Kinect.JointType.HandLeft].Position.Y + "  " + body.Joints[Kinect.JointType.HandLeft].Position.Z);

/* 各种自带手势
 * if (body.HandRightState == Kinect.HandState.Closed)
            print("rightClose");
        if (body.HandRightState == Kinect.HandState.Open)
            print("rightOpen");
        if (body.HandRightState == Kinect.HandState.Lasso)
            print("rightLesso");
        if (body.HandLeftState == Kinect.HandState.Closed)
            print("leftClose");
        if (body.HandRightState == Kinect.HandState.Open)
            print("leftOpen");
        if (body.HandRightState == Kinect.HandState.Lasso)
            print("leftLesso");*/
        float leftX = body.Joints[Kinect.JointType.HandLeft].Position.X * 10;
        float leftY = body.Joints[Kinect.JointType.HandLeft].Position.Y * 10;
        float leftZ = body.Joints[Kinect.JointType.HandLeft].Position.Z * 10;
        float rightX = body.Joints[Kinect.JointType.HandRight].Position.X * 10;
        float rightY = body.Joints[Kinect.JointType.HandRight].Position.Y * 10;
        float rightZ = body.Joints[Kinect.JointType.HandRight].Position.Z * 10;

        float leftDist = (float)System.Math.Sqrt(System.Math.Pow(leftX - a.transform.position.x, 2)
            + System.Math.Pow(leftY - a.transform.position.y, 2)
            + System.Math.Pow(leftZ - a.transform.position.z, 2));

        float rightDist = (float)System.Math.Sqrt(System.Math.Pow(rightX - a.transform.position.x, 2)
            + System.Math.Pow(rightY - a.transform.position.y, 2)
            + System.Math.Pow(rightZ - a.transform.position.z, 2));

        if (leftDist < 1 && rightDist > 1)//左手在操作范围内
        {
            if (body.HandLeftState == Kinect.HandState.Closed)//左手闭合
            {
                a.transform.position = new Vector3(leftX, leftY, leftZ);
            }
        }
        if (rightDist < 1 && leftDist>1)//右手在操作范围内
        {
            if (body.HandRightState == Kinect.HandState.Closed)//右手闭合
            {
                a.transform.position = new Vector3(rightX, rightY, rightZ);
            }
        }
        if (rightDist < 1 && leftDist < 1)//双手均在操作范围内
        {
            if (body.HandLeftState == Kinect.HandState.Closed && body.HandRightState == Kinect.HandState.Closed)//双手闭合
            { }
            else if (body.HandLeftState == Kinect.HandState.Closed && body.HandRightState == Kinect.HandState.Open)//左手闭合，右手张开
            {
                a.transform.position = new Vector3(leftX, leftY, leftZ);
            }
            else if (body.HandLeftState == Kinect.HandState.Open && body.HandRightState == Kinect.HandState.Closed)//右手闭合，左手张开
            {
                a.transform.position = new Vector3(rightX, rightY, rightZ);
            }
        }

        //a.transform.position = new Vector3(body.Joints[Kinect.JointType.HandLeft].Position.X,body.Joints[Kinect.JointType.HandLeft].Position.Y,body.Joints[Kinect.JointType.HandLeft].Position.Z);
        //a.transform.position *= 10;
    }
    
    private static Color GetColorForState(Kinect.TrackingState state)
    {
        switch (state)
        {
        case Kinect.TrackingState.Tracked:
            return Color.green;

        case Kinect.TrackingState.Inferred:
            return Color.red;

        default:
            return Color.black;
        }
    }
    
    private static Vector3 GetVector3FromJoint(Kinect.Joint joint)
    {
        return new Vector3(joint.Position.X * 10, joint.Position.Y * 10, joint.Position.Z * 10);
    }
}
