using UnityEngine;
using System.Collections;

public class Movement : MonoBehaviour {

    public Vector3 target;
    public float speed = 1;
	// Use this for initialization
	void Start () {
	    
	}
	
	// Update is called once per frame
	void Update () {
        var MoveVector = target - this.transform.position;
        MoveVector.Normalize();
        this.transform.position += MoveVector * speed;
	}
}
