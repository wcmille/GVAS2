﻿using Sandbox.ModAPI;
using System;
using System.Collections.Generic;
using VRage.Game.ModAPI;

namespace GVA.NPCControl.Server
{
    public interface IAntagonist
    {
        int Military { get; }

        /// <summary>
        /// Fight the other account.
        /// </summary>
        /// <returns>
        /// true is fight was lost
        /// </returns>
        /// <param name="account"></param>
        bool Fight();
    }

    public class PirateAccount : IServerAccount, IAntagonist
    {
        readonly Random r;
        readonly IMyFaction faction;
        const int pirateRepDrop = 25;
        const int alliedBound = 500;
        const int pirateRepThreshold = -750; //Natural Decay won't make your rep worse than this.
        readonly ServerLog log;
        readonly IFactionsApi server;

        public PirateAccount(ServerLog pirateLog, IFactionsApi server, int seed = 0)
        {
            this.server = server;
            this.log = pirateLog;
            faction = MyAPIGateway.Session.Factions.TryGetFactionByTag("SPRT");

            if (seed != 0) { r = new Random(seed); }
            else r = new Random();
            OwningNPCTag = SharedConstants.BlackFactionTag;
            ColorFaction = SharedConstants.BlackFactionColor;

            int mil;
            double uu;

            MyAPIGateway.Utilities.GetVariable($"{ColorFaction}{SharedConstants.MilitaryStr}", out mil);
            MyAPIGateway.Utilities.GetVariable($"{ColorFaction}{SharedConstants.CreditsStr}", out uu);

            Military = mil;
            UnspentUnits = uu;
            log.Read();
        }
        public double UnspentUnits { get; private set; }

        public string ColorFaction { get; private set; }

        public string OwningNPCTag { get; private set; }
        public string OwningPCTag { get { return null; } }

        public int Military { get; private set; }

        public IMyFaction OwningPCFaction
        {
            get
            {
                return null;
            }
        }

        public void AddUnspent(IMyFaction donor)
        {
            if (donor != null)
            {
                UnspentUnits += 1.0;

                int memberCount = donor.Members.Count;
                foreach (var playerId in donor.Members.Keys)
                {
                    //var playerId = ply.Value.PlayerId;
                    var rep = MyAPIGateway.Session.Factions.GetReputationBetweenPlayerAndFaction(playerId, faction.FactionId);
                    rep += (alliedBound - rep) / (2 * memberCount);
                    server.SetReputation(playerId, faction.FactionId, rep);
                    //MyAPIGateway.Session.Factions.SetReputationBetweenPlayerAndFaction(playerId, faction.FactionId, rep);
                }
                log?.Log($"{donor.Tag} funded pirates.");
            }
        }

        public bool RemoveUnspent()
        {
            return false;
        }

        public void TimePeriod(IAntagonist pirates)
        {
            Military += (int)Math.Truncate(UnspentUnits);
            UnspentUnits -= Math.Truncate(UnspentUnits);
            if (Military < 25)
            {
                Military += 1;
            }
            var list = new List<IMyIdentity>();
            MyAPIGateway.Players.GetAllIdentites(list, x => MyAPIGateway.Players.TryGetSteamId(x.IdentityId) != 0);
            foreach (IMyIdentity identity in list) 
            {
                var rep = MyAPIGateway.Session.Factions.GetReputationBetweenPlayerAndFaction(identity.IdentityId, faction.FactionId);
                if (rep > pirateRepThreshold) server.SetReputation(identity.IdentityId, faction.FactionId, rep-pirateRepDrop);
                //if (rep > pirateRepThreshold) MyAPIGateway.Session.Factions.SetReputationBetweenPlayerAndFaction(identity.IdentityId, faction.FactionId, rep - pirateRepDrop);
            }
        }

        public void Read()
        {
            int mil;
            double uu;

            MyAPIGateway.Utilities.GetVariable($"{ColorFaction}{SharedConstants.CreditsStr}", out uu);
            MyAPIGateway.Utilities.GetVariable($"{ColorFaction}{SharedConstants.MilitaryStr}", out mil);
            UnspentUnits = uu;
            Military = mil;
        }

        public void Write()
        {
            MyAPIGateway.Utilities.SetVariable($"{ColorFaction}{SharedConstants.CreditsStr}", UnspentUnits);
            MyAPIGateway.Utilities.SetVariable($"{ColorFaction}{SharedConstants.MilitaryStr}", Military);
        }

        public bool Fight()
        {
            int result = r.Next(4);
            if (result == 0) return true; //opponent lost.
            if (result == 3) Military--;
            return false;
        }
    }
}
