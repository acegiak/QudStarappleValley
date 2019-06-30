
using System;
using XRL.Rules;
using XRL.UI;
using XRL.Core;
using XRL.World.Parts.Effects;
using System.Collections.Generic;
using System.Text;
using XRL.Liquids;

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


        public void Water(){
            IPart.AddPlayerMessage("startwater");
            string text = PickDirectionS();
            if (!string.IsNullOrEmpty(text))
            {
            IPart.AddPlayerMessage("gotdir");
                Cell cellFromDirection = ParentObject.Equipped.pPhysics.CurrentCell.GetCellFromDirection(text);
                if (cellFromDirection != null)
                {
                    IPart.AddPlayerMessage("gotcell");
                    LiquidVolume volume = ParentObject.GetPart<LiquidVolume>();
                    if(volume == null || volume.Volume <= 0){
                        Popup.Show(ParentObject.The+ParentObject.DisplayNameOnly+" doesn't contain any liquid.");
                        return;
                    }
                    IPart.AddPlayerMessage("pour");
                    volume.PourIntoCell(ParentObject, cellFromDirection,1);
                }
            }
        }
		public override bool FireEvent(Event E)
		{
            if (E.ID == "GetInventoryActions")
            {
                E.GetParameter<EventParameterGetInventoryActions>("Actions").AddAction("Water", 'W', false, "&WW&yater", "CommandWater", 5);
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
                if(ParentObject.Equipped == null){
                    Popup.Show("You must have a watering can equipped to water.");
                }
                E.RequestInterfaceExit();
                Water();
            }else
            if (E.ID == "Equipped")
			{
				GameObject GO = E.GetGameObjectParameter("EquippingObject");
                ActivatedAbilities part = GO.GetPart<ActivatedAbilities>();
				if (part != null)
				{
                    ActivatedAbilityID = part.AddAbility("Water", "CommandWater", "Gear", -1);
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







