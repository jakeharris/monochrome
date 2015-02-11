using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class BonesGraphics : MonoBehaviour {

	// Variables and editor configuration
	private List<Volume> volumes;
    private List<Volume> perceivableVolumes;
	private NavMeshAgent nav;
    private SkinnedMeshRenderer ren;

    private float perceivedVolume = 0f;
	private float amplitude = 2f / 3;

    public float opacity = 1f;
    public bool isTesting = false;
    public bool isEnabled = true;

    [System.NonSerialized]
    public static int OPAQUE_THRESHOLD = 14;

	// Unity hooks
	void Awake () {
		nav = gameObject.GetParent().GetParent().GetComponentInChildren<NavMeshAgent> ();
        
        ren = gameObject.GetComponent<SkinnedMeshRenderer>();
        volumes = new List<Volume>(GameObject.FindObjectsOfType<Volume>());

        Debug.Log("# of Volume objects: " + volumes.Count);

        perceivableVolumes = new List<Volume>();
	}

	void Update () {
        perceivedVolume = 0;

        DeterminePerceivableVolumeSources();
        foreach (Volume v in perceivableVolumes)
        {
            if (isTesting) Debug.Log("Volume source: " + v.gameObject.name + ", current perceived volume: " + perceivedVolume);
			perceivedVolume += (nav.remainingDistance > 0) ? v.GetVolume () * (v.GetVolume () / (nav.CalculatePathLength(v.gameObject.transform.position) / amplitude)) : 0;
            if (isTesting) Debug.Log("Post-perception volume: " + perceivedVolume);
        }
        // TODO: Name this better
        RemovePerceivableVolumeSources();

        if (!isEnabled) return;
        opacity = perceivedVolume / (float)OPAQUE_THRESHOLD;

        if (opacity > 1f) opacity = 1f;

        if (isTesting) Debug.Log("Opacity: " + opacity);

        Color c = ren.materials[0].color;
        c.a = opacity;

        // materials[0] is skeletonBody,
        // materials[1] is skeletonHead
        ren.materials[0].color = c;
        ren.materials[1].color = c;
	}

	// Extra methods
    public void DeterminePerceivableVolumeSources()
    {
        foreach (Volume v in volumes)
        {
            if (v.GetVolume() / nav.CalculatePathLength(v.gameObject.transform.position) < 1) {
                if (perceivableVolumes.Contains(v))
                {
                    perceivableVolumes.Remove(v);
                }
            }
            else if(!perceivableVolumes.Contains(v))
            {
                perceivableVolumes.Add(v);
            }
        }
    }

    public void RemovePerceivableVolumeSources()
    {

    }
}
