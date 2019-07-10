using System;
using System.Collections.Generic;
using System.Text;
using XRL.Core;
using XRL.Messages;
using XRL.Rules;
using XRL.World;
using XRL.World.Parts;
using XRL.World.Parts.Effects;
using UnityEngine;

namespace XRL.Liquids
{
	[Serializable]
	public class acegiak_LiquidFurlingAgent : BaseLiquid
	{
		public new const int ID = 132;

		public new const string Name = "furlingagent";

		[NonSerialized]
		public static List<string> Colors = new List<string>(2)
		{
			"g",
			"W"
		};

		static acegiak_LiquidFurlingAgent(){

		}
		public acegiak_LiquidFurlingAgent()
			: base(Convert.ToByte(ID), "furlingagent", 350, 2000)
		{
		}

		public override List<string> GetColors()
		{
			return Colors;
		}

		public override string GetColor()
		{
			return "g";
		}

		public override string GetName(LiquidVolume Liquid)
		{
			return "&Wfurling agent";
		}

		public override string GetAdjective(LiquidVolume Liquid)
		{
			return "&Wswirling";
		}

		public override string GetSmearedName(LiquidVolume Liquid)
		{
			return "&Wmatted";
		}

		public override float GetValuePerDram()
		{
			return 60f;
		}

		public override bool Drank(LiquidVolume Liquid, int Volume, XRL.World.GameObject Target, StringBuilder Message, ref bool ExitInterface)
		{
			Message.Append("&WDisgusting!");
			return true;
		}

		public override void RenderBackground(LiquidVolume Liquid, RenderEvent eRender)
		{
			eRender.ColorString = "^K" + eRender.ColorString;
		}

		public override void RenderPrimary(LiquidVolume Liquid, RenderEvent eRender)
		{
			if (Liquid.ParentObject.IsFreezing())
			{
				eRender.RenderString = "~";
				eRender.ColorString = "&Y^k";
				return;
			}
			Render pRender = Liquid.ParentObject.pRender;
			int num = (XRLCore.CurrentFrame + Liquid.nFrameOffset) % 60;
			if (Stat.RandomCosmetic(1, 60) == 1 || pRender.ColorString == "&b")
			{
				if (num < 15)
				{
					pRender.RenderString = string.Empty + '÷';
					pRender.ColorString = "&Y^k";
				}
				else if (num < 30)
				{
					pRender.RenderString = "~";
					pRender.ColorString = "&Y^k";
				}
				else if (num < 45)
				{
					pRender.RenderString = string.Empty + '÷';
					pRender.ColorString = "&Y^k";
				}
				else
				{
					pRender.RenderString = "~";
					pRender.ColorString = "&Y^k";
				}
			}
		}

		public override void RenderSecondary(LiquidVolume Liquid, RenderEvent eRender)
		{
			eRender.ColorString += "&K";
		}

		public override void RenderSmearPrimary(LiquidVolume Liquid, RenderEvent eRender)
		{
			int num = XRLCore.CurrentFrame % 60;
			if (num > 5 && num < 15)
			{
				eRender.ColorString = "&K";
			}
			base.RenderSmearPrimary(Liquid, eRender);
		}

		public override void ObjectEnteredCell(LiquidVolume Liquid, XRL.World.GameObject GO)
		{
			if (Liquid.MaxVolume == -1 && GO.PhaseAndFlightMatches(Liquid.ParentObject) && GO.GetIntProperty("Slimewalking") <= 0 && GO.HasPart("Body"))
			{
				int difficulty = 10 + (int)((double)(Liquid.ComponentLiquids[2] * 5) / 1000.0);
				if (!GO.MakeSave("Strength,Agility", difficulty, null, null, "Furling Agent Restraint"))
				{
					GO.ApplyEffect(new cloneStuck(12, Liquid.ComponentLiquids[2] / 100, null));
				}
			}
		}

	}
}
