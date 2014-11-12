using UnityEngine;
using System.Collections;

[RequireComponent (typeof (BonesDetection))]

public class BonesAI : MonoBehaviour, IEnemyAI {

    #region Variables and editor configuration
	private BonesDetection det;
	private NavMeshAgent nav;
	private Animation anim;
	private BonesRattleTracker brt;
    private AudioSource aud;

    [System.NonSerialized]
    public State state = State.PATROLLING;

	[System.Serializable]
	public class BonesAIHound  {
		public bool isTesting = false;
		[System.NonSerialized]
		public bool isStillTesting = true;
		public float speed = 5f;
		public float stoppingDistance = 4f;

		public void ConfigureNav (Vector3 target, ref NavMeshAgent nav) {
			if (nav.speed != speed)
				nav.speed = speed;
			if (nav.stoppingDistance != stoppingDistance)
				nav.stoppingDistance = stoppingDistance;
			if (nav.destination != target)
				nav.destination = target;
		}

		public void ConfigureAnim (bool shouldIdle, ref Animation anim) {
			if (shouldIdle)
				anim.CrossFadeOrPlay ("idle");
			else
				anim.CrossFadeOrPlay ("run");
		}

		public bool ShouldIdle(NavMeshAgent nav) {
			return nav.remainingDistance <= nav.stoppingDistance;
		}
	}
	[System.Serializable]
	public class BonesAIInvestigate {
		private GameObject investigationTarget;
		public Color testingColor;

		public bool isTesting = false;
		[System.NonSerialized]
		public bool isStillTesting = true;
		public float speed = 2f;
		public float stoppingDistance = 2f;
		public int timeLimit = 3; //in s

		[System.NonSerialized]
		public float timeElapsed = 0;

		public void ConfigureNav(Vector3 target, ref NavMeshAgent nav) {
			if (nav.speed != speed)
				nav.speed = speed;
			if (nav.stoppingDistance != stoppingDistance) {
				nav.stoppingDistance = stoppingDistance;
			}
			if (nav.destination != target)
				nav.destination = target;
		}

		public void ConfigureAnim(bool shouldIdle, ref Animation anim) {
			if (shouldIdle)
				anim.CrossFadeOrPlay ("idle");
			else
				anim.CrossFadeOrPlay ("run");
		}

		public bool ShouldIdle(NavMeshAgent nav) {
			return nav.remainingDistance <= nav.stoppingDistance;
		}

		public void SetupTestEnvironment(GameObject target) {
			investigationTarget = target;
			if (investigationTarget != null)
				investigationTarget.renderer.material.color = testingColor;
		}

		public void MoveTestTargetTo(Vector3 target) {
			if (target.y > 0.02f)
				target.y = 0.02f;
			investigationTarget.transform.position = target;
		}
	}
	[System.Serializable]
	public class BonesAIPatrol {
		public bool isTesting = false;
		[System.NonSerialized]
		public bool isStillTesting = true;
		public float speed = 2f;
		public float stoppingDistance = 0;
		public Transform[] waypoints;

		public void ConfigureNav(ref NavMeshAgent nav) {
			if (nav.speed != speed)
				nav.speed = speed;
			if (nav.stoppingDistance != stoppingDistance)
				nav.stoppingDistance = stoppingDistance;
			if (nav.destination != GetCurrentWaypoint ())
				nav.destination = GetCurrentWaypoint ();
		}

		public void ConfigureAnim(bool shouldIdle, ref Animation anim){
			if (shouldIdle)
				anim.CrossFadeOrPlay ("idle");
			else
				anim.CrossFadeOrPlay ("run");
		}

		public bool ShouldIdle(NavMeshAgent nav){
			return nav.remainingDistance == Mathf.Infinity || nav.remainingDistance == 0;
		}

		public Vector3 GetCurrentWaypoint () {
			//TODO: Fill this shit out.
			// Needs to cycle through the waypoints.
			return waypoints[0].position;
		}
	}

	public BonesAIHound hound = new BonesAIHound ();
	public BonesAIInvestigate investigate = new BonesAIInvestigate ();
	public BonesAIPatrol patrol = new BonesAIPatrol ();

    public enum State
    {
        HOUNDING, INVESTIGATING, PATROLLING
    }
    #endregion
    #region Unity hooks
    void Awake () {
		det = gameObject.GetComponentInChildren<BonesDetection> ();
		nav = gameObject.GetComponent<NavMeshAgent> ();
		anim = gameObject.GetComponentInChildren<Animation> ();
		brt = GameObject.FindWithTag ("GameController").GetComponent<BonesRattleTracker> ();
        aud = gameObject.GetComponent<AudioSource>();

		if (investigate.isTesting)
			investigate.SetupTestEnvironment (gameObject.transform.Find ("Graphics/Investigation Target").gameObject);
		else
			Destroy (gameObject.transform.Find ("Graphics/Investigation Target").gameObject);
	}

	void Update () {
		if (det.DetectsPlayer ()) {
			Hound ();
		} else if (det.HasTarget ()) {
			Investigate ();
		} else {
			Patrol ();
		}
	}
    #endregion
    #region Interface implementation
    // Interface implementation
	public void Hound() {
		if (hound.isTesting && hound.isStillTesting) {
			Debug.Log ("Hounding");
			hound.isStillTesting = false;
			investigate.isStillTesting = true;
			patrol.isStillTesting = true;

			Debug.Log ("Hounding target: " + det.GetTarget ());
		}
		if (!det.HasTarget ())
			throw new UnassignedReferenceException ("Hound method is running, but we don't have a target set. This is wrong, since DetectsPlayer() should set our target.");

		hound.ConfigureNav (det.GetTarget (), ref nav);
		hound.ConfigureAnim (hound.ShouldIdle (nav), ref anim);

		// We use this so that when the player is within the stopping 
		// distance, the Bones still watch the player. Creepy!
		transform.LookAt (new Vector3(
			det.GetTarget ().x,
			Vector3.up.y,
			det.GetTarget ().z
		));

		// Rattle, since we've got a new pursuit.
        // (New pursuit means:
        //   1. We weren't already hounding anyone, and
        //   2. no one else was hounding this target.)
		if (!(state == State.HOUNDING) && !IsTargetBeingFollowed()) {
            Rattle();
            state = State.HOUNDING;
		}
	}
	public void Investigate() {
		if (investigate.isTesting && investigate.isStillTesting) {
			Debug.Log ("Investigating");
			hound.isStillTesting = true;
			investigate.isStillTesting = false;
			patrol.isStillTesting = true;
		}
		if (nav.pathStatus == NavMeshPathStatus.PathInvalid)
			Debug.Log ("NavMeshAgent couldn't calculate path to target.");
		if (investigate.isTesting && det.HasTarget ())
			investigate.MoveTestTargetTo (det.GetTarget ());

        if (state != State.INVESTIGATING) state = State.INVESTIGATING;

		investigate.ConfigureNav (det.GetTarget (), ref nav);
		investigate.ConfigureAnim (investigate.ShouldIdle (nav), ref anim);

		investigate.timeElapsed += Time.deltaTime;
		if (investigate.timeElapsed >= investigate.timeLimit) {
			investigate.timeElapsed = 0;
			det.ClearTarget ();
		}
	}
	public void Patrol() {
		if (patrol.isTesting && patrol.isStillTesting) {
			Debug.Log ("Patrolling");
			hound.isStillTesting = true;
			investigate.isStillTesting = true;
			patrol.isStillTesting = false;
		}

        if (state != State.PATROLLING) state = State.PATROLLING;

		patrol.ConfigureNav (ref nav);
		patrol.ConfigureAnim (patrol.ShouldIdle (nav), ref anim);
	}
    #endregion
    #region Extra methods
    void Rattle () {
        // brt.SetLatestRattlePosition will do nothing if we
        // attempt to rattle too soon, so we need to be certain
        // we rattled successfully before deciding to play the
        // audio.
        Vector3 previousRattlePosition = brt.GetLatestRattlePosition();
        brt.SetLatestRattlePosition (det.GetTarget ());
        if(previousRattlePosition != brt.GetLatestRattlePosition())
            aud.Play();
	}
    bool IsTargetBeingFollowed()
    {
        // I know this isn't good logic (should be vacuously true),
        // but that isn't helpful
        if (!det.HasTarget()) return false;

        ArrayList possibleFollowers = det.GetTarget().GetNearbyObjectsWithTag("Bones");
        foreach (GameObject follower in possibleFollowers)
        {
            var followerAI = follower.GetComponent<BonesAI>();
            if (followerAI != null && followerAI.state == State.HOUNDING) return true;
        }

        return false;
    }
    #endregion
}
