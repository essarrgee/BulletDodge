using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BulletDetect : MonoBehaviour {
	
	private Enemy _enemyScript;
    private BoxCollider _collision;
	
	void Awake() {
		_enemyScript = transform.parent.gameObject.GetComponent<Enemy>();
		_collision = GetComponent<BoxCollider>();
	}
	
	void OnTriggerEnter(Collider collide) {
		GameObject bullet = collide.gameObject;
		if (bullet && bullet.tag == "PlayerBullet") {
			Destroy(bullet);
			_enemyScript.TakeDamage(1);
		}
	}
}
