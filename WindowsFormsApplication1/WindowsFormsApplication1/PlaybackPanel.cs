using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WindowsFormsApplication1.Properties;

namespace WindowsFormsApplication1
{
    class PlaybackPanel: System.Windows.Forms.Panel
    {
        System.Windows.Forms.Button m_playPauseButton;
        System.Windows.Forms.Button m_stepForwardButton;
        System.Windows.Forms.Button m_stepBackwardButton;
        System.Windows.Forms.Button m_speedButton;
        System.Windows.Forms.TrackBar m_timeline;

        System.Windows.Forms.ToolTip m_toolTip;

        System.Windows.Forms.Timer m_updatePositionTimer;

        System.DateTime m_startTime;
        System.DateTime m_endTime;

        bool m_bSettingValue = false;
        bool m_bPaused = false;

        AvigilonDotNet.IStreamGroup m_streamGroup;

        // Speed popup controls
        System.Windows.Forms.ToolStripDropDown m_speedPopup;
        System.Windows.Forms.TrackBar m_speed;

        const int k_buttonSize = 28;
        const int k_largeButtonSize = 40;
        const int k_trackbarSize = 25;
        const int k_panelHeight = 32;
        const int k_speedBarHeight = 30;
        const int k_speedBarWidth = 120;

        public PlaybackPanel()
        {
            m_toolTip = new System.Windows.Forms.ToolTip();

            m_playPauseButton = new System.Windows.Forms.Button();
            m_playPauseButton.Image = Resources.Pause;
            m_playPauseButton.Size = new System.Drawing.Size(k_buttonSize, k_buttonSize);
            m_playPauseButton.Click += new System.EventHandler(OnPlayPauseButtonClick_);

            m_stepForwardButton = new System.Windows.Forms.Button();
            m_stepForwardButton.Image = Resources.StepForward;
            m_stepForwardButton.Size = new System.Drawing.Size(k_buttonSize, k_buttonSize);
            m_stepForwardButton.Click += new System.EventHandler(OnStepForwardButtonClick_);
            m_toolTip.SetToolTip(m_stepForwardButton, "Step Forward");

            m_stepBackwardButton = new System.Windows.Forms.Button();
            m_stepBackwardButton.Image = Resources.StepBackward;
            m_stepBackwardButton.Size = new System.Drawing.Size(k_buttonSize, k_buttonSize);
            m_stepBackwardButton.Click += new System.EventHandler(OnStepBackwardButtonClick_);
            m_toolTip.SetToolTip(m_stepBackwardButton, "Step Backward");

            m_speedButton = new System.Windows.Forms.Button();
            m_speedButton.Text = "1x";
            m_speedButton.Image = Resources.DropArrow;
            m_speedButton.TextImageRelation = System.Windows.Forms.TextImageRelation.TextBeforeImage;
            m_speedButton.Size = new System.Drawing.Size(k_largeButtonSize, k_buttonSize);
            m_speedButton.Click += new System.EventHandler(OnSpeedButtonClick_);
            m_toolTip.SetToolTip(m_speedButton, "Change Playback Speed");

            m_timeline = new System.Windows.Forms.TrackBar();
            m_timeline.Minimum = 0;
            m_timeline.Maximum = 1000;
            m_timeline.Value = 0;
            m_timeline.SmallChange = 1;
            m_timeline.LargeChange = 50;
            m_timeline.ValueChanged += new System.EventHandler(OnTimelineValueChanged_);
            m_timeline.AutoSize = false;
            m_timeline.Height = k_trackbarSize;

            m_speedPopup = new System.Windows.Forms.ToolStripDropDown();
            m_speedPopup.Padding = new System.Windows.Forms.Padding(0);
            m_speedPopup.Margin = new System.Windows.Forms.Padding(0);
            m_speed = new System.Windows.Forms.TrackBar();
            m_speed.AutoSize = false;
            m_speed.Size = new System.Drawing.Size(k_speedBarWidth, k_speedBarHeight);
            m_speed.Minimum = 1;
            m_speed.Maximum = 12;
            m_speed.Value = 9;
            m_speed.ValueChanged += new System.EventHandler(OnSpeedChanged_);

            System.Windows.Forms.ToolStripControlHost host =
                new System.Windows.Forms.ToolStripControlHost(m_speed);

            m_speedPopup.Items.Add(host);

            m_updatePositionTimer = new System.Windows.Forms.Timer();
            m_updatePositionTimer.Interval = 250;
            m_updatePositionTimer.Tick += new System.EventHandler(OnUpdatePositionTimerTick_);

            Controls.Add(m_playPauseButton);
            Controls.Add(m_stepBackwardButton);
            Controls.Add(m_stepForwardButton);
            Controls.Add(m_speedButton);
            Controls.Add(m_timeline);

            Height = k_panelHeight;

            DoLayout_();
            UpdatePlayPauseButton_();
        }

        public void BeginStreaming(
            AvigilonDotNet.IStreamGroup streamGroup,
            System.DateTime startTime,
            System.DateTime endTime)
        {
            m_streamGroup = streamGroup;
            m_startTime = startTime;
            m_endTime = endTime;
            m_updatePositionTimer.Start();
        }

        public void EndStreaming()
        {
            m_updatePositionTimer.Stop();
            m_streamGroup = null;
        }

        protected override void OnSizeChanged(System.EventArgs e)
        {
            base.OnSizeChanged(e);

            DoLayout_();
        }

        void DoLayout_()
        {
            m_playPauseButton.Location = new System.Drawing.Point(2, 2);

            m_stepBackwardButton.Location = new System.Drawing.Point(
                m_playPauseButton.Right + 2,
                m_playPauseButton.Top);

            m_stepForwardButton.Location = new System.Drawing.Point(
                m_stepBackwardButton.Right + 2,
                m_stepBackwardButton.Top);

            m_speedButton.Location = new System.Drawing.Point(
                m_stepForwardButton.Right + 2,
                m_stepForwardButton.Top);

            m_timeline.Location = new System.Drawing.Point(
                m_speedButton.Right + 2,
                m_playPauseButton.Top + ((m_playPauseButton.Height - m_timeline.Height) / 2));
            m_timeline.Width = ClientSize.Width - m_timeline.Left - 2;
        }

        void OnPlayPauseButtonClick_(System.Object sender, System.EventArgs e)
        {
            if (m_streamGroup == null)
                return;

            if (m_streamGroup.IsPaused)
                m_streamGroup.Play();
            else
                m_streamGroup.Pause();
        }

        void OnStepForwardButtonClick_(System.Object sender, System.EventArgs e)
        {
            if (m_streamGroup == null)
                return;

            m_streamGroup.Step(true);
        }

        void OnStepBackwardButtonClick_(System.Object sender, System.EventArgs e)
        {
            if (m_streamGroup == null)
                return;

            m_streamGroup.Step(false);
        }

        void UpdatePlayPauseButton_()
        {
            if (m_streamGroup == null ||
                !m_streamGroup.IsPaused)
            {
                m_playPauseButton.Image = Resources.Pause;
                m_toolTip.SetToolTip(m_playPauseButton, "Pause");
            }

            else
            {
                m_playPauseButton.Image = Resources.Play;
                m_toolTip.SetToolTip(m_playPauseButton, "Play");
            }

            m_toolTip.Hide(m_playPauseButton);
        }

        void OnUpdatePositionTimerTick_(System.Object sender, System.EventArgs e)
        {
            if (m_streamGroup == null ||
                m_timeline.Capture ||
                m_streamGroup.Mode != AvigilonDotNet.PlaybackMode.Recorded)
                return;

            if (m_streamGroup.IsPaused != m_bPaused)
            {
                UpdatePlayPauseButton_();
                m_bPaused = m_streamGroup.IsPaused;
            }

            System.DateTime dateTime = m_streamGroup.CurrentPosition;
            long currentTicks = (dateTime - m_startTime).Ticks;
            long totalTicks = (m_endTime - m_startTime).Ticks;

            m_bSettingValue = true;

            double percent = (double)currentTicks / (double)totalTicks;
            try
            {
                m_timeline.Value = (int)(percent * 1000);
            }
            catch (System.Exception) { }

            m_bSettingValue = false;
        }

        void OnTimelineValueChanged_(System.Object sender, System.EventArgs e)
        {
            if (m_bSettingValue ||
                m_streamGroup == null)
                return;

            double percent = ((double)m_timeline.Value) / 1000.0;

            long ticks = (long)((m_endTime - m_startTime).Ticks * percent);

            System.DateTime newTime = m_startTime.AddTicks(ticks);

            m_streamGroup.SetCurrentPositionWithBounds(m_startTime, m_endTime, newTime);
        }

        void OnSpeedButtonClick_(System.Object sender, System.EventArgs e)
        {
            System.Drawing.Point point = PointToScreen(
                new System.Drawing.Point(m_speedButton.Left, m_speedButton.Bottom));

            m_speedPopup.Show(point, System.Windows.Forms.ToolStripDropDownDirection.BelowRight);
        }

        void OnSpeedChanged_(System.Object sender, System.EventArgs e)
        {
            double speed = 1.0;
            string label = "1x";

            switch (m_speed.Value)
            {
                case 1: speed = -8.0; label = "-8x"; break;
                case 2: speed = -4.0; label = "-4x"; break;
                case 3: speed = -2.0; label = "-2x"; break;
                case 4: speed = -1.0; label = "-1x"; break;
                case 5: speed = -0.5; label = "-½x"; break;
                case 6: speed = -0.25; label = "-¼x"; break;
                case 7: speed = 0.25; label = "¼x"; break;
                case 8: speed = 0.5; label = "½x"; break;
                case 9: speed = 1.0; label = "1x"; break;
                case 10: speed = 2.0; label = "2x"; break;
                case 11: speed = 4.0; label = "4x"; break;
                case 12: speed = 8.0; label = "8x"; break;
            }

            if (m_streamGroup != null)
            {
                m_streamGroup.Speed = speed;
            }

            m_speedButton.Text = label;
        }
    }
}
