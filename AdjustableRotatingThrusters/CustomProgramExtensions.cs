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
    public static class CustomProgramExtensions
    {

        // projects a onto b
        public static Vector3D Projected(this Vector3D a, Vector3D b)
        {
            double aDotB = Vector3D.Dot(a, b);
            double bDotB = Vector3D.Dot(b, b);
            return b * aDotB / bDotB;
        }

        public static Vector3D Rejected(this Vector3D a, Vector3D b)
        {
            return Vector3D.Reject(a, b);
        }

        public static Vector3D Normalized(this Vector3D vec)
        {
            return Vector3D.Normalize(vec);
        }

        public static Vector3D Crossed(this Vector3D a, Vector3D b)
        {
            return Vector3D.Cross(a, b);
        }
        public static double Dotted(this Vector3D a, Vector3D b)
        {
            return Vector3D.Dot(a, b);
        }
    }
}
