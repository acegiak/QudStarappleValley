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
using XRL;

namespace XRL.World.Parts
{
	[Serializable]
	public class acegiak_FarmingLiquidPopulationHotloader : IPart
	{

        public acegiak_FarmingLiquidPopulationHotloader(){

                AddToPopTable("RandomLiquid", new PopulationObject { Blueprint = "beer" });
                AddToPopTable("RandomLiquid", new PopulationObject { Blueprint = "mead" });
                AddToPopTable("RandomRareLiquid", new PopulationObject { Blueprint = "furlingagent" });
                AddToPopTable("RandomRareLiquid", new PopulationObject { Blueprint = "soothingagent" });
                AddToPopTable("RandomRareLiquid", new PopulationObject { Blueprint = "growthagent" });
                AddToPopTable("RandomRareLiquid", new PopulationObject { Blueprint = "restrainingagent" });
                AddToPopTable("Ingredients_EarlyTiers", new PopulationObject { Blueprint = "MeadWaterskin_Ingredient", Weight = 5 });
                AddToPopTable("Ingredients_EarlyTiers", new PopulationObject { Blueprint = "BeerWaterskin_Ingredient", Weight = 5 });
                AddToPopTable("Ingredients_EarlyTiers", new PopulationObject { Blueprint = "Moxy Leaves", Weight = 2 });
                AddToPopTable("Ingredients_EarlyTiers", new PopulationObject { Blueprint = "Dried Moxy", Weight = 2 });
                AddToPopTable("AppleMerchantInventory", new PopulationObject { Blueprint = "Growth Agent Phial", Chance = "5", Number = "1-2" });
                AddToPopTable("AppleMerchantInventory", new PopulationObject { Blueprint = "Furling Agent Phial", Chance = "5", Number = "1-2" });
                AddToPopTable("AppleMerchantInventory", new PopulationObject { Blueprint = "Soothing Agent Phial", Chance = "5", Number = "1-2" });
                AddToPopTable("AppleMerchantInventory", new PopulationObject { Blueprint = "Restraining Agent Phial", Chance = "5", Number = "1-2" });
                AddToPopTable("AppleFarmerInventory", new PopulationObject { Blueprint = "Growth Agent Phial", Chance = "5", Number = "1-2" });
                AddToPopTable("AppleFarmerInventory", new PopulationObject { Blueprint = "Furling Agent Phial", Chance = "5", Number = "1-2" });
                AddToPopTable("AppleFarmerInventory", new PopulationObject { Blueprint = "Soothing Agent Phial", Chance = "5", Number = "1-2" });
                AddToPopTable("AppleFarmerInventory", new PopulationObject { Blueprint = "Restraining Agent Phial", Chance = "5", Number = "1-2" });
                AddToPopTable("VintnerInventory", new PopulationObject { Blueprint = "Growth Agent Phial", Chance = "5", Number = "1-2" });
                AddToPopTable("VintnerInventory", new PopulationObject { Blueprint = "Furling Agent Phial", Chance = "5", Number = "1-2" });
                AddToPopTable("VintnerInventory", new PopulationObject { Blueprint = "Soothing Agent Phial", Chance = "5", Number = "1-2" });
                AddToPopTable("VintnerInventory", new PopulationObject { Blueprint = "Restraining Agent Phial", Chance = "5", Number = "1-2" });
                // AddToPopTable("RandomFaction", new PopulationObject { Blueprint = "SomeFaction" });
                // AddToPopTable("LairOwners_Saltdunes", new PopulationTable { Name = "DynamicObjectsTable:SomeCreature", Weight = 5 });
        }
        // Helper method to fudge into the most common/simple pop tables.
        public static bool AddToPopTable(string table, params PopulationItem[] items) {
            PopulationInfo info;
            if (!PopulationManager.Populations.TryGetValue(table, out info))
                return false;
                
            // If this is a single group population, add to that group.
            if (info.Items.Count == 1 && info.Items[0] is PopulationGroup) { 
                var group = info.Items[0] as PopulationGroup;
                group.Items.AddRange(items);
                return true;
            }

            info.Items.AddRange(items);
            return true;
        }



    }
}