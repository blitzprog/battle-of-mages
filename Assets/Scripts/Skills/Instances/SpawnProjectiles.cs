using BoM.Core;
using UnityEngine;
using UnityEngine.Pool;

namespace BoM.Skills.Instances {
	public class SpawnProjectiles : Instance {
		public Projectile projectilePrefab;
		public Transform spawn;
		public float radius;
		public float interval;
		public float duration;
		public bool randomizeStartPoint;
		public bool randomizeEndPoint;

		private float totalTime;
		private float intervalTime;
		private ObjectPool<Instance> projectilePool;

		private void Awake() {
			projectilePool = PoolManager.GetPool(projectilePrefab);
		}

		public override void Init() {
			totalTime = 0f;
			intervalTime = 0f;
		}

		private void Update() {
			totalTime += Time.deltaTime;
			intervalTime += Time.deltaTime;

			while(intervalTime >= interval) {
				intervalTime -= interval;
				SpawnProjectile();
			}

			if(totalTime >= duration) {
				Release();
			}
		}

		void SpawnProjectile() {
			var offset = Vector3.zero;

			if(randomizeStartPoint) {
				var random = Random.insideUnitCircle * radius;
				offset = new Vector3(random.x, 0f, random.y);
			}
			
			var projectile = projectilePool.Get();
			projectile.transform.SetLayer(gameObject.layer);
			projectile.transform.localPosition = spawn.position + offset;

			if(randomizeEndPoint) {
				var targetPoint = Random.insideUnitCircle * radius;
				var targetOffset = new Vector3(targetPoint.x, 0f, targetPoint.y);
				var target = transform.localPosition + targetOffset;
				projectile.transform.localRotation = Quaternion.LookRotation(target - projectile.transform.position);
			} else {
				projectile.transform.localRotation = spawn.localRotation;
			}

			projectile.skill = skill;
			projectile.caster = caster;
			projectile.pool = projectilePool;
			projectile.Init();
		}
	}
}
