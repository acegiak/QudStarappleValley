using System;
using System.Collections.Generic;
using System.Text;
using XRL.Core;
using XRL.Messages;
using XRL.Rules;
using XRL.World;
using XRL.World.Parts;
using XRL.World.Parts.Effects;
using UnityEngine;
using XRL.Liquids;
using XRL.World.Tinkering;

namespace XRL.World.Parts
{
    public class acegiak_AgriculturalLiquidInitializer : IPart
    {
        //this code runs early during game boot - the game creates a temporary instance of every object from the blueprints
        public acegiak_AgriculturalLiquidInitializer()
        {
            Debug.Log("Initializing Agricultural Liquids.");
            

			LiquidVolume.ComponentLiquidTypes.Add(Convert.ToByte(acegiak_LiquidFurlingAgent.ID), new acegiak_LiquidFurlingAgent());
			LiquidVolume.ComponentLiquidNameMap.Add("furlingagent", LiquidVolume.ComponentLiquidTypes[Convert.ToByte(acegiak_LiquidFurlingAgent.ID)]);


			LiquidVolume.ComponentLiquidTypes.Add(Convert.ToByte(acegiak_LiquidGrowthAgent.ID), new acegiak_LiquidGrowthAgent());
			LiquidVolume.ComponentLiquidNameMap.Add("growthagent", LiquidVolume.ComponentLiquidTypes[Convert.ToByte(acegiak_LiquidGrowthAgent.ID)]);

			LiquidVolume.ComponentLiquidTypes.Add(Convert.ToByte(acegiak_LiquidRestrainingAgent.ID), new acegiak_LiquidRestrainingAgent());
			LiquidVolume.ComponentLiquidNameMap.Add("restrainingagent", LiquidVolume.ComponentLiquidTypes[Convert.ToByte(acegiak_LiquidRestrainingAgent.ID)]);


			LiquidVolume.ComponentLiquidTypes.Add(Convert.ToByte(acegiak_LiquidSoothingAgent.ID), new acegiak_LiquidSoothingAgent());
			LiquidVolume.ComponentLiquidNameMap.Add("soothingagent", LiquidVolume.ComponentLiquidTypes[Convert.ToByte(acegiak_LiquidSoothingAgent.ID)]);


			LiquidVolume.ComponentLiquidTypes.Add(Convert.ToByte(acegiak_LiquidBeer.ID), new acegiak_LiquidBeer());
			LiquidVolume.ComponentLiquidNameMap.Add("beer", LiquidVolume.ComponentLiquidTypes[Convert.ToByte(acegiak_LiquidBeer.ID)]);

			LiquidVolume.ComponentLiquidTypes.Add(Convert.ToByte(acegiak_LiquidMead.ID), new acegiak_LiquidMead());
			LiquidVolume.ComponentLiquidNameMap.Add("mead", LiquidVolume.ComponentLiquidTypes[Convert.ToByte(acegiak_LiquidMead.ID)]);


			// LiquidVolume.ComponentLiquidTypes.Add(Convert.ToByte(acegiak_LiquidGrapeJuice.ID), new acegiak_LiquidGrapeJuice());
			// LiquidVolume.ComponentLiquidNameMap.Add("grapejuice", LiquidVolume.ComponentLiquidTypes[Convert.ToByte(acegiak_LiquidGrapeJuice.ID)]);


			// LiquidVolume.ComponentLiquidTypes.Add(Convert.ToByte(acegiak_LiquidAppleJuice.ID), new acegiak_LiquidAppleJuice());
			// LiquidVolume.ComponentLiquidNameMap.Add("applejuice", LiquidVolume.ComponentLiquidTypes[Convert.ToByte(acegiak_LiquidAppleJuice.ID)]);



			// BitType item = new BitType(113, 'w', "&wwood scrap");
			// BitType.BitTypes.Add(item);
            // BitType.BitMap.Add(item.Color, item);
            // if (!BitType.LevelMap.ContainsKey(item.Level))
            // {
            //     BitType.LevelMap.Add(item.Level, new List<BitType>());
            // }
            // BitType.LevelMap[item.Level].Add(item);
            // BitType.BitSortOrder.Add('w',133);
			
        }
    }
}