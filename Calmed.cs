using System;

namespace XRL.World.Parts.Effects
{
	[Serializable]
	public class acegiak_Calmed : Effect
	{

		private int Bonus;
        public GameObject who;

		public acegiak_Calmed()
		{
			base.DisplayName = "Sonically Calmed";
		}

		public acegiak_Calmed(int _Duration)
			: this()
		{
			Duration = _Duration;
		}

		public override bool SameAs(Effect e)
		{
			return false;
		}

		public override string GetDescription()
		{
			return "calmed";
		}

		public override string GetDetails()
		{
			return "Sonically calmed.";
		}

		public override bool Apply(GameObject Object)
		{
			if (Object.HasEffect("Emboldened"))
			{
				Emboldened emboldened = Object.GetEffect("Emboldened") as Emboldened;
				if (Duration > emboldened.Duration)
				{
					emboldened.Duration = Duration;
				}
				return false;
			}
			
			return true;
		}

		public override void Remove(GameObject Object)
		{
			
		}

		public override void Register(GameObject Object)
		{
			Object.RegisterEffectEvent(this, "acegiak_SeedGrow");
			base.Register(Object);
		}

		public override void Unregister(GameObject Object)
		{
			Object.UnregisterEffectEvent(this, "acegiak_SeedGrow");
		}

		public override bool FireEvent(Event E)
		{
			if (E.ID == "acegiak_SeedGrow" && Duration > 0)
			{
				GameObject tree = E.GetGameObjectParameter("To");
                if(tree != null && tree.pBrain != null){
                    if(who != null){
                        tree.pBrain.BecomeCompanionOf(who);
                        if (tree.pBrain.GetFeeling(who) < 0)
                        {
                            tree.pBrain.SetFeeling(who, 0);
                        }
                    }

                    tree.pBrain.Goals.Clear();
                    tree.UpdateVisibleStatusColor();
                }
			}
			return true;
		}
	}
}
