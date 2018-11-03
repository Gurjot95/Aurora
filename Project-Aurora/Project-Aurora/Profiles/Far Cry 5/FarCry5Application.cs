using Aurora.Settings;
using Newtonsoft.Json;
using System;
using System.IO;
using System.Text;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Media.Imaging;

namespace Aurora.Profiles.FarCry5
{
    public class FarCry5 : Application
    {
        public FarCry5()
            : base(new LightEventConfig { Name = "FarCry5", ID = "FarCry5", ProcessNames = new[] { "FarCry5.exe"}, ProfileType = typeof(WrapperProfile), OverviewControlType = typeof(Control_FarCry5), GameStateType = typeof(GameState_Wrapper), Event = new GameEvent_Generic(), IconURI = "Resources/FarCry5_128x128.png" })
        {
            Config.ExtraAvailableLayers.Add("WrapperLights");
        }
    }
}
