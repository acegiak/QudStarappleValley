using HistoryKit;
using System;
using XRL.Messages;
using XRL.Rules;
using XRL.UI;
using XRL.World.Parts.Effects;

namespace XRL.World.Parts
{
	[Serializable]
	public class acegiak_Smoked : IPart
	{
		

		public string Message = "&yThat hits the spot!";
        public string Effect;
        public string Duration;

		public override bool SameAs(IPart p)
		{
			acegiak_Smoked food = p as acegiak_Smoked;
			if (food.Message != Message)
			{
				return false;
			}
			return base.SameAs(p);
		}

		public override bool AllowStaticRegistration()
		{
			return true;
		}

		public override void Register(GameObject Object)
		{
			Object.RegisterPartEvent(this, "GetInventoryActions");
			Object.RegisterPartEvent(this, "InvCommandSmoke");
			base.Register(Object);
		}

        public bool SmokeThis(GameObject who, bool FromDialog)
		{
			string verb = "take";
			string preposition = "a puff on";
			GameObject parentObject = ParentObject;
			bool fromDialog = FromDialog;
			IPart.XDidYToZ(who, verb, preposition, parentObject, null, null, fromDialog);
			for (int i = 2; i < 5; i++)
			{
				ParentObject.Smoke(150, 180);
			}
			// if (who.IsPlayer())
			// {
			// 	MetricsManager.LogEvent("HookahsSmoked");
			// }
			who.UseEnergy(1000, "Item");
			who.FireEvent(Event.New("Smoked", "Object", ParentObject));
			return true;
		}

		public override bool FireEvent(Event E)
		{
		if (E.ID == "GetInventoryActions")
        {
            E.GetParameter<EventParameterGetInventoryActions>("Actions").AddAction("Smoke", 's',  false, "&Ws&ymoke", "InvCommandSmoke", 10);
        }
        else if (E.ID == "InvCommandSmoke" )
        {
            SmokeThis(E.GetGameObjectParameter("Owner"),  true);
            E.RequestInterfaceExit();
            if(Effect != null){
                Effect effect = Activator.CreateInstance(ModManager.ResolveType("XRL.World.Parts.Effects." + Effect)) as Effect;
				if (!string.IsNullOrEmpty(Duration))
				{
					effect.Duration = Stat.Roll(Duration);
				}

				GameObject parameter = E.GetGameObjectParameter("Owner");
				parameter.ApplyEffect(effect);
            }
            ParentObject.FireEvent(Event.New("DestroyObject"));
        }
						
			
		return base.FireEvent(E);
		}
	}
}
