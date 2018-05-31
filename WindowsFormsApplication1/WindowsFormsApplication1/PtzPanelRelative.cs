using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WindowsFormsApplication1
{
    class PtzPanelRelative
        : System.Windows.Forms.Panel
    {
        // Pan/tilt controls
        System.Windows.Forms.GroupBox m_ptzGroupBox;
        System.Windows.Forms.Label m_topLabel;
        System.Windows.Forms.NumericUpDown m_top;
        System.Windows.Forms.Label m_leftLabel;
        System.Windows.Forms.NumericUpDown m_left;
        System.Windows.Forms.Label m_widthLabel;
        System.Windows.Forms.NumericUpDown m_width;
        System.Windows.Forms.Label m_heightLabel;
        System.Windows.Forms.NumericUpDown m_height;
        System.Windows.Forms.Button m_go;

        System.Windows.Forms.ToolTip m_toolTip;

        const int k_miscWidth = 55;
        const int k_zfiButtonWidth = 30;

        const int k_groupBoxSideBuffer = 9;
        const int k_groupBoxTopBuffer = 14;

        const int k_largeBuffer = 5;
        const int k_smallBuffer = 3;

        AvigilonDotNet.IEntityPtz m_ptz = null;

        public PtzPanelRelative()
        {
            m_toolTip = new System.Windows.Forms.ToolTip();

            m_ptzGroupBox = new System.Windows.Forms.GroupBox();
            m_ptzGroupBox.Text = "Relative PTZ";

            m_topLabel = new System.Windows.Forms.Label();
            m_topLabel.AutoSize = true;
            m_topLabel.Text = "Top:";

            m_top = new System.Windows.Forms.NumericUpDown();
            m_top.DecimalPlaces = 2;
            m_top.Value = new System.Decimal(0.0);
            m_top.Increment = new System.Decimal(0.01);
            m_top.Minimum = new decimal(-1.0);
            m_top.Maximum = new decimal(1.0);
            m_toolTip.SetToolTip(m_top, "Top");

            m_leftLabel = new System.Windows.Forms.Label();
            m_leftLabel.AutoSize = true;
            m_leftLabel.Text = "Left:";

            m_left = new System.Windows.Forms.NumericUpDown();
            m_left.DecimalPlaces = 2;
            m_left.Value = new System.Decimal(0.0);
            m_left.Increment = new System.Decimal(0.01);
            m_left.Minimum = new decimal(-1.0);
            m_left.Maximum = new decimal(1.0);
            m_toolTip.SetToolTip(m_left, "Left");

            m_widthLabel = new System.Windows.Forms.Label();
            m_widthLabel.AutoSize = true;
            m_widthLabel.Text = "Width:";

            m_width = new System.Windows.Forms.NumericUpDown();
            m_width.DecimalPlaces = 2;
            m_width.Value = new System.Decimal(0.0);
            m_width.Increment = new System.Decimal(0.01);
            m_width.Minimum = new decimal(-1.0);
            m_width.Maximum = new decimal(1.0);
            m_toolTip.SetToolTip(m_width, "Width");

            m_heightLabel = new System.Windows.Forms.Label();
            m_heightLabel.AutoSize = true;
            m_heightLabel.Text = "Height:";

            m_height = new System.Windows.Forms.NumericUpDown();
            m_height.DecimalPlaces = 2;
            m_height.Value = new System.Decimal(0.0);
            m_height.Increment = new System.Decimal(0.01);
            m_height.Minimum = new decimal(-1.0);
            m_height.Maximum = new decimal(1.0);
            m_toolTip.SetToolTip(m_height, "Height");

            m_go = new System.Windows.Forms.Button();
            m_go.MouseDown += new System.Windows.Forms.MouseEventHandler(OnGoPress_);
            m_go.Width = k_zfiButtonWidth;
            m_go.Text = "Go";
            m_go.Width = k_miscWidth;
            m_toolTip.SetToolTip(m_go, "Go");

            Controls.Add(m_topLabel);
            Controls.Add(m_top);
            Controls.Add(m_leftLabel);
            Controls.Add(m_left);
            Controls.Add(m_widthLabel);
            Controls.Add(m_width);
            Controls.Add(m_heightLabel);
            Controls.Add(m_height);
            Controls.Add(m_go);
            Controls.Add(m_ptzGroupBox);

            m_ptzGroupBox.Location = new System.Drawing.Point(k_largeBuffer, k_smallBuffer);

            m_leftLabel.Location = new System.Drawing.Point(
                m_ptzGroupBox.Left + k_groupBoxSideBuffer,
                m_ptzGroupBox.Top + k_groupBoxTopBuffer);
            m_left.Location = new System.Drawing.Point(
                m_leftLabel.Right + k_groupBoxSideBuffer,
                m_ptzGroupBox.Top + k_groupBoxTopBuffer);

            m_widthLabel.Location = new System.Drawing.Point(
                m_left.Right + k_groupBoxSideBuffer,
                m_ptzGroupBox.Top + k_groupBoxTopBuffer);
            m_width.Location = new System.Drawing.Point(
                m_widthLabel.Right + k_groupBoxSideBuffer,
                m_ptzGroupBox.Top + k_groupBoxTopBuffer);

            m_topLabel.Location = new System.Drawing.Point(
                m_ptzGroupBox.Left + k_groupBoxSideBuffer,
                m_widthLabel.Bottom + k_groupBoxTopBuffer);
            m_top.Location = new System.Drawing.Point(
                m_topLabel.Right + k_groupBoxSideBuffer,
                m_widthLabel.Bottom + k_groupBoxTopBuffer);

            m_heightLabel.Location = new System.Drawing.Point(
                m_top.Right + k_groupBoxSideBuffer,
                m_widthLabel.Bottom + k_groupBoxTopBuffer);
            m_height.Location = new System.Drawing.Point(
                m_heightLabel.Right + k_groupBoxSideBuffer,
                m_widthLabel.Bottom + k_groupBoxTopBuffer);

            m_go.Location = new System.Drawing.Point(
                m_height.Right + k_groupBoxSideBuffer,
                m_widthLabel.Bottom + k_groupBoxTopBuffer);
            m_ptzGroupBox.Size = new System.Drawing.Size(m_go.Right + k_smallBuffer, m_go.Bottom + k_smallBuffer);

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
            bool bHasRelativeFOVPtz = false;
            if (m_ptz != null)
            {
                AvigilonDotNet.PtzCapapabilities caps = m_ptz.Capabilities;
                bHasRelativeFOVPtz =
                    (caps & AvigilonDotNet.PtzCapapabilities.HasRelativeFovPanTilt) != 0 &&
                    (caps & AvigilonDotNet.PtzCapapabilities.HasRelativeFovZoom) != 0;
            }

            m_top.Enabled = bHasRelativeFOVPtz;
            m_left.Enabled = bHasRelativeFOVPtz;
            m_width.Enabled = bHasRelativeFOVPtz;
            m_height.Enabled = bHasRelativeFOVPtz;
            m_go.Enabled = bHasRelativeFOVPtz;
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
            System.Drawing.RectangleF rect = new System.Drawing.RectangleF((float)m_left.Value, (float)m_top.Value, (float)m_width.Value, (float)m_height.Value);
            CheckError_(m_ptz.PtzRelativeFov(rect));
        }
    }
}
