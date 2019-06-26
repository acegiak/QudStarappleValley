using System;
using XRL.Rules;
using XRL.UI;
using XRL.World.Parts.Effects;
using System.Collections.Generic;
using System.Linq;

namespace XRL.World.Parts
{
	[Serializable]
	public class acegiak_Buildable : IPart
	{
		public int Count;


		public string Result;
		public string ResultName;

		public bool bNoSmartUse;

		public acegiak_Buildable()
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
			Object.RegisterPartEvent(this, "GetInventoryActions");
			Object.RegisterPartEvent(this, "InvCommandBuild");
			base.Register(Object);
		}

        public void Build(GameObject who){
            Cell cell = ParentObject.CurrentCell;
            if(cell == null){
                Popup.Show("Put things on the ground to build with them.");
                return;
            }
			GameObjectBlueprint bp = getBuilds().FirstOrDefault();
			if(bp == null){
                Popup.Show("There's nothing that uses those parts.");
                return;
			}
			GameObject go = bp.createOne();
            go.GetPart<acegiak_CanBuild>().Build(cell);
        }

		public List<GameObjectBlueprint> getBuilds(){
			List<GameObjectBlueprint> ret = new List<GameObjectBlueprint>();
			foreach (GameObjectBlueprint blueprint in GameObjectFactory.Factory.BlueprintList)
			{
				if(acegiak_CanBuild.ExplodeNeeds(blueprint.GetPartParameter("acegiak_CanBuild","Needs")).ContainsKey(ParentObject.GetBlueprint().Name)){
					ret.Add(blueprint);
				}
			}
			return ret;
		}


		public override bool FireEvent(Event E)
		{
				if (E.ID == "GetInventoryActions")
				{
					if (ParentObject.Understood() && ParentObject.pPhysics.CurrentCell != null)
					{
						E.GetParameter<EventParameterGetInventoryActions>("Actions").AddAction("Build", 'B', false, "&WB&yuild", "InvCommandBuild", 5);
					}
				}
				else if (E.ID == "InvCommandBuild")
				{
					Build(E.GetParameter<GameObject>("Owner"));
                    E.RequestInterfaceExit();
				}
			
			return base.FireEvent(E);
		}
	}
}
