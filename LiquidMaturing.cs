using System;
using System.Collections.Generic;
using System.Text;
using XRL.Core;
using XRL.Messages;
using XRL.Rules;
using XRL.World;
using XRL.World.Parts;
using Qud.API;

namespace XRL.Liquids
{
	[Serializable]
	public class acegiak_LiquidGrowthAgent : BaseLiquid
	{
		public new const int ID = 131;

		public new const string Name = "growthagent";

		[NonSerialized]
		public static List<string> Colors = new List<string>(2)
		{
			"b",
			"G"
		};

		public acegiak_LiquidGrowthAgent()
			: base(Convert.ToByte(ID), "growthagent", 350, 2000)
		{
		}

		public override List<string> GetColors()
		{
			return Colors;
		}

		public override string GetColor()
		{
			return "b";
		}

		public override string GetName(LiquidVolume Liquid)
		{
			return "&bgrowth agent";
		}

		public override string GetAdjective(LiquidVolume Liquid)
		{
			return "&bfertile";
		}

		public override string GetSmearedName(LiquidVolume Liquid)
		{
			return "&bburgeoning";
		}

		public override float GetValuePerDram()
		{
			return 100f;
		}

        public override bool PouredOn(LiquidVolume Liquid, GameObject GO)
		{
			if (Liquid.ComponentLiquids.Count != 1)
			{
				return true;
			}
			if (Liquid.Volume > 0 && GO.HasPart("Stomach") && GO.HasPart("Body") && GO.GetIntProperty("Inorganic") == 0 && GO.HasStat("Toughness"))
			{
				BodyPart p = GO.GetPart<Body>()._Body.GetParts().GetRandomElement();
                BodyPart p2 = GO.GetPart<Body>()._Body.GetParts().GetRandomElement();
                p2.AddPart(p.DeepCopy(GO,GO.GetPart<Body>()));
                GO.Statistics["Toughness"].Penalty += 3;
                IPart.AddPlayerMessage(GO.The+GO.DisplayNameOnly+"'s "+p2.Name+" sprout"+(p2.Plural?"":"s")+" "+(p.Plural?"":"a ")+p.Name+"!");
			}
			return true;
		}



		public override bool Drank(LiquidVolume Liquid, int Volume, GameObject Target, StringBuilder Message, ref bool ExitInterface)
		{
			Message.Append("&bDisgusting!\nYour form bulges.");
            if(Target.HasStat("Strength") && Target.HasStat("Toughness")){
                Target.Statistics["Strength"].Bonus += 1;
                Target.Statistics["Toughness"].Penalty += 2;
                Target.GetPart<Physics>().Weight += 10;
            }
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
			eRender.ColorString += "&b";
		}

		public override void RenderSmearPrimary(LiquidVolume Liquid, RenderEvent eRender)
		{
			int num = XRLCore.CurrentFrame % 60;
			if (num > 5 && num < 15)
			{
				eRender.ColorString = "&b";
			}
			base.RenderSmearPrimary(Liquid, eRender);
		}

		public override void ObjectEnteredCell(LiquidVolume Liquid, GameObject GO)
		{
			if (Liquid.MaxVolume != -1 || !GO.HasPart("Body") || !GO.PhaseAndFlightMatches(Liquid.ParentObject) || GO.GetIntProperty("Slimewalking") > 0 || Liquid.Volume > 1000)
			{
				return;
			}
			int num = 10 + (int)((double)(Liquid.ComponentLiquids[Convert.ToByte(ID)] * 5) / 1000.0);
			if (!GO.MakeSave("Agility,Toughness", num - GO.GetIntProperty("Stable"), null, null, "Unwanted Growth"))
			{

                BodyPart bp = null;

				GO.GetPart<Body>()._Body.ForeachPart(delegate(BodyPart BP){
                    if(BP.GetTotalMobility() > 0 && (bp == null || BP.GetTotalMobility() > bp.GetTotalMobility() )){
                        bp = BP;
                    }
                });

                if(bp != null){
                    if (GO.IsPlayer())
                    {
                        MessageQueue.AddPlayerMessage("&CYour "+bp.Name+" grow"+(bp.Plural?"":"s")+" a little!");
                    }
                    GO.pPhysics.Weight += 1;
                }
			}
		}

	}
}
