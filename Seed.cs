using System;
using XRL.Rules;
using XRL.UI;
using XRL.Core;
using XRL.World.Effects;
using System.Collections.Generic;
using System.Text;
using XRL.Liquids;
using XRL.World.Parts.Mutation;
using XRL.World.Capabilities;
using Qud.API;

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

        public int stageLength = 400;
        public int drowamount = 15;

        public bool Dead = false;

        public string ResultName;

        public string displayname;
        public string description;


        public static LiquidVolume GetPuddle(Cell cell){
			LiquidVolume ret = null;
            foreach(GameObject GO in cell.GetObjects()){
                LiquidVolume volume = GO.GetPart<LiquidVolume>();
                if(volume != null){

					if(ret != null){
						ret.MixWith(volume);
						GO.Destroy();
					}else{
						ret = volume;
					}
                    if(volume.MaxVolume == -1){
                        volume.MaxVolume = 10000;
                    }
                }
            }
            return ret;
        }

        public static void DoWater(GameObject who,GameObject from, Cell to, int PourAmount){
                if(PourAmount > from.GetPart<LiquidVolume>().Volume){
                    PourAmount = from.GetPart<LiquidVolume>().Volume;
                }
                if(PourAmount <= 0){
                    return;
                }
				for(int i = 0; i < PourAmount;i++){
                    from.GetPart<LiquidVolume>().PourIntoCell(who,to,1,true);
				}

                // if(GetPuddle(to) == null && PourAmount > 0){
                //     from.GetPart<LiquidVolume>().PourIntoCell(who,to,1,true);
                //     //IPart.AddPlayerMessage("PourFirst");
                //     PourAmount = PourAmount - 1;

                //     if(PourAmount > 0 && GetPuddle(to) != null){
                //         //IPart.AddPlayerMessage("PourRestStart");
                //         GetPuddle(to).MixWith(from.GetPart<LiquidVolume>().Split(PourAmount));
                //         //IPart.AddPlayerMessage("PourRest");
                //     }

                // }else{
                //     from.GetPart<LiquidVolume>().PourIntoCell(who,to,PourAmount,true);
                // }
                CellSplash(to,"&"+from.GetPart<LiquidVolume>().GetPrimaryLiquidColor());
        }

        public static void CellSplash(Cell cell,string Color){
            if ( cell.ParentZone == XRLCore.Core.Game.ZoneManager.ActiveZone)
			{
				for (int i = 0; i < 3; i++)
				{
					float num = 0f;
					float num2 = 0f;
					float num3 = (float)XRL.Rules.Stat.RandomCosmetic(0, 359) / 58f;
					num = (float)Math.Sin(num3) / 3f;
					num2 = (float)Math.Cos(num3) / 3f;
					XRLCore.ParticleManager.Add(ConsoleLib.Console.ColorUtility.StripBackgroundFormatting(Color+"."), cell.X, cell.Y, num, num2, 5, 0f, 0f);
				}
			}
        }

		public acegiak_Seed()
		{
		}
        
		public override bool SameAs(IPart p)
		{
            if(p is acegiak_Seed){
                acegiak_Seed s = p as acegiak_Seed;
                if(s.Result == this.Result && s.ResultName == this.ResultName && s.growth == this.growth && s.stage == this.stage){
                    return true;
                }
            }
			return false;
		}

		public override bool AllowStaticRegistration()
		{
			return true;
		}

	
        public void Plant(GameObject who){
            GameObject thisone = ParentObject.RemoveOne();
            if(thisone == null){
                Popup.Show("got a null object?");
                return;
            }

            if(thisone.CurrentCell == null){
                //Popup.Show("dropped one");
                XRLCore.Core.Game.Player.Body.CurrentCell.AddObject(thisone);
                //thisone.InInventory.FireEvent(Event.New("CommandDropObject", "Object", thisone, "Forced", 1).SetSilent(Silent: true));
                //return;
            }
            Cell cell = thisone.CurrentCell;
            if(cell == null){
                Popup.Show("Put things on the ground to plant them.");
                return;
            }
            // if(thisone.GetPart<Stacker>() != null && thisone.GetPart<Stacker>().StackCount > 1){
            //     GameObject gameObject = thisone.DeepCopy(true);
            //     gameObject.GetPart<Stacker>().StackCount = thisone.GetPart<Stacker>().StackCount -1;
            //     thisone.GetPart<Stacker>().StackCount = 1;
            //     who.GetPart<Inventory>().AddObject(gameObject);
            //     IPart.AddPlayerMessage("You plant one "+thisone.DisplayNameOnly+" and collect the rest");
            // }

            thisone.pPhysics.Takeable = false;
            thisone.GetPart<acegiak_Seed>().LengthMultiplier();
            thisone.GetPart<acegiak_Seed>().stage = 1;
            //ParentObject.pPhysics.Category = "Plant";
            //ParentObject.RemovePart<NoEffects>();
            thisone.GetPart<acegiak_Seed>().tileupdate();
            // Statistic statistic = new Statistic("Energy", 0, 10000, 0, ParentObject);
            // statistic.Owner = ParentObject;
            // ParentObject.Statistics.Add("Energy", statistic);

            // XRLCore.Core.Game.ActionManager.AddActiveObject(ParentObject);

        }

        public void Water(GameObject who){
            if(ParentObject.CurrentCell == null || who.GetPart<Inventory>() == null){
                return;
            }


            List<string> ChoiceList = new List<string>();
            List<char> HotkeyList = new List<char>();
            char ch = 'a';

            List<GameObject> containers = new List<GameObject>();


            foreach(GameObject container in who.GetPart<Inventory>().GetObjects()){
                if(container.GetPart<LiquidVolume>() != null && container.GetPart<LiquidVolume>().Volume > 0){
                    containers.Add(container);
                    ChoiceList.Add(container.DisplayName);
                    HotkeyList.Add(ch);
                    ch = (char)(ch + 1);
                    
                }
            }

            int designNumber = Popup.ShowOptionList(string.Empty, ChoiceList.ToArray(), HotkeyList.ToArray(), 0, "Choose what to water with.", 60,  false,  true);
            if(designNumber < 0 ){
                return;
            }

            int PourAmount = 1;
            string getamount = Popup.AskString("How many drams?","1",3,1,"0123456789");
            PourAmount = Int32.Parse(getamount);
            DoWater(who,containers[designNumber],ParentObject.CurrentCell,PourAmount);

            //Absorb(drams);

        }

        public LiquidVolume GetPuddle(){
            Cell cell = ParentObject.CurrentCell;
            return GetPuddle(cell);
        }

        public void LengthMultiplier(){
            GameObject GO = GameObjectFactory.Factory.CreateSampleObject(this.Result);
            int multiplier = 1;
            if(GO.GetPart<Brain>() != null){
                multiplier++;
            }
            if(GO.GetPart<Body>() != null){
                multiplier++;
            }
            if(GO.GetPart<LiquidFont>() != null){
                multiplier += 2;
            }
            if(GO.GetPart<Harvestable>() != null){
                multiplier++;
            }
            if(GO.GetPart<SporePuffer>() != null){
                multiplier++;
            }
            this.stageLength = this.stageLength*multiplier;
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

                //IPart.AddPlayerMessage("tickcheck:"+this.growth.ToString()+"+"+newGrowth.ToString()+"="+(this.growth+newGrowth).ToString());
                this.growth += newGrowth;



                if(this.growth >=stageLength){

                    //IPart.AddPlayerMessage((growth/stageLength).ToString()+" plant growth ticks!");

                    for(int i = 0; i < growth/stageLength;i++){

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



                if(GetPuddle() != null){
                    if(GetPuddle().ComponentLiquids.ContainsKey(acegiak_LiquidRestrainingAgent.ID)){
                       growInto.pPhysics.Takeable = true;
                       growInto.pPhysics.Weight = growInto.pPhysics.Weight /10;
                       growInto.pRender.DisplayName += " bonsai";
                    }
                }

                if(GetPuddle() != null){
                    if(GetPuddle().ComponentLiquids.ContainsKey(acegiak_LiquidFurlingAgent.ID)){
                        GameObject furled = GameObject.create("FurledPlant");
                        furled.GetPart<Render>().DisplayName = "Furled "+growInto.DisplayNameOnly;
                        furled.GetPart<Render>().DetailColor = ParentObject.pRender.DetailColor;
                        furled.GetPart<Render>().TileColor = ParentObject.pRender.TileColor;
                        furled.GetPart<Render>().RenderString = ParentObject.pRender.RenderString;
                        furled.GetPart<Render>().ColorString = ParentObject.pRender.ColorString;
                        furled.GetPart<DeploymentGrenade>().Blueprint = growInto.Blueprint;
                        growInto = furled;
                    }
                }

                



                if(GetPuddle() != null){
                    if(GetPuddle().ComponentLiquids.ContainsKey(acegiak_LiquidSoothingAgent.ID)){
                        if(growInto.GetPart<Brain>() != null){
                            growInto.GetPart<Brain>().PerformReequip();
                            growInto.GetPart<Brain>().BecomeCompanionOf(growInto.ThePlayer);
                            growInto.GetPart<Brain>().IsLedBy(growInto.ThePlayer);
                            growInto.GetPart<Brain>().SetFeeling(growInto.ThePlayer,100);
                            growInto.GetPart<Brain>().Goals.Clear();
                            growInto.GetPart<Brain>().Calm = false;
                            growInto.GetPart<Brain>().Hibernating = false;
                            growInto.GetPart<Brain>().FactionMembership.Clear();
                            growInto.AddPart(new Combat());
                        }
                    }
                }

                if(growInto.GetPart<Brain>() != null){
                            XRLCore.Core.Game.ActionManager.AddActiveObject(growInto);
                }


                cell.AddObject(growInto);


                ParentObject.FireEvent(new Event("acegiak_SeedGrow","From",ParentObject,"To",growInto));
                cell.RemoveObject(ParentObject);
                ParentObject.Destroy("matured",true,true);
            }
        }

        public void Tick(){
            if(GetPuddle() == null
            || GetPuddle().GetPrimaryLiquid().ID != "water"
            || GetPuddle().Volume <= 0
            || GetPuddle().Volume > drowamount){
                health--;
            }else{
                if(health <0){
                    health += 4;
                }
                health++;
                if(GetPuddle() != null
                && GetPuddle().ComponentLiquids.ContainsKey(acegiak_LiquidGrowthAgent.ID)
                && GetPuddle().ComponentLiquids[acegiak_LiquidGrowthAgent.ID]>0){
                    health+= 4;
                    GetPuddle().ComponentLiquids[acegiak_LiquidGrowthAgent.ID] -= 1;
                }

            }
            if(health >= 5){
                health = 0;
                stage++;
            }
            if(health <= -120/(stageLength/400)){
                this.Dead = true;
            }
            if(GetPuddle() != null){
                GetPuddle().ComponentLiquids["water"]--;
                GetPuddle().Volume--;
                if (GetPuddle().Volume <= 0)
                {
                    GetPuddle().Empty();
					GetPuddle().ParentObject.Destroy();
                }
                else
                {
                    GetPuddle().NormalizeProportions();
                }
                GetPuddle().RecalculatePrimary();
                GetPuddle().RecalculateProperties();
                GetPuddle().FlushWeightCaches();
            }
        }

        public string debugstring(){
            // return "Water:"+wateramount.ToString()+" Health:"+health.ToString()+" Stage:"+stage.ToString()+(Dead?" Dead":(wateramount>7?"Drowning":(wateramount<=2?"Dry":" Alive")))+this.growth.ToString();
            if(Dead){
                return "dead";
            }
            if(GetPuddle() == null){
                return "dry";
            }
            if(GetPuddle().GetPrimaryLiquid() == null){
                return "dry";
            }
            if(GetPuddle().GetPrimaryLiquid().ID != "water"){
                return "choking on "+GetPuddle().GetPrimaryLiquid().ID;
            }
            if(GetPuddle().Volume > drowamount){
                return "drowning";
            }
            if(GetPuddle().Volume <3){
                return "dry";
            }
            
            return "thriving";
        }


        public void tileupdate(){
            if(this.Dead){
                ParentObject.pRender.Tile = "Items/plantedseeddead.png";
                this.displayname = "husk";
                this.description = "This plant has withered and died.";
            }
            if(this.stage == 1){
                ParentObject.pRender.Tile = "Items/plantedseed1.png";
                this.displayname = "seed";
                this.description = "The seed has been planted in the earth.";
            }
            if(this.stage == 2){
                ParentObject.pRender.Tile = "Items/plantedseed2.png";
                this.displayname = "sprout";
                this.description = "It has sprouted from the earth.";
            }
            if(this.stage == 3){
                ParentObject.pRender.Tile = "Items/plantedseed3.png";
                this.displayname = "seedling";
                this.description = "A "+this.ResultName+" sprout has grown into a seedling.";
            }
            if(this.stage == 4){
                ParentObject.pRender.Tile = "Items/plantedseed4.png";
                this.displayname = "plant";
                this.description = "A "+this.ResultName+" plant is growing here.";
            }
        }


		public override bool WantEvent(int ID, int cascade)
		{
			return base.WantEvent(ID, cascade)
            || ID == CanSmartUseEvent.ID
            || ID == CommandSmartUseEvent.ID
            || ID == EndTurnEvent.ID
            || ID == GetDisplayNameEvent.ID
            || ID == GetInventoryActionsEvent.ID
            || ID == GetShortDescriptionEvent.ID
            || ID == GetDisplayNameEvent.ID
            || ID == InventoryActionEvent.ID;
		}

	
        public override bool HandleEvent(GetShortDescriptionEvent E){

            if (this.stage > 0){
                string debug = "";
                if(Scanning.HasScanningFor(XRLCore.Core.Game.Player.Body,Scanning.Scan.Bio)){
                    int drams = 0;
                    if(GetPuddle() != null){
                        if(debugstring() == "thriving" || debugstring() == "dry"){
                            drams = GetPuddle().Volume;
                        }
                    }
                    int count = drams * stageLength;
                    int days = (int)Math.Floor((double)count/1200);
                    int hours = (int)Math.Floor((double)(count%1200)/(1200/24));
                    E.Postfix.Append("&gBIOSCAN: Suitable water for "+days.ToString()+"d "+hours.ToString()+"h.");
                }
            }
            return true;
        }


		public override bool HandleEvent(GetDisplayNameEvent E)
		{
            if(this.stage > 0){
				E.AddClause("&y["+debugstring()+"&y]");
            }
            return true;
        }


		public override bool HandleEvent(InventoryActionEvent E)
		{
            if(E.Command == "Plant")
            {
                Plant(E.Actor);
                E.Actor.UseEnergy(1000, "Item");
                E.RequestInterfaceExit();
            }
            else if (E.Command == "Water")
            {
                Water(E.Actor);
                E.Actor.UseEnergy(1000, "Item");
                E.RequestInterfaceExit();
            }
            return true;
        }


		public override bool HandleEvent(GetInventoryActionsEvent E)
		{
                if (ParentObject.pPhysics.CurrentCell != null || ParentObject.InInventory != null){
                    if(ParentObject.pPhysics.Takeable)
                    {
                        E.AddAction("plant","&Wp&ylant","Plant",null, 'p', false,  5);
                    }
                }
                if(ParentObject.pPhysics.CurrentCell != null && ! ParentObject.pPhysics.Takeable){
                    E.AddAction("water","&Ww&yater","Water",null, 'w', false,  5);
                }
                return true;
        }


		public override bool HandleEvent(EndTurnEvent E){
                Ticks();
                return true;
        }


		public override bool HandleEvent(CanSmartUseEvent E)
		{
            if(this.stage > 0){
                return false;
            }
            return true;
        }


		public override bool HandleEvent(CommandSmartUseEvent E)
		{
			Water(E.Actor);
            return true;
        }

    }
}
