using UnityEngine;
using System.Collections;

public class MapGenerator : MonoBehaviour {

	public GameObject Floor1;
	public GameObject Wall1;
	public GameObject Roof1;
	// Use this for initialization
	void Start () {
		for (float y = -0.5f; y < 100; y++) {
			for (float x = -0.5f; x < 100; x++) {
				Instantiate(Floor1, new Vector3(x*0.5f, 0, y*0.5f), Floor1.transform.rotation);
			}
		}
		for (int y = 0; y < 50; y++) {
			for (int x = 0; x < 50; x++) {
				Instantiate(Roof1, new Vector3(x, 1, y), Roof1.transform.rotation);
			}
		}
		for (int y = 0; y < 50; y++) {
			Instantiate(Wall1, new Vector3(y, 0.5f, -0.5f), Wall1.transform.rotation);
		}
	}
	
	// Update is called once per frame
	void Update () {
	
	}
}
