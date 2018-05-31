using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WindowsFormsApplication1
{
    class PtzPanelDiscrete
        : System.Windows.Forms.Panel
    {
        // Pan/tilt controls
        System.Windows.Forms.GroupBox m_ptzGroupBox;
        System.Windows.Forms.Label m_leftLabel;
        System.Windows.Forms.NumericUpDown m_left;
        System.Windows.Forms.Label m_topLabel;
        System.Windows.Forms.NumericUpDown m_top;
        System.Windows.Forms.Label m_rightLabel;
        System.Windows.Forms.NumericUpDown m_right;
        System.Windows.Forms.Label m_bottomLabel;
        System.Windows.Forms.NumericUpDown m_bottom;
        System.Windows.Forms.Button m_go;

        System.Windows.Forms.ToolTip m_toolTip;

        const int k_ptButtonSize = 24;
        const float k_smallFontSize = 7.0f;
        const int k_comboBoxWidth = 110;
        const int k_smallPresetPatternWidth = 45;
        const int k_largePresetPatternWidth = 100;
        const int k_miscWidth = 55;
        const int k_trackBarWidth = 25;
        const int k_zfiWidth = 50;
        const int k_zfiButtonWidth = 30;
        const int k_presetPatternWidth = 275;

        const int k_groupBoxSideBuffer = 9;
        const int k_groupBoxTopBuffer = 14;

        const int k_largeBuffer = 5;
        const int k_smallBuffer = 3;

        const int k_panelHeight = 98;

        AvigilonDotNet.IEntityPtz m_ptz = null;

        public PtzPanelDiscrete()
        {
            m_toolTip = new System.Windows.Forms.ToolTip();

            m_ptzGroupBox = new System.Windows.Forms.GroupBox();
            m_ptzGroupBox.Text = "Discrete PTZ";

            m_leftLabel = new System.Windows.Forms.Label();
            m_leftLabel.AutoSize = true;
            m_leftLabel.Text = "Left:";

            m_left = new System.Windows.Forms.NumericUpDown();
            m_left.DecimalPlaces = 2;
            m_left.Value = new System.Decimal(0.0);
            m_left.Minimum = new System.Decimal(-10.0);
            m_left.Maximum = new System.Decimal(10.0);
            m_left.Increment = new System.Decimal(0.01);
            m_left.ValueChanged += new System.EventHandler(LeftValueChanged_);
            m_toolTip.SetToolTip(m_left, "Left");

            m_topLabel = new System.Windows.Forms.Label();
            m_topLabel.AutoSize = true;
            m_topLabel.Text = "Top:";

            m_top = new System.Windows.Forms.NumericUpDown();
            m_top.DecimalPlaces = 2;
            m_top.Value = new System.Decimal(0.0);
            m_top.Minimum = new System.Decimal(-10.0);
            m_top.Maximum = new System.Decimal(10.0);
            m_top.Increment = new System.Decimal(0.01);
            m_top.ValueChanged += new System.EventHandler(TopValueChanged_);
            m_toolTip.SetToolTip(m_left, "Top");

            m_rightLabel = new System.Windows.Forms.Label();
            m_rightLabel.AutoSize = true;
            m_rightLabel.Text = "Right:";

            m_right = new System.Windows.Forms.NumericUpDown();
            m_right.DecimalPlaces = 2;
            m_right.Value = new System.Decimal(1.0);
            m_right.Minimum = new System.Decimal(-10.0);
            m_right.Maximum = new System.Decimal(10.0);
            m_right.Increment = new System.Decimal(0.01);
            m_right.ValueChanged += new System.EventHandler(RightValueChanged_);
            m_toolTip.SetToolTip(m_right, "Right");

            m_bottomLabel = new System.Windows.Forms.Label();
            m_bottomLabel.AutoSize = true;
            m_bottomLabel.Text = "Bottom:";

            m_bottom = new System.Windows.Forms.NumericUpDown();
            m_bottom.DecimalPlaces = 2;
            m_bottom.Value = new System.Decimal(1.0);
            m_bottom.Minimum = new System.Decimal(-10.0);
            m_bottom.Maximum = new System.Decimal(10.0);
            m_bottom.Increment = new System.Decimal(0.01);
            m_bottom.ValueChanged += new System.EventHandler(BottomValueChanged_);
            m_toolTip.SetToolTip(m_bottom, "Right");

            m_go = new System.Windows.Forms.Button();
            m_go.MouseDown += new System.Windows.Forms.MouseEventHandler(OnGoPress_);
            m_go.Width = k_zfiButtonWidth;
            m_go.Text = "Go";
            m_go.Width = k_miscWidth;
            m_toolTip.SetToolTip(m_go, "Go");

            Controls.Add(m_leftLabel);
            Controls.Add(m_left);
            Controls.Add(m_topLabel);
            Controls.Add(m_top);
            Controls.Add(m_rightLabel);
            Controls.Add(m_right);
            Controls.Add(m_bottomLabel);
            Controls.Add(m_bottom);
            Controls.Add(m_go);
            Controls.Add(m_ptzGroupBox);

            m_ptzGroupBox.Location = new System.Drawing.Point(k_largeBuffer, k_smallBuffer);

            m_topLabel.Location = new System.Drawing.Point(
                m_ptzGroupBox.Left + k_groupBoxSideBuffer + m_leftLabel.Width + k_smallBuffer + m_left.Width + k_smallBuffer,
                m_ptzGroupBox.Top + k_groupBoxTopBuffer);
            m_top.Location = new System.Drawing.Point(
                m_topLabel.Right + k_smallBuffer,
                m_ptzGroupBox.Top + k_groupBoxTopBuffer);

            m_leftLabel.Location = new System.Drawing.Point(
                m_ptzGroupBox.Left + k_groupBoxSideBuffer,
                m_topLabel.Bottom + k_smallBuffer);
            m_left.Location = new System.Drawing.Point(
                m_leftLabel.Right + k_smallBuffer,
                m_topLabel.Bottom + k_smallBuffer);

            m_bottomLabel.Location = new System.Drawing.Point(
                m_ptzGroupBox.Left + k_groupBoxSideBuffer + m_leftLabel.Width + k_smallBuffer + m_left.Width + k_smallBuffer,
                m_leftLabel.Bottom + k_smallBuffer);
            m_bottom.Location = new System.Drawing.Point(
                m_bottomLabel.Right + k_smallBuffer,
                m_leftLabel.Bottom + k_smallBuffer);

            m_rightLabel.Location = new System.Drawing.Point(
                System.Math.Max(m_top.Right, m_bottom.Right) + k_smallBuffer,
                m_topLabel.Bottom + k_smallBuffer);
            m_right.Location = new System.Drawing.Point(
                m_rightLabel.Right + k_smallBuffer,
                m_topLabel.Bottom + k_smallBuffer);

            m_go.Location = new System.Drawing.Point(
                m_right.Right + k_groupBoxSideBuffer,
                m_ptzGroupBox.Top + k_groupBoxTopBuffer);

            m_ptzGroupBox.Size = new System.Drawing.Size(m_go.Right + k_smallBuffer, m_bottom.Bottom + k_smallBuffer);

            Height = m_ptzGroupBox.Bottom + k_smallBuffer;

            UpdateControls_();
        }

        void BottomValueChanged_(object sender, System.EventArgs e)
        {
            if (m_top.Value > m_bottom.Value)
            {
                decimal newVal = m_bottom.Value - m_top.Increment;
                if (newVal < m_top.Minimum) newVal = m_top.Minimum;

                m_top.Value = newVal;
            }
        }

        void RightValueChanged_(object sender, System.EventArgs e)
        {
            if (m_left.Value > m_right.Value)
            {
                decimal newVal = m_right.Value - m_left.Increment;
                if (newVal < m_left.Minimum) newVal = m_left.Minimum;

                m_left.Value = newVal;
            }
        }

        void TopValueChanged_(object sender, System.EventArgs e)
        {
            if (m_top.Value > m_bottom.Value)
            {
                decimal newVal = m_top.Value + m_bottom.Increment;
                if (newVal > m_bottom.Maximum) newVal = m_bottom.Maximum;

                m_bottom.Value = newVal;
            }
        }

        void LeftValueChanged_(object sender, System.EventArgs e)
        {
            if (m_left.Value > m_right.Value)
            {
                decimal newVal = m_left.Value + m_right.Increment;
                if (newVal > m_right.Maximum) newVal = m_right.Maximum;

                m_right.Value = newVal;
            }
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
            bool bHasPtzDiscrete = false;
            if (m_ptz != null)
            {
                AvigilonDotNet.PtzCapapabilities caps = m_ptz.Capabilities;
                bHasPtzDiscrete =
                    (caps & AvigilonDotNet.PtzCapapabilities.HasRelativeFovPanTilt) != 0 &&
                    (caps & AvigilonDotNet.PtzCapapabilities.HasRelativeFovZoom) != 0;
            }

            m_left.Enabled = bHasPtzDiscrete;
            m_top.Enabled = bHasPtzDiscrete;
            m_right.Enabled = bHasPtzDiscrete;
            m_bottom.Enabled = bHasPtzDiscrete;
            m_go.Enabled = bHasPtzDiscrete;
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

            System.Drawing.RectangleF rect = new System.Drawing.RectangleF(
                (float)m_left.Value,
                (float)m_top.Value,
                (float)(m_right.Value - m_left.Value),
                (float)(m_bottom.Value - m_top.Value));
            CheckError_(m_ptz.PtzRelativeFov(rect));
        }
    }
}
