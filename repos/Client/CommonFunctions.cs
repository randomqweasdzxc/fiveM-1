using CitizenFX.Core;
using CitizenFX.Core.Native;
using System.Threading.Tasks;

namespace Client
{
    class CommonFunctions : BaseScript
    {
        public static void DisplayMessage(string msg, int time)
        {
            API.ClearPrints();
            API.SetTextEntry_2("STRING");
            API.AddTextComponentString(msg);
            API.DrawSubtitleTimed(time, true);
        }
        public static async Task<bool> LoadModel(uint model)
        {
            if (!API.IsModelInCdimage(model))
            {
                //Debug.WriteLine($"invalid {model}");
                return false;
            }
            API.RequestModel(model);
            while (!API.HasModelLoaded(model))
            {
                //System.Diagnostics.Debug.WriteLine($"waiting {model} load");
                await BaseScript.Delay(100);
            }
            return true;
        }
    }
}
