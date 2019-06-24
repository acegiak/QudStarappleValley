using System;
using XRL.Rules;
using XRL.UI;
using XRL.World.Parts.Effects;
using System.Collections.Generic;

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
            List<GameObject> bits = cell.GetObjects(ParentObject.Blueprint);
            int bitcount = 0;
            foreach(GameObject go in bits){
                bitcount+= go.Count;
            }
            if(bitcount < this.Count){
                Popup.Show("You need "+ParentObject.DisplayNameOnly+"x"+this.Count.ToString()+" to build: "+this.ResultName+".");
                return;
            }
            foreach(GameObject go in bits){
                cell.RemoveObject(go);
            }
            cell.AddObject(GameObject.create(Result));
        }


		public override bool FireEvent(Event E)
		{
				if (E.ID == "GetInventoryActions")
				{
					if (ParentObject.Understood() && ParentObject.pPhysics.CurrentCell != null)
					{
						E.GetParameter<EventParameterGetInventoryActions>("Actions").AddAction("Build", 'B', false, "&WB&yuild: "+this.ResultName, "InvCommandBuild", 5);
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
