using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy : MonoBehaviour {
	public int Health = 20;
	public float Speed = 20f;
	
	private float stopAtPositionZ = 9f;
	
	public enum ShootModeEnum {Forward, TowardTarget, HomingTarget};
	public ShootModeEnum ShootMode = ShootModeEnum.Forward;
	
	private GameObject _model;
	private Rigidbody _rigidbody;
	
	public GameObject BulletPrefab;
	public float BulletSpeed = 30f;
	private float BulletLifetime = 6f;
	
	public Vector3 moveDirection = new Vector3(0,0,-1f);
	private bool shooting;
	private float shootTick;
	private float shootCooldown;
	private GameObject target;
	private float damageTick;
	
	void Awake() {
		
		stopAtPositionZ = Random.Range(7,11);
		
		shooting = false;
		shootTick = 0f;
		shootCooldown = 2f;

		//_model = transform.Find("Model").gameObject;
		_rigidbody = GetComponent<Rigidbody>();
	}
	
	void Update() {
		Shooting();
		if (damageTick > 0) {
			damageTick -= Time.deltaTime;
			if (Mathf.Round(damageTick*10)%2 == 1)
				transform.localScale = new Vector3(0,0,0);
			else
				transform.localScale = new Vector3(1.5f,1f,1.5f);
		}
		CheckDead();
	}
	
	void FixedUpdate() {
		Shoot();
		Move();
	}
	
	public void SetHealth(int amount) {
		if (amount > 0) {
			Health = amount;
		}
	}
	
	public void SetBulletSpeed(float amount) {
		if (amount > 0) {
			BulletSpeed = amount;
			BulletLifetime -= (BulletSpeed/10);
		}
	}
	
	public void SetShootMode(int level) {
		float random = (Random.Range(0f,3f));
		if (level < 3) {
			ShootMode = ShootModeEnum.Forward;
		}
		else if (level >= 3 && level < 5) {
			if (random < 1.2f) {
				ShootMode = ShootModeEnum.Forward;
			}
			else {
				ShootMode = ShootModeEnum.TowardTarget;
			}
		}
		else if (level >= 5 && level <= 8) {
			ShootMode = ShootModeEnum.TowardTarget;
		}
	}
	
	void Shooting() {
		shooting = true;
	}
	
	void Move() {
		if (_rigidbody && _rigidbody.position.z >= stopAtPositionZ) {
			_rigidbody.velocity = moveDirection*Speed;
		}
		else if (_rigidbody && _rigidbody.position.z < stopAtPositionZ) {
			_rigidbody.velocity = moveDirection*0;
		}
	}
	
	void Shoot() {
		if (shooting && shootTick >= shootCooldown) {
			shootTick = 0f;
			GameObject newBullet = GameObject.Instantiate(BulletPrefab, _rigidbody.position, Quaternion.identity);
			Rigidbody bulletRigidbody = newBullet.GetComponent<Rigidbody>();
			newBullet.name = BulletPrefab.name;
			newBullet.tag = "EnemyBullet";
			if (ShootMode == ShootModeEnum.Forward) {
				bulletRigidbody.AddForce(transform.forward*BulletSpeed, ForceMode.Impulse);
			}
			else if (ShootMode == ShootModeEnum.TowardTarget) {
				GameObject player = GameObject.Find("Player");
				Vector3 aimDirection = (player.transform.position - newBullet.transform.position).normalized;
				bulletRigidbody.AddForce(aimDirection*BulletSpeed, ForceMode.Impulse);
			}
			GameObject.Destroy(newBullet, BulletLifetime);
		}
		else {
			shootTick += Time.deltaTime;
		}
	}
	
	public void TakeDamage(int amount) {
		if (amount > 0) {
			Health -= amount;
			damageTick = 1;
		}
	}
	
	public void CheckDead() {
		if (Health <= 0) {
			GameManager.Instance.DecreaseSpawnCount(1);
			Destroy(gameObject);
		}
	}
}
