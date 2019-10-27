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
    partial class Program : MyGridProgram
    {

        public readonly string stabKeyword = "Stab";

        public readonly double kPstab = 3d;
        public readonly double kIstab = 1d;
        public readonly double kDstab = -0.005d;
        public readonly double timeStep = 1/100d;


        public readonly Logger logger;
        public readonly RotorEngineController rotorEngineController;

        private IMyShipController GetMyShipController()
        {
            List<IMyShipController> blocks = new List<IMyShipController>();
            GridTerminalSystem.GetBlocksOfType(blocks);
            return blocks[0];
        }


        private Dictionary<IMyMotorAdvancedStator, IMyThrust> GetLiftingRotorsDict()
        {
            var liftingRotorsDict = new Dictionary<IMyMotorAdvancedStator, IMyThrust>();
            var rotors = Helpers.GetBlocksByNameOfType<IMyMotorAdvancedStator>(GridTerminalSystem, stabKeyword);
            foreach (var rotor in rotors)
            {
                var assocThruster = Helpers.GetBlocksByNameOfType<IMyThrust>(GridTerminalSystem, stabKeyword, rotor.TopGrid.ToString())[0];
                liftingRotorsDict.Add(rotor, assocThruster);
            }
            return liftingRotorsDict;
        }


        public Program()
        {
            Runtime.UpdateFrequency = UpdateFrequency.Update1;
            var myShipController = this.GetMyShipController();
            var liftingRotorsDict = this.GetLiftingRotorsDict();
            var logger = new Logger(Echo, Me, 20);
            this.logger = logger;
            this.rotorEngineController = new RotorEngineController(liftingRotorsDict, myShipController, logger, kPstab, kIstab, kDstab, timeStep);
        }

        public void Main(string argument, UpdateType updateSource)
        {
            this.rotorEngineController.Tick();
            this.logger.Print();
        }
    }
}