using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public static class ExtensionMethods {
	public static void CrossFadeOrPlay(this Animation anim, string animationName) {
		if (anim.IsPlaying (animationName))
			return;
		if (anim.isPlaying)
			anim.CrossFade (animationName);
		else
			anim.Play (animationName);
	}
	public static float CalculatePathLength(this NavMeshAgent nav, Vector3 target) {
		if (!nav.enabled)
			throw new UnityException ("Nav mesh agent of GameObject " + nav.gameObject.name + " was not enabled when path length calculation was attempted.");
		NavMeshPath path = new NavMeshPath ();
		nav.CalculatePath (target, path);

		Vector3[] corners = new Vector3[path.corners.Length + 2];
		corners [0] = nav.transform.position;
		corners [corners.Length - 1] = target;

		for (int i = 0; i < path.corners.Length; i++)
			corners [i + 1] = path.corners [i];

		float pathLength = 0f;

		for (int i = 0; i < corners.Length - 1; i++)
			pathLength += Vector3.Distance (corners [i], corners [i + 1]);

		return pathLength;
	}
	public static GameObject GetParent(this GameObject g){
		return g.transform.parent.gameObject;
	}
    public static ArrayList GetNearbyObjectsWithTag(this Vector3 v, string tag, float r = 5f)
    {
        ArrayList objects = new ArrayList();
        Collider[] colliders = Physics.OverlapSphere(v, r);

        foreach (Collider c in colliders)
            if (c.gameObject.tag == tag) 
                objects.Add(c.gameObject);

        return objects;
    }
}
