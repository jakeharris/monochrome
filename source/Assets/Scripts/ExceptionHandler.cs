using UnityEngine;
using System.Collections;

public class ExceptionHandler : MonoBehaviour {

	public static ExceptionHandler instance { get; private set; }

	// Based on code by @yoyo and @Lone Coder (http://answers.unity3d.com/questions/47659/callback-for-unhandled-exceptions.html)
	void Awake() {
		instance = this;
		Application.RegisterLogCallback (HandleException);
	}
	private void HandleException(string condition, string stackTrace, LogType type) {
		if (type == LogType.Exception) {
			Debug.Log (type + ": " + "\n\tCaused by: " + condition + "\n\tStack trace: " + stackTrace);
		}
	}
}
