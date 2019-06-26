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
	public class acegiak_CanBuild : IPart
	{
		public string Needs;

        public static Dictionary<string,int> ExplodeNeeds(string needs){
            Dictionary<string,int> ret = new Dictionary<string,int>();
            if(needs == null){
                return ret;
            }
            foreach(string row in needs.Split(';')){
                string[] bits = row.Split(':');
                ret.Add(bits[0],Convert.ToInt32(bits[1]));
            }
            return ret;
        }

		public acegiak_CanBuild()
		{
		}

        public bool Build(Cell cell){
            foreach(KeyValuePair<string, int> entry in ExplodeNeeds(this.Needs))
            {
                List<GameObject> bits = cell.GetObjects(entry.Key);
                int bitcount = 0;
                foreach(GameObject go in bits){
                    bitcount+= go.Count;
                }
                if(bitcount < entry.Value){
                    Popup.Show("You need "+entry.Key+"x"+entry.Value.ToString()+" to build: "+ParentObject.DisplayNameOnly+".");
                    return false;
                }
            }

            foreach(KeyValuePair<string, int> entry in ExplodeNeeds(this.Needs))
            {
                List<GameObject> bits = cell.GetObjects(entry.Key);
                int bitcount = 0;
                foreach(GameObject go in bits){
                    bitcount+= go.Count;
                    if(bitcount < entry.Value){
                        cell.RemoveObject(go);
                    }
                    if(bitcount + go.Count > entry.Value){
                        int amount = (bitcount+go.Count)-entry.Value;
                        Event @event = Event.New("SplitStack", "Number", amount);
                        @event.AddParameter("OwningObject", XRLCore.Core.Game.Player.Body);
                        go.FireEvent(@event);
                        cell.RemoveObject(go);

                        break;
                    }
                }
            }

            cell.AddObject(ParentObject);
            return true;
        }

	

	

      
	}
}
