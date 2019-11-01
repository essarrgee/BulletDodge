using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player {
	private int _health;
	private float _speed;
	
	private GameObject _object;
	private GameObject _model;
	private Rigidbody _rigidbody;
	
	private Camera _camera;
	
	private GameObject _bulletPrefab;
	private float _bulletSpeed;
	private float _bulletLifetime;
	
	private Vector3 moveDirection;
	private bool shooting;
	private float shootTick;
	private float shootCooldown;
	private float damageTick;
	
	public int health {
		get => _health; 
		set {
			_health = value;
			if (value < 0) {
				_health = 0;
			}
		}
	}
	public float speed {
		get => _speed; 
		set {
			_speed = value;
		}
	}
	
    public Player() {
		health = 3;
		speed = 10f;
		
		shooting = false;
		shootTick = 0f;
		shootCooldown = 0.04f;
		
		_bulletSpeed = 30f;
		_bulletLifetime = 2f;
	}
	
	public void Update(float deltaTime) {
		Move();
		Shoot();
		CheckDead();
		if (_object && damageTick > 0) {
			damageTick -= deltaTime;
			if (Mathf.Round(damageTick*10)%2 == 1)
				_object.transform.localScale = new Vector3(0,0,0);
			else
				_object.transform.localScale = new Vector3(1f,1f,1f);
		}
	}
	
	public void SetObject(GameObject _object) {
		this._object = _object;
		//_model = _object.transform.Find("Model").gameObject;
		_rigidbody = _object.GetComponent<Rigidbody>();
		if (_rigidbody) {
			this._rigidbody = _rigidbody;
		}
	}
	
	public void SetBulletPrefab(GameObject _object) {
		this._bulletPrefab = _object;
	}
	
	public void SetCamera(Camera _camera) {
		this._camera = _camera;
	}
	
	public void Movement() {
		float xdir = Input.GetAxisRaw("Horizontal");
		float zdir = Input.GetAxisRaw("Vertical");
		moveDirection = new Vector3(xdir, 0, zdir).normalized; 
		//Quaternion.Euler(0, cam.transform.rotation.eulerAngles.y, 0) * 
		//UpdateModelDirection(); //tilt model if moving left or right
	}
	
	public void Shooting() {
		if (Input.GetKeyDown(KeyCode.Space)) {
			shootTick = 0f;
		}
		if (Input.GetKey(KeyCode.Space)) {
			shooting = true;
		}
		else {
			shooting = false;
		}
	}
	
	public void Move() {
		if (_rigidbody) {
			_rigidbody.velocity = moveDirection*_speed;
		}
	}
	
	public void Shoot() {
		if (_rigidbody) {
			if (shooting && shootTick >= shootCooldown) {
				shootTick = 0f;
				GameObject newBullet = GameObject.Instantiate(_bulletPrefab, _rigidbody.position, Quaternion.identity);
				Rigidbody bulletRigidbody = newBullet.GetComponent<Rigidbody>();
				newBullet.name = _bulletPrefab.name;
				newBullet.tag = "PlayerBullet";
				bulletRigidbody.AddForce(_object.transform.forward*_bulletSpeed, ForceMode.Impulse);
				GameObject.Destroy(newBullet, _bulletLifetime);
			}
			else {
				shootTick += Time.deltaTime;
			}
		}
	}
	
	public void OnTriggerEnter(Collider collide) {
		GameObject bullet = collide.gameObject;
		if (bullet && bullet.tag == "EnemyBullet") {
			GameObject.Destroy(bullet);
			TakeDamage(1);
		}
	}
	
	void TakeDamage(int amount) {
		if (damageTick <= 0f) {
			health -= amount;
			damageTick = 2;
		}
	}
	
	public void CheckDead() {
		if (_health <= 0) {
			GameObject.Destroy(_object);
		}
	}
	
	//GameObject.Find("Player");
}
