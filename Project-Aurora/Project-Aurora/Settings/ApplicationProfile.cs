using Aurora.Settings;
using Aurora.Settings.Layers;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;

namespace Aurora.Settings
{
    public abstract class Settings : INotifyPropertyChanged, ICloneable
    {
        public event PropertyChangedEventHandler PropertyChanged;

        protected void InvokePropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        public object Clone()
        {
            string str = JsonConvert.SerializeObject(this, Formatting.None, new JsonSerializerSettings { TypeNameHandling = TypeNameHandling.All, Binder = Aurora.Utils.JSONUtils.SerializationBinder });

            return JsonConvert.DeserializeObject(
                    str,
                    this.GetType(),
                    new JsonSerializerSettings { ObjectCreationHandling = ObjectCreationHandling.Replace, TypeNameHandling = TypeNameHandling.All, Binder = Aurora.Utils.JSONUtils.SerializationBinder }
                    );
        }
    }

    public class ScriptSettings : Settings
    {
        #region Private Properties
        private KeySequence _Keys;

        private bool _Enabled = false;

        private bool _ExceptionHit = false;

        private Exception _Exception = null;
        #endregion

        #region Public Properties
        public KeySequence Keys { get { return _Keys; } set { _Keys = value; InvokePropertyChanged(); } }

        public bool Enabled { get { return _Enabled; }
            set {
                _Enabled = value;
                if (value)
                {
                    ExceptionHit = false;
                    Exception = null;
                }
                InvokePropertyChanged();
            }
        }

        [JsonIgnore]
        public bool ExceptionHit { get { return _ExceptionHit; } set { _ExceptionHit = value; InvokePropertyChanged(); } }

        [JsonIgnore]
        public Exception Exception { get { return _Exception; } set { _Exception = value; InvokePropertyChanged(); } }
        #endregion

        public ScriptSettings(dynamic script)
        {
            if (script?.DefaultKeys != null && script?.DefaultKeys is KeySequence)
                Keys = script.DefaultKeys;
        }
    }

    public class ApplicationProfile : Settings, IDisposable
    {
        #region Private Properties
        private string _ProfileName = "";

        private Keybind _triggerKeybind;

        private Dictionary<string, ScriptSettings> _ScriptSettings;

        private ObservableCollection<Layer> _Layers;
        #endregion

        #region Public Properties
        public string ProfileName { get { return _ProfileName; } set { _ProfileName = value; InvokePropertyChanged(); } }

        public Keybind TriggerKeybind { get { return _triggerKeybind; } set { _triggerKeybind = value; InvokePropertyChanged(); } }

        [JsonIgnore]
        public string ProfileFilepath { get; set; }

        public Dictionary<string, ScriptSettings> ScriptSettings { get { return _ScriptSettings; } set { _ScriptSettings = value; InvokePropertyChanged(); } }

        public ObservableCollection<Layer> Layers { get { return _Layers; } set { _Layers = value; InvokePropertyChanged(); } }
        #endregion

        public ApplicationProfile()
        {
            this.Reset();
        }

        public virtual void Reset()
        {
            _Layers = new ObservableCollection<Layer>();
            _ScriptSettings = new Dictionary<string, Aurora.Settings.ScriptSettings>();
            _triggerKeybind = new Keybind();
        }

        /// <summary>
        /// Helper function that returns a list of all AnimationLayerHandlers that are currently visible.
        /// </summary>
        private IEnumerable<AnimationLayerHandler> VisibleAnimationLayerHandlers => Layers
            .Where(layer => layer.Enabled) // Only include visible layers
            .Where(layer => layer.Handler is AnimationLayerHandler) // Check if animation layers
            .Select(layer => layer.Handler as AnimationLayerHandler); // Return the handler

        /// <summary>
        /// Returns the highest value of the durations of any animation layers in this profile. Default to 0 if there are none.
        /// </summary>
        public float MaxVisibleAnimationLayerDuration => VisibleAnimationLayerHandlers
            .Max(handler => handler.Properties._AnimationDuration) ?? 0;

        /// <summary>
        /// Returns a boolean indicating whether all animation layers in this profile have a duration of exactly 1.
        /// </summary>
        public bool AllAnimationLayersDuration1 => VisibleAnimationLayerHandlers
            .All(handler => handler.Properties._AnimationDuration == 1); // Return true if all durations are 1

        public virtual void Dispose()
        {
            foreach (Layer l in _Layers)
                l.Dispose();
        }
    }
}
