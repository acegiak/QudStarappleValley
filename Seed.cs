using System;
using XRL.Rules;
using XRL.UI;
using XRL.World.Parts.Effects;
using System.Collections.Generic;

namespace XRL.World.Parts
{
	[Serializable]
	public class acegiak_Seed : IPart
	{

		public string Result;


		public acegiak_Seed()
		{
		}



		public override bool AllowStaticRegistration()
		{
			return true;
		}

		public override void Register(GameObject Object)
		{
			Object.RegisterPartEvent(this, "GetInventoryActions");
			Object.RegisterPartEvent(this, "InvCommandPlant");
            Object.RegisterPartEvent(this, "ApplyEffect");
            Object.RegisterPartEvent(this, "ApplyWet");
			base.Register(Object);
		}

        public void Plant(GameObject who){
            Cell cell = ParentObject.CurrentCell;
            if(cell == null){
                Popup.Show("Put things on the ground to plant them.");
                return;
            }

            ParentObject.pPhysics.Takeable = false;
            ParentObject.pRender.Tile = "Assets_Content_Textures_Tiles_sw_watervine1.bmp";
        }

        public void Grow(){
            Cell cell = ParentObject.CurrentCell;
            if(cell == null){
                Popup.Show("Put things on the ground to plant them.");
                return;
            }
            cell.RemoveObject(ParentObject);
            cell.AddObject(GameObject.create(Result));
        }


		public override bool FireEvent(Event E)
		{
            if (E.ID == "GetInventoryActions")
            {
                if (ParentObject.pPhysics.CurrentCell != null && ParentObject.pPhysics.Takeable)
                {
                    E.GetParameter<EventParameterGetInventoryActions>("Actions").AddAction("Plant", 'P', false, "&WP&ylant", "InvCommandPlant", 5);
                }
            }
            else if (E.ID == "InvCommandPlant")
            {
                Plant(E.GetParameter<GameObject>("Owner"));
                        E.RequestInterfaceExit();
            }else
            if(E.ID == "ApplyEffect"){
                Effect effect = E.GetParameter("Effect") as Effect;     
                if(effect != null && effect is LiquidCovered){
                    LiquidVolume volume = ((LiquidCovered)effect).Liquid;
                    if(volume.IsFreshWater()){
                        Grow();
                    }
                }           
            }else
            if(E.ID == "ApplyWet"){
                        Grow();
            }
        
			return base.FireEvent(E);
		}
	}
}
