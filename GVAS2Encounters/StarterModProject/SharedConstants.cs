﻿using System;
using VRageMath;

namespace GVA.NPCControl
{
    internal class SharedConstants
    {
        public static readonly string ModName = "GVAS3";

        public static readonly string CivilianStr = "Civilian";
        public static readonly string MilitaryStr = "Military";
        public static readonly string CreditsStr = "Credits";
        public static readonly string IncursionsStr = "Incursions";
        public static readonly string SellCreditsStr = "Sell";

        public static readonly string OwnerId = "OwnerId";
        [Obsolete]
        public static readonly string OwnerStr = "Owner";
        public static readonly string NPCStr = "NPCOwner";
        public static readonly string TimeStr = "LastRunTime";

        public static readonly string BlueFactionColor = "Blue";
        public static readonly string BlueFactionTag = "JAGH";

        public static readonly string RedFactionColor = "Red";
        public static readonly string RedFactionTag = "KAJI";

        public static readonly string BlackFactionColor = "Black";
        public static readonly string BlackFactionTag = "SPRT";
        public const long tokenPrice = 20000000;
        public const double refundFactor = 0.75;

        public const int TimeDeltaHours = 8;

        public const int AlliedRep = 501;
        public const int DefaultRep = 0;
        public const int EnemyRep = -1000;

        public static readonly string ReportCommand = "Report";

        public static readonly string ModNamespace = "GVA.NPCControl";

        public static readonly string PointCheck = "GVA.NPCControl.NPCPoints";

        public static readonly Vector3D WorldCenter = new Vector3D(-870575, 131504, 4105278);
        public static readonly BoundingSphereD NpcSphere = new BoundingSphereD(WorldCenter, 3000000.0);
    }
}
