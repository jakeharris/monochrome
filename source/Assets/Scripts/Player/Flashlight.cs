using UnityEngine;
using System.Collections;

[RequireComponent (typeof (Light))]

public class Flashlight : MonoBehaviour {

	public float intensity = 3.87f;

	void Awake () {
		this.transform.light.intensity = intensity;
	}

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

	// NOTES:
	// In the future I'll want to make this flicker.
	// To do so, I think I want the following:
	// - float timer -> Every <timer> seconds, generate a random number.
	// - float repeatTimer -> After <repeatTimer> seconds, stop 
	//                        increasing the chances of a repeat flicker.

	// - if we <are flickering>, 
	// --- set <light.intensity> to be <intensity>
	// --- and continue;

	// - if random number plus a repeat modifier is above threshold
	// --- then we are flickering,
	// --- an infinite cast of torches, perfectly arranged,
	// --- sordid rows of sorted rose.
	// --- and it's scary, that's the truth of it.
	// --- so turn the lights out,
	// --- dim us down, 
	// --- hear us quiet and lap at the dark,
	// --- offcolor flames each armed to the teeth
	// --- with mountainous flowers and a flourish of heat.
	// --- suck it dry, rubbing the whole damn thing
	// --- we flicker, not ours, losing self,
	// --- together.
	// --- here, in order, lives the greatest chaos,
	// --- and we are not dead yet.

	// --- ...
	// --- Anyways.
	// --- then we are flickering (<isFlickering>),
	// --- so turn <light.intensity> to 0,
	// --- and,
	// --- if <repeat modifier> is greater than 0,
	// ----- subtract <usage amount> from <repeat modifier>.
	// --- otherwise,
	// ----- set <repeat modifier> to <repeat modifier base amount>.
}
