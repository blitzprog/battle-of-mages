using BoM.Core;
using UnityEngine;
using UnityEngine.VFX;

namespace BoM.Skills.Instances {
	// Data
	public abstract class ProjectileData : Instance {
		[SerializeField] protected Rigidbody rigidBody;
		[SerializeField] protected SphereCollider collision;
		[SerializeField] protected VisualEffect particles;
		[SerializeField] protected Light lighting;
		[SerializeField] protected Explosion explosionPrefab;
		[SerializeField] protected float directHitDamage;
		[SerializeField] protected float splashDamage;
		[SerializeField] protected float speed;
		[SerializeField] protected float maxLifeTime;
		[SerializeField] protected float fadeOutTime;

		protected bool isAlive;
		protected float aliveTime;
		protected float deadTime;
		protected float lightIntensity;
		protected float fadeOutSpeed;
	}

	// Logic
	public class Projectile : ProjectileData {
		private const float finalGroundDistance = 0.15f;

		private void Awake() {
			lightIntensity = lighting.intensity;
			fadeOutSpeed = 1f / fadeOutTime;
		}

		public override void Init() {
			aliveTime = 0f;
			deadTime = 0f;
			lighting.intensity = lightIntensity;
			Revive();
		}

		private void Update() {
			if(isAlive) {
				AliveTimer();
			} else {
				DeadTimer();
			}
		}

		private void AliveTimer() {
			aliveTime += Time.deltaTime;

			if(aliveTime > maxLifeTime) {
				Die();
			}
		}

		private void DeadTimer() {
			deadTime += Time.deltaTime;
			lighting.intensity = Mathf.Lerp(lightIntensity, 0f, deadTime * fadeOutSpeed);

			if(deadTime > fadeOutTime) {
				Release();
			}
		}

		private void OnTriggerEnter(Collider other) {
			if(GetCollisionPoint(out RaycastHit hit)) {
				transform.localPosition = hit.point - transform.forward * finalGroundDistance;
			}

			// Direct hit damage
			if(other.tag == "Player") {
				var health = other.GetComponent<IHealth>();
				health.TakeDamage(directHitDamage, skill, caster);
			}

			Die();
			Explode();
		}

		private bool GetCollisionPoint(out RaycastHit hit) {
			Vector3 forward = transform.forward;
			Vector3 backward = -forward;
			float maxDistance = 1f;

			return Physics.Raycast(transform.localPosition + backward * maxDistance, forward, out hit, maxDistance, Physics.DefaultRaycastLayers);
		}

		private void Explode() {
			var explosion = PoolManager.Instantiate(explosionPrefab, transform.localPosition, Const.NoRotation);
			explosion.GetComponent<Explosion>().Damage = splashDamage;
			explosion.transform.SetLayer(gameObject.layer);
			explosion.skill = skill;
			explosion.caster = caster;
			explosion.Init();
		}

		private void Revive() {
			isAlive = true;
			collision.enabled = true;
			rigidBody.WakeUp();
			rigidBody.velocity = transform.forward * speed;
			particles.Play();
		}

		private void Die() {
			isAlive = false;
			collision.enabled = false;
			rigidBody.Sleep();
			particles.Stop();
		}
	}
}
