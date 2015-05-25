using System;

namespace Model
{
	public abstract class Effect : Alive
	{
		public abstract void onEffectStart(Monster m);

		public virtual void onEffectEnd(Monster m)
		{
			return;
		}
	}

	public class HpChangeEffect : Effect
	{
		private float changeHpValue;

		public HpChangeEffect(float value)
		{
			changeHpValue = value;
		}

		public override void onEffectStart(Monster m)
		{
			m.realHp += changeHpValue;
		}
	}

	public class HpTmpChangeEffect : Effect
	{
		private float changeHpValue;

		public HpTmpChangeEffect(float value)
		{
			changeHpValue = value;
		}

		public override void onEffectStart(Monster m)
		{
			m.realHp += changeHpValue;
		}

		public override void onEffectEnd(Monster m)
		{
			m.realHp -= changeHpValue;
		}
	}

	public class AttackChangeEffect : Effect
	{
		private float changeAttackValue;

		public AttackChangeEffect(float value)
		{
			changeAttackValue = value;
		}

		public override void onEffectStart(Monster m)
		{
			m.realAttack += changeAttackValue;
		}
	}

	public class DefenseTmpChangeEffect : Effect
	{
		private float changeDefenseValue;

		public DefenseTmpChangeEffect(float value)
		{
			changeDefenseValue = value;
		}

		public override void onEffectStart(Monster m)
		{
			m.realDefense += changeDefenseValue;
		}

		//Actually no need to override this method since defense will be set to 0 when battle starts
		public override void onEffectEnd(Monster m)
		{
			m.realDefense -= changeDefenseValue;
		}
	}
}
