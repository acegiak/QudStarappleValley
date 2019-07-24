using System;
using System.Collections.Generic;
using System.Text;
using XRL.Core;
using XRL.Rules;
using XRL.World;
using XRL.World.Parts;
using XRL.World.Parts.Effects;

namespace XRL.Liquids
{
	[Serializable]
	internal class LiquidBeer : BaseLiquid
	{
		public new const int ID = 135;

		public new const string Name = "beer";

		[NonSerialized]
		public static List<string> Colors = new List<string>(2)
		{
			"y",
			"w"
		};

		public LiquidBeer()
			: base(15, "beer", 350, 2000, 1.1f)
		{
		}

		public override List<string> GetColors()
		{
			return Colors;
		}

		public override string GetColor()
		{
			return "y";
		}

		public override string GetName(LiquidVolume Liquid)
		{
			return "&wbeer";
		}

		public override string GetAdjective(LiquidVolume Liquid)
		{
			return "&wyeasty";
		}

		public override string GetSmearedName(LiquidVolume Liquid)
		{
			return "&wyeasty";
		}

		public override string GetPreparedCookingIngredient()
		{
			return "quicknessMinor";
		}

		public override bool Drank(LiquidVolume Liquid, int Volume, GameObject Target, StringBuilder Message, ref bool ExitInterface)
		{
			long turns = XRLCore.Core.Game.Turns;
			if (Target.HasPart("Stomach"))
			{
				Stomach part = Target.GetPart<Stomach>();
				Target.FireEvent(Event.New("AddWater", "Amount", 2 * Volume, "Forced", 1));
				Target.FireEvent(Event.New("AddFood", "Satiation", "Snack"));
				Message.Append("It is bitter and delicious.\n");
				Message.Append("You are now " + part.FoodStatus() + ".");
			}
			if (!Target.Property.ContainsKey("ConfuseOnEatTurnWine"))
			{
				Target.Property.Add("ConfuseOnEatTurnWine", XRLCore.Core.Game.Turns.ToString());
			}
			if (!Target.Property.ContainsKey("ConfuseOnEatAmountWine"))
			{
				Target.Property.Add("ConfuseOnEatAmountWine", "0");
			}
			long longProperty = Target.GetLongProperty("ConfuseOnEatTurnWine");
			int num = (int)Target.GetLongProperty("ConfuseOnEatAmountWine");
			if (turns - longProperty > 80)
			{
				num = 0;
			}
			if (num > Math.Max(1, Target.Statistics["Toughness"].Modifier * 2) && Target.ApplyEffect(new Confused(Stat.Roll("5d5"), 1)))
			{
				ExitInterface = true;
			}
			Target.SetLongProperty("ConfuseOnEatTurnWine", turns);
			Target.Property["ConfuseOnEatAmountWine"] = (num + 1).ToString();
			return true;
		}

		public override void RenderBackground(LiquidVolume Liquid, RenderEvent eRender)
		{
			eRender.ColorString = "^y" + eRender.ColorString;
		}

		public override void RenderPrimary(LiquidVolume Liquid, RenderEvent eRender)
		{
			Physics physics = Liquid.ParentObject.GetPart("Physics") as Physics;
			if (physics.Temperature <= physics.FreezeTemperature)
			{
				eRender.RenderString = "~";
				eRender.ColorString = "&w^y";
				return;
			}
			Render render = Liquid.ParentObject.GetPart("Render") as Render;
			int num = (XRLCore.CurrentFrame + Liquid.nFrameOffset) % 60;
			if (Stat.RandomCosmetic(1, 600) == 1)
			{
				eRender.RenderString = string.Empty + '\u000f';
				eRender.ColorString = "&w^y";
			}
			if (Stat.RandomCosmetic(1, 60) == 1 || render.ColorString == "&b")
			{
				if (num < 15)
				{
					render.RenderString = string.Empty + '÷';
					render.ColorString = "&y^w";
				}
				else if (num < 30)
				{
					render.RenderString = "~";
					render.ColorString = "&y^w";
				}
				else if (num < 45)
				{
					render.RenderString = string.Empty + '\t';
					render.ColorString = "&y^w";
				}
				else
				{
					render.RenderString = "~";
					render.ColorString = "&y^w";
				}
			}
		}

		public override void RenderSecondary(LiquidVolume Liquid, RenderEvent eRender)
		{
			eRender.ColorString += "&w";
		}

		public override void RenderSmearPrimary(LiquidVolume Liquid, RenderEvent eRender)
		{
			int num = XRLCore.CurrentFrame % 60;
			if (num > 5 && num < 15)
			{
				eRender.ColorString = "&w";
			}
			base.RenderSmearPrimary(Liquid, eRender);
		}

		public override void ObjectEnteredCell(LiquidVolume Liquid, GameObject GO)
		{
		}

		public override float GetValuePerDram()
		{
			return 3.8f;
		}

		public override int GetNavigationWeight(LiquidVolume Liquid, GameObject GO, bool Smart, bool Slimewalking, ref bool Uncacheable)
		{
			return 0;
		}
	}
}
