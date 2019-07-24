using System;
using XRL.Rules;
using XRL.UI;
using XRL.Core;
using XRL.World.Parts.Effects;
using System.Collections.Generic;
using System.Text;
using XRL.Liquids;
using System.Linq;

namespace XRL.World.Parts
{
	[Serializable]
	public class acegiak_Fermenter : IPart
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

        public string displayname;
        public string description;



		public acegiak_Fermenter()
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
			Object.RegisterPartEvent(this, "InvCommandPlant");
            Object.RegisterPartEvent(this, "ApplyEffect");
            Object.RegisterPartEvent(this, "EndTurn");
            Object.RegisterPartEvent(this, "GetDisplayName");
            Object.RegisterPartEvent(this, "GetShortDisplayName");
            Object.RegisterPartEvent(this, "GetShortDescription");
            Object.RegisterPartEvent(this, "AccelerateRipening");
			base.Register(Object);
		}


        public void Water(int drams){
            Ticks();
            this.wateramount += drams;
            //Absorb(drams);

        }

        public LiquidVolume GetPuddle(){
            Cell cell = ParentObject.CurrentCell;
            foreach(GameObject GO in cell.GetObjects()){
                LiquidVolume volume = GO.GetPart<LiquidVolume>();
                if(volume != null){
                    return volume;
                }
            }
            return null;
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
                        //IPart.AddPlayerMessage("TICKS!");

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
                GameObject growInto = GameObject.create(Result);
            }
        }

        public void Tick(){
            LiquidVolume volume = ParentObject.GetPart<LiquidVolume>();
            if(volume == null){
                throw new Exception("Fermenter must have LiquidVolume");
            }
            if(this.pPhysics.Temperature <15){
                //Do nothing;
            }else if(this.pPhysics.Temperature > 30){
                //Rot;
            }else{

            }
        }
]

        
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
            // if(E.ID == "ApplyEffect"){

            //     Popup.Show("'e got an effect");
            //     Effect effect = E.GetParameter("Effect") as Effect;     
            //     if(effect != null && effect is LiquidCovered){
            //         Popup.Show("'e got liquidcovered:");

            //         LiquidVolume volume = ((LiquidCovered)effect).Liquid;
            //         if(volume.GetLiquidName().Contains("fresh water")){
            //              Popup.Show("'e got wet:"+((LiquidCovered)effect).ContactDrams);
            //             Water(((LiquidCovered)effect).ContactDrams);
            //             volume.Volume = 0;
            //         }
            //     }
            // }
            if (E.ID == "EndTurn" || E.ID == "AccelerateRipening"){
                Ticks();
            }
            if (E.ID == "GetShortDescription" && this.stage > 0){
                string debug = "";
                // debug += 
                // GetPuddle().GetPrimaryLiquid().GetKeyString()+":"
                // +GetPuddle().ComponentLiquids[GetPuddle.bPrimary].ToString()
                // +GetPuddle().GetSecondaryLiquid().GetKeyString()+":"
                // +GetPuddle().ComponentLiquids[GetPuddle.bSecondary].ToString()
                // E.SetParameter("ShortDescription", this.description);
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
