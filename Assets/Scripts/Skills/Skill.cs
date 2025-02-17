using BoM.Core;
using UnityEngine;
using UnityEngine.Pool;

namespace BoM.Skills {
	[CreateAssetMenu(fileName = "Skill", menuName = "BoM/Skill", order = 1)]
	public class Skill : ScriptableObject, ISkill {
		public short Id { get; set; }
		public string Name { get => name; }
		public Sprite Icon { get => icon; }
		public float CoolDown { get => coolDown; }
		public float EnergyCost { get => energyCost; }

		[SerializeField] protected Sprite icon;
		public Instance prefab;
		public PositionType position;
		public RotationType rotation;
		[SerializeField] protected float coolDown;
		[SerializeField] protected float energyCost;
		public ObjectPool<Instance> pool;
	}
}
