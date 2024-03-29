using System;
using XRL.Rules;
using XRL.UI;
using XRL.Core;
using XRL.World.Effects;
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
        public static Dictionary<string,string> ferments;



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
			//IPart.AddPlayerMessage("fermenter ticks:"+growth.ToString()+"/"+stageLength.ToString());

            if(this.growth >=stageLength){
            
                for(int i = 0; i < growth/stageLength;i++){
                    //IPart.AddPlayerMessage("TICKS!");

                    Tick();
                }
                this.growth = this.growth % stageLength;

            }
        }

        public void Tick(){
            if(ParentObject.OnWorldMap() || (ParentObject.InInventory != null && ParentObject.InInventory.OnWorldMap())){
                return;
            }
            GetFerments();
			//IPart.AddPlayerMessage("fermentation:\n"+ferments.ToString());
            LiquidVolume volume = ParentObject.GetPart<LiquidVolume>();
            if(volume == null){

                return;
            }

            if(ParentObject.pPhysics.Temperature <15){
			//IPart.AddPlayerMessage("too cold");
                return;
            }
            int bads = 0;
            int total = 0;

            
            var test = volume.ComponentLiquids;

            int adram = (int)Math.Ceiling(1000f/Math.Max(volume.Volume,1));

            foreach(string f in ferments.Keys.ToList()){

                IPart.AddPlayerMessage("ferments:"+f+":"+ferments[f]);
            }

            foreach(string id in volume._ComponentLiquids.Keys.ToList()){
                IPart.AddPlayerMessage("checking liquid:"+id);
                if(ferments.ContainsKey(id)
                || ferments.Values.ToList().Contains(id)){

                    IPart.AddPlayerMessage("good");
                }else{
                    bads += volume._ComponentLiquids[id];

                    IPart.AddPlayerMessage("bad");
                }
                total += volume._ComponentLiquids[id];
            }
            total = Math.Max(total,1);


            if(ParentObject.pPhysics.Temperature > 30f  || Stat.Rnd2.NextDouble() < bads/total){

                string result = "putrid";
                if(volume.GetPrimaryLiquid() != null){
                    volume._ComponentLiquids[volume.GetPrimaryLiquid().ID] -= adram;
                }
                if(!volume._ComponentLiquids.ContainsKey(result)){
                    volume._ComponentLiquids[result] = 0;
                }
                volume._ComponentLiquids[result] += adram;
                
            }else{
                foreach(string id in volume._ComponentLiquids.Keys.ToList()){
                    if(LiquidVolume.getLiquid(id) != null){
                        string SourceLiquidName = id;
                    
                        if(ferments.ContainsKey(SourceLiquidName)){ //Its a liquid that ferments

                            string ResultName = ferments[SourceLiquidName];


                            if(LiquidVolume.getLiquid(SourceLiquidName) != null){

                                if(LiquidVolume.getLiquid(ResultName) != null){
                                    string ResultLiquidId = ResultName;

                                    volume._ComponentLiquids[id] = volume._ComponentLiquids[id] - adram;

                                    if(volume._ComponentLiquids[id] <= 0){
                                        volume._ComponentLiquids.Remove(id);
                                    }

                                    if(!volume._ComponentLiquids.ContainsKey(ResultLiquidId)){
                                        volume._ComponentLiquids[ResultLiquidId] = 0;
                                    }

                                    volume._ComponentLiquids[ResultLiquidId] = volume._ComponentLiquids[ResultLiquidId] + adram;

                                }else{
                                    volume._ComponentLiquids[id] = volume._ComponentLiquids[id] - adram;

                                    if(volume._ComponentLiquids[id] <= 0){
                                        volume._ComponentLiquids.Remove(id);
                                    }
                                    volume.Volume -= 1;
                                    ParentObject.GetPart<Inventory>().AddObject(GameObject.create(ResultName));
                                }
                            }
                        }
                    }
                }


                List<string> seen = new List<string>();

                foreach(GameObject GO in ParentObject.GetPart<Inventory>().GetObjects()){

                    if(GO.HasTag("FermentTo")){
                        string result = GO.GetTag("FermentTo");

                        if(!seen.Contains(GO.GetBlueprint().Name)){
                            if(GO.Count > 1){
                                GO.RemoveOne();
                            }else{
                                GO.Destroy();
                            }
                            seen.Add(GO.GetBlueprint().Name);

                            if(LiquidVolume.getLiquid(result) != null){
                                volume.Volume += 1;

                                if(LiquidVolume.getLiquid(result) == null){
                                    IPart.AddPlayerMessage("ERROR: Tried to ferment "+GO.DisplayName+" but output was invalid:"+result);
                                }else{
                                    string LiquidID = result;
                                
                                    if(!volume._ComponentLiquids.ContainsKey(LiquidID)){
                                        volume._ComponentLiquids[LiquidID] = 0;
                                    }

                                    volume._ComponentLiquids[LiquidID] = volume._ComponentLiquids[LiquidID] + adram;
                                }
                            }else{
                                ParentObject.GetPart<Inventory>().AddObject(GameObject.create(result));
                            }
                            
                        }
                    }
                }

            }
            
            volume.NormalizeProportions();
            volume.RecalculatePrimary();
            volume.RecalculateProperties();
            volume.FlushWeightCaches();
       
        }

        

        public static void GetFerments(){

            if(ferments == null){
            ferments = new Dictionary<string,string>();
            List<GameObjectBlueprint> bps = GameObjectFactory.Factory.GetBlueprintsWithTag("FermentTo");
            foreach(GameObjectBlueprint bp in bps){
                if(bp.allparts.ContainsKey("LiquidVolume")){
                    string[] liquidbits = bp.allparts["LiquidVolume"].GetParameter("InitialLiquid").Split('-');
                    //IPart.AddPlayerMessage("Tryadd initliq:"+liquidbits[0]+" / "+String.Join(", ",LiquidVolume.ComponentLiquidNameMap.Keys.ToList().ToArray()));

                    if(LiquidVolume.getLiquid(liquidbits[0]) != null){
                        BaseLiquid L = LiquidVolume.getLiquid(liquidbits[0]);
                        if(LiquidVolume.getLiquid(bp.GetTag("FermentTo")) != null){
                            BaseLiquid F = LiquidVolume.getLiquid(bp.GetTag("FermentTo"));
                            //ferments[L.Name] = Convert.ToByte(F.ID);
                            IPart.AddPlayerMessage(L.ID+" can ferment to:"+F.ID+"!");
                            ferments[L.ID] = F.ID;
                        }else{
                            ferments[L.ID] = bp.GetTag("FermentTo");
                            IPart.AddPlayerMessage(L.ID+" can ferment to:"+bp.GetTag("FermentTo")+"!");

                        }
                    }
                }else{
                    if(LiquidVolume.getLiquid(bp.GetTag("FermentTo")) != null){
                        BaseLiquid F = LiquidVolume.getLiquid(bp.GetTag("FermentTo"));
                        //ferments[bp.Name] = Convert.ToByte(F.ID);
                        ferments[bp.Name] = F.ID;
                        IPart.AddPlayerMessage(bp.Name+" can  ferments to:"+F.ID+"!");
                    }else{
                        ferments[bp.Name] = bp.GetTag("FermentTo");
                        IPart.AddPlayerMessage(bp.Name+" can ferment to:"+bp.GetTag("FermentTo")+"!");

                    }
                }
                
            }

            }
            
        }


        



		public override bool WantEvent(int ID, int cascade)
		{
			return base.WantEvent(ID, cascade)
            || ID == EndTurnEvent.ID
            || ID == GetDisplayNameEvent.ID
            || ID == GetDisplayNameEvent.ID;
		}


		public override bool HandleEvent(GetDisplayNameEvent E)
		{
            if(ParentObject.GetPart<Inventory>() != null && ParentObject.GetPart<Inventory>().Objects.Count() >0){
                int count = 0;
                foreach(GameObject GO in ParentObject.GetPart<Inventory>().GetObjects()){
                    count += GO.Count;
                }
                E.AddClause(" &y["+count+" items]");
            
            }
            return true;
        }
        		public override bool HandleEvent(EndTurnEvent E){
                                    Ticks();
                    return true;
                }


	}
}
