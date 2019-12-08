using System;
using XRL.Rules;
using XRL.UI;
using XRL.Core;
using XRL.World.Parts.Effects;
using XRL.World.Parts.Skill;
using System.Collections.Generic;

namespace XRL.World.Parts
{
	[Serializable]
	public class acegiak_SeedDropper : IPart
	{
		public int Chance = 35;

		public string Seed = null;

        public long last = 0;

        public bool hasSeed = false;

		public bool bNoSmartUse;

		public acegiak_SeedDropper()
		{
		}

		public override bool SameAs(IPart p)
		{

			return base.SameAs(p);
		}

		public override bool AllowStaticRegistration()
		{
			return true;
		}

		public override void Register(GameObject Object)
		{
			Object.RegisterPartEvent(this, "BeforeDeathRemoval");
			Object.RegisterPartEvent(this, "GetInventoryActions");
			Object.RegisterPartEvent(this, "InvCommandCollectSeeds");
			Object.RegisterPartEvent(this, "CommandSmartUse");
			Object.RegisterPartEvent(this, "CanSmartUse");
			base.Register(Object);
            
		}

        public GameObject CreateSeed(){
            if(Seed != null){
                return GameObject.create(Seed);
            }
            GameObject seed = GameObject.create("Seed");
            acegiak_Seed seedpart = seed.GetPart<acegiak_Seed>();
            seedpart.Result = ParentObject.GetBlueprint().Name;
            seedpart.ResultName = ParentObject.GetBlueprint().DisplayName().Replace(" Tree","");
            seedpart.displayname = "seed";
            seedpart.description = "A seed from "+ParentObject.a+ParentObject.DisplayNameOnly+".";
            seed.pRender.DisplayName = seedpart.ResultName+" "+seedpart.displayname;
            seed.pRender.ColorString = ParentObject.pRender.ColorString;
            seed.pRender.TileColor = ParentObject.pRender.TileColor;
            seed.pRender.DetailColor = ParentObject.pRender.DetailColor;
            return seed;
        }

        public void CollectSeeds(GameObject who){
            if(who.HasSkill("CookingAndGathering_Harvestry")){
                if(last==0){
                    this.hasSeed = Stat.Rnd2.NextDouble()<Chance/100f;
                }
                if(last == 0 || XRLCore.Core.Game.TimeTicks - last > 1200*5){
                    if(this.hasSeed){
                        GameObject gameObject = CreateSeed();
                        string verb = "harvest";
                        string extra = gameObject.a + gameObject.ShortDisplayName;
                        IPart.XDidY(who, verb, extra, null, false, null, who);
                        
                        who.TakeObject(gameObject, true, 0);
                    }else{
                        IPart.AddPlayerMessage(ParentObject.The+ParentObject.DisplayNameOnly+ParentObject.GetVerb("have")+" no seeds.");

                    }
                    this.hasSeed = Stat.Rnd2.NextDouble()<Chance/100f;

                    this.last = XRLCore.Core.Game.TimeTicks;
                }else{
                    IPart.AddPlayerMessage(ParentObject.The+ParentObject.DisplayNameOnly+ParentObject.GetVerb("have")+" no seeds.");
                }
            }
        }

		public override bool FireEvent(Event E)
		{
				
			if (E.ID == "BeforeDeathRemoval")
			{
				Cell dropCell = ParentObject.GetDropCell();
				if (dropCell != null)
				{
                    if(Chance != 0 && Stat.Rnd2.NextDouble()<Chance/100f){
                        dropCell.AddObject(CreateSeed());
                    }
                }
            }
            if (E.ID == "GetInventoryActions")
            {
                if (ParentObject.pPhysics.CurrentCell != null)
                {
                    E.GetParameter<EventParameterGetInventoryActions>("Actions").AddAction("CollectSeeds", 'C', false, "&WC&yollect seeds", "InvCommandCollectSeeds", 5);
                }
            }
            else if (E.ID == "InvCommandCollectSeeds")
            {
                CollectSeeds(E.GetParameter<GameObject>("Owner"));
                E.RequestInterfaceExit();
            }
            if (E.ID == "CanSmartUse")
			{
				return false;
			}
            if (E.ID == "CommandSmartUse")
			{
				//if(E.GetGameObjectParameter("User").GetPart<acegiak_SongBook>() != null){
				CollectSeeds(E.GetGameObjectParameter("User"));
				//}
			}
            
			
			return base.FireEvent(E);
		}
	}
}
