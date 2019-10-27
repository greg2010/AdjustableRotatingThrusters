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
        public class RotorEngineController
        {

            private readonly Dictionary<IMyMotorAdvancedStator, IMyThrust> liftingRotorsDict;
            private readonly IMyShipController myShipController;
            private readonly PID pid;

            public readonly Logger logger;
            private double calculateGravityForce()
            {
                double gravityVectorLength = myShipController.GetNaturalGravity().Length();
                float currentMass = myShipController.CalculateShipMass().TotalMass;           
                return gravityVectorLength * currentMass;

            }

            public RotorEngineController(Dictionary<IMyMotorAdvancedStator, IMyThrust> liftingRotorsDict, IMyShipController myShipController, Logger logger, double kP, double kI, double kD, double timeStep)
            {
                this.liftingRotorsDict = liftingRotorsDict;
                this.myShipController = myShipController;
                this.logger = logger;
                this.pid = new PID(kP, kI, kD, timeStep);
            }

            private double PointRotorAtVector(IMyMotorAdvancedStator rotor, Vector3D currentDirection, Vector3D rotorPlaneNormal, Vector3D targetDirection)
            {
                // Get max RPM for a rotor
                float maxRPM = rotor.GetMaximum<float>("Velocity");

                // maxRPM rad
                double errorScale = Math.PI * maxRPM;

                // Grab target vector projected onto the plane of the engine
                Vector3D targetOnPlane = targetDirection - targetDirection.Projected(rotorPlaneNormal);

                // Normalized (set length to 1)
                var targetNormalized = targetOnPlane.Normalized();
                var currentNormalized = currentDirection.Normalized();

                // Calculate angle between target and current direction, in [-pi to pi]. The first value is a triple product
                double angle = Math.Atan2(rotorPlaneNormal.Normalized().Dotted(targetNormalized.Crossed(currentNormalized)), targetNormalized.Dotted(currentNormalized));
                if (angle > Math.PI) { angle -= 2 * Math.PI; }
                else if (angle <= -Math.PI) { angle += 2 * Math.PI; }

                // Find rotation speed value using a PID
                var rotVal = pid.Update(angle * errorScale);

                //logger.Log($"angle: {angle} rotVal: {rotVal}");

                // The rpm value must be -maxRPM <= rpm <= maxRPM
                double rpm = MathHelper.Clamp(rotVal, maxRPM * -1, maxRPM);
                rotor.TargetVelocityRPM = (float)rpm;

                return angle;
            }

            public void Tick()
            {
                var thrustRequired = this.calculateGravityForce();
                //logger.Log($"tr: {thrustRequired}");
                var thrustPerEngine = thrustRequired / liftingRotorsDict.Count();
                //logger.Log($"Engine count: {liftingRotorsDict.Count()} Total thrust required: {thrustRequired} Per engine: {thrustPerEngine}");
                if (thrustRequired > 0)
                {
                    foreach (var rt in liftingRotorsDict)
                    {
                        var currentDirection = rt.Value.WorldMatrix.Forward;
                        var rotorPlaneNormal = rt.Value.WorldMatrix.Backward.Crossed(rt.Value.WorldMatrix.Right);
                        var targetDirection = myShipController.GetNaturalGravity();
                        var angle = this.PointRotorAtVector(rt.Key, currentDirection, rotorPlaneNormal, targetDirection);
                        if (angle <= 0.1)
                        {
                            rt.Value.ThrustOverride = (float)thrustPerEngine;
                            //logger.Log($"Setting {rt.Value.ToString()} override to {thrustPerEngine}");
                        }
                        else
                        {
                            //logger.Log($"Angle {angle} {rt.Value.ToString()} override to 0");
                            rt.Value.ThrustOverride = 0f;
                        }
                    }
                }
                else
                {
                    foreach (var rt in liftingRotorsDict)
                    {
                        rt.Value.ThrustOverride = 0f;
                        rt.Key.TargetVelocityRPM = 0f;
                    }
                }
            }
        }
    }
}
