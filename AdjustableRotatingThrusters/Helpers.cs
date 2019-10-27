using Sandbox.Game.EntityComponents;
using Sandbox.ModAPI.Ingame;
using Sandbox.ModAPI.Interfaces;
using SpaceEngineers.Game.ModAPI.Ingame;
using System.Collections.Generic;
using System.Collections;
using System.Linq;
using System.Text;
using System;
using VRage.Collections;
using VRage.Game.Components;
using VRage.Game.GUI.TextPanel;
using VRage.Game.ModAPI.Ingame.Utilities;
using VRage.Game.ModAPI.Ingame;
using VRage.Game.ObjectBuilders.Definitions;
using VRage.Game;
using VRage;
using VRageMath;

namespace IngameScript
{
    partial class Program
    {
        public static class Helpers
        {
            public static List<T> GetBlocksByNameOfType<T>(IMyGridTerminalSystem GridTerminalSystem, string name, string gridId = "")
            {
                List<IMyTerminalBlock> blocks = new List<IMyTerminalBlock>();
                List<T> casted = new List<T>();
                if (gridId != "")
                {
                    GridTerminalSystem.SearchBlocksOfName(name, blocks, block => (block is T) && (block.CubeGrid.ToString() == gridId));
                }
                else
                {
                    GridTerminalSystem.SearchBlocksOfName(name, blocks, block => block is T);
                }

                foreach (var block in blocks)
                {
                    casted.Add((T)block);
                }
                return casted;
            }
        }
    }
}
