using CitizenFX.Core;
using CitizenFX.Core.Native;
using System;
using System.Threading.Tasks;
using Server;

namespace Client
{
    public class MainC : BaseScript
    {
        public MainC()
        {
            TriggerServerEvent("onSRStart");
            //Tick += OnTick;
        }
        private static int gTimer;
        private async Task OnTick()
        {
            API.SetFollowPedCamViewMode(4);
            API.SetFollowVehicleCamViewMode(4);
            Client.NdLoop();
            if (!Client.eventEnd)
            {
                if (API.GetGameTimer() - gTimer >= 1000)
                {
                    gTimer = API.GetGameTimer();
                    Client.Loop();
                    await BaseScript.Delay(0);
                }
            }
        }
    }

}
