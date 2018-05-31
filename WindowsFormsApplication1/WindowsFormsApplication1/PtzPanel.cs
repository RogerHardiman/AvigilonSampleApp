using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WindowsFormsApplication1.Properties;

namespace WindowsFormsApplication1
{
    class PtzPresetInfo
    {
        public PtzPresetInfo(
            string name_,
            uint index_)
        {
            name = name_;
            index = index_;
        }

        public override string ToString()
        {
            return string.Format("{0}. " + name, index);
        }

        public string name;
        public uint index;
    }


    class PtzPanel
        : System.Windows.Forms.Panel
    {
        // Pan/tilt controls
        System.Windows.Forms.GroupBox m_panTiltGroupBox;
        System.Windows.Forms.Button m_ptLeft;
        System.Windows.Forms.Button m_ptUp;
        System.Windows.Forms.Button m_ptRight;
        System.Windows.Forms.Button m_ptDown;
        System.Windows.Forms.Button m_ptLeftUp;
        System.Windows.Forms.Button m_ptLeftDown;
        System.Windows.Forms.Button m_ptRightUp;
        System.Windows.Forms.Button m_ptRightDown;
        System.Windows.Forms.Label m_ptSpeedLabel;
        System.Windows.Forms.TrackBar m_ptSpeed;

        // Zoom controls
        System.Windows.Forms.GroupBox m_zoomGroupBox;
        System.Windows.Forms.Button m_zoomIn;
        System.Windows.Forms.Button m_zoomOut;

        // Focus controls
        System.Windows.Forms.GroupBox m_focusGroupBox;
        System.Windows.Forms.Button m_focusNear;
        System.Windows.Forms.Button m_focusFar;

        // Iris controls
        System.Windows.Forms.GroupBox m_irisGroupBox;
        System.Windows.Forms.Button m_irisOpen;
        System.Windows.Forms.Button m_irisClose;

        // Preset controls
        System.Windows.Forms.GroupBox m_presetGroupBox;
        System.Windows.Forms.ComboBox m_presetComboBox;
        System.Windows.Forms.Button m_presetSet;
        System.Windows.Forms.Button m_presetClear;
        System.Windows.Forms.Button m_presetGoTo;

        // Pattern controls
        System.Windows.Forms.GroupBox m_patternGroupBox;
        System.Windows.Forms.ComboBox m_patternComboBox;
        System.Windows.Forms.Button m_patternStartStopRecord;
        System.Windows.Forms.Button m_patternRun;
        bool m_bPatternRecording = false;

        // Misc controls
        System.Windows.Forms.GroupBox m_miscGroupBox;
        System.Windows.Forms.Button m_menu;
        System.Windows.Forms.Button m_lock;

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

        public PtzPanel()
        {
            m_toolTip = new System.Windows.Forms.ToolTip();

            m_panTiltGroupBox = new System.Windows.Forms.GroupBox();
            m_panTiltGroupBox.Text = "Pan/Tilt";

            m_ptLeft = new System.Windows.Forms.Button();
            m_ptLeft.MouseDown += new System.Windows.Forms.MouseEventHandler(OnPtLeftPress_);
            m_ptLeft.MouseUp += new System.Windows.Forms.MouseEventHandler(OnPtRelease_);
            m_ptLeft.Size = new System.Drawing.Size(k_ptButtonSize, k_ptButtonSize);
            m_ptLeft.Image = Resources.Left;
            m_toolTip.SetToolTip(m_ptLeft, "Pan Left");

            m_ptUp = new System.Windows.Forms.Button();
            m_ptUp.MouseDown += new System.Windows.Forms.MouseEventHandler(OnPtUpPress_);
            m_ptUp.MouseUp += new System.Windows.Forms.MouseEventHandler(OnPtRelease_);
            m_ptUp.Size = new System.Drawing.Size(k_ptButtonSize, k_ptButtonSize);
            m_ptUp.Image = Resources.Up;
            m_toolTip.SetToolTip(m_ptUp, "Tilt Up");

            m_ptRight = new System.Windows.Forms.Button();
            m_ptRight.MouseDown += new System.Windows.Forms.MouseEventHandler(OnPtRightPress_);
            m_ptRight.MouseUp += new System.Windows.Forms.MouseEventHandler(OnPtRelease_);
            m_ptRight.Size = new System.Drawing.Size(k_ptButtonSize, k_ptButtonSize);
            m_ptRight.Image = Resources.Right;
            m_toolTip.SetToolTip(m_ptRight, "Pan Right");

            m_ptDown = new System.Windows.Forms.Button();
            m_ptDown.MouseDown += new System.Windows.Forms.MouseEventHandler(OnPtDownPress_);
            m_ptDown.MouseUp += new System.Windows.Forms.MouseEventHandler(OnPtRelease_);
            m_ptDown.Size = new System.Drawing.Size(k_ptButtonSize, k_ptButtonSize);
            m_ptDown.Image = Resources.Down;
            m_toolTip.SetToolTip(m_ptDown, "Tilt Down");

            m_ptLeftUp = new System.Windows.Forms.Button();
            m_ptLeftUp.MouseDown += new System.Windows.Forms.MouseEventHandler(OnPtLeftUpPress_);
            m_ptLeftUp.MouseUp += new System.Windows.Forms.MouseEventHandler(OnPtRelease_);
            m_ptLeftUp.Size = new System.Drawing.Size(k_ptButtonSize, k_ptButtonSize);
            m_ptLeftUp.Image = Resources.LeftUp;
            m_toolTip.SetToolTip(m_ptLeftUp, "Pan Up/Left");

            m_ptLeftDown = new System.Windows.Forms.Button();
            m_ptLeftDown.MouseDown += new System.Windows.Forms.MouseEventHandler(OnPtLeftDownPress_);
            m_ptLeftDown.MouseUp += new System.Windows.Forms.MouseEventHandler(OnPtRelease_);
            m_ptLeftDown.Size = new System.Drawing.Size(k_ptButtonSize, k_ptButtonSize);
            m_ptLeftDown.Image = Resources.LeftDown;
            m_toolTip.SetToolTip(m_ptLeftDown, "Pan Down/Left");

            m_ptRightUp = new System.Windows.Forms.Button();
            m_ptRightUp.MouseDown += new System.Windows.Forms.MouseEventHandler(OnPtRightUpPress_);
            m_ptRightUp.MouseUp += new System.Windows.Forms.MouseEventHandler(OnPtRelease_);
            m_ptRightUp.Size = new System.Drawing.Size(k_ptButtonSize, k_ptButtonSize);
            m_ptRightUp.Image = Resources.RightUp;
            m_toolTip.SetToolTip(m_ptRightUp, "Pan Up/Right");

            m_ptRightDown = new System.Windows.Forms.Button();
            m_ptRightDown.MouseDown += new System.Windows.Forms.MouseEventHandler(OnPtRightDownPress_);
            m_ptRightDown.MouseUp += new System.Windows.Forms.MouseEventHandler(OnPtRelease_);
            m_ptRightDown.Size = new System.Drawing.Size(k_ptButtonSize, k_ptButtonSize);
            m_ptRightDown.Image = Resources.RightDown;
            m_toolTip.SetToolTip(m_ptRightDown, "Pan Down/right");

            m_ptSpeedLabel = new System.Windows.Forms.Label();
            m_ptSpeedLabel.Text = "Speed";
            m_ptSpeedLabel.AutoSize = true;
            m_ptSpeedLabel.Font = new System.Drawing.Font(
                System.Drawing.SystemFonts.DialogFont.FontFamily,
                k_smallFontSize);

            m_ptSpeed = new System.Windows.Forms.TrackBar();
            m_ptSpeed.AutoSize = false;
            m_ptSpeed.Orientation = System.Windows.Forms.Orientation.Vertical;
            m_ptSpeed.Padding = new System.Windows.Forms.Padding(0);
            m_ptSpeed.Margin = new System.Windows.Forms.Padding(0);
            m_ptSpeed.Minimum = 0;
            m_ptSpeed.Maximum = 100;
            m_ptSpeed.Value = 50;
            m_toolTip.SetToolTip(m_ptSpeed, "Pan/Tilt Speed");

            m_zoomGroupBox = new System.Windows.Forms.GroupBox();
            m_zoomGroupBox.Text = "Zoom";

            m_zoomIn = new System.Windows.Forms.Button();
            m_zoomIn.MouseDown += new System.Windows.Forms.MouseEventHandler(OnZoomInPress_);
            m_zoomIn.MouseUp += new System.Windows.Forms.MouseEventHandler(OnZoomRelease_);
            m_zoomIn.Width = k_zfiButtonWidth;
            m_zoomIn.Image = Resources.Plus;
            m_toolTip.SetToolTip(m_zoomIn, "Zoom In");

            m_zoomOut = new System.Windows.Forms.Button();
            m_zoomOut.MouseDown += new System.Windows.Forms.MouseEventHandler(OnZoomOutPress_);
            m_zoomOut.MouseUp += new System.Windows.Forms.MouseEventHandler(OnZoomRelease_);
            m_zoomOut.Width = k_zfiButtonWidth;
            m_zoomOut.Image = Resources.Minus;
            m_toolTip.SetToolTip(m_zoomOut, "Zoom Out");

            m_focusGroupBox = new System.Windows.Forms.GroupBox();
            m_focusGroupBox.Text = "Focus";

            m_focusNear = new System.Windows.Forms.Button();
            m_focusNear.MouseDown += new System.Windows.Forms.MouseEventHandler(OnFocusNearPress_);
            m_focusNear.MouseUp += new System.Windows.Forms.MouseEventHandler(OnFocusRelease_);
            m_focusNear.Width = k_zfiButtonWidth;
            m_focusNear.Image = Resources.Plus;
            m_toolTip.SetToolTip(m_focusNear, "Focus Near");

            m_focusFar = new System.Windows.Forms.Button();
            m_focusFar.MouseDown += new System.Windows.Forms.MouseEventHandler(OnFocusFarPress_);
            m_focusFar.MouseUp += new System.Windows.Forms.MouseEventHandler(OnFocusRelease_);
            m_focusFar.Width = k_zfiButtonWidth;
            m_focusFar.Image = Resources.Minus;
            m_toolTip.SetToolTip(m_focusFar, "Focus Far");

            m_irisGroupBox = new System.Windows.Forms.GroupBox();
            m_irisGroupBox.Text = "Iris";

            m_irisOpen = new System.Windows.Forms.Button();
            m_irisOpen.MouseDown += new System.Windows.Forms.MouseEventHandler(OnIrisOpenPress_);
            m_irisOpen.MouseUp += new System.Windows.Forms.MouseEventHandler(OnIrisRelease_);
            m_irisOpen.Width = k_zfiButtonWidth;
            //m_irisOpen.Image = Properties.Resources.Plus;
            m_toolTip.SetToolTip(m_irisOpen, "Open Iris");

            m_irisClose = new System.Windows.Forms.Button();
            m_irisClose.MouseDown += new System.Windows.Forms.MouseEventHandler(OnIrisClosePress_);
            m_irisClose.MouseUp += new System.Windows.Forms.MouseEventHandler(OnIrisRelease_);
            m_irisClose.Width = k_zfiButtonWidth;
            m_irisClose.Image = Resources.Minus;
            m_toolTip.SetToolTip(m_irisClose, "Close Iris");

            m_presetGroupBox = new System.Windows.Forms.GroupBox();
            m_presetGroupBox.Text = "Presets";

            m_presetComboBox = new System.Windows.Forms.ComboBox();
            m_presetComboBox.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            m_presetComboBox.Width = k_comboBoxWidth;

            m_presetSet = new System.Windows.Forms.Button();
            m_presetSet.Click += new System.EventHandler(OnPresetSet_);
            m_presetSet.Text = "Set";
            m_presetSet.Width = k_smallPresetPatternWidth;
            m_toolTip.SetToolTip(m_presetSet, "Set Preset to Current Location");

            m_presetClear = new System.Windows.Forms.Button();
            m_presetClear.Click += new System.EventHandler(OnPresetClear_);
            m_presetClear.Text = "Clear";
            m_presetClear.Width = k_smallPresetPatternWidth;
            m_toolTip.SetToolTip(m_presetClear, "Clear Preset");

            m_presetGoTo = new System.Windows.Forms.Button();
            m_presetGoTo.Click += new System.EventHandler(OnPresetGoTo_);
            m_presetGoTo.Text = "Go";
            m_presetGoTo.Width = k_smallPresetPatternWidth;
            m_toolTip.SetToolTip(m_presetGoTo, "Go to Preset");

            m_patternGroupBox = new System.Windows.Forms.GroupBox();
            m_patternGroupBox.Text = "Patterns";

            m_patternComboBox = new System.Windows.Forms.ComboBox();
            m_patternComboBox.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            m_patternComboBox.Width = k_comboBoxWidth;

            m_patternStartStopRecord = new System.Windows.Forms.Button();
            m_patternStartStopRecord.Click += new System.EventHandler(OnPatternStartStopRecord_);
            m_patternStartStopRecord.Text = "Start Recording";
            m_patternStartStopRecord.Width = k_largePresetPatternWidth;
            m_toolTip.SetToolTip(m_patternStartStopRecord, "Record Pattern");

            m_patternRun = new System.Windows.Forms.Button();
            m_patternRun.Click += new System.EventHandler(OnPatternRun_);
            m_patternRun.Text = "Run";
            m_patternRun.Width = k_smallPresetPatternWidth;
            m_toolTip.SetToolTip(m_patternRun, "Run Pattern");

            m_miscGroupBox = new System.Windows.Forms.GroupBox();
            m_miscGroupBox.Text = "Misc";

            m_menu = new System.Windows.Forms.Button();
            m_menu.Click += new System.EventHandler(OnMenu_);
            m_menu.Text = "Menu";
            m_menu.Width = k_miscWidth;
            m_toolTip.SetToolTip(m_menu, "Show OSD Menu");

            m_lock = new System.Windows.Forms.Button();
            m_lock.Click += new System.EventHandler(OnLock_);
            m_lock.Text = "Lock";
            m_lock.Width = k_miscWidth;
            m_toolTip.SetToolTip(m_lock, "Lock PTZ Controls");

            Controls.Add(m_ptLeft);
            Controls.Add(m_ptUp);
            Controls.Add(m_ptRight);
            Controls.Add(m_ptDown);
            Controls.Add(m_ptLeftUp);
            Controls.Add(m_ptLeftDown);
            Controls.Add(m_ptRightUp);
            Controls.Add(m_ptRightDown);
            Controls.Add(m_ptSpeedLabel);
            Controls.Add(m_ptSpeed);
            Controls.Add(m_panTiltGroupBox);

            Controls.Add(m_zoomIn);
            Controls.Add(m_zoomOut);
            Controls.Add(m_zoomGroupBox);

            Controls.Add(m_focusNear);
            Controls.Add(m_focusFar);
            Controls.Add(m_focusGroupBox);

            Controls.Add(m_irisOpen);
            Controls.Add(m_irisClose);
            Controls.Add(m_irisGroupBox);

            Controls.Add(m_presetComboBox);
            Controls.Add(m_presetSet);
            Controls.Add(m_presetClear);
            Controls.Add(m_presetGoTo);
            Controls.Add(m_presetGroupBox);

            Controls.Add(m_patternComboBox);
            Controls.Add(m_patternStartStopRecord);
            Controls.Add(m_patternRun);
            Controls.Add(m_patternGroupBox);

            Controls.Add(m_menu);
            Controls.Add(m_lock);
            Controls.Add(m_miscGroupBox);

            m_panTiltGroupBox.Location = new System.Drawing.Point(k_largeBuffer, k_smallBuffer);
            m_ptLeftUp.Location = new System.Drawing.Point(
                m_panTiltGroupBox.Left + k_groupBoxSideBuffer,
                m_panTiltGroupBox.Top + k_groupBoxTopBuffer);
            m_ptLeft.Location = new System.Drawing.Point(m_ptLeftUp.Left, m_ptLeftUp.Bottom);
            m_ptLeftDown.Location = new System.Drawing.Point(m_ptLeftUp.Left, m_ptLeft.Bottom);

            m_ptUp.Location = new System.Drawing.Point(m_ptLeftUp.Right, m_ptLeftUp.Top);
            m_ptDown.Location = new System.Drawing.Point(m_ptUp.Left, m_ptLeftDown.Top);

            m_ptRightUp.Location = new System.Drawing.Point(m_ptUp.Right, m_ptUp.Top);
            m_ptRight.Location = new System.Drawing.Point(m_ptRightUp.Left, m_ptRightUp.Bottom);
            m_ptRightDown.Location = new System.Drawing.Point(m_ptRight.Left, m_ptRight.Bottom);

            m_ptSpeedLabel.Location = new System.Drawing.Point(
                m_ptRight.Right,
                m_ptUp.Top);
            m_ptSpeed.Location = new System.Drawing.Point(m_ptRight.Right + k_smallBuffer, m_ptSpeedLabel.Bottom - 8);
            m_ptSpeed.Size = new System.Drawing.Size(
                k_trackBarWidth,
                m_ptDown.Bottom - m_ptSpeed.Top);

            m_panTiltGroupBox.Size = new System.Drawing.Size(m_ptSpeed.Right + k_smallBuffer, m_ptRightDown.Bottom + k_smallBuffer);

            m_zoomGroupBox.Bounds = new System.Drawing.Rectangle(
                m_panTiltGroupBox.Right + k_smallBuffer,
                m_panTiltGroupBox.Top,
                k_zfiWidth,
                m_panTiltGroupBox.Height);
            m_zoomIn.Location = new System.Drawing.Point(
                m_zoomGroupBox.Left + ((m_zoomGroupBox.Width - m_zoomIn.Width) / 2),
                m_panTiltGroupBox.Top + 22);
            m_zoomOut.Location = new System.Drawing.Point(m_zoomIn.Left, m_panTiltGroupBox.Top + 53);

            m_focusGroupBox.Bounds = new System.Drawing.Rectangle(
                m_zoomGroupBox.Right + 3,
                m_zoomGroupBox.Top,
                k_zfiWidth,
                m_zoomGroupBox.Height);
            m_focusNear.Location = new System.Drawing.Point(
                m_focusGroupBox.Left + ((m_focusGroupBox.Width - m_focusNear.Width) / 2),
                m_zoomIn.Top);
            m_focusFar.Location = new System.Drawing.Point(m_focusNear.Left, m_zoomOut.Top);

            m_irisGroupBox.Bounds = new System.Drawing.Rectangle(
                m_focusGroupBox.Right + 3,
                m_focusGroupBox.Top,
                k_zfiWidth,
                m_focusGroupBox.Height);
            m_irisOpen.Location = new System.Drawing.Point(
                m_irisGroupBox.Left + ((m_irisGroupBox.Width - m_irisOpen.Width) / 2),
                m_zoomIn.Top);
            m_irisClose.Location = new System.Drawing.Point(m_irisOpen.Left, m_zoomOut.Top);

            m_presetGroupBox.Bounds = new System.Drawing.Rectangle(
                m_irisGroupBox.Right + 3,
                m_irisGroupBox.Top,
                k_presetPatternWidth,
                (m_irisGroupBox.Height - 6) / 2);
            m_presetComboBox.Location = new System.Drawing.Point(
                m_presetGroupBox.Left + 5,
                m_presetGroupBox.Top + 14);
            m_presetGoTo.Location = new System.Drawing.Point(
                m_presetComboBox.Right + 5,
                m_presetComboBox.Top + ((m_presetComboBox.Height - m_presetGoTo.Height) / 2));
            m_presetSet.Location = new System.Drawing.Point(
                m_presetGoTo.Right + 5,
                m_presetGoTo.Top);
            m_presetClear.Location = new System.Drawing.Point(
                m_presetSet.Right + 5,
                m_presetGoTo.Top);

            m_patternGroupBox.Bounds = new System.Drawing.Rectangle(
                m_irisGroupBox.Right + 3,
                m_presetGroupBox.Bottom + 4,
                m_presetGroupBox.Width,
                m_irisGroupBox.Bottom - m_presetGroupBox.Bottom - 4);
            m_patternComboBox.Location = new System.Drawing.Point(
                m_patternGroupBox.Left + 5,
                m_patternGroupBox.Top + 14);
            m_patternRun.Location = new System.Drawing.Point(
                m_patternComboBox.Right + 5,
                m_patternComboBox.Top + ((m_patternComboBox.Height - m_patternRun.Height) / 2));
            m_patternStartStopRecord.Location = new System.Drawing.Point(
                m_patternRun.Right + 5,
                m_patternRun.Top);

            m_miscGroupBox.Bounds = new System.Drawing.Rectangle(
                m_patternGroupBox.Right + 5,
                m_presetGroupBox.Top,
                65,
                m_patternGroupBox.Bottom - m_presetGroupBox.Top);

            m_menu.Location = new System.Drawing.Point(
                m_miscGroupBox.Left + 5,
                m_irisOpen.Top);
            m_lock.Location = new System.Drawing.Point(
                m_menu.Left,
                m_irisClose.Top);

            Height = k_panelHeight;

            UpdateControls_();
            UpdateLockStatus_();
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
            bool bAnyPtz = false;
            bool bPanTilt = false;
            bool bPanTiltDiagonal = false;
            bool bZoom = false;
            bool bFocus = false;
            bool bIris = false;
            bool bPresets = false;
            bool bPatterns = false;
            bool bMenu = false;

            if (m_ptz != null)
            {
                AvigilonDotNet.PtzCapapabilities caps = m_ptz.Capabilities;
                bPanTilt = (caps & AvigilonDotNet.PtzCapapabilities.HasPanTilt) != 0;
                bPanTiltDiagonal = (caps & AvigilonDotNet.PtzCapapabilities.HasSimPanTilt) != 0;
                bZoom = (caps & AvigilonDotNet.PtzCapapabilities.HasZoomControl) != 0;
                bFocus = (caps & AvigilonDotNet.PtzCapapabilities.HasFocusControl) != 0;
                bIris = (caps & AvigilonDotNet.PtzCapapabilities.HasIrisControl) != 0;
                bPresets = (caps & AvigilonDotNet.PtzCapapabilities.HasPresets) != 0 && m_ptz.Presets.Count != 0;
                bPatterns = (caps & AvigilonDotNet.PtzCapapabilities.HasPatterns) != 0;
                bMenu = (caps & AvigilonDotNet.PtzCapapabilities.HasMenu) != 0;

                bAnyPtz =
                    bPanTilt ||
                    bPanTiltDiagonal ||
                    bZoom ||
                    bFocus ||
                    bIris ||
                    bPresets ||
                    bPatterns ||
                    bMenu;
            }

            m_ptLeft.Enabled = bPanTilt;
            m_ptUp.Enabled = bPanTilt;
            m_ptRight.Enabled = bPanTilt;
            m_ptDown.Enabled = bPanTilt;
            m_ptLeftUp.Enabled = bPanTiltDiagonal;
            m_ptLeftDown.Enabled = bPanTiltDiagonal;
            m_ptRightUp.Enabled = bPanTiltDiagonal;
            m_ptRightDown.Enabled = bPanTiltDiagonal;
            m_zoomIn.Enabled = bZoom;
            m_zoomOut.Enabled = bZoom;
            m_irisOpen.Enabled = bIris;
            m_irisClose.Enabled = bIris;
            m_focusNear.Enabled = bFocus;
            m_focusFar.Enabled = bFocus;
            m_presetComboBox.Enabled = bPresets;
            m_presetSet.Enabled = bPresets;
            m_presetClear.Enabled = bPresets;
            m_presetGoTo.Enabled = bPresets;
            m_patternComboBox.Enabled = bPatterns;
            m_patternStartStopRecord.Enabled = bPatterns;
            m_patternRun.Enabled = bPatterns;
            m_menu.Enabled = bMenu;
            m_lock.Enabled = bAnyPtz;

            m_presetComboBox.BeginUpdate();
            m_presetComboBox.Items.Clear();


            if (bPresets)
            {
                foreach (AvigilonDotNet.PtzPreset preset in m_ptz.Presets)
                {
                    m_presetComboBox.Items.Add(new PtzPresetInfo(preset.name, preset.index));
                }
                m_presetComboBox.SelectedIndex = 0;
            }

            m_presetComboBox.EndUpdate();

            m_patternComboBox.BeginUpdate();
            m_patternComboBox.Items.Clear();

            if (bPatterns)
            {
                for (byte i = byte.MinValue; i < byte.MaxValue; ++i)
                    m_patternComboBox.Items.Add(i.ToString());
                m_patternComboBox.SelectedIndex = 0;
            }

            m_patternComboBox.EndUpdate();
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

        float PtSpeed_()
        {
            return (float)m_ptSpeed.Value / 100.0f;
        }

        void OnPtLeftPress_(System.Object sender, System.Windows.Forms.MouseEventArgs e)
        {
            if (m_ptz == null) return;
            CheckError_(m_ptz.MovePanTilt(AvigilonDotNet.PanTiltDirection.Left, PtSpeed_()));
        }

        void OnPtUpPress_(System.Object sender, System.Windows.Forms.MouseEventArgs e)
        {
            if (m_ptz == null) return;
            CheckError_(m_ptz.MovePanTilt(AvigilonDotNet.PanTiltDirection.Up, PtSpeed_()));
        }

        void OnPtRightPress_(System.Object sender, System.Windows.Forms.MouseEventArgs e)
        {
            if (m_ptz == null) return;
            CheckError_(m_ptz.MovePanTilt(AvigilonDotNet.PanTiltDirection.Right, PtSpeed_()));
        }

        void OnPtDownPress_(System.Object sender, System.Windows.Forms.MouseEventArgs e)
        {
            if (m_ptz == null) return;
            CheckError_(m_ptz.MovePanTilt(AvigilonDotNet.PanTiltDirection.Down, PtSpeed_()));
        }

        void OnPtLeftUpPress_(System.Object sender, System.Windows.Forms.MouseEventArgs e)
        {
            if (m_ptz == null) return;
            CheckError_(m_ptz.MovePanTilt(AvigilonDotNet.PanTiltDirection.UpLeft, PtSpeed_()));
        }

        void OnPtLeftDownPress_(System.Object sender, System.Windows.Forms.MouseEventArgs e)
        {
            if (m_ptz == null) return;
            CheckError_(m_ptz.MovePanTilt(AvigilonDotNet.PanTiltDirection.DownLeft, PtSpeed_()));
        }

        void OnPtRightUpPress_(System.Object sender, System.Windows.Forms.MouseEventArgs e)
        {
            if (m_ptz == null) return;
            CheckError_(m_ptz.MovePanTilt(AvigilonDotNet.PanTiltDirection.UpRight, PtSpeed_()));
        }

        void OnPtRightDownPress_(System.Object sender, System.Windows.Forms.MouseEventArgs e)
        {
            if (m_ptz == null) return;
            CheckError_(m_ptz.MovePanTilt(AvigilonDotNet.PanTiltDirection.DownRight, PtSpeed_()));
        }

        void OnPtRelease_(System.Object sender, System.Windows.Forms.MouseEventArgs e)
        {
            if (m_ptz == null) return;
            m_ptz.StopPanTilt();
        }

        void OnZoomInPress_(System.Object sender, System.Windows.Forms.MouseEventArgs e)
        {
            if (m_ptz == null) return;
            CheckError_(m_ptz.MoveZoom(true));
        }

        void OnZoomOutPress_(System.Object sender, System.Windows.Forms.MouseEventArgs e)
        {
            if (m_ptz == null) return;
            CheckError_(m_ptz.MoveZoom(false));
        }

        void OnZoomRelease_(System.Object sender, System.Windows.Forms.MouseEventArgs e)
        {
            if (m_ptz == null) return;
            m_ptz.StopZoom();
        }

        void OnFocusNearPress_(System.Object sender, System.Windows.Forms.MouseEventArgs e)
        {
            if (m_ptz == null) return;
            CheckError_(m_ptz.MoveFocus(true));
        }

        void OnFocusFarPress_(System.Object sender, System.Windows.Forms.MouseEventArgs e)
        {
            if (m_ptz == null) return;
            CheckError_(m_ptz.MoveFocus(false));
        }

        void OnFocusRelease_(System.Object sender, System.Windows.Forms.MouseEventArgs e)
        {
            if (m_ptz == null) return;
            m_ptz.StopFocus();
        }

        void OnIrisOpenPress_(System.Object sender, System.Windows.Forms.MouseEventArgs e)
        {
            if (m_ptz == null) return;
            CheckError_(m_ptz.MoveIris(true));
        }

        void OnIrisClosePress_(System.Object sender, System.Windows.Forms.MouseEventArgs e)
        {
            if (m_ptz == null) return;
            CheckError_(m_ptz.MoveIris(false));
        }

        void OnIrisRelease_(System.Object sender, System.Windows.Forms.MouseEventArgs e)
        {
            if (m_ptz == null) return;
            m_ptz.StopIris();
        }

        bool DoPresetAction_(AvigilonDotNet.PtzPresetAction action)
        {
            if (m_ptz == null ||
                m_presetComboBox.SelectedIndex < 0)
                return false;

            PtzPresetInfo selPreset = m_presetComboBox.SelectedItem as PtzPresetInfo;
            if (selPreset == null)
                return false;

            return CheckError_(m_ptz.DoPtzPresetAction(action, (byte)selPreset.index));
        }

        void OnPresetSet_(System.Object sender, System.EventArgs e)
        {
            DoPresetAction_(AvigilonDotNet.PtzPresetAction.PresetSet);
        }

        void OnPresetClear_(System.Object sender, System.EventArgs e)
        {
            DoPresetAction_(AvigilonDotNet.PtzPresetAction.PresetClear);
        }

        void OnPresetGoTo_(System.Object sender, System.EventArgs e)
        {
            DoPresetAction_(AvigilonDotNet.PtzPresetAction.PresetGoto);
        }

        bool DoPatternAction_(AvigilonDotNet.PtzPatternAction action)
        {
            if (m_ptz == null ||
                m_patternComboBox.SelectedIndex < 0)
                return false;

            byte idx = 0;
            if (!System.Byte.TryParse(m_patternComboBox.SelectedItem.ToString(), out idx))
                return false;

            return CheckError_(m_ptz.DoPtzPatternAction(action, idx));
        }

        void OnPatternStartStopRecord_(System.Object sender, System.EventArgs e)
        {
            if (m_bPatternRecording)
            {
                if (DoPatternAction_(AvigilonDotNet.PtzPatternAction.StopRecord))
                {
                    m_patternStartStopRecord.Text = "Start Recording";
                    m_bPatternRecording = false;
                    m_toolTip.SetToolTip(m_patternStartStopRecord, "Record Pattern");
                }
            }
            else
            {
                if (DoPatternAction_(AvigilonDotNet.PtzPatternAction.StartRecord))
                {
                    m_patternStartStopRecord.Text = "Stop Recording";
                    m_bPatternRecording = true;
                    m_toolTip.SetToolTip(m_patternStartStopRecord, "Stop Recording Pattern");
                }
            }
        }

        void OnPatternRun_(System.Object sender, System.EventArgs e)
        {
            DoPatternAction_(AvigilonDotNet.PtzPatternAction.Run);
        }

        void OnMenu_(System.Object sender, System.EventArgs e)
        {
            if (m_ptz == null)
                return;

            CheckError_(m_ptz.DisplayOSD());
        }

        void OnLock_(System.Object sender, System.EventArgs e)
        {
            if (m_ptz == null)
                return;

            if (!m_ptz.HasPtzControlsLock)
            {
                if (!m_ptz.AcquirePtzControlsLock())
                {
                    System.Windows.Forms.MessageBox.Show(
                        "Could not lock PTZ controls.\n" +
                        "The controls may be locked by another user.");
                }
            }
            else
            {
                m_ptz.ReleasePtzControlsLock();
            }

            UpdateLockStatus_();
        }

        void UpdateLockStatus_()
        {
            if (m_ptz == null)
                return;

            if (m_ptz.HasPtzControlsLock)
            {
                m_lock.Text = "Unlock";
                m_toolTip.SetToolTip(m_lock, "Unlock PTZ Controls");
            }
            else
            {
                m_lock.Text = "Lock";
                m_toolTip.SetToolTip(m_lock, "Lock PTZ Controls");
            }
        }

    }
}
