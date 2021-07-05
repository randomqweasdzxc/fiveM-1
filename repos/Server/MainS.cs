using CitizenFX.Core;
using CitizenFX.Core.Native;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


namespace Server
{
    public class MainS : BaseScript
    {
        public MainS()
        {
            Debug.WriteLine($"qwerty load1");
            //EventHandlers["onSRStart"] += new Action<string, bool>(OnSRStart);
        }
        public void OnSRStart(string resourceName, bool q = false)
        {
            if (!q)
            {
                q = true;
                Debug.WriteLine($"qwerty {resourceName} load2");
                Debug.WriteLine($"qwerty {q} load3");
                
            }
            
        }
    }
}
