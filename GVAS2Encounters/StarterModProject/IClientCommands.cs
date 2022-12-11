using Sandbox.ModAPI;

namespace GVA.NPCControl
{
    public interface IClientCommands
    {
        void IncreaseNPCAndNotify(IMyTerminalBlock block, string faction, string shipType);
    }
}
