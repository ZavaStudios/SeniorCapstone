using UnityEngine;
using System.Collections;

public class FireballSparkFlashDisabler : MonoBehaviour {

    public float lightFlashTime = 0.05f;

    void Start () {
	
        Destroy(gameObject,lightFlashTime);

	}
}
