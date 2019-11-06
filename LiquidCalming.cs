using System;
using System.Collections.Generic;
using System.Text;
using XRL.Core;
using XRL.Messages;
using XRL.Rules;
using XRL.World;
using XRL.World.Parts;
using Qud.API;
using XRL.World.Parts.Effects;

namespace XRL.Liquids
{
	[Serializable]
	[IsLiquid]
	public class acegiak_LiquidSoothingAgent : BaseLiquid
	{
		public new const string ID = "soothingagent";

		public new const string Name = "soothingagent";

		[NonSerialized]
		public static List<string> Colors = new List<string>(2)
		{
			"m",
			"B"
		};

		public acegiak_LiquidSoothingAgent()
			: base("soothingagent", 350, 2000)
		{
		}

		public override List<string> GetColors()
		{
			return Colors;
		}

		public override string GetColor()
		{
			return "m";
		}

		public override string GetName(LiquidVolume Liquid)
		{
			return "&msoothing agent";
		}

		public override string GetAdjective(LiquidVolume Liquid)
		{
			return "&mmirrored";
		}

		public override string GetSmearedName(LiquidVolume Liquid)
		{
			return "&mellow";
		}

		public override float GetValuePerDram()
		{
			return 100f;
		}





		public override bool Drank(LiquidVolume Liquid, int Volume, GameObject Target, StringBuilder Message, ref bool ExitInterface)
		{
			Message.Append("&mIt is disgusting but incredibly calming.");
			Target.ApplyEffect(new Asleep(12, true,true));

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
			if (Stat.RandomCosmetic(1, 60) == 1 || pRender.ColorString == "&m")
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
			eRender.ColorString += "&m";
		}

		public override void RenderSmearPrimary(LiquidVolume Liquid, RenderEvent eRender)
		{
			int num = XRLCore.CurrentFrame % 60;
			if (num > 5 && num < 15)
			{
				eRender.ColorString = "&m";
			}
			base.RenderSmearPrimary(Liquid, eRender);
		}

		

	}
}
