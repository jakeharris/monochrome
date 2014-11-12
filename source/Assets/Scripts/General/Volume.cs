using UnityEngine;
using System.Collections;

public class Volume : MonoBehaviour, System.IEquatable<Volume> {

	private int v = 7;

	public int GetVolume () {
		return v;
	}
	// i can be negative.
	public void Adjust (int i) {
		v += i;
	}

	public void SetVolume(int i) {
		v = i;
	}

    public override bool Equals(object o)
    {
        if (o == null) return false;
        Volume v = o as Volume;
        if (v == null) return false;
        return Equals(v);
    }
    public bool Equals(Volume v)
    {
        return 
            (v.gameObject.GetParent().name == this.gameObject.GetParent().name)
            &&
            (v.GetVolume() == this.GetVolume());
    }
    public override int GetHashCode()
    {
        return base.GetHashCode();
    }
}
