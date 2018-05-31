using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WindowsFormsApplication1.Properties;

namespace WindowsFormsApplication1
{
    class ViewerPanel: System.Windows.Forms.Panel
    {
        AvigilonDotNet.ImagePanel m_imagePanel;
        AvigilonDotNet.IEntityCamera m_entityCamera;
        AvigilonDotNet.IEntityPtz m_entityPtz;
        AvigilonDotNet.IStreamWindow m_streamWindow;
        AvigilonDotNet.IStreamGroup m_streamGroup;

        PlaybackPanel m_playbackPanel;
        PtzPanel m_ptzPanel;
        PtzPanelContinuous m_ptzPanelContinuous = null; // only set in debug
        PtzPanelDiscrete m_ptzPanelDiscrete = null; // only set in debug
        PtzPanelRelative m_ptzPanelRelative = null; // only set in debug

        // Toolbar controls
        System.Windows.Forms.ToolStrip m_toolStrip;
        System.Windows.Forms.ToolStripButton m_panButton;
        System.Windows.Forms.ToolStripButton m_zoomInButton;
        System.Windows.Forms.ToolStripButton m_zoomOutButton;
        System.Windows.Forms.ToolStripButton m_fullScreenButton;

        System.Windows.Forms.FormWindowState m_originalState;

        System.Drawing.Bitmap m_avigilonLogo;

        public ViewerPanel()
        {
            // buffering settings
            SetStyle(System.Windows.Forms.ControlStyles.OptimizedDoubleBuffer, true);
            SetStyle(System.Windows.Forms.ControlStyles.UserPaint, true);
            SetStyle(System.Windows.Forms.ControlStyles.AllPaintingInWmPaint, true);

            m_avigilonLogo = Resources.App.ToBitmap();

            m_imagePanel = new AvigilonDotNet.ImagePanel();
            m_imagePanel.Visible = false;
            m_imagePanel.DefaultMouseAction = AvigilonDotNet.ImagePanelMouseAction.Pan;
            m_imagePanel.MouseDown += new System.Windows.Forms.MouseEventHandler(ImagePanelMouseDown_);

            m_toolStrip = new System.Windows.Forms.ToolStrip();
            m_toolStrip.GripStyle = System.Windows.Forms.ToolStripGripStyle.Hidden;

            m_panButton = new System.Windows.Forms.ToolStripButton("Pan", Resources.Pan);
            m_panButton.Checked = true;
            m_panButton.Click += new System.EventHandler(OnPanButtonClick_);

            m_zoomInButton = new System.Windows.Forms.ToolStripButton("Zoom In", Resources.ZoomIn);
            m_zoomInButton.Click += new System.EventHandler(OnZoomInButtonClick_);

            m_zoomOutButton = new System.Windows.Forms.ToolStripButton("Zoom Out", Resources.ZoomOut);
            m_zoomOutButton.Click += new System.EventHandler(OnZoomOutButtonClick_);

            m_fullScreenButton = new System.Windows.Forms.ToolStripButton("Full Screen", Resources.FullScreen);
            m_fullScreenButton.Click += new System.EventHandler(OnFullScreenClick_);

            m_toolStrip.Items.Add(m_panButton);
            m_toolStrip.Items.Add(new System.Windows.Forms.ToolStripSeparator());
            m_toolStrip.Items.Add(m_zoomInButton);
            m_toolStrip.Items.Add(new System.Windows.Forms.ToolStripSeparator());
            m_toolStrip.Items.Add(m_zoomOutButton);
            m_toolStrip.Items.Add(new System.Windows.Forms.ToolStripSeparator());
            m_toolStrip.Items.Add(m_fullScreenButton);

            m_playbackPanel = new PlaybackPanel();
            m_playbackPanel.Visible = false;

            m_ptzPanel = new PtzPanel();
            m_ptzPanel.Visible = false;

#if DEBUG
            m_ptzPanelContinuous = new PtzPanelContinuous();
            m_ptzPanelContinuous.Visible = false;

            m_ptzPanelDiscrete = new PtzPanelDiscrete();
            m_ptzPanelDiscrete.Visible = false;

            m_ptzPanelRelative = new PtzPanelRelative();
            m_ptzPanelRelative.Visible = false;
#endif

            Controls.Add(m_toolStrip);
            Controls.Add(m_imagePanel);
            Controls.Add(m_playbackPanel);
            Controls.Add(m_ptzPanel);

            if (m_ptzPanelContinuous != null)
                Controls.Add(m_ptzPanelContinuous);

            if (m_ptzPanelDiscrete != null)
                Controls.Add(m_ptzPanelDiscrete);

            if (m_ptzPanelRelative != null)
                Controls.Add(m_ptzPanelRelative);
        }

        public void BeginStreaming(
            AvigilonDotNet.IEntityCamera camera,
            AvigilonDotNet.IStreamWindow streamWindow,
            AvigilonDotNet.IStreamGroup streamGroup,
            System.DateTime startTime,
            System.DateTime endTime)
        {
            m_entityCamera = camera;
            m_entityPtz = camera.GetControllingPtz();
            m_streamWindow = streamWindow;
            m_streamGroup = streamGroup;

            m_imagePanel.StreamWindow = m_streamWindow;
            m_imagePanel.Visible = true;

            m_streamWindow.Enable = true;
            m_playbackPanel.BeginStreaming(streamGroup, startTime, endTime);
            m_ptzPanel.Entity = m_entityPtz;

            if (m_ptzPanelContinuous != null)
                m_ptzPanelContinuous.Entity = m_entityPtz;

            if (m_ptzPanelDiscrete != null)
                m_ptzPanelDiscrete.Entity = m_entityPtz;

            if (m_ptzPanelRelative != null)
                m_ptzPanelRelative.Entity = m_entityPtz;

            if (m_streamGroup.Mode == AvigilonDotNet.PlaybackMode.Recorded)
            {
                m_playbackPanel.Visible = true;
                m_streamGroup.Play();

                DoLayout_();
            }

            else
            {
                AvigilonDotNet.PtzCapapabilities caps =
                    (m_entityPtz == null) ? AvigilonDotNet.PtzCapapabilities.None : m_entityPtz.Capabilities;

                if (caps != AvigilonDotNet.PtzCapapabilities.None)
                {
                    m_ptzPanel.Visible = true;
                }

                if ((caps & AvigilonDotNet.PtzCapapabilities.HasContinuousPanTilt) != 0 &&
                    (caps & AvigilonDotNet.PtzCapapabilities.HasContinuousZoom) != 0 &&
                    m_ptzPanelContinuous != null)
                {
                    m_ptzPanelContinuous.Visible = true;
                }

                if ((caps & AvigilonDotNet.PtzCapapabilities.HasRelativeFovPanTilt) != 0 &&
                    (caps & AvigilonDotNet.PtzCapapabilities.HasRelativeFovZoom) != 0 &&
                    m_ptzPanelDiscrete != null)
                {
                    m_ptzPanelDiscrete.Visible = true;
                }

                if ((caps & AvigilonDotNet.PtzCapapabilities.HasRelativeFovPanTilt) != 0 &&
                    (caps & AvigilonDotNet.PtzCapapabilities.HasRelativeFovZoom) != 0 &&
                    m_ptzPanelRelative != null)
                {
                    m_ptzPanelRelative.Visible = true;
                }

                DoLayout_();
            }
        }

        public void EndStreaming()
        {
            // Finished streaming, clear all members
            m_entityCamera = null;
            m_entityPtz = null;
            m_streamWindow = null;
            m_streamGroup = null;

            m_playbackPanel.Visible = false;
            m_playbackPanel.EndStreaming();

            m_ptzPanel.Entity = null;
            m_ptzPanel.Visible = false;

            if (m_ptzPanelContinuous != null)
            {
                m_ptzPanelContinuous.Entity = null;
                m_ptzPanelContinuous.Visible = false;
            }

            if (m_ptzPanelDiscrete != null)
            {
                m_ptzPanelDiscrete.Entity = null;
                m_ptzPanelDiscrete.Visible = false;
            }

            if (m_ptzPanelRelative != null)
            {
                m_ptzPanelRelative.Entity = null;
                m_ptzPanelRelative.Visible = false;
            }
        }

        protected override void OnSizeChanged(System.EventArgs e)
        {
            base.OnSizeChanged(e);

            DoLayout_();
        }

        void DoLayout_()
        {
            int bottomPanelHeight = 0;

            if (m_playbackPanel.Visible)
            {
                bottomPanelHeight = m_playbackPanel.Height;
            }
            else
            {
                if (m_ptzPanel.Visible)
                    bottomPanelHeight += m_ptzPanel.Height;

                if (m_ptzPanelContinuous != null && m_ptzPanelContinuous.Visible)
                    bottomPanelHeight += m_ptzPanelContinuous.Height;

                if (m_ptzPanelDiscrete != null && m_ptzPanelDiscrete.Visible)
                    bottomPanelHeight += m_ptzPanelDiscrete.Height;

                if (m_ptzPanelRelative != null && m_ptzPanelRelative.Visible)
                    bottomPanelHeight += m_ptzPanelRelative.Height;
            }

            m_toolStrip.Location = new System.Drawing.Point(0, 0);
            m_imagePanel.Location = new System.Drawing.Point(0, m_toolStrip.Bottom);
            m_imagePanel.Size = new System.Drawing.Size(ClientSize.Width, ClientSize.Height - m_imagePanel.Top - bottomPanelHeight);

            m_playbackPanel.Location = new System.Drawing.Point(
                0,
                ClientSize.Height - m_playbackPanel.Height);
            m_playbackPanel.Width = ClientSize.Width;

            // Stack the ptz panels
            int nextBottom = ClientSize.Height;
            if (m_ptzPanelRelative != null && m_ptzPanelRelative.Visible)
            {
                m_ptzPanelRelative.Location = new System.Drawing.Point(
                    0,
                    nextBottom - m_ptzPanelRelative.Height);
                nextBottom -= m_ptzPanelRelative.Height;
                m_ptzPanelRelative.Width = ClientSize.Width;
            }

            if (m_ptzPanelDiscrete != null && m_ptzPanelDiscrete.Visible)
            {
                m_ptzPanelDiscrete.Location = new System.Drawing.Point(
                    0,
                    nextBottom - m_ptzPanelDiscrete.Height);
                nextBottom -= m_ptzPanelDiscrete.Height;
                m_ptzPanelDiscrete.Width = ClientSize.Width;
            }

            if (m_ptzPanelContinuous != null && m_ptzPanelContinuous.Visible)
            {
                m_ptzPanelContinuous.Location = new System.Drawing.Point(
                    0,
                    nextBottom - m_ptzPanelContinuous.Height);
                nextBottom -= m_ptzPanelContinuous.Height;
                m_ptzPanelContinuous.Width = ClientSize.Width;
            }

            if (m_ptzPanel.Visible)
            {
                m_ptzPanel.Location = new System.Drawing.Point(
                    0,
                    nextBottom - m_ptzPanel.Height);
                nextBottom -= m_ptzPanel.Height;
                m_ptzPanel.Width = ClientSize.Width;
            }

            if (!m_imagePanel.Visible)
                Invalidate();
        }

        void ImagePanelMouseDown_(object sender, System.Windows.Forms.MouseEventArgs e)
        {
            if (e.Button == System.Windows.Forms.MouseButtons.Middle &&
                m_imagePanel != null &&
                m_entityPtz != null &&
                ((m_entityPtz.Capabilities & AvigilonDotNet.PtzCapapabilities.HasRelativeFovPanTilt) != 0))
            {
                // Center on click point for center mouse button clicks

                // Relative click in current view
                System.Drawing.Rectangle targetRect = m_streamWindow.DisplayTargetRect;
                float xClickRel = (float)(e.X - targetRect.Left) / (float)targetRect.Width;
                float yClickRel = (float)(e.Y - targetRect.Top) / (float)targetRect.Height;
                float xClickVp = xClickRel * m_streamWindow.DisplayedSourceRectF.Width;
                float yClickVp = yClickRel * m_streamWindow.DisplayedSourceRectF.Height;

                // Relative click as a % of total displayed source
                float xTotal = m_streamWindow.DisplayedSourceRectF.X + xClickVp;
                float yTotal = m_streamWindow.DisplayedSourceRectF.Y + yClickVp;

                // calculate translation based on currently viewed area
                float vpCenterX = m_streamWindow.DisplayedSourceRectF.X + m_streamWindow.DisplayedSourceRectF.Width / 2;
                float vpCenterY = m_streamWindow.DisplayedSourceRectF.Y + m_streamWindow.DisplayedSourceRectF.Height / 2;
                float vpTransX = xTotal - vpCenterX;
                float vpTransY = yTotal - vpCenterY;

                // Apply ptz action
                System.Drawing.RectangleF centerPoint = new System.Drawing.RectangleF(vpTransX, vpTransY, 1.0f, 1.0f);
                m_entityPtz.PtzRelativeFov(centerPoint);
            }
        }

        void OnPanButtonClick_(System.Object sender, System.EventArgs e)
        {
            m_panButton.Checked = true;
            m_zoomInButton.Checked = false;
            m_zoomOutButton.Checked = false;

            m_imagePanel.DefaultMouseAction = AvigilonDotNet.ImagePanelMouseAction.Pan;
        }

        void OnZoomInButtonClick_(System.Object sender, System.EventArgs e)
        {
            m_panButton.Checked = false;
            m_zoomInButton.Checked = true;
            m_zoomOutButton.Checked = false;

            m_imagePanel.DefaultMouseAction = AvigilonDotNet.ImagePanelMouseAction.ZoomIn;
        }

        void OnZoomOutButtonClick_(System.Object sender, System.EventArgs e)
        {
            m_panButton.Checked = false;
            m_zoomInButton.Checked = false;
            m_zoomOutButton.Checked = true;

            m_imagePanel.DefaultMouseAction = AvigilonDotNet.ImagePanelMouseAction.ZoomOut;
        }

        void OnFullScreenClick_(System.Object sender, System.EventArgs e)
        {
            System.Windows.Forms.Form form = FindForm();

            if (form != null)
            {
                if (form.FormBorderStyle == System.Windows.Forms.FormBorderStyle.None)
                {
                    form.FormBorderStyle = System.Windows.Forms.FormBorderStyle.Sizable;
                    form.WindowState = m_originalState;
                    m_fullScreenButton.Text = "Full Screen";
                }

                else
                {
                    m_originalState = form.WindowState;
                    form.WindowState = System.Windows.Forms.FormWindowState.Normal;
                    form.FormBorderStyle = System.Windows.Forms.FormBorderStyle.None;
                    form.WindowState = System.Windows.Forms.FormWindowState.Maximized;
                    m_fullScreenButton.Text = "End Full Screen";
                }
            }
        }

        protected override void OnPaint(System.Windows.Forms.PaintEventArgs e)
        {
            base.OnPaint(e);

            if (m_imagePanel.Visible)
                return;

            int width = 150;
            int height = 50;

            int x = (ClientSize.Width - width) / 2;

            e.Graphics.DrawImage(
                m_avigilonLogo,
                new System.Drawing.Rectangle(x, (ClientSize.Height - m_avigilonLogo.Height) / 2, m_avigilonLogo.Width, m_avigilonLogo.Height),
                new System.Drawing.Rectangle(0, 0, m_avigilonLogo.Width, m_avigilonLogo.Height),
                System.Drawing.GraphicsUnit.Pixel);

            System.Windows.Forms.TextRenderer.DrawText(
                e.Graphics,
                "Connecting to camera,\nplease wait...",
                System.Drawing.SystemFonts.DialogFont,
                new System.Drawing.Rectangle(x + m_avigilonLogo.Width, (ClientSize.Height - height) / 2, width - m_avigilonLogo.Width, height),
                System.Drawing.SystemColors.ControlText,
                System.Windows.Forms.TextFormatFlags.Left |
                System.Windows.Forms.TextFormatFlags.VerticalCenter);
        }
    }
}
