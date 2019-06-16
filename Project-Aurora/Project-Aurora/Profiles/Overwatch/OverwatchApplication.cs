﻿using Aurora.Settings;
using Newtonsoft.Json;
using System;
using System.IO;
using System.Text;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Media.Imaging;

namespace Aurora.Profiles.Overwatch
{
    public class Overwatch : Application
    {
        //Renaming this temporary so overwatch doesn't use old wrapper method
        public Overwatch()
            : base(new LightEventConfig { Name = "Overwatch", ID = "overwatch", ProcessNames = new[] { "overwatch2.exe" }, ProfileType = typeof(WrapperProfile), OverviewControlType = typeof(Control_Overwatch), GameStateType = typeof(GameState_Wrapper), Event = new GameEvent_Generic(), IconURI = "Resources/overwatch_icon.png" })
        {
            Config.ExtraAvailableLayers.Add("WrapperLights");
        }
    }
}
