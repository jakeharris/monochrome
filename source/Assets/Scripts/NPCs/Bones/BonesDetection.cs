using UnityEngine;
using System.Collections;

public class BonesDetection : MonoBehaviour, IEnemyDetection {
	// Variables and editor configuration
	public Vector3 target = LOCATION_NULL;
	public Vector3 latestRattle = LOCATION_NULL;
	private NavMeshAgent nav;
	private GameObject player;
	private BonesRattleTracker brt;

    // Sight variables
    public float fovAngle = 110f;  //55 degrees to either side of straight ahead
    public float sightRadius = 8f;


	[System.NonSerialized]
	public static Vector3 LOCATION_NULL = new Vector3 (-1, -1, -1);

	// Unity hooks
	void Awake () {
		nav = gameObject.GetComponent<NavMeshAgent> ();
		player = GameObject.FindWithTag ("Player");
		brt = GameObject.FindWithTag ("GameController").GetComponent<BonesRattleTracker> ();
	}

	void Update () {
		if (IsDetectable (player.GetComponent<Collider> ()) && target != player.transform.position)
			target = player.transform.position;
		else if (brt.GetLatestRattlePosition () != latestRattle) {
			latestRattle = brt.GetLatestRattlePosition ();
			target = latestRattle;
		}
	}

	// Interface implementation
	public bool IsDetectable (Collider c) {
        // Bones detect the enemy through sight.
        Vector3 direction = c.transform.position - transform.position;
        float angle = Vector3.Angle(direction, transform.forward);

        if (angle < (fovAngle / 2))
        {
            RaycastHit hit;

            // IF we hit the player within this zone...
            if (Physics.Raycast(transform.position + transform.up, direction.normalized, out hit, sightRadius))
            {
                if (hit.collider.gameObject == player)
                {
                    return true;
                }
            }
        }

		// Bones also detect the enemy through hearing. Therefore, we
		// should check if the Bones can hear the colliding object.
		Volume volsc = c.gameObject.GetComponent<Volume> ();
		if (volsc == null) return false;

		int vol = volsc.GetVolume();
		if (vol <= 0) return false;

		// NOT straight-line distance -- travel/nav-mesh distance.
		return vol - nav.CalculatePathLength(c.gameObject.transform.position) > 0;
	}
	public void OnTriggerStay (Collider c) {
		// Consider changing this strategy to one where we look specifically
		// at the player object in an Update method and determine its detectability,
		// instead of one where we check within a sphere.
		//if (c.gameObject.name == player.name && IsDetectable (c) && target != c.gameObject.transform.position)
			//target = c.gameObject.transform.position;

	}
	public void OnTriggerExit (Collider c) {

	}

	// Extra methods
	public bool DetectsPlayer () {
		if (IsDetectable (player.GetComponent<Collider> ()) && HasTarget()) {
			return true;
		} else
			return false;
	}
	public bool HasTarget () {
		return target != LOCATION_NULL;
	}
	public Vector3 GetTarget () {
		return target;
	}
	public void ClearTarget () {
		target = LOCATION_NULL;
	}
}
