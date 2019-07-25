using NAudio.CoreAudioApi;
using System;
using System.ServiceProcess;

namespace MuteAudioOnLidService
{
    /*
     * Windows Service for LCB laptops. 
     * The service mutes the master audio when the 
     * laptop lid opens or closes.
     * Modified version of: https://github.com/rowandh/lidstatusservice
     * Requires: NAudio by Mark Heath. Install via NuGet.  GitHub URL: https://github.com/naudio/NAudio
     */
    public partial class MuteAudioOnLidService : ServiceBase
    {
        private MMDeviceEnumerator _deviceEnumerator = new MMDeviceEnumerator();
        private MMDevice _playbackDevice;
        private Lid _lid;

        public MuteAudioOnLidService()
        {
            InitializeComponent();
            // Get a handle to the master audio playback device.
            _playbackDevice = _deviceEnumerator.GetDefaultAudioEndpoint(DataFlow.Render, Role.Multimedia); 
        }

        // When the service starts.
        protected override void OnStart(string[] args)
        {
            _lid = new Lid();
            // Define Action delegate to handle the event.
            Action<bool> lidEventHandler = status => MuteAudio(status);
            // Register our event handler Action.
            var registeredNotficationsSuccess = _lid.RegisterLidEventNotifications(ServiceHandle, ServiceName, lidEventHandler);
        }

        // When the service stops. 
        // This seems to be long running. We have to stop the service with Task Manager, which works.
        // The only time we would stop it is when we are uninstalling it.
        // Looking into this.
        protected override void OnStop()
        {
            _lid.UnregisterLidEventNotifications();
        }

        // Event handler delegate.
        private void MuteAudio(bool status)
        {
            // status == true  Lid is open.
            // We don't really care here. We want to mute on any lid event.
            // Mute the system playback device.
            _playbackDevice.AudioEndpointVolume.Mute = true;
        }
    }
}
