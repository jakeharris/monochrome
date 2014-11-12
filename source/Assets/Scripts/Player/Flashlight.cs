using UnityEngine;
using System.Collections;

[RequireComponent (typeof (Light))]

public class Flashlight : MonoBehaviour {

	public float intensity = 3.87f;

	// Update is called once per frame
	void Update () {
		if (Input.GetButtonUp ("Flashlight")) {
			this.transform.light.intensity = (IsOn()) ? 0 : intensity;
		}
		this.transform.eulerAngles = new Vector3 (
			this.transform.parent.FindChild ("Main Camera").transform.eulerAngles.x,
			this.transform.eulerAngles.y,
			this.transform.eulerAngles.z
		);
	}

	private bool IsOn() {
		return this.transform.light.intensity != 0;
	}
}
