using WindowsFormsApplication1.Properties;
using AvigilonDotNet;

namespace WindowsFormsApplication1
{
    class CameraViewer
    {
        System.Net.IPEndPoint m_endPoint = null;
        System.Net.IPAddress m_address = null;
        bool m_camSpecifiedLogical = true;
        System.UInt32 m_cameraLogicalId = 0;
        System.String m_cameraDeviceId = "";
        bool m_bLive = false;
        System.DateTime m_startTime;
        System.DateTime m_endTime;
        string m_userName = null;
        string m_password = null;

        AvigilonSdk m_sdk = null;
        IAvigilonControlCenter m_controlCenter = null;
        IEntityCamera m_camera = null;
        IStreamWindow m_streamWindow = null;
        IStreamGroup m_streamGroup = null;

        System.Windows.Forms.Form m_viewerForm = null;
        ViewerPanel m_viewerPanel = null;

        const int k_nvrConnectWait = 10; 
        const int k_cameraInfoWait = 10; 

        delegate void ErrorHandler(string errorMessage);

        public CameraViewer()
        {
        }

        public void Run()
        {
            System.Windows.Forms.Application.EnableVisualStyles();

            // Parse commandline options.
            if (!ParseCommandLine())
            {
                ShowCommandLineUsage_();
                return;
            }

            // Create the form to show while connecting to the NVR.
            m_viewerForm = new System.Windows.Forms.Form();
            m_viewerForm.Size = new System.Drawing.Size(646, 500);
            m_viewerForm.MinimumSize = new System.Drawing.Size(646, 500);
            m_viewerForm.Text = "Avigilon Viewer";
            m_viewerForm.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            m_viewerForm.Icon = Resources.App;

            m_viewerPanel = new ViewerPanel();
            m_viewerPanel.Dock = System.Windows.Forms.DockStyle.Fill;

            m_viewerForm.Controls.Add(m_viewerPanel);

            // Ensure handle is created so BeginInvoke() works
            if (!m_viewerForm.IsHandleCreated)
            {
                System.IntPtr handle = m_viewerForm.Handle;
            }

            // Create and initialize the control center SDK.
            AvigilonDotNet.SdkInitParams initParams = new AvigilonDotNet.SdkInitParams(
                AvigilonDotNet.AvigilonSdk.MajorVersion,
                AvigilonDotNet.AvigilonSdk.MinorVersion);
            initParams.AutoDiscoverNvrs = false;
            initParams.ServiceMode = false;

            m_sdk = new AvigilonDotNet.AvigilonSdk();
            m_controlCenter = m_sdk.CreateInstance(initParams);

            if (m_controlCenter == null)
            {
                System.Windows.Forms.MessageBox.Show("The Avigilon SDK could not be initialized.");
                return;
            }

            // Start up the initialization thread, which will connect to the NVR
            // and find the right device in the background.
            System.Threading.Thread thread = new System.Threading.Thread(
                new System.Threading.ThreadStart(InitializationThread_));

            thread.Start();

            // Run the main form
            System.Windows.Forms.Application.Run(m_viewerForm);

            m_viewerPanel.EndStreaming();

            if (m_streamWindow != null)
                m_streamWindow.Dispose();

            if (m_streamGroup != null)
                m_streamGroup.Dispose();

            // Shut down control center
            if (m_controlCenter != null)
                m_controlCenter.Dispose();

            m_sdk.Shutdown();
        }

        void InitializationThread_()
        {
            // Add the, incase it is not on the local network. Specify and address and 
            // not an endpoint so the port can be determined by the configuration file
            if (m_address != null)
            {
                m_endPoint = new System.Net.IPEndPoint(m_address, m_controlCenter.DefaultNvrPortNumber);
            }
            m_controlCenter.AddNvr(m_endPoint);

            // Wait for the NVR to appear in the list
            System.DateTime waitEnd = System.DateTime.Now + new System.TimeSpan(0, 0, k_nvrConnectWait);
            AvigilonDotNet.INvr nvr = null;

            while (System.DateTime.Now < waitEnd &&
                nvr == null)
            {
                nvr = m_controlCenter.GetNvr(m_endPoint.Address);

                if (nvr == null)
                    System.Threading.Thread.Sleep(500);
            }

            if (nvr == null)
            {
                m_viewerForm.BeginInvoke(
                    new ErrorHandler(OnInitializationError_),
                    "An error occurred while connecting to the NVR.");
                return;
            }

            // Log in to the NVR
            AvigilonDotNet.LoginResult loginResult = nvr.Login(
                m_userName,
                m_password);
            if (loginResult != AvigilonDotNet.LoginResult.Successful)
            {
                m_viewerForm.BeginInvoke(
                    new ErrorHandler(OnInitializationError_),
                    "Failed to login to NVR: " + loginResult.ToString());
                return;
            }

            // Try to get our device, give a few seconds because it may take awhile
            // for the device info to come in from the NVR
            waitEnd = System.DateTime.Now + new System.TimeSpan(0, 0, k_cameraInfoWait);
            AvigilonDotNet.IEntityCamera camera = null;

            while (System.DateTime.Now < waitEnd && camera == null)
            {
                if (m_camSpecifiedLogical)
                {
                    // Try to grab the entity from each device
                    System.Collections.Generic.List<AvigilonDotNet.IDevice> devices = nvr.Devices;
                    foreach (AvigilonDotNet.IDevice device in devices)
                    {
                        camera = (AvigilonDotNet.IEntityCamera)device.GetEntityByLogicalId(m_cameraLogicalId);
                        if (camera != null)
                            break;
                    }
                }
                else
                {
                    camera = (AvigilonDotNet.IEntityCamera)nvr.GetEntityById(m_cameraDeviceId);
                }

                if (camera == null)
                    System.Threading.Thread.Sleep(500);
            }

            // Device isn't a part of the NVR
            if (camera == null)
            {
                m_viewerForm.BeginInvoke(
                    new ErrorHandler(OnInitializationError_),
                    "The given camera is not connected to the NVR.");
                return;
            }

            // We have our device, tell the GUI thread to start streaming.
            m_camera = camera;

            m_viewerForm.BeginInvoke(
                new System.Windows.Forms.MethodInvoker(OnInitializationSuccess_));
        }

        void OnInitializationError_(string errorMessage)
        {
            if (m_viewerForm == null)
                return;

            // An error occurred initializing the control center SDK, or starting a
            // stream from the device.  Show an error message and shut down the main form.
            System.Windows.Forms.MessageBox.Show(errorMessage);

            m_viewerForm.Close();
        }

        void OnInitializationSuccess_()
        {
            if (m_viewerForm == null)
                return;

            m_viewerForm.Text = m_camera.DisplayName + " - Avigilon Viewer";

            // We successfully logged into the NVR and found the requested device, now we
            // can begin streaming from the device.
            if (m_bLive)
            {
                m_streamGroup = m_controlCenter.CreateStreamGroup(AvigilonDotNet.PlaybackMode.Live);
            }
            else
            {
                m_streamGroup = m_controlCenter.CreateStreamGroup(AvigilonDotNet.PlaybackMode.Recorded);

                if (m_streamGroup != null)
                {
                    m_streamGroup.SetCurrentPositionWithBounds(
                        m_startTime,
                        m_endTime,
                        m_startTime);
                }
            }

            if (m_streamGroup != null)
            {
                m_controlCenter.CreateStreamWindow(
                     // m_controlCenter.CreateStreamAttr(),
                     m_camera,
                     m_streamGroup,
                     out m_streamWindow);
            }

            if (m_streamWindow == null ||
                m_streamGroup == null)
            {
                System.Windows.Forms.MessageBox.Show(
                    "An error occurred while connecting to the NVR.");

                m_viewerForm.Close();

                return;
            }

            m_viewerPanel.BeginStreaming(
                m_camera,
                m_streamWindow,
                m_streamGroup,
                m_startTime,
                m_endTime);
        }

        bool ParseCommandLine()
        {
            // Parse the RS2 arguments from the commandline.

            System.DateTime date = new System.DateTime();
            System.DateTime startTime = new System.DateTime();
            System.DateTime endTime = new System.DateTime();

            foreach (string arg in System.Environment.GetCommandLineArgs())
            {
                // We only want arguments of the form "-xyyyyyy"
                if (arg.Length < 2 ||
                    arg[0] != '-')
                    continue;

                string value = arg.Substring(2);
                //if our value is empty, it is not valid
                if (!(value.Length > 0))
                {
                    continue;
                }

                // -s192.168.111.111: NVR IP address
                if (arg[1] == 's')
                {
                    System.Net.IPAddress address;

                    // Try to parse plain IP address
                    if (System.Net.IPAddress.TryParse(value, out address))
                    {
                        m_address = address;
                        continue;
                    }

                    // Try to parse IP:Port address
                    int colonIdx = value.LastIndexOf(':');
                    if (colonIdx >= 0 &&
                        colonIdx < (value.Length - 1))
                    {
                        string addressString = value.Substring(0, colonIdx);
                        string portString = value.Substring(colonIdx + 1);
                        int port;

                        if (System.Net.IPAddress.TryParse(addressString, out address) &&
                            System.Int32.TryParse(portString, out port))
                        {
                            m_endPoint = new System.Net.IPEndPoint(address, port);
                        }
                    }
                }

                // -c3: camera ID
                else if (arg[1] == 'c')
                {
                    if (value[0] == 'd')
                    {
                        m_camSpecifiedLogical = false;
                        m_cameraDeviceId = value.Substring(1);
                    }
                    else
                    {
                        m_camSpecifiedLogical = true;
                        System.UInt32.TryParse(value, out m_cameraLogicalId);
                    }
                }

                // -dlive, or -d01052008: live playback, or the day for recorded playback
                else if (arg[1] == 'd')
                {
                    if (value == "live")
                    {
                        m_bLive = true;
                    }

                    else
                    {
                        int day = -1;
                        int month = -1;
                        int year = -1;

                        if (value.Length == 8 &&
                            System.Int32.TryParse(value.Substring(0, 2), out month) &&
                            System.Int32.TryParse(value.Substring(2, 2), out day) &&
                            System.Int32.TryParse(value.Substring(4, 4), out year) &&
                            month >= 1 &&
                            month <= 12 &&
                            day >= 1 &&
                            day <= 31 &&
                            year >= 0)
                        {
                            date = new System.DateTime(year, month, day); ;
                        }
                    }
                }

                // -b12:05:10: playback start time
                else if (arg[1] == 'b')
                {
                    System.DateTime.TryParse(value, out startTime);
                }

                // -e04:12:01: playback end time
                else if (arg[1] == 'e')
                {
                    System.DateTime.TryParse(value, out endTime);
                }

                // -uusername: NVR user name
                else if (arg[1] == 'u')
                {
                    m_userName = value;
                }

                // -ppassword: NVR password
                else if (arg[1] == 'p')
                {
                    m_password = value;
                }
            }

            if ((m_address == null && m_endPoint == null) ||
                (!m_camSpecifiedLogical && m_cameraDeviceId == null) ||
                (!m_camSpecifiedLogical && m_cameraDeviceId.Length == 0))
            {
                return false;
            }

            if (!m_bLive)
            {
                System.DateTime emptyTime = new System.DateTime();

                if (date == emptyTime ||
                    startTime == emptyTime ||
                    endTime == emptyTime)
                {
                    return false;
                }

                m_startTime = new System.DateTime(
                    date.Year,
                    date.Month,
                    date.Day,
                    startTime.Hour,
                    startTime.Minute,
                    startTime.Second);

                m_endTime = new System.DateTime(
                    date.Year,
                    date.Month,
                    date.Day,
                    endTime.Hour,
                    endTime.Minute,
                    endTime.Second);
            }

            return true;
        }

        void ShowCommandLineUsage_()
        {
            string appName = System.Environment.GetCommandLineArgs()[0];
            appName = System.IO.Path.GetFileName(appName);

            string errorMessage =

                "Usage for live streaming:\n\n" +
                "   " + appName + " -s[IP address] -c[camera] -dlive -u[user] -p[password]\n\n" +
                "Usage for playback:\n\n" +
                "   " + appName + " -s[IP address] -c[camera] -d[date] -b[start time] -e[end time] -u[user] -p[password]\n\n\n" +
                "Arguments:\n\n" +
                "	-s[IP address]	: The IP address, and optional port, of the NVR to stream from.\n" +
                "			: ex: 192.168.10.10 or 192.168.10.10:49000\n" +
                "	-c[camera]	: The logical ID of the camera to stream from.\n" +
                "	-d[date]		: The date to play back from, in mmddyyyy format.\n" +
                "	-b[start time]	: The playback start time, in hh:mm:ss format.\n" +
                "	-e[end time]	: The playback end time, in hh:mm:ss format.\n" +
                "	-u[user]		: The user name to log in to the NVR as.\n" +
                "	-p[password]	: The password to log in to the NVR with.";

            System.Windows.Forms.MessageBox.Show(errorMessage, appName);
        }
    }
}
