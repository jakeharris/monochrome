using UnityEngine;
using System.Collections;

public class BonesRattleTracker : MonoBehaviour {

	[System.NonSerialized]
	private Vector3 latestRattlePosition;
    [System.NonSerialized]
    private float timeSinceLatestRattle = 0f;

    public static float MINIMUM_TIME_BETWEEN_RATTLES = 3f;

    public void Update () {
        if (timeSinceLatestRattle < MINIMUM_TIME_BETWEEN_RATTLES)
            timeSinceLatestRattle += Time.deltaTime;
    }

	public void SetLatestRattlePosition(Vector3 v){
        // Prevent rattles from happening so frequently
        if (timeSinceLatestRattle < MINIMUM_TIME_BETWEEN_RATTLES)
            return;

		latestRattlePosition = v;
        timeSinceLatestRattle = 0;
	}
	public Vector3 GetLatestRattlePosition() {
		return latestRattlePosition;
	}
}
