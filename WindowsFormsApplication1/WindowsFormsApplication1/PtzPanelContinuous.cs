using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WindowsFormsApplication1
{
    class PtzPanelContinuous: System.Windows.Forms.Panel
    {
        System.Windows.Forms.GroupBox m_ptzGroupBox;
        System.Windows.Forms.Label m_panLabel;
        System.Windows.Forms.NumericUpDown m_pan;
        System.Windows.Forms.Label m_tiltLabel;
        System.Windows.Forms.NumericUpDown m_tilt;
        System.Windows.Forms.Label m_zoomLabel;
        System.Windows.Forms.NumericUpDown m_zoom;
        System.Windows.Forms.Button m_go;
        System.Windows.Forms.Button m_stop;

        System.Windows.Forms.ToolTip m_toolTip;

        const int k_miscWidth = 55;
        const int k_zfiButtonWidth = 30;

        const int k_groupBoxSideBuffer = 9;
        const int k_groupBoxTopBuffer = 14;

        const int k_largeBuffer = 5;
        const int k_smallBuffer = 3;

        AvigilonDotNet.IEntityPtz m_ptz = null;

        public PtzPanelContinuous()
        {
            m_toolTip = new System.Windows.Forms.ToolTip();

            m_ptzGroupBox = new System.Windows.Forms.GroupBox();
            m_ptzGroupBox.Text = "Continuous PTZ";

            m_panLabel = new System.Windows.Forms.Label();
            m_panLabel.AutoSize = true;
            m_panLabel.Text = "Pan:";

            m_pan = new System.Windows.Forms.NumericUpDown();
            m_pan.DecimalPlaces = 2;
            m_pan.Value = new System.Decimal(0.0);
            m_pan.Increment = new System.Decimal(0.01);
            m_pan.Minimum = new decimal(-1.0);
            m_pan.Maximum = new decimal(1.0);
            m_toolTip.SetToolTip(m_pan, "Pan Speed");

            m_tiltLabel = new System.Windows.Forms.Label();
            m_tiltLabel.AutoSize = true;
            m_tiltLabel.Text = "Tilt:";

            m_tilt = new System.Windows.Forms.NumericUpDown();
            m_tilt.DecimalPlaces = 2;
            m_tilt.Value = new System.Decimal(0.0);
            m_tilt.Increment = new System.Decimal(0.01);
            m_tilt.Minimum = new decimal(-1.0);
            m_tilt.Maximum = new decimal(1.0);
            m_toolTip.SetToolTip(m_tilt, "Tilt Speed");

            m_zoomLabel = new System.Windows.Forms.Label();
            m_zoomLabel.AutoSize = true;
            m_zoomLabel.Text = "Zoom:";

            m_zoom = new System.Windows.Forms.NumericUpDown();
            m_zoom.DecimalPlaces = 2;
            m_zoom.Value = new System.Decimal(0.0);
            m_zoom.Increment = new System.Decimal(0.01);
            m_zoom.Minimum = new decimal(-1.0);
            m_zoom.Maximum = new decimal(1.0);
            m_toolTip.SetToolTip(m_zoom, "Zoom Speed");

            m_go = new System.Windows.Forms.Button();
            m_go.MouseDown += new System.Windows.Forms.MouseEventHandler(OnGoPress_);
            m_go.Width = k_zfiButtonWidth;
            m_go.Text = "Go";
            m_go.Width = k_miscWidth;
            m_toolTip.SetToolTip(m_go, "Go");

            m_stop = new System.Windows.Forms.Button();
            m_stop.MouseDown += new System.Windows.Forms.MouseEventHandler(OnStopPress_);
            m_stop.Width = k_zfiButtonWidth;
            m_stop.Text = "Stop";
            m_stop.Width = k_miscWidth;
            m_toolTip.SetToolTip(m_stop, "Stop");

            Controls.Add(m_panLabel);
            Controls.Add(m_pan);
            Controls.Add(m_tiltLabel);
            Controls.Add(m_tilt);
            Controls.Add(m_zoomLabel);
            Controls.Add(m_zoom);
            Controls.Add(m_go);
            Controls.Add(m_stop);
            Controls.Add(m_ptzGroupBox);

            m_ptzGroupBox.Location = new System.Drawing.Point(k_largeBuffer, k_smallBuffer);

            m_panLabel.Location = new System.Drawing.Point(
                m_ptzGroupBox.Left + k_groupBoxSideBuffer,
                m_ptzGroupBox.Top + k_groupBoxTopBuffer);
            m_pan.Location = new System.Drawing.Point(
                m_ptzGroupBox.Left + k_groupBoxSideBuffer,
                m_panLabel.Bottom + k_largeBuffer);

            m_tiltLabel.Location = new System.Drawing.Point(
                m_pan.Right + k_groupBoxSideBuffer,
                m_ptzGroupBox.Top + k_groupBoxTopBuffer);
            m_tilt.Location = new System.Drawing.Point(
                m_pan.Right + k_groupBoxSideBuffer,
                m_tiltLabel.Bottom + k_largeBuffer);

            m_zoomLabel.Location = new System.Drawing.Point(
                m_tilt.Right + k_groupBoxSideBuffer,
                m_ptzGroupBox.Top + k_groupBoxTopBuffer);
            m_zoom.Location = new System.Drawing.Point(
                m_tilt.Right + k_groupBoxSideBuffer,
                m_zoomLabel.Bottom + k_largeBuffer);

            m_go.Location = new System.Drawing.Point(
                m_zoom.Right + k_groupBoxSideBuffer,
                m_ptzGroupBox.Top + k_groupBoxTopBuffer);
            m_stop.Location = new System.Drawing.Point(
                m_zoom.Right + k_groupBoxSideBuffer,
                m_go.Bottom + k_largeBuffer);

            m_ptzGroupBox.Size = new System.Drawing.Size(m_stop.Right + k_smallBuffer, m_stop.Bottom + k_smallBuffer);

            Height = m_ptzGroupBox.Bottom + k_smallBuffer;

            UpdateControls_();
        }

        public AvigilonDotNet.IEntityPtz Entity
        {
            set
            {
                m_ptz = value;
                UpdateControls_();
            }
        }

        void UpdateControls_()
        {
            bool bHasContinuousPtz = false;
            if (m_ptz != null)
            {
                AvigilonDotNet.PtzCapapabilities caps = m_ptz.Capabilities;
                bHasContinuousPtz =
                    (caps & AvigilonDotNet.PtzCapapabilities.HasContinuousPanTilt) != 0 &&
                    (caps & AvigilonDotNet.PtzCapapabilities.HasContinuousZoom) != 0;
            }

            m_pan.Enabled = bHasContinuousPtz;
            m_tilt.Enabled = bHasContinuousPtz;
            m_zoom.Enabled = bHasContinuousPtz;
            m_go.Enabled = bHasContinuousPtz;
            m_stop.Enabled = bHasContinuousPtz;
        }

        bool CheckError_(AvigilonDotNet.AvgError result)
        {
            if (result == AvigilonDotNet.AvgError.DevicePtzLocked)
            {
                System.Windows.Forms.MessageBox.Show(
                    "The PTZ controls are locked by another user.");

                return false;
            }

            else if (AvigilonDotNet.ErrorHelper.IsError(result))
            {
                System.Windows.Forms.MessageBox.Show(
                    "An error occurred sending the PTZ command.");

                return false;
            }

            return true;
        }

        void OnGoPress_(System.Object sender, System.Windows.Forms.MouseEventArgs e)
        {
            if (m_ptz == null) return;
            CheckError_(m_ptz.PtzContinuous(AvigilonDotNet.PtzMovement.PanTilt | AvigilonDotNet.PtzMovement.Zoom, (float)m_pan.Value, (float)m_tilt.Value, (float)m_zoom.Value));
        }

        void OnStopPress_(System.Object sender, System.Windows.Forms.MouseEventArgs e)
        {
            if (m_ptz == null) return;
            CheckError_(m_ptz.StopPanTilt());
            CheckError_(m_ptz.StopZoom());
        }
    }
}
