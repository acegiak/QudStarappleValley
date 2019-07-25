using System;
using XRL.Rules;
using XRL.UI;
using XRL.Core;
using XRL.World.Parts.Effects;
using System.Collections.Generic;
using System.Text;
using XRL.Liquids;
using System.Linq;
using XRL.World;

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
        [NonSerialized]
        public static Dictionary<string,byte> ferments;



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
			// Object.RegisterPartEvent(this, "GetInventoryActions");
			// Object.RegisterPartEvent(this, "InvCommandPlant");
            // Object.RegisterPartEvent(this, "ApplyEffect");
            Object.RegisterPartEvent(this, "EndTurn");
            // Object.RegisterPartEvent(this, "GetDisplayName");
            // Object.RegisterPartEvent(this, "GetShortDisplayName");
            // Object.RegisterPartEvent(this, "GetShortDescription");
            // Object.RegisterPartEvent(this, "AccelerateRipening");
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

            }
        }

        public void Tick(){

            //IPart.AddPlayerMessage("do tick!");
            GetFerments();
            LiquidVolume volume = ParentObject.GetPart<LiquidVolume>();
            if(volume == null){
                //IPart.AddPlayerMessage("no volume!");

                return;
            }

            if(ParentObject.pPhysics.Temperature <15){
                //IPart.AddPlayerMessage("cold!");
                return;
            }
            int bads = 0;
            int total = 0;

            
            var test = volume.ComponentLiquids;

            int adram = (int)Math.Ceiling(1000f/Math.Max(volume.Volume,1));


            //IPart.AddPlayerMessage("Amounts:");
            foreach(byte id in volume._ComponentLiquids.Keys.ToList()){
                //IPart.AddPlayerMessage(LiquidVolume.ComponentLiquidTypes[id].Name+": "+volume._ComponentLiquids[id].ToString());
            }


            foreach(byte id in volume._ComponentLiquids.Keys.ToList()){
                if(ferments.ContainsKey(LiquidVolume.ComponentLiquidTypes[id].Name) || ferments.Values.ToList().Contains(id)){

                }else{
                    bads += volume._ComponentLiquids[id];
                }
                total += volume._ComponentLiquids[id];
            }
            total = Math.Max(total,1);


            if(ParentObject.pPhysics.Temperature > 30f  || Stat.Rnd2.NextDouble() < bads/total){

                //IPart.AddPlayerMessage("putrid!");
                byte result = LiquidVolume.GetLiquidId("putrid");
                if(volume.GetPrimaryLiquid() != null){
                    volume._ComponentLiquids[Convert.ToByte(volume.GetPrimaryLiquid().ID)] -= adram;
                }
                volume._ComponentLiquids[result] += adram;
                
            }else{
                //IPart.AddPlayerMessage("test contents!");
                foreach(byte id in volume._ComponentLiquids.Keys.ToList()){
                    //IPart.AddPlayerMessage(LiquidVolume.ComponentLiquidTypes[id].Name+" test?");
                    if(ferments.ContainsKey(LiquidVolume.ComponentLiquidTypes[id].Name)){

                        byte result = ferments[LiquidVolume.ComponentLiquidTypes[id].Name];


                        //IPart.AddPlayerMessage(LiquidVolume.ComponentLiquidTypes[id].Name+" ferments to "+LiquidVolume.ComponentLiquidTypes[result].Name+"!");

                        volume._ComponentLiquids[id] = volume._ComponentLiquids[id] - adram;

                        if(volume._ComponentLiquids[id] <= 0){
                            volume._ComponentLiquids.Remove(id);
                        }

                        // volume.SetComponent(id, volume._ComponentLiquids[id]-1);
                        // if(!volume._ComponentLiquids.ContainsKey(result)){
                        //     volume._ComponentLiquids[result] = 0;
                        // }
                        // volume._ComponentLiquids[result] += 1;

                        if(!volume._ComponentLiquids.ContainsKey(result)){
                            volume._ComponentLiquids[result] = 0;
                        }

                        volume._ComponentLiquids[result] = volume._ComponentLiquids[result] + adram;

                        // volume.MixWith(
                        //     new LiquidVolume(result,1)
                        //     );
                    }
                }


                List<string> seen = new List<string>();

                foreach(GameObject GO in ParentObject.GetPart<Inventory>().GetObjects()){
                    //IPart.AddPlayerMessage(GO.DisplayName+" test?");

                    if(GO.HasTag("FermentTo")){
                        string result = GO.GetTag("FermentTo");

                        //IPart.AddPlayerMessage(GO.GetBlueprint().Name+" ferments to: "+result+"!");
                        if(!seen.Contains(GO.GetBlueprint().Name)){
                            //IPart.AddPlayerMessage("not seen!");
                            if(GO.Count > 1){
                                GO.RemoveOne();
                                //IPart.AddPlayerMessage("removeone!");
                            }else{
                                GO.Destroy();
                                //IPart.AddPlayerMessage("destroyed!");
                            }
                            seen.Add(GO.GetBlueprint().Name);

                            volume.Volume += 1;

                            if(!volume._ComponentLiquids.ContainsKey(Convert.ToByte(LiquidVolume.ComponentLiquidNameMap[result].ID))){
                                volume._ComponentLiquids[Convert.ToByte(LiquidVolume.ComponentLiquidNameMap[result].ID)] = 0;
                            }

                            volume._ComponentLiquids[Convert.ToByte(LiquidVolume.ComponentLiquidNameMap[result].ID)] = volume._ComponentLiquids[Convert.ToByte(LiquidVolume.ComponentLiquidNameMap[result].ID)] + adram;

                            
                            // volume.MixWith(
                            //     new LiquidVolume(Convert.ToByte(LiquidVolume.ComponentLiquidNameMap[result].ID),1)
                            // );
                            
                        }
                    }
                }

            }
            
            volume.NormalizeProportions();
            volume.RecalculatePrimary();
            volume.RecalculateProperties();
            volume.FlushWeightCaches();


            //IPart.AddPlayerMessage("New Amounts:");
            foreach(byte id in volume._ComponentLiquids.Keys.ToList()){
                //IPart.AddPlayerMessage(LiquidVolume.ComponentLiquidTypes[id].Name+": "+volume._ComponentLiquids[id].ToString());
            }
       
        }

        

        public static void GetFerments(){

            if(ferments == null){
            ferments = new Dictionary<string,byte>();
            List<GameObjectBlueprint> bps = GameObjectFactory.Factory.GetBlueprintsWithTag("FermentTo");
            foreach(GameObjectBlueprint bp in bps){
                if(bp.allparts.ContainsKey("LiquidVolume")){
                    string[] liquidbits = bp.allparts["LiquidVolume"].GetParameter("InitialLiquid").Split('-');
                    if(LiquidVolume.ComponentLiquidNameMap.ContainsKey(liquidbits[0]) && LiquidVolume.ComponentLiquidNameMap.ContainsKey(bp.GetTag("FermentTo"))){

                        BaseLiquid L = LiquidVolume.ComponentLiquidNameMap[liquidbits[0]];
                        BaseLiquid F = LiquidVolume.ComponentLiquidNameMap[bp.GetTag("FermentTo")];
                        ferments[L.Name] = Convert.ToByte(F.ID);
                        //IPart.AddPlayerMessage(L.Name+" can ferment to:"+F.Name+"!");


                    }
                }else{
                    if(LiquidVolume.ComponentLiquidNameMap.ContainsKey(bp.GetTag("FermentTo"))){
                        BaseLiquid F = LiquidVolume.ComponentLiquidNameMap[bp.GetTag("FermentTo")];
                        ferments[bp.Name] = Convert.ToByte(F.ID);

                        //IPart.AddPlayerMessage(bp.Name+" can  ferments to:"+F.Name+"!");
                    }
                }
                
            }

            }
            
        }


        
		public override bool FireEvent(Event E)
		{

            if (E.ID == "EndTurn"){
                Ticks();
            }
            // if (E.ID == "GetShortDescription" && this.stage > 0){
            //     string debug = "";
            //     // debug += 
            //     // GetPuddle().GetPrimaryLiquid().GetKeyString()+":"
            //     // +GetPuddle()._ComponentLiquids[GetPuddle.bPrimary].ToString()
            //     // +GetPuddle().GetSecondaryLiquid().GetKeyString()+":"
            //     // +GetPuddle()._ComponentLiquids[GetPuddle.bSecondary].ToString()
            //     // E.SetParameter("ShortDescription", this.description);
            // }
            // if (E.ID == "GetDisplayName" || E.ID == "GetShortDisplayName"){
            //      if(this.stage > 0){
			// 		 E.AddParameter("DisplayName",new StringBuilder(this.ResultName+" "+this.displayname+ " &y["+debugstring()+"]"));
            //     }
					
            // }
			return base.FireEvent(E);
		}
	}
}
