using UnityEngine;
using System.Collections;

public interface IEnemyDetection {
	bool IsDetectable (Collider c);
	void OnTriggerStay(Collider c);
	void OnTriggerExit(Collider c);
}