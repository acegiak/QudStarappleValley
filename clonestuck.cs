using System;
using XRL.Core;
using XRL.UI;
using XRL.World.Parts;

namespace XRL.World.Effects
{
	[Serializable]
	public class cloneStuck : Effect
	{
		public GameObject DestroyOnBreak;

		public string Text = "stuck";

		public int SaveTarget = 15;

		public cloneStuck()
		{
			base.DisplayName = "stuck";
		}

		public cloneStuck(int Duration, int SaveTarget, GameObject DestroyOnBreak)
			: this()
		{
			base.Duration = Duration;
			this.SaveTarget = SaveTarget;
			this.DestroyOnBreak = DestroyOnBreak;
		}

		public cloneStuck(int Duration, int SaveTarget, GameObject DestroyOnBreak, string Text)
			: this(Duration, SaveTarget, DestroyOnBreak)
		{
			this.Text = Text;
		}

		public override bool SameAs(Effect e)
		{
			cloneStuck stuck = e as cloneStuck;
			if (stuck.DestroyOnBreak != DestroyOnBreak)
			{
				return false;
			}
			if (stuck.Text != Text)
			{
				return false;
			}
			if (stuck.SaveTarget != SaveTarget)
			{
				return false;
			}
			return base.SameAs(e);
		}

		public override string GetDetails()
		{
			return "Can't move.";
		}

		public override bool Apply(GameObject Object)
		{
			if (!Object.Statistics.ContainsKey("Energy"))
			{
				return false;
			}
			if (!Object.CanChangeMovementMode(ShowMessage: false, Involuntary: true))
			{
				return false;
			}
			if (Object.FireEvent("ApplyStuck"))
			{
				string verb = "are";
				string text = Text;
				string terminalPunctuation = "!";
				DidX("are", Text, "!");
				if (!Object.IsPlayer() && Visible())
				{
					Object.ParticleText(Effect.ConsequentialColor(null, Object) + "*" + Text + "*");
				}
				Object.Statistics["Energy"].BaseValue = 0;
				Object.MovementModeChanged(Involuntary: true);
				return true;
			}
			return false;
		}

		public override void Register(GameObject Object)
		{
			Object.RegisterEffectEvent(this, "BeginAttack");
			Object.RegisterEffectEvent(this, "BodyPositionChanged");
			Object.RegisterEffectEvent(this, "CanChangeBodyPosition");
			Object.RegisterEffectEvent(this, "CanChangeMovementMode");
			Object.RegisterEffectEvent(this, "EndTurn");
			Object.RegisterEffectEvent(this, "IsMobile");
			Object.RegisterEffectEvent(this, "LeaveCell");
			Object.RegisterEffectEvent(this, "MovementModeChanged");
			base.Register(Object);
		}

		public override void Unregister(GameObject Object)
		{
			Object.UnregisterEffectEvent(this, "BeginAttack");
			Object.UnregisterEffectEvent(this, "BodyPositionChanged");
			Object.UnregisterEffectEvent(this, "CanChangeBodyPosition");
			Object.UnregisterEffectEvent(this, "CanChangeMovementMode");
			Object.UnregisterEffectEvent(this, "EndTurn");
			Object.UnregisterEffectEvent(this, "IsMobile");
			Object.UnregisterEffectEvent(this, "LeaveCell");
			Object.UnregisterEffectEvent(this, "MovementModeChanged");
			if (DestroyOnBreak != null)
			{
				if (DestroyOnBreak.IsValid())
				{
					DestroyOnBreak.Destroy();
				}
				DestroyOnBreak = null;
			}
			base.Unregister(Object);
		}

		public override bool FireEvent(Event E)
		{
			if (E.ID == "IsMobile")
			{
				if (Duration > 0)
				{
					return false;
				}
			}
			else if (E.ID == "EndTurn")
			{
				if (DestroyOnBreak != null && DestroyOnBreak.IsInvalid())
				{
					DestroyOnBreak = null;
				}
				Duration--;
			}
			else if (E.ID == "LeaveCell" || E.ID == "BeginAttack")
			{
				if (E.HasParameter("Teleporting"))
				{
					Duration = 0;
				}
				else if (Duration > 0)
				{
					if (Object.MakeSave("Strength", SaveTarget - Object.GetIntProperty("Stable"), null, null, "Web Restraint"))
					{
						if (Object.IsPlayer())
						{
							Effect.AddPlayerMessage("&gYou break free!");
						}
						Object.RemoveEffect(this);
					}
					else
					{
						if (Object.IsPlayer())
						{
							Effect.AddPlayerMessage("You are " + Text + "&y!");
						}
						if (!E.HasParameter("Dragging") && !E.HasParameter("Forced"))
						{
							Object.UseEnergy(1000);
						}
						if (E.ID == "LeaveCell")
						{
							return false;
						}
					}
				}
			}
			else if (E.ID == "CanChangeMovementMode" || E.ID == "CanChangeBodyPosition")
			{
				if (Duration > 0)
				{
					if (E.GetIntParameter("ShowMessage", 0) > 0 && Object.IsPlayer())
					{
						Popup.Show("You are " + Text + "&y!");
					}
					return false;
				}
			}
			else if ((E.ID == "MovementModeChanged" || E.ID == "BodyPositionChanged") && Duration > 0)
			{
				Object.RemoveEffect(this);
			}
			return base.FireEvent(E);
		}

		public override bool Render(RenderEvent E)
		{
			if (Duration > 0)
			{
				int num = XRLCore.CurrentFrame % 60;
				if (num > 50 && num < 60)
				{
					E.Tile = "terrain/sw_web.bmp";
					E.RenderString = string.Empty + '\u000f';
					E.ColorString = "&Y^K";
				}
			}
			return true;
		}
	}
}
