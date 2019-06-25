using System;
using XRL.Rules;
using XRL.UI;
using XRL.Core;
using XRL.World.Parts.Effects;
using System.Collections.Generic;
using System.Text;

namespace XRL.World.Parts
{
	[Serializable]
	public class acegiak_Seed : IPart
	{

		public string Result;

        public int wateramount = 0;
        public long growth = 0;
        public int stage = 0;

        public int health = 0;

        public long lastseen = 0;

        public int stageLength = 200;

        public bool Dead = false;

        public string ResultName;

        private string displayname;
        private string description;



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
            Object.RegisterPartEvent(this, "EndTurn");
            Object.RegisterPartEvent(this, "GetDisplayName");
            Object.RegisterPartEvent(this, "GetShortDisplayName");
            Object.RegisterPartEvent(this, "GetShortDescription");
			base.Register(Object);
		}

        public void Plant(GameObject who){
            Cell cell = ParentObject.CurrentCell;
            if(cell == null){
                Popup.Show("Put things on the ground to plant them.");
                return;
            }

            ParentObject.pPhysics.Takeable = false;
            this.stage = 1;
            tileupdate();
            // Statistic statistic = new Statistic("Energy", 0, 10000, 0, ParentObject);
            // statistic.Owner = ParentObject;
            // ParentObject.Statistics.Add("Energy", statistic);

            // XRLCore.Core.Game.ActionManager.AddActiveObject(ParentObject);

        }

        public void Water(int drams){
            Ticks();
            this.wateramount += drams;

        }


        public void Ticks(){
            if(stage < 1){
                return;
            }
            if(!this.Dead){
                long newGrowth = (XRLCore.Core.Game.TimeTicks - this.lastseen);

                if(this.lastseen == 0){
                    newGrowth = 0;
                }
                this.lastseen = XRLCore.Core.Game.TimeTicks;
                this.growth += newGrowth;

                if(this.growth >=stageLength){
                
                    for(int i = 0; i < growth/stageLength;i++){
                        IPart.AddPlayerMessage("TICKS!");

                        Tick();
                    }
                    
                    this.growth = this.growth % stageLength;
                    tileupdate();

                }
                
            }




            if(this.stage >=5 && !this.Dead){
                Cell cell = ParentObject.CurrentCell;
                if(cell == null){
                    Popup.Show("Things must grow in the ground.");
                    return;
                }
                cell.AddObject(GameObject.create(Result));
                cell.RemoveObject(ParentObject);
            }
        }

        public void Tick(){

            wateramount--;
            if(wateramount < 0){
                wateramount = 0;
            }
            if(wateramount <= 0 || wateramount >=10){
                health--;
            }else{
                health++;
            }
            if(health >= 5){
                health = 0;
                stage++;
            }
            if(health <= -5){
                this.Dead = true;
            }
        }

        public string debugstring(){
            // return "Water:"+wateramount.ToString()+" Health:"+health.ToString()+" Stage:"+stage.ToString()+(Dead?" Dead":(wateramount>7?"Drowning":(wateramount<=2?"Dry":" Alive")))+this.growth.ToString();
            return (Dead?"dead":(wateramount>7?"drowning":(wateramount<=2?"dry":"thriving")));
        }


        public void tileupdate(){
            if(this.Dead){
                ParentObject.pRender.Tile = "Items/sw_smallstone.bmp";
                this.displayname = "husk";
                this.description = "This plant has withered and died.";
            }
            if(this.stage == 1){
                ParentObject.pRender.Tile = "creatures/sw_yonderbrush.bmp";
                this.displayname = "seed";
                this.description = "The seed has been planted in the earth.";
            }
            if(this.stage == 2){
                ParentObject.pRender.Tile = "Creatures/sw_sprouting_orb.bmp";
                this.displayname = "sprout";
                this.description = "It has sprouted from the earth.";
            }
            if(this.stage == 3){
                ParentObject.pRender.Tile = "creatures/sw_weeds1.bmp";
                this.displayname = "seedling";
                this.description = "A "+this.ResultName+" sprout has grown into a seedling.";
            }
            if(this.stage == 4){
                ParentObject.pRender.Tile = "Assets_Content_Textures_Tiles_sw_watervine1.bmp";
                this.displayname = "plant";
                this.description = "A "+this.ResultName+" plant is growing here.";
            }
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
            if(E.ID == "ApplyEffect" && !ParentObject.pPhysics.Takeable){
                Effect effect = E.GetParameter("Effect") as Effect;     
                if(effect != null && effect is LiquidCovered){
                    LiquidVolume volume = ((LiquidCovered)effect).Liquid;
                    if(volume.GetLiquidName().Contains("fresh water")){
                        // Popup.Show("'e got wet:"+((LiquidCovered)effect).ContactDrams);
                        Water(((LiquidCovered)effect).ContactDrams);
                    }
                }
            }
            if (E.ID == "EndTurn"){
                Ticks();
            }
            if (E.ID == "GetShortDescription" && this.stage > 0){
                E.SetParameter("ShortDescription", this.description );
            }
            if (E.ID == "GetDisplayName" || E.ID == "GetShortDisplayName"){
                 if(this.stage > 0){
					 E.AddParameter("DisplayName",new StringBuilder(this.ResultName+" "+this.displayname+ " &y["+debugstring()+"]"));
                }
					
            }
			return base.FireEvent(E);
		}
	}
}
