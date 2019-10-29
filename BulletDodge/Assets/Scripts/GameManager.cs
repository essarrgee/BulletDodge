using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour {
	[Header("Prefabs")]
	public GameObject _playerPrefab;
	public GameObject _enemyPrefab;
	public GameObject _bulletPrefab;
	
    private Player _player;
	private Camera _camera;
	
	private List<GameObject> _enemyList;
	
	private List<GameObject> _powerupList;
	
    void Awake() {
		GameObject newPlayerObject = Instantiate(_playerPrefab, new Vector3(0,0,0), Quaternion.identity);
        newPlayerObject.name = _playerPrefab.name;
		_player = new Player();
		_player.SetObject(newPlayerObject);
		_camera = Camera.main;
		_player.SetCamera(_camera);
		_player.SetBulletPrefab(_bulletPrefab);
		
		Physics.IgnoreLayerCollision(0,11); //default, bullet
    }


    void Update() {
		UpdateCamera();
        _player.Movement();
		_player.Shooting();
    }
	
	void FixedUpdate() {
		_player.Move();
		_player.Shoot();
	}
	
	void UpdateCamera() {
		_camera.transform.position = new Vector3(0,20,-5);
		_camera.transform.rotation = Quaternion.Euler(60,0,0);
	}
}
