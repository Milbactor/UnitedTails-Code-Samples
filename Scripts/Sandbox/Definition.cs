namespace WhiteKnight.ValueObjects
{
	public class Definition
	{
		public const string WeaponTag = "Weapon";

		public const string IsEquiped = "IsEquiped";
		public const string TriggerEquip = "TriggerEquip";
		public const string TriggerUnequip = "TriggerUnequip";

		public const string IsAttacking = "isAttacking";
		public const string IsAirAttacking = "isAirAttacking";
		public const string attackTrigger = "TriggerAttack";
		public const string airAttackTrigger = "TriggerAirAttack";
		public const string attackTypeFloat = "AttackType";

		public const string attackStateName = "Attack";
		public const string TriggerJumpFlipStart = "TriggerJumpFlip";
		public const string TriggerJumpFlipStop = "TriggerJumpFlipStop";

		public const string TriggerDamage = "TriggerDamage";

		public const string EnemyTag = "Enemy";

		public const int AirAttackLayerIndex = 3;
		public const int AttackLayerIndex = 2;
		public const int EquipLayerIndex = 1;

		public const int MaxPlayersCount = 4;

		public const string playerTag = "Player";
		public const string blackPillarTag = "";

		public enum EnemyState
		{
			Walk,
			Wait,
			Chase,
			Attack,
			Freeze,
			Damage,
			Dead
		};
	}
}


