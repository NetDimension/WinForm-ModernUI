using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Linq;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace NetDimension.WinForm
{


    public class ModernUIForm : Form
    {
        private const int designTimeDpi = 96;
        private int oldDpi;
        private int currentDpi;
        private bool scaling = false;
        private bool isDpiScalingSuspended = false;


        private static readonly Point minimizedFormLocation = new Point(-32000, -32000);
        private static readonly Point InvalidPoint = new Point(-10000, -10000);
        private Rectangle regionRect = Rectangle.Empty;

        private int isInitializing = 0;
        private bool shouldUpdateOnResumeLayout = false;
        private bool forceInitialized = false;
        private static Padding? SavedBorders = null;


        private Size minimumClientSize;
        private Size maximumClientSize;
        private Size? minimumSize = null;
        private Size? maximumSize = null;


        private bool _activated;
        private bool _windowActive;
        private bool _trackingMouse;
        private bool _captured;
        private bool _disposing;
        private static bool? isDesingerProcess = null;


        private IntPtr _screenDC;

        internal ChromeDecorator shadowDecorator;


        #region Reflected

        private FieldInfo clientWidthField;
        private FieldInfo clientHeightField;
        private FieldInfo formStateSetClientSizeField;
        private FieldInfo formStateField;

        #endregion

        Color shadowColor = Color.Black;
        Color borderColor = ColorTranslator.FromHtml("#1883D7");
        Padding borders = new Padding(1, 1, 1, 1);
        /// <summary>
        /// 设置或获取活动状态窗口投影颜色
        /// </summary>
        [Category("NanUI")]
        public Color ShadowColor
        {
            get => shadowColor;
            set => shadowColor = shadowDecorator.ShadowColor = value;
        }

        [Category("NanUI")]
        public Color BorderColor
        {
            get
            {
                return borderColor;
            }
            set
            {
                borderColor = value;

                InvalidateNonClient();
            }
        }

        private Color InactiveBorderColor
        {
            get
            {
                var r = Convert.ToInt32(BorderColor.R * .7f);
                var g = Convert.ToInt32(BorderColor.G * .7f);
                var b = Convert.ToInt32(BorderColor.B * .7f);
                var color = Color.FromArgb(255, r, g, b);

                return color;
            }
        }

        [Category("NanUI")]
        public Padding Borders
        {
            get
            {
                return borders;
            }
            set
            {
                borders = value;
                RecalcClientSize();
            }
        }


        [Browsable(false), EditorBrowsable(EditorBrowsableState.Never)]
        protected static bool IsDesingerProcess
        {
            get
            {
                if (isDesingerProcess == null)
                {
                    isDesingerProcess = System.Diagnostics.Process.GetCurrentProcess().ProcessName == "devenv";
                }

                return isDesingerProcess.Value;
            }
        }

        [Browsable(false), EditorBrowsable(EditorBrowsableState.Never)]
        protected bool IsDesignMode => DesignMode || LicenseManager.UsageMode == LicenseUsageMode.Designtime || IsDesingerProcess;

        [Browsable(false), EditorBrowsable(EditorBrowsableState.Never)]
        protected bool IsInitializing => !forceInitialized && (this.isInitializing != 0 || IsLayoutSuspendedCore);


        /// <summary>
        /// Occurs when the active window setting changes.
        /// </summary>
        [Browsable(false)]
        [EditorBrowsable(EditorBrowsableState.Never)]
        public event EventHandler WindowActiveChanged;

        public ModernUIForm()
        {
            SetStyle(ControlStyles.ResizeRedraw |
                ControlStyles.AllPaintingInWmPaint |
                ControlStyles.UserPaint |
                ControlStyles.OptimizedDoubleBuffer, true);

            _screenDC = Win32.CreateCompatibleDC(IntPtr.Zero);
            //_needPaintDelegate = new NeedPaintHandler(OnNeedPaint);


            AutoScaleMode = AutoScaleMode.None;

            InitializeReflectedFields();

            shadowDecorator = new ChromeDecorator(this, false);

            if (!IsDesignMode)
            {
                this.minimumClientSize = Size.Empty;
                this.maximumClientSize = Size.Empty;

            }
        }

        private void InitializeReflectedFields()
        {
            clientWidthField = typeof(Control).GetField("clientWidth", BindingFlags.NonPublic | BindingFlags.Instance);
            clientHeightField = typeof(Control).GetField("clientHeight", BindingFlags.NonPublic | BindingFlags.Instance);
            formStateSetClientSizeField = typeof(Form).GetField("FormStateSetClientSize", BindingFlags.NonPublic | BindingFlags.Static);
            formStateField = typeof(Form).GetField("formState", BindingFlags.NonPublic | BindingFlags.Instance);
        }

        private SizeF dpiScaleFactor = new SizeF(1f, 1f);


        public override Size MinimumSize
        {
            get
            {
                if (minimumSize.HasValue)
                    return minimumSize.Value;
                return base.MinimumSize;
            }
            set
            {
                minimumSize = value;
                if (IsInitializing)
                {
                    return;
                }
                Size maxSize = MaximumSize;
                base.MinimumSize = value;
                if (maxSize != MaximumSize)
                    MaximumClientSize = ClientSizeFromSize(MaximumSize);
            }
        }
        public override Size MaximumSize
        {
            get
            {
                if (maximumSize.HasValue)
                    return maximumSize.Value;
                return base.MaximumSize;
            }
            set
            {
                maximumSize = value;
                if (IsInitializing)
                {
                    return;
                }
                Size minSize = MinimumSize;
                base.MaximumSize = value;
                if (MinimumSize != minSize)
                    MinimumClientSize = ClientSizeFromSize(MinimumSize);
            }
        }

        [Browsable(false), EditorBrowsable(EditorBrowsableState.Never), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public Size MinimumClientSize
        {
            get { return minimumClientSize; }
            set
            {
                value = ConstrainMinimumClientSize(value);
                if (MinimumClientSize == value) return;
                minimumClientSize = value;
                OnMinimumClientSizeChanged();
            }
        }
        [Browsable(false), EditorBrowsable(EditorBrowsableState.Never), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public Size MaximumClientSize
        {
            get { return maximumClientSize; }
            set
            {
                if (MaximumClientSize == value) return;
                maximumClientSize = value;
                OnMaximumClientSizeChanged();
            }
        }

        protected Size ConstrainMinimumClientSize(Size value)
        {
            value.Width = Math.Max(0, value.Width);
            value.Height = Math.Max(0, value.Height);
            return value;
        }

        protected virtual void OnMinimumClientSizeChanged()
        {
            if (IsInitializing) return;
            MinimumSize = GetConstrainSize(MinimumClientSize);
        }

        protected virtual void OnMaximumClientSizeChanged()
        {
            if (IsInitializing) return;
            MaximumSize = GetConstrainSize(MaximumClientSize);
        }

        public new void SuspendLayout()
        {

            base.SuspendLayout();

            isInitializing++;
        }
        public new void ResumeLayout()
        {
            ResumeLayout(true);
        }
        public new void ResumeLayout(bool performLayout)
        {

            if (this.isInitializing > 0)
                this.isInitializing--;

            if (this.isInitializing == 0)
            {
                //CheckForceModernUIChangedCore();

                CheckForceUIChangedCore();
            }

            base.ResumeLayout(performLayout);

            if (!IsInitializing)
            {
                CheckMinimumSize();
                CheckMaximumSize();
            }
        }

        private void CheckMinimumSize()
        {
            if (this.minimumSize == null) return;
            Size msize = (Size)minimumSize;
            if (!msize.IsEmpty)
            {
                if (msize.Width > 0) msize.Width = Math.Min(msize.Width, Size.Width);
                if (msize.Height > 0) msize.Height = Math.Min(msize.Height, Size.Height);
                if (this.maximumSize != null && !this.maximumSize.Value.IsEmpty)
                {
                    if (this.maximumSize.Value.Width == this.minimumSize.Value.Width)
                        msize.Width = Size.Width;
                    if (this.maximumSize.Value.Height == this.minimumSize.Value.Height)
                        msize.Height = Size.Height;
                }
            }
            this.minimumSize = null;
            base.MinimumSize = msize;
        }

        private void CheckMaximumSize()
        {
            if (this.maximumSize == null) return;
            Size msize = (Size)maximumSize;
            if (!msize.IsEmpty)
            {
                if (msize.Width > 0) msize.Width = Math.Max(msize.Width, Size.Width);
                if (msize.Height > 0) msize.Height = Math.Max(msize.Height, Size.Height);
                if (this.minimumSize != null && !this.minimumSize.Value.IsEmpty)
                {
                    if (this.maximumSize.Value.Width == this.minimumSize.Value.Width)
                        msize.Width = Size.Width;
                    if (this.maximumSize.Value.Height == this.minimumSize.Value.Height)
                        msize.Height = Size.Height;
                }
            }
            this.maximumSize = null;
            base.MaximumSize = msize;
        }


        private void CheckForceUIChangedCore()
        {
            if (this.isInitializing != 0) return;
            if (LayoutSuspendCountCore == 1 && this.shouldUpdateOnResumeLayout)
            {
                this.forceInitialized = true;
                try
                {
                    OnUIChangedCore();
                }
                finally
                {
                    this.forceInitialized = false;
                }
            }
        }

        protected internal void SetLayoutDeferred()
        {
            const int STATE_LAYOUTDEFERRED = 512;
            MethodInfo mi = typeof(Control).GetMethod("SetState", BindingFlags.NonPublic | BindingFlags.Instance, null, new Type[] { typeof(int), typeof(bool) }, null);
            mi.Invoke(this, new object[] { STATE_LAYOUTDEFERRED, true });
        }



        private void OnUIChangedCore()
        {
            if (IsInitializing)
            {
                if (Visible && IsLayoutSuspendedCore)
                {
                    SetLayoutDeferred();
                }
                this.shouldUpdateOnResumeLayout = true;
                return;
            }
            this.shouldUpdateOnResumeLayout = false;
            bool shouldUpdateSize = CheckUpdateUI();
            Size clientSize = ClientSize;
            OnMinimumClientSizeChanged();
            OnMaximumClientSizeChanged();
            FieldInfo fiBounds = typeof(Form).GetField("restoredWindowBounds", BindingFlags.NonPublic | BindingFlags.Instance);
            FieldInfo fiBoundsSpec = typeof(Form).GetField("restoredWindowBoundsSpecified", BindingFlags.NonPublic | BindingFlags.Instance);
            FieldInfo fiBounds2 = typeof(Form).GetField("restoreBounds", BindingFlags.NonPublic | BindingFlags.Instance);
            Rectangle restoredWinBounds = (Rectangle)fiBounds.GetValue(this);
            Rectangle restoreBounds = (Rectangle)fiBounds2.GetValue(this);
            BoundsSpecified restoredWinBoundsSpec = (BoundsSpecified)fiBoundsSpec.GetValue(this);

            GetFormStateExWindowBoundsIsClientSize(out int frmStateExWindowBoundsWidthIsClientSize, out int frmStateExWindowBoundsHeightIsClientSize);

            int windowState = SaveFormStateWindowState();
            bool normalState = SaveControlStateNormalState();
            if (shouldUpdateSize)
                Size = SizeFromClientSize(clientSize);
            if ((restoredWinBoundsSpec & BoundsSpecified.Width) != 0 && (restoredWinBoundsSpec & BoundsSpecified.Height) != 0) restoreBounds.Size = SizeFromClientSize(restoredWinBounds.Size);
            if (WindowState != FormWindowState.Normal && IsHandleCreated)
            {
                fiBounds.SetValue(this, restoredWinBounds);
                fiBounds2.SetValue(this, restoreBounds);
                SetFormStateExWindowBoundsIsClientSize(frmStateExWindowBoundsWidthIsClientSize, frmStateExWindowBoundsHeightIsClientSize);
            }
            if (IsMdiChild)
            {
                RestoreFormStateWindowState(windowState);
                RestoreControlStateNormalState(normalState);
            }


        }

        protected bool CheckUpdateUI()
        {
            Padding? savedMargins = null;

            Size savedClientSize = ClientSize;

            if (DesignMode && IsInitializing)
            {
                savedMargins = SavedBorders;
            }

            var needReset = false;

            if (savedMargins != null && !object.Equals(savedMargins.Value, RealWindowBorders))
            {
                ClientSize = savedClientSize;
            }

            if (IsHandleCreated)
            {
                Win32.SetWindowTheme(this.Handle, "", "");
                Refresh();
            }
            return needReset;
        }

        private int SaveFormStateWindowState()
        {
            FieldInfo formStateWindowState = typeof(Form).GetField("FormStateWindowState", BindingFlags.NonPublic | BindingFlags.Static);
            BitVector32.Section formStateWindowStateSection = ((BitVector32.Section)formStateWindowState.GetValue(this));
            BitVector32 formStateData = (BitVector32)formStateField.GetValue(this);
            return formStateData[formStateWindowStateSection];
        }

        private void RestoreFormStateWindowState(int state)
        {
            FieldInfo formStateWindowState = typeof(Form).GetField("FormStateWindowState", BindingFlags.NonPublic | BindingFlags.Static);
            BitVector32.Section formStateWindowStateSection = ((BitVector32.Section)formStateWindowState.GetValue(this));
            BitVector32 formStateData = (BitVector32)formStateField.GetValue(this);
            formStateData[formStateWindowStateSection] = state;
            formStateField.SetValue(this, formStateData);
        }

        private bool SaveControlStateNormalState()
        {
            FieldInfo state = typeof(Control).GetField("state", BindingFlags.NonPublic | BindingFlags.Instance);
            return ((int)state.GetValue(this) & 0x10000) != 0;
        }


        private void RestoreControlStateNormalState(bool isNormal)
        {
            FieldInfo state = typeof(Control).GetField("state", BindingFlags.NonPublic | BindingFlags.Instance);
            int value = (int)state.GetValue(this);
            state.SetValue(this, isNormal ? (value | 0x10000) : (value & (~0x10000)));
        }
        private void GetFormStateExWindowBoundsIsClientSize(out int width, out int height)
        {
            FieldInfo formStateExInfo = typeof(Form).GetField("formStateEx", BindingFlags.NonPublic | BindingFlags.Instance);
            FieldInfo formStateExWindowBoundsWidthIsClientSizeInfo = typeof(Form).GetField("FormStateExWindowBoundsWidthIsClientSize", BindingFlags.NonPublic | BindingFlags.Static);
            FieldInfo formStateExWindowBoundsHeightIsClientSizeInfo = typeof(Form).GetField("FormStateExWindowBoundsHeightIsClientSize", BindingFlags.NonPublic | BindingFlags.Static);
            BitVector32.Section widthSection = (BitVector32.Section)formStateExWindowBoundsWidthIsClientSizeInfo.GetValue(this);
            BitVector32.Section heightSection = (BitVector32.Section)formStateExWindowBoundsHeightIsClientSizeInfo.GetValue(this);
            BitVector32 formState = (BitVector32)formStateExInfo.GetValue(this);
            width = formState[widthSection];
            height = formState[heightSection];
        }
        private void SetFormStateExWindowBoundsIsClientSize(int width, int height)
        {
            FieldInfo formStateExInfo = typeof(Form).GetField("formStateEx", BindingFlags.NonPublic | BindingFlags.Instance);
            FieldInfo formStateExWindowBoundsWidthIsClientSizeInfo = typeof(Form).GetField("FormStateExWindowBoundsWidthIsClientSize", BindingFlags.NonPublic | BindingFlags.Static);
            FieldInfo formStateExWindowBoundsHeightIsClientSizeInfo = typeof(Form).GetField("FormStateExWindowBoundsHeightIsClientSize", BindingFlags.NonPublic | BindingFlags.Static);
            BitVector32.Section widthSection = (BitVector32.Section)formStateExWindowBoundsWidthIsClientSizeInfo.GetValue(this);
            BitVector32.Section heightSection = (BitVector32.Section)formStateExWindowBoundsHeightIsClientSizeInfo.GetValue(this);
            BitVector32 formState = (BitVector32)formStateExInfo.GetValue(this);
            formState[widthSection] = width;
            formState[heightSection] = height;
            formStateExInfo.SetValue(this, formState);
        }


        protected override void ScaleCore(float x, float y)
        {

            MaximumClientSize = new Size((int)Math.Round(MaximumClientSize.Width * x), (int)Math.Round(MaximumClientSize.Height * y));
            base.ScaleCore(x, y);
            MinimumClientSize = new Size((int)Math.Round(MinimumClientSize.Width * x), (int)Math.Round(MinimumClientSize.Height * y));

        }

        protected override Rectangle GetScaledBounds(Rectangle bounds, SizeF factor, BoundsSpecified specified)
        {
            Rectangle rect = base.GetScaledBounds(bounds, factor, specified);

            Size sz = SizeFromClientSize(Size.Empty);
            if (!GetStyle(ControlStyles.FixedWidth) && ((specified & BoundsSpecified.Width) != BoundsSpecified.None))
            {
                int clientWidth = bounds.Width - sz.Width;
                rect.Width = ((int)Math.Round((double)(clientWidth * factor.Width))) + sz.Width;
            }
            if (!GetStyle(ControlStyles.FixedHeight) && ((specified & BoundsSpecified.Height) != BoundsSpecified.None))
            {
                int clientHeight = bounds.Height - sz.Height;
                rect.Height = ((int)Math.Round((double)(clientHeight * factor.Height))) + sz.Height;
            }
            return rect;
        }

        /// <summary>
        /// Releases all resources used by the Control. 
        /// </summary>
        /// <param name="disposing">true to release both managed and unmanaged resources; false to release only unmanaged resources.</param>
        protected override void Dispose(bool disposing)
        {
            _disposing = true;

            if (disposing)
            {
                // Must unhook from the palette paint events

            }

            base.Dispose(disposing);


            if (_screenDC != IntPtr.Zero)
                Win32.DeleteDC(_screenDC);
        }


        [Browsable(false)]
        [EditorBrowsable(EditorBrowsableState.Never)]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public SizeF DpiScaleFactor
        {
            get { return this.dpiScaleFactor; }
            protected set { this.dpiScaleFactor = value; }
        }


        /// <summary>
        /// Gets the size of the borders requested by the real window.
        /// </summary>
        /// <returns>Border sizing.</returns>
        [Browsable(false)]
        [EditorBrowsable(EditorBrowsableState.Never)]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public Padding RealWindowBorders
        {
            get
            {
                // Use the form level create params to get the real borders
                return FormStyleHelper.GetWindowBorders(CreateParams);
            }
        }

        /// <summary>
        /// Gets and sets the active state of the window.
        /// </summary>
        [Browsable(false)]
        [EditorBrowsable(EditorBrowsableState.Never)]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public bool WindowActive
        {
            get { return _windowActive; }

            set
            {
                if (_windowActive != value)
                {
                    _windowActive = value;
                    OnWindowActiveChanged();
                }
            }
        }






        /// <summary>
        /// Request the non-client area be recalculated.
        /// </summary>
        public void RecalcNonClient()
        {
            if (!IsDisposed && !Disposing && IsHandleCreated)
            {
                Win32.SetWindowPos(Handle, IntPtr.Zero, 0, 0, 0, 0,
                                (uint)(Win32.SWP_NOACTIVATE | Win32.SWP_NOMOVE |
                                       Win32.SWP_NOZORDER | Win32.SWP_NOSIZE |
                                       Win32.SWP_NOOWNERZORDER | Win32.SWP_FRAMECHANGED));
            }
        }

        /// <summary>
        /// Convert a screen location to a window location.
        /// </summary>
        /// <param name="screenPt">Screen point.</param>
        /// <returns>Point in window coordinates.</returns>
        protected Point ScreenToWindow(Point screenPt)
        {
            // First of all convert to client coordinates
            Point clientPt = PointToClient(screenPt);

            // Now adjust to take into account the top and left borders
            Padding borders = RealWindowBorders;
            clientPt.Offset(borders.Left, borders.Top);

            return clientPt;
        }

        /// <summary>
        /// Request the non-client area be repainted.
        /// </summary>
        public void RedrawNonClient()
        {
            InvalidateNonClient(Rectangle.Empty, true);
        }

        /// <summary>
        /// Request the non-client area be repainted.
        /// </summary>
        public void InvalidateNonClient()
        {
            InvalidateNonClient(Rectangle.Empty, true);
        }

        /// <summary>
        /// Request the non-client area be repainted.
        /// </summary>
        /// <param name="invalidRect">Area to invalidate.</param>
        protected void InvalidateNonClient(Rectangle invalidRect)
        {
            InvalidateNonClient(invalidRect, true);
        }

        /// <summary>
        /// Request the non-client area be repainted.
        /// </summary>
        /// <param name="invalidRect">Area to invalidate.</param>
        /// <param name="excludeClientArea">Should client area be excluded.</param>
        protected void InvalidateNonClient(Rectangle invalidRect,
                                           bool excludeClientArea)
        {
            if (!IsDisposed && !Disposing && IsHandleCreated)
            {
                if (invalidRect.IsEmpty)
                {
                    Rectangle realWindowRectangle = RealWindowRectangle;

                    invalidRect = new Rectangle(realWindowRectangle.Left,
                                                realWindowRectangle.Top,
                                                realWindowRectangle.Width,
                                                realWindowRectangle.Height);
                }

                using (Region invalidRegion = new Region(invalidRect))
                {
                    if (excludeClientArea)
                    {
                        Win32.GetClientRect(Handle, out var clientRect);
                        Rectangle clientBounds = new Rectangle(clientRect.left, clientRect.top, clientRect.right, clientRect.bottom);
                        clientBounds.Offset(-clientBounds.Left, -clientBounds.Top);
                        clientBounds.Offset(ClientMargin.Left, ClientMargin.Top);

                        invalidRegion.Exclude(clientBounds);

                    }

                    using (Graphics g = Graphics.FromHwnd(Handle))
                    {
                        IntPtr hRgn = invalidRegion.GetHrgn(g);

                        Win32.RedrawWindow(Handle, IntPtr.Zero, hRgn,
                                        (uint)(Win32.RDW_FRAME | Win32.RDW_UPDATENOW | Win32.RDW_INVALIDATE));

                        Win32.DeleteObject(hRgn);
                    }
                }
            }
        }

        /// <summary>
        /// Gets rectangle that is the real window rectangle based on Win32 API call.
        /// </summary>
        protected Rectangle RealWindowRectangle
        {
            get
            {
                // Grab the actual current size of the window, this is more accurate than using
                // the 'this.Size' which is out of date when performing a resize of the window.
                Win32.RECT windowRect = new Win32.RECT();
                Win32.GetWindowRect(Handle, ref windowRect);

                // Create rectangle that encloses the entire window
                return new Rectangle(0, 0, windowRect.right - windowRect.left, windowRect.bottom - windowRect.top);
            }
        }



        /// <summary>
        /// Raises the HandleCreated event.
        /// </summary>
        /// <param name="e">An EventArgs containing the event data.</param>
        protected override void OnHandleCreated(EventArgs e)
        {
            // Can fail on versions before XP SP1
            try
            {
                // Prevent the OS from drawing the non-client area in classic look
                // if the application stops responding to windows messages
                Win32.DisableProcessWindowsGhosting();
                Win32.SetWindowTheme(Handle, "", "");
            }
            catch { }

            base.OnHandleCreated(e);
        }

        protected override void OnCreateControl()
        {
            base.OnCreateControl();
        }

        protected override void OnLoad(EventArgs e)
        {
            base.OnLoad(e);


            OnMinimumClientSizeChanged();
            OnMaximumClientSizeChanged();
            CalcFormBounds();
            //if (!IsDesignMode)
            //{
            //    if (StartPosition == FormStartPosition.CenterParent && Owner != null)
            //    {
            //        Location = new Point(Owner.Location.X + Owner.Width / 2 - Width / 2,
            //        Owner.Location.Y + Owner.Height / 2 - Height / 2);


            //    }
            //    else if (StartPosition == FormStartPosition.CenterScreen || (StartPosition == FormStartPosition.CenterParent && Owner == null))
            //    {
            //        var currentScreen = Screen.FromPoint(MousePosition);
            //        Location = new Point(currentScreen.WorkingArea.Left + (currentScreen.WorkingArea.Width / 2 - this.Width / 2), currentScreen.WorkingArea.Top + (currentScreen.WorkingArea.Height / 2 - this.Height / 2));

            //    }
            //}
            UpdateFormShadow();
        }

        protected override void OnShown(EventArgs e)
        {
            base.OnShown(e);

            if (IsDesignMode) return;

            var action = new Action(() =>
            {
                shadowDecorator.Enable(true);

                if (IsActive)
                {
                    shadowDecorator.SetFocus();
                }
            });

            Task.Factory.StartNew(() =>
            {
                System.Threading.Thread.Sleep(180);

                if (InvokeRequired)
                {
                    Invoke(new MethodInvoker(action));
                }
                else
                {
                    action.Invoke();
                }
            });
        }

        private void UpdateFormShadow()
        {
            if (IsDesignMode) return;

            if (!IsMdiChild && Parent == null)
            {
                shadowDecorator.InitializeShadows();

                if (Owner != null)
                {
                    shadowDecorator.SetOwner(Owner.Handle);
                }

            }
        }

        protected Rectangle formBounds = Rectangle.Empty;
        protected Rectangle prevFormBounds = Rectangle.Empty;
        protected bool isFormPainted = false;
        protected bool boundsUpdated = false;
        protected bool IsRegionPainted { get; private set; }

        protected internal void CalcFormBounds()
        {
            if (IsMdiChild || !IsHandleCreated) return;
            var correctFormBounds = new Win32.RECT();
            Win32.GetWindowRect(this.Handle, ref correctFormBounds);
            Rectangle currentBounds = new Rectangle(correctFormBounds.left, correctFormBounds.top, correctFormBounds.right - correctFormBounds.left, correctFormBounds.bottom - correctFormBounds.top);

            if (IsMinimizedState(currentBounds)) currentBounds = Rectangle.Empty;
            if (formBounds == currentBounds && (boundsUpdated || !IsRegionPainted))
                return;
            this.isFormPainted = false;
            prevFormBounds = formBounds;
            formBounds = currentBounds;
            if (prevFormBounds != formBounds) boundsUpdated = true;
        }


        protected internal bool IsMinimizedState(Rectangle bounds)
        {
            return WindowState == FormWindowState.Minimized && bounds.Location == minimizedFormLocation;
        }


        private Size cachedClientSize = new Size(-1, -1);


        protected override void CreateHandle()
        {
            if (!this.IsDisposed)
            {
                base.CreateHandle();
            }

            if (WindowState != FormWindowState.Minimized)
            {
                Size = SizeFromClientSize(ClientSize);
            }

            if (this.cachedClientSize != new Size(-1, -1))
            {
                this.SetClientSizeCore(cachedClientSize.Width, cachedClientSize.Height);
                this.cachedClientSize = new Size(-1, -1);
            }
        }



        /// <summary>
        /// Gets the margin that determines the position and size of the client
        /// area of the Form.
        /// </summary>
        protected virtual Padding ClientMargin
        {
            get
            {
                Padding clientMargin = Borders;
                return clientMargin;
            }
        }

        protected virtual void RecalcClientSize()
        {
            Win32.GetClientRect(Handle, out var clientRect);
            Rectangle clientBounds = new Rectangle(clientRect.left, clientRect.top, clientRect.right, clientRect.bottom);
            clientBounds.Offset(-clientBounds.Left, -clientBounds.Top);
            clientBounds.Offset(ClientMargin.Left, ClientMargin.Top);

            SetClientSizeCore(clientBounds.Width, clientBounds.Height);

            InvalidateNonClient();
        }


        protected override void SetClientSizeCore(int x, int y)
        {
            if (!this.IsHandleCreated)
            {
                this.cachedClientSize = new Size(x, y);
                base.SetClientSizeCore(x, y);
                return;
            }


            if ((((clientWidthField != null))
                && ((clientHeightField != null)
                && (formStateField != null)))
                && (formStateSetClientSizeField != null))
            {

                //Size sizeToSet = new Size(x + this.ClientMargin.Horizontal, y + this.ClientMargin.Vertical);

                //base.Size = sizeToSet;

                this.Size = SizeFromClientSize(new Size(x, y));

                clientWidthField.SetValue(this, x);
                clientHeightField.SetValue(this, y);
                BitVector32.Section section = (BitVector32.Section)formStateSetClientSizeField.GetValue(this);
                BitVector32 vector = (BitVector32)formStateField.GetValue(this);
                vector[section] = 1;
                formStateField.SetValue(this, vector);
                this.OnClientSizeChanged(EventArgs.Empty);
                vector[section] = 0;
                formStateField.SetValue(this, vector);
            }
            else
            {
                base.SetClientSizeCore(x, y);
            }

        }

        /// <summary>
        /// Performs the work of setting the specified bounds of this control.
        /// </summary>
        /// <param name="x">The new Left property value of the control.</param>
        /// <param name="y">The new Top property value of the control.</param>
        /// <param name="width">The new Width property value of the control.</param>
        /// <param name="height">The new Height property value of the control.</param>
        /// <param name="specified">A bitwise combination of the BoundsSpecified values.</param>
        protected override void SetBoundsCore(int x, int y, int width, int height, BoundsSpecified specified)
        {

            if (IsDesignMode)
            {
                base.SetBoundsCore(x, y, width, height, specified);
                return;
            }

            var size = PatchFormSizeInRestoreWindowBoundsIfNecessary(width, height);
            size = CalcPreferredSizeCore(size);



            if (y != Top)
            {
                y = Top;

            }

            if(x != Left)
            {
                x = Left;
            }

            base.SetBoundsCore(x, y, size.Width, size.Height, specified);
        }

        protected virtual Size PatchFormSizeInRestoreWindowBoundsIfNecessary(int width, int height)
        {
            if (WindowState == FormWindowState.Normal)
            {
                try
                {
                    FieldInfo restoredWindowBoundsSpecified = typeof(Form).GetField("restoredWindowBoundsSpecified", BindingFlags.NonPublic | BindingFlags.Instance);
                    BoundsSpecified restoredSpecified = (BoundsSpecified)restoredWindowBoundsSpecified.GetValue(this);
                    if ((restoredSpecified & BoundsSpecified.Size) != BoundsSpecified.None)
                    {
                        FieldInfo formStateExWindowBoundsFieldInfo = typeof(Form).GetField("FormStateExWindowBoundsWidthIsClientSize", BindingFlags.NonPublic | BindingFlags.Static);
                        FieldInfo formStateExFieldInfo = typeof(Form).GetField("formStateEx", BindingFlags.NonPublic | BindingFlags.Instance);
                        FieldInfo restoredBoundsFieldInfo = typeof(Form).GetField("restoredWindowBounds", BindingFlags.NonPublic | BindingFlags.Instance);

                        if (formStateExWindowBoundsFieldInfo != null && formStateExFieldInfo != null && restoredBoundsFieldInfo != null)
                        {
                            Rectangle restoredWindowBounds = (Rectangle)restoredBoundsFieldInfo.GetValue(this);
                            BitVector32.Section section = (BitVector32.Section)formStateExWindowBoundsFieldInfo.GetValue(this);
                            BitVector32 vector = (BitVector32)formStateExFieldInfo.GetValue(this);
                            if (vector[section] == 1)
                            {
                                width = restoredWindowBounds.Width + ClientMargin.Horizontal;
                                height = restoredWindowBounds.Height + ClientMargin.Vertical;
                            }
                        }
                    }
                }
                catch
                {
                }
            }

            return new Size(width, height);

        }

        protected virtual Size CalcPreferredSizeCore(Size size)
        {
            return size;
        }

        /// <summary>
        /// Raises the Activated event.
        /// </summary>
        /// <param name="e">An EventArgs containing the event data.</param>
        protected override void OnActivated(EventArgs e)
        {
            WindowActive = true;
            base.OnActivated(e);
        }

        /// <summary>
        /// Raises the Deactivate event.
        /// </summary>
        /// <param name="e">An EventArgs containing the event data.</param>
        protected override void OnDeactivate(EventArgs e)
        {
            WindowActive = false;
            base.OnDeactivate(e);
        }

        private PropertyInfo piLayout = null;
        [Browsable(false)]
        protected internal bool IsLayoutSuspendedCore
        {
            get
            {
                if (piLayout == null) piLayout = typeof(Control).GetProperty("IsLayoutSuspended", BindingFlags.Instance | BindingFlags.NonPublic);
                if (piLayout != null) return (bool)piLayout.GetValue(this, null);
                return false;
            }
        }

        private FieldInfo fiLayoutSuspendCount = null;
        [Browsable(false)]
        protected internal byte LayoutSuspendCountCore
        {
            get
            {
                if (fiLayoutSuspendCount == null) fiLayoutSuspendCount = typeof(Control).GetField("layoutSuspendCount", BindingFlags.Instance | BindingFlags.NonPublic);
                if (fiLayoutSuspendCount != null) return (byte)fiLayoutSuspendCount.GetValue(this);
                return 1;
            }
        }

        private FieldInfo formStateCoreField;
        private FieldInfo FormStateCoreField
        {
            get
            {
                if (formStateCoreField == null)
                    formStateCoreField = typeof(Form).GetField("formState", BindingFlags.NonPublic | BindingFlags.Instance);
                return formStateCoreField;
            }
        }

        private FieldInfo formStateWindowActivated;
        private bool isEnterSizeMoveMode;

        private FieldInfo FormStateWindowActivatedField
        {
            get
            {
                if (formStateWindowActivated == null)
                    formStateWindowActivated = typeof(Form).GetField("FormStateIsWindowActivated", BindingFlags.NonPublic | BindingFlags.Static);
                return formStateWindowActivated;
            }
        }

        [Browsable(false)]
        protected bool IsActive
        {
            get
            {
                BitVector32 bv = (BitVector32)FormStateCoreField.GetValue(this);
                BitVector32.Section s = (BitVector32.Section)FormStateWindowActivatedField.GetValue(this);
                return bv[s] == 1;
            }
        }

        protected override void OnSizeChanged(EventArgs e)
        {
            if (FormBorderStyle != FormBorderStyle.None && WindowState != FormWindowState.Minimized)
            {
                PatchClientSize();
            }

            CalcFormBounds();


            base.OnSizeChanged(e);
        }

        protected override Size SizeFromClientSize(Size clientSize)
        {
            clientSize.Width += ClientMargin.Horizontal;
            clientSize.Height += ClientMargin.Vertical;

            return clientSize;
        }

        protected virtual void PatchClientSize()
        {
            var size = ClientSizeFromSize(Size);
            FieldInfo fiWidth = typeof(Control).GetField("clientWidth", BindingFlags.Instance | BindingFlags.NonPublic);
            FieldInfo fiHeight = typeof(Control).GetField("clientHeight", BindingFlags.Instance | BindingFlags.NonPublic);
            fiWidth.SetValue(this, size.Width);
            fiHeight.SetValue(this, size.Height);

        }

        protected virtual Size GetConstrainSize(Size clientSize)
        {
            if (clientSize == Size.Empty) return Size.Empty;
            return SizeFromClientSize(clientSize);
        }

        protected virtual Size ClientSizeFromSize(Size formSize)
        {
            if (formSize == Size.Empty)
            {
                return Size.Empty;
            }
            Size sz = SizeFromClientSize(Size.Empty);

            Size res = new Size(formSize.Width - sz.Width, formSize.Height - sz.Height);




            if (WindowState != FormWindowState.Maximized)
                return res;
            var rect = new Win32.RECT();

            rect.left = 0;
            rect.top = 0;
            rect.right = res.Width - RealWindowBorders.Horizontal + sz.Width;
            rect.bottom = res.Height - RealWindowBorders.Bottom * 2 + sz.Height;

            return new Size(rect.right, rect.bottom);
        }


        /// <summary>
        /// Process Windows-based messages.
        /// </summary>
        /// <param name="m">A Windows-based message.</param>
        protected override void WndProc(ref Message m)
        {
            bool processed = false;

            // We do not process the message if on an MDI child, because doing so prevents the 
            // LayoutMdi call on the parent from working and cascading/tiling the children
            if ((m.Msg == Win32.WM_NCCALCSIZE) &&
                ((MdiParent == null)))
            {
                NCCalcSize(ref m);
            }

            // Do we need to override message processing?
            if (!IsDisposed && !Disposing)
            {
                switch (m.Msg)
                {
                    case Win32.WM_NCPAINT:
                        OnWMNCPaint(ref m);
                        processed = true;

                        break;
                    case Win32.WM_SIZE:
                        OnWMSize(ref m);

                        System.Diagnostics.Debug.WriteLine("SIZed");

                        break;
                    case Win32.WM_ACTIVATEAPP:
                        Win32.SendFrameChanged(Handle);
                        break;
                    case Win32.WM_NCHITTEST:
                        processed = OnWMNCHITTEST(ref m);
                        break;
                    case Win32.WM_NCACTIVATE:
                        processed = OnWMNCACTIVATE(ref m);
                        m.Result = Win32.MESSAGE_HANDLED;
                        Win32.SendFrameChanged(Handle);

                        break;
                    case Win32.WM_NCMOUSEMOVE:
                        processed = OnWMNCMOUSEMOVE(ref m);
                        Win32.SendFrameChanged(Handle);
                        break;
                    case Win32.WM_NCLBUTTONDOWN:
                        processed = OnWMNCLBUTTONDOWN(ref m);
                        Win32.SendFrameChanged(Handle);

                        break;
                    case Win32.WM_NCLBUTTONUP:
                        processed = OnWMNCLBUTTONUP(ref m);
                        Win32.SendFrameChanged(Handle);

                        break;
                    case Win32.WM_MOUSEMOVE:
                        if (_captured)
                            processed = OnWMMOUSEMOVE(ref m);
                        break;
                    case Win32.WM_LBUTTONUP:
                        if (_captured)
                            processed = OnWMLBUTTONUP(ref m);
                        break;
                    case Win32.WM_NCMOUSELEAVE:
                        if (!_captured)
                            processed = OnWMNCMOUSELEAVE(ref m);
                        break;
                    case Win32.WM_MOVE:
                        OnWMMove(ref m);
                        break;
                    //case Win32.WM_ENTERSIZEMOVE:
                    //    var rect = new Win32.RECT();
                    //    Win32.GetWindowRect(Handle, ref rect);
                    //    isEnterSizeMoveMode = true;
                    //    break;

                    //case Win32.WM_EXITSIZEMOVE:
                    //    isEnterSizeMoveMode = false;

                    //    if (shadowDecorator != null  && !shadowDecorator.IsEnabled)
                    //    {
                    //        shadowDecorator.Enable(true);
                    //    }
                    //    break;

                    //case Win32.WM_SIZING:

                    //    System.Diagnostics.Debug.WriteLine("SIZING");

                    //    if (IsHandleCreated && isEnterSizeMoveMode == true && shadowDecorator.IsEnabled)
                    //    {
                    //        shadowDecorator.Enable(false);
                    //    }
                    //    break;
                    case Win32.WM_NCLBUTTONDBLCLK:
                        processed = OnWMNCLBUTTONDBLCLK(ref m);
                        break;
                    case Win32.WM_SYSCOMMAND:
                        // Is this the command for closing the form?
                        if ((int)m.WParam.ToInt64() == Win32.SC_CLOSE)
                        {
                            PropertyInfo pi = typeof(Form).GetProperty("CloseReason",BindingFlags.Instance | BindingFlags.SetProperty | BindingFlags.NonPublic);

                            // Update form with the reason for the close
                            pi.SetValue(this, CloseReason.UserClosing, null);
                        }

                        if ((int)m.WParam.ToInt64() != 61696)
                            processed = OnPaintNonClient(ref m);
                        break;
                    case 0x00AE:
                    case 0xC1BC:
                        m.Result = (IntPtr)(0);
                        processed = true;
                        Win32.SendFrameChanged(Handle);

                        break;
                }
            }

            // If the message has not been handled, let base class process it
            if (!processed)
                base.WndProc(ref m);
        }



        private void OnWMMove(ref Message m)
        {
            Point screenPoint = new Point((int)m.LParam.ToInt64());


        }

        protected virtual Region GetDefaultFormRegion(ref Rectangle rect)
        {
            rect = Rectangle.Empty;
            return null;
        }

        private void SetRegion(Region region, Rectangle rect)
        {
            if (this.regionRect == rect)
            {
                if (region != null)
                    region.Dispose();
                return;
            }

            if (Region != null)
            {
                Region.Dispose();
            }

            Region = region;

            if (object.Equals(region, Region))
                this.regionRect = rect;
        }

        #region Windows Message Handlers


        private void OnWMSize(ref Message m)
        {
            if (!IsHandleCreated)
            {
                return;
            }

            if (WindowState == FormWindowState.Maximized)
            {
                Screen screen = Screen.FromHandle(Handle);
                if (screen == null) return;
                Rectangle bounds = FormBorderStyle == FormBorderStyle.None ? screen.Bounds : screen.WorkingArea;


                Win32.RECT windowRect = new Win32.RECT();
                Win32.GetWindowRect(Handle, ref windowRect);

                Rectangle formBounds = new Rectangle(windowRect.left, windowRect.top, windowRect.right - windowRect.left, windowRect.bottom - windowRect.top);

                if (formBounds.X == -10000 || formBounds.Y == -10000)
                    return;

                Rectangle r = new Rectangle(bounds.X - formBounds.X, bounds.Y - formBounds.Y, formBounds.Width - (formBounds.Width - bounds.Width), formBounds.Height - (formBounds.Height - bounds.Height));

                SetRegion(new Region(r), r);
            }
            else if (WindowState == FormWindowState.Minimized)
            {
                SetRegion(null, Rectangle.Empty);

                return;
            }
            else
            {


                Rectangle rect = new Rectangle();
                Region region = GetDefaultFormRegion(ref rect);
                SetRegion(region, rect);

            }

            


        }

        private void NCCalcSize(ref Message m)
        {


            if (m.WParam == (IntPtr)1)
            {


                var ncMargin = FormStyleHelper.GetWindowRealNCMargin(this, CreateParams);



                var ncCalcSizeParams = (Win32.NCCALCSIZE_PARAMS)Marshal.PtrToStructure(m.LParam, typeof(Win32.NCCALCSIZE_PARAMS));

                if (SavedBorders == null)
                {
                    SavedBorders = RealWindowBorders;
                }

                Padding calculatedClientMargin = this.ClientMargin;



                if (FormBorderStyle == FormBorderStyle.None)
                {
                    calculatedClientMargin = Padding.Empty;
                    ncMargin = Padding.Empty;
                }





                ncCalcSizeParams.rectProposed.top -= ncMargin.Top;


                ncCalcSizeParams.rectBeforeMove = ncCalcSizeParams.rectProposed;



                if (WindowState != FormWindowState.Maximized)
                {
                    ncCalcSizeParams.rectProposed.right += ncMargin.Right;
                    ncCalcSizeParams.rectProposed.bottom += ncMargin.Bottom;
                    ncCalcSizeParams.rectProposed.left -= ncMargin.Left;

                    ncCalcSizeParams.rectProposed.top += calculatedClientMargin.Top;
                    ncCalcSizeParams.rectProposed.left += calculatedClientMargin.Left;
                    ncCalcSizeParams.rectProposed.right -= calculatedClientMargin.Right;
                    ncCalcSizeParams.rectProposed.bottom -= calculatedClientMargin.Bottom;

                }
                else if (WindowState == FormWindowState.Maximized)
                {
                    ncCalcSizeParams.rectProposed.top += ncMargin.Bottom;
                }




                Marshal.StructureToPtr(ncCalcSizeParams, m.LParam, false);
                m.Result = (IntPtr)0x400;



            }

        }

        private void OnWMNCPaint(ref Message m)
        {
            if (!this.IsHandleCreated)
            {
                return;
            }

            Win32.RECT windowRect = new Win32.RECT();


            Win32.GetWindowRect(Handle, ref windowRect);
            Win32.OffsetRect(ref windowRect, -windowRect.left, -windowRect.top);


            Rectangle bounds = RealWindowRectangle;

            Win32.GetClientRect(Handle, out var clientRect);
            Rectangle clientBounds = new Rectangle(clientRect.left, clientRect.top, clientRect.right, clientRect.bottom);
            clientBounds.Offset(-clientBounds.Left, -clientBounds.Top);
            clientBounds.Offset(ClientMargin.Left, ClientMargin.Top);



            if (bounds.Width <= 0 || bounds.Height <= 0)
            {
                return;
            }

            uint getDCEXFlags = Win32.DCX_WINDOW | Win32.DCX_CACHE | Win32.DCX_CLIPSIBLINGS /*| Win32.DCX_VALIDATE*/;
            IntPtr hRegion = IntPtr.Zero;

            if (m.WParam != (IntPtr)1)
            {
                getDCEXFlags |= Win32.DCX_INTERSECTRGN;
                hRegion = m.WParam;
            }


            IntPtr hDC = Win32.GetDCEx(Handle, hRegion, getDCEXFlags);

            try
            {
                if (hDC != IntPtr.Zero)
                {


                    var chromeRegion = new Region(bounds);

                    if (WindowState == FormWindowState.Maximized)
                    {
                        chromeRegion.Exclude(bounds);
                    }
                    else
                    {
                        chromeRegion.Exclude(clientBounds);
                    }


                    using (Graphics drawingSurface = Graphics.FromHdc(hDC))
                    {
                        var borderColor = WindowActive ? BorderColor : InactiveBorderColor;



                        drawingSurface.FillRegion(new SolidBrush(borderColor), chromeRegion);

                    }
                }
            }
            finally
            {
                Win32.ReleaseDC(m.HWnd, hDC);
            }

            m.Result = Win32.MESSAGE_PROCESS;

        }




        /// <summary>
        /// Process the WM_NCHITTEST message when overriding window chrome.
        /// </summary>
        /// <param name="m">A Windows-based message.</param>
        /// <returns>True if the message was processed; otherwise false.</returns>
        protected virtual bool OnWMNCHITTEST(ref Message m)
        {
            // Extract the point in screen coordinates
            Point screenPoint = new Point((int)m.LParam.ToInt64());

            // Convert to window coordinates
            Point windowPoint = ScreenToWindow(screenPoint);

            // Perform hit testing
            m.Result = WindowChromeHitTest(windowPoint, false);

            // Message processed, do not pass onto base class for processing
            return true;
        }

        /// <summary>
        /// Process the WM_NCACTIVATE message when overriding window chrome.
        /// </summary>
        /// <param name="m">A Windows-based message.</param>
        /// <returns>True if the message was processed; otherwise false.</returns>
        protected virtual bool OnWMNCACTIVATE(ref Message m)
        {
            // Cache the new active state
            WindowActive = (m.WParam == (IntPtr)(1));

            InvalidateNonClient();

            // The first time an MDI child gets an WM_NCACTIVATE, let it process as normal
            if ((MdiParent != null) && !_activated)
                _activated = true;
            else
            {
                // Allow default processing of activation change
                m.Result = (IntPtr)(1);
                // Message processed, do not pass onto base class for processing
                return true;
            }

            return false;
        }

        /// <summary>
        /// Process a windows message that requires the non client area be repainted.
        /// </summary>
        /// <param name="m">A Windows-based message.</param>
        /// <returns>True if the message was processed; otherwise false.</returns>
        protected virtual bool OnPaintNonClient(ref Message m)
        {
            // Let window be updated with new text
            DefWndProc(ref m);

            // Need a repaint to show change
            InvalidateNonClient();

            // Message processed, do not pass onto base class for processing
            return true;
        }

        /// <summary>
        /// Process the WM_NCMOUSEMOVE message when overriding window chrome.
        /// </summary>
        /// <param name="m">A Windows-based message.</param>
        /// <returns>True if the message was processed; otherwise false.</returns>
        protected virtual bool OnWMNCMOUSEMOVE(ref Message m)
        {
            // Extract the point in screen coordinates
            Point screenPoint = new Point((int)m.LParam.ToInt64());

            // Convert to window coordinates
            Point windowPoint = ScreenToWindow(screenPoint);


            // Perform actual mouse movement actions
            WindowChromeNonClientMouseMove(windowPoint);

            // If we are not tracking when the mouse leaves the non-client window
            if (!_trackingMouse)
            {
                Win32.TRACKMOUSEEVENTS tme = new Win32.TRACKMOUSEEVENTS();

                // This structure needs to know its own size in bytes
                tme.cbSize = (uint)Marshal.SizeOf(typeof(Win32.TRACKMOUSEEVENTS));
                tme.dwHoverTime = 100;

                // We need to know then the mouse leaves the non client window area
                tme.dwFlags = (int)(Win32.TME_LEAVE | Win32.TME_NONCLIENT);

                // We want to track our own window
                tme.hWnd = Handle;

                // Call Win32 API to start tracking
                Win32.TrackMouseEvent(ref tme);

                // Do not need to track again until mouse reenters the window
                _trackingMouse = true;
            }

            // Indicate that we processed the message
            m.Result = IntPtr.Zero;

            // Message processed, do not pass onto base class for processing
            return true;
        }


        /// <summary>
        /// Process the WM_NCLBUTTONDOWN message when overriding window chrome.
        /// </summary>
        /// <param name="m">A Windows-based message.</param>4
        /// <returns>True if the message was processed; otherwise false.</returns>
        protected virtual bool OnWMNCLBUTTONDOWN(ref Message m)
        {
            // Extract the point in screen coordinates
            Point screenPoint = new Point((int)m.LParam.ToInt64());

            // Convert to window coordinates
            Point windowPoint = ScreenToWindow(screenPoint);

            // Perform actual mouse down processing
            return WindowChromeLeftMouseDown(windowPoint);
        }

        /// <summary>
        /// Process the WM_LBUTTONUP message when overriding window chrome.
        /// </summary>
        /// <param name="m">A Windows-based message.</param>
        /// <returns>True if the message was processed; otherwise false.</returns>
        protected virtual bool OnWMNCLBUTTONUP(ref Message m)
        {
            // Extract the point in screen coordinates
            Point screenPoint = new Point((int)m.LParam.ToInt64());

            // Convert to window coordinates
            Point windowPoint = ScreenToWindow(screenPoint);


            // Perform actual mouse up processing
            return WindowChromeLeftMouseUp(windowPoint);
        }

        private bool OnWMNCLBUTTONDBLCLK(ref Message m)
        {
            return true;
        }

        /// <summary>
        /// Process the WM_NCMOUSELEAVE message when overriding window chrome.
        /// </summary>
        /// <param name="m">A Windows-based message.</param>
        /// <returns>True if the message was processed; otherwise false.</returns>
        protected virtual bool OnWMNCMOUSELEAVE(ref Message m)
        {
            // Next time the mouse enters the window we need to track it leaving
            _trackingMouse = false;

            // Perform actual mouse leave actions
            WindowChromeMouseLeave();

            // Indicate that we processed the message
            m.Result = IntPtr.Zero;

            // Need a repaint to show change
            InvalidateNonClient();

            // Message processed, do not pass onto base class for processing
            return true;
        }

        /// <summary>
        /// Process the OnWM_MOUSEMOVE message when overriding window chrome.
        /// </summary>
        /// <param name="m">A Windows-based message.</param>
        /// <returns>True if the message was processed; otherwise false.</returns>
        protected virtual bool OnWMMOUSEMOVE(ref Message m)
        {
            // Extract the point in client coordinates
            Point clientPoint = new Point((int)m.LParam);

            // Convert to screen coordinates
            Point screenPoint = PointToScreen(clientPoint);

            // Convert to window coordinates
            Point windowPoint = ScreenToWindow(screenPoint);

            // Perform actual mouse movement actions
            WindowChromeNonClientMouseMove(windowPoint);

            return true;
        }

        /// <summary>
        /// Process the WM_LBUTTONUP message when overriding window chrome.
        /// </summary>
        /// <param name="m">A Windows-based message.</param>
        /// <returns>True if the message was processed; otherwise false.</returns>
        protected virtual bool OnWMLBUTTONUP(ref Message m)
        {
            // Capture has now expired
            _captured = false;
            Capture = false;

            // No longer have a target element for events
            //_capturedElement = null;

            // Next time the mouse enters the window we need to track it leaving
            _trackingMouse = false;

            // Extract the point in client coordinates
            Point clientPoint = new Point((int)m.LParam);

            // Convert to screen coordinates
            Point screenPoint = PointToScreen(clientPoint);

            // Convert to window coordinates
            Point windowPoint = ScreenToWindow(screenPoint);


            // Need a repaint to show change
            InvalidateNonClient();

            return true;
        }

        /// <summary>
        /// Called when the active state of the window changes.
        /// </summary>
        protected virtual void OnWindowActiveChanged()
        {
            WindowActiveChanged?.Invoke(this, EventArgs.Empty);
        }


        /// <summary>
        /// Perform hit testing.
        /// </summary>
        /// <param name="pt">Point in window coordinates.</param>
        /// <param name="composition">Are we performing composition.</param>
        /// <returns></returns>
        protected virtual IntPtr WindowChromeHitTest(Point pt, bool composition)
        {
            return (IntPtr)Win32.HTCLIENT;
        }


        /// <summary>
        /// Perform painting of the window chrome.
        /// </summary>
        /// <param name="g">Graphics instance to use for drawing.</param>
        /// <param name="bounds">Bounds enclosing the window chrome.</param>
        protected virtual void WindowChromePaint(Graphics g, Rectangle bounds)
        {
            g.FillRectangle(new SolidBrush(Color.Red), bounds);
        }

        /// <summary>
        /// Perform non-client mouse movement processing.
        /// </summary>
        /// <param name="pt">Point in window coordinates.</param>
        protected virtual void WindowChromeNonClientMouseMove(Point pt)
        {

        }

        /// <summary>
        /// Process the left mouse down event.
        /// </summary>
        /// <param name="pt">Window coordinate of the mouse up.</param>
        /// <returns>True if event is processed; otherwise false.</returns>
        protected virtual bool WindowChromeLeftMouseDown(Point pt)
        {
            
            
            return false;
        }

        /// <summary>
        /// Process the left mouse up event.
        /// </summary>
        /// <param name="pt">Window coordinate of the mouse up.</param>
        /// <returns>True if event is processed; otherwise false.</returns>
        protected virtual bool WindowChromeLeftMouseUp(Point pt)
        {
            // By default, we have not handled the mouse up event
            return false;
        }

        /// <summary>
        /// Perform mouse leave processing.
        /// </summary>
        protected virtual void WindowChromeMouseLeave()
        {

        }
        #endregion




    }
}
