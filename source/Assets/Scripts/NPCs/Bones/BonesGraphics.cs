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

    public float opacity = 1f;
    public bool isTesting = false;

    [System.NonSerialized]
    public static int OPAQUE_THRESHOLD = 7;

	// Unity hooks
	void Awake () {
		nav = gameObject.GetParent ().GetComponent<NavMeshAgent> ();
        ren = gameObject.transform.FindChild("skeleton").GetComponent<SkinnedMeshRenderer>();
        volumes = new List<Volume>(GameObject.FindObjectsOfType<Volume>());

        perceivableVolumes = new List<Volume>();
	}

	void Update () {
        perceivedVolume = 0;

        DeterminePerceivableVolumeSources();
        foreach (Volume v in perceivableVolumes)
        {
            Debug.Log("Volume source: " + v.gameObject.name + ", current perceived volume: " + perceivedVolume);
            perceivedVolume += (v.GetVolume() - nav.remainingDistance / 3 > 0) ? v.GetVolume() - nav.remainingDistance / 3 : 0;
            Debug.Log("Post-perception volume: " + perceivedVolume);
        }
        // TODO: Name this better
        RemovePerceivableVolumeSources();

        if (isTesting) return;
        opacity = perceivedVolume / (float)OPAQUE_THRESHOLD;

        if (opacity > 1f) opacity = 1f;

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
            if (v.GetVolume() - nav.CalculatePathLength(v.gameObject.transform.position) <= 0)
                if (perceivableVolumes.Contains(v))
                {
                    perceivableVolumes.Remove(v);
                    volumes.Add(v);
                }
                else
                {
                    perceivableVolumes.Add(v);
                    volumes.Remove(v);
                }
        }
    }

    public void RemovePerceivableVolumeSources()
    {
        foreach (Volume v in perceivableVolumes)
        {
            if (volumes.Contains(v)) volumes.Remove(v);
        }
    }
}
