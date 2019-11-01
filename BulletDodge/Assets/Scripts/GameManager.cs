using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class GameManager : MonoBehaviour {
	[Header("Prefabs")]
	public GameObject _playerPrefab;
	public GameObject _enemyPrefab;
	public GameObject _bulletPrefab;

	[Header("Stats")]
	public int Level = 1;
	
	private Player _player;
	private Camera _camera;
	
	private List<GameObject> _enemyList;
	
	private List<GameObject> _powerupList;
	
	private float spawnTick = 0;
	private float spawnTime = 3;
	private int spawnCount = 0;
	private int despawnCount = 0;
	private int despawnCountTotal = 0;
	private int spawnCountMax = 3;
	private int spawnCountTotal = 0;
	
	private GameObject _hudUI;
	private TextMeshProUGUI _levelUI;
	private TextMeshProUGUI _healthUI;
	
	public static GameManager Instance {get; set;}
	
    void Awake() {
		if (!Instance) {
			Instance = this;
		}
		else {
			Destroy(gameObject);
		}
		
		Level = 1;
		
		_hudUI = GameObject.Find("HUD");
		_levelUI = _hudUI.transform.Find("Level").gameObject.GetComponent<TextMeshProUGUI>();
		_healthUI = _hudUI.transform.Find("Health").gameObject.GetComponent<TextMeshProUGUI>();
		
		GameObject newPlayerObject = Instantiate(_playerPrefab, new Vector3(0,0,0), Quaternion.identity);
        newPlayerObject.name = _playerPrefab.name;
		_player = new Player();
		_player.SetObject(newPlayerObject);
		_camera = Camera.main;
		_player.SetCamera(_camera);
		_player.SetBulletPrefab(_bulletPrefab);
		
		Physics.IgnoreLayerCollision(9,11); //player, bullet
		Physics.IgnoreLayerCollision(10,11); //enemy, bullet
		Physics.IgnoreLayerCollision(11,11); //bullet, bullet
		Physics.IgnoreLayerCollision(10,12); //enemy, wall
		Physics.IgnoreLayerCollision(11,12); //bullet, wall
    }


    void Update() {
		UpdateCamera();
		UpdateHUD();
        _player.Movement();
		_player.Shooting();
		CheckCanSpawnEnemy();
		CheckCanIncreaseLevel();
    }
	
	void FixedUpdate() {
		_player.Update(Time.deltaTime);
	}
	
	void UpdateCamera() {
		_camera.transform.position = new Vector3(0,20,5);
		_camera.transform.rotation = Quaternion.Euler(90,0,0);
	}
	
	public void OnPlayerTriggerEnter(Collider collide) {
		_player.OnTriggerEnter(collide);
	}
	
	void CheckCanIncreaseLevel() {
		if (Level < 10 && despawnCount > (Level*3)) {
			despawnCount = 0;
			IncreaseLevel();
		}
	}
	
	void IncreaseLevel() {
		if (Level < 10) {
			Level += 1;
			spawnTime -= 0.15f;
			spawnCountMax += (int)(Level%2);
		}
		else {
			Level = 10;
		}
	}
	
	void CheckCanSpawnEnemy() {
		if (spawnTick > spawnTime) {
			spawnTick = 0;
			if (spawnCount < spawnCountMax) {
				SpawnEnemy();
			}
		}
		else {
			spawnTick += Time.deltaTime;
		}
	}
	
	public void IncreaseSpawnCount(int amount) {
		spawnCount += amount;
		spawnCountTotal += amount;
	}
	
	public void DecreaseSpawnCount(int amount) {
		spawnCount -= amount;
		despawnCount += amount;
		despawnCountTotal += amount;
	}
	
	void SpawnEnemy() {
		Vector3 spawnPosition = new Vector3(Random.Range(-11f,11f),0,17f);
		Quaternion spawnRotation = Quaternion.Euler(0,180f,0);
		GameObject newEnemy = Instantiate(_enemyPrefab, spawnPosition, spawnRotation);
		Enemy enemyScript = newEnemy.GetComponent<Enemy>();
		//Scales stats based on level
		enemyScript.SetHealth(5 + Random.Range(0,5+(Level*5)));
		enemyScript.SetBulletSpeed(4f+(float)Level*2f);
		enemyScript.SetShootMode(Level);
		IncreaseSpawnCount(1);
	}
	
	void UpdateHUD() {
		_levelUI.text = "Level: " + Level;
		_healthUI.text = "Health: " + _player.health;
	}
}
