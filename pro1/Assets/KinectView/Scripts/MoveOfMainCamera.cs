using UnityEngine;
using System.Collections;

public class test : MonoBehaviour {

	// Use this for initialization
    private int speed = 5;
    public GameObject _pre_Cube;
    private GameObject a;
	void Start () {
        a = GameObject.Instantiate(_pre_Cube);
        a.transform.rotation = Quaternion.identity;
        a.transform.position = new Vector3(0, 0, 0);
	}
	// Update is called once per frame
	void Update () {
        float x = Input.GetAxis("Horizontal") * Time.deltaTime * speed;
        float z = Input.GetAxis("Vertical") * Time.deltaTime * speed;
        transform.Translate(x, 0, z);
        
	}
}
