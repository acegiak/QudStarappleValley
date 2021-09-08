
using System;
using XRL.Core;
using XRL.World.Effects;
using System.Collections.Generic;
using System.Text;
using XRL.Liquids;
using XRL.Rules;
using XRL.UI;
using XRL.World.AI.GoalHandlers;
using System.Linq;

namespace XRL.World.Parts
{
	[Serializable]
	public class acegiak_Watering : IPart
	{

		public Guid ActivatedAbilityID = Guid.Empty;

		public acegiak_Watering()
		{
		}
		public override bool SameAs(IPart p)
		{
            
			return false;
		}




		public override bool AllowStaticRegistration()
		{
			return true;
		}

		public override void Register(GameObject Object)
		{
			Object.RegisterPartEvent(this, "GetInventoryActions");
			Object.RegisterPartEvent(this, "OwnerGetInventoryActions");
			Object.RegisterPartEvent(this, "CommandWater");
			Object.RegisterPartEvent(this, "Equipped");
			Object.RegisterPartEvent(this, "Unequipped");
			base.Register(Object);
		}


        public void Water(GameObject who){
			LiquidVolume volume = ParentObject.GetPart<LiquidVolume>();
			if(volume == null || volume.Volume <= 0){
				Popup.Show(ParentObject.The+ParentObject.DisplayNameOnly+" is empty.");
				return;
			}
            //IPart.AddPlayerMessage("startwater");

			List<Cell> list = PickFieldAdjacent(9,who,"Water").Where(b=>b.IsPassable(null,false)).ToList();

			//List<Cell> list = PickBurst(1,3,false,AllowVis.OnlyVisible);
			//IPart.AddPlayerMessage("picked");

			if (list == null)
			{
				//IPart.AddPlayerMessage("none");
				return;
			}
			//IPart.AddPlayerMessage("do");

            int PourAmount = (int)Math.Max(1,Math.Floor((double)(ParentObject.GetPart<LiquidVolume>().Volume/list.Count)));
            string getamount = Popup.AskString("How many drams each?",PourAmount.ToString(),3,1,"0123456789");
            PourAmount = Int32.Parse(getamount);
			foreach (Cell item in list)
			{
				//IPart.AddPlayerMessage("cell");
				if(item.IsPassable(null,false)){
					if(volume == null || volume.Volume <= 0){
						Popup.Show(ParentObject.The+ParentObject.DisplayNameOnly+" has run dry.");
						return;
					}
					acegiak_Seed.DoWater(who,ParentObject,item,PourAmount);
					//IPart.AddPlayerMessage("pour");
					//volume.PourIntoCell(ParentObject, item,1);
				}
			}

           
        }


		public override bool FireEvent(Event E)
		{
            if (E.ID == "GetInventoryActions")
            {
                E.GetParameter<EventParameterGetInventoryActions>("Actions").AddAction("Water", 'W', false, "&WW&yater", "CommandWater");
            }
            // else if (E.ID == "OwnerGetInventoryActions")
            // {
            //     GameObject GO = E.GetGameObjectParameter("Object");
            //     if(GO.GetPart<acegiak_Seed>() != null){

            //     }
            //     E.GetParameter<EventParameterGetInventoryActions>("Actions").AddAction("Water", 'W', false, "&WW&yater", "CommandWater", 5);
            // }
            else if (E.ID == "CommandWater")
            {
                // if(ParentObject.Equipped == null){
                //     Popup.Show("You must have a watering can equipped to water.");
                // }
				GameObject who = E.GetGameObjectParameter("Owner");
				if(who == null){
					who = ParentObject.Equipped;
				}
				if(who == null){
					who = ParentObject;
				}
                Water(who);
				E.RequestInterfaceExit();

            }else
            if (E.ID == "Equipped")
			{
				GameObject GO = E.GetGameObjectParameter("EquippingObject");
                ActivatedAbilities part = GO.GetPart<ActivatedAbilities>();
				if (part != null)
				{
                    ActivatedAbilityID = part.AddAbility("Water", "CommandWater", "Gear");
                    ActivatedAbilityEntry activatedAbilityEntry = part.AbilityByGuid[ActivatedAbilityID];
                    GO.RegisterPartEvent(this, "CommandWater");

                }
                
			}
			else if (E.ID == "Unequipped")
			{
				GameObject GO = E.GetGameObjectParameter("UnequippingObject");
				ActivatedAbilities part = GO.GetPart<ActivatedAbilities>();
				if (part != null)
				{
					part.RemoveAbility(ActivatedAbilityID);
                    GO.UnregisterPartEvent(this, "CommandWater");

                }

			}
			return base.FireEvent(E);
		}
	}
}







