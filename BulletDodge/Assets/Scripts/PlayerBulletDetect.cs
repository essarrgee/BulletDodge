using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerBulletDetect : MonoBehaviour {
	//private Player _player;
	
    void Awake() {
		//_player = GameManager.Instance._player;
    }

    void OnTriggerEnter(Collider collide) {
		//_player.OnTriggerEnter(collide);
		GameManager.Instance.OnPlayerTriggerEnter(collide);
	}
}
