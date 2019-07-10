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


			BitType item = new BitType(113, 'w', "&wwood scrap");
			BitType.BitTypes.Add(item);
            BitType.BitMap.Add(item.Color, item);
            if (!BitType.LevelMap.ContainsKey(item.Level))
            {
                BitType.LevelMap.Add(item.Level, new List<BitType>());
            }
            BitType.LevelMap[item.Level].Add(item);
            BitType.BitSortOrder.Add('w',133);
			
        }
    }
}