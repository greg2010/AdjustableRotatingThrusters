﻿using Sandbox.Game.EntityComponents;
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
    class PID
    {
        readonly private double kP;
        readonly private double kI;
        readonly private double kD;

        readonly private double timeStep;

        private double prevError;

        private double cP;
        private double cI;
        private double cD;


        public PID(double kP, double kI, double kD, double timeStep)
        {
            this.kP = kP;
            this.kI = kI;
            this.kD = kD;
            this.timeStep = timeStep;
            this.Reset();
        }

        public void Reset()
        {
            this.prevError = 0d;
            this.cP = 0d;
            this.cI = 0d;
            this.cD = 0d;
        }

        public double Update(double error)
        {
            var deltaError = error - this.prevError;
            this.cP = error;
            this.cI = error * timeStep;
            this.cD = timeStep != 0 ? deltaError / timeStep : 0;
            this.prevError = error;
            return (this.kP * this.cP) + (this.kI * this.cI) + (this.kD * this.cD);
        }
    }
}
