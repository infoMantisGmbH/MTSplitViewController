//  MTSplitViewController
//  https://github.com/Krumelur/MTSplitViewController
// 
//  Ported to Monotouch by René Ruppert, October 2011
//  Ported to Xamarin.iOS by infoMantis GmbH, April 2015
//
//  Based on code by Matt Gemmell on 26/07/2010.
//  Copyright 2010 Instinctive Code.
//  https://github.com/mattgemmell/MGSplitViewController

using System;
using System.Diagnostics.CodeAnalysis;
using CoreGraphics;
using Foundation;
using UIKit;

namespace MTSplitViewLib
{
	public class MTSplitViewController : UIViewController
	{
		// default width of master view in UISplitViewController.
		private static readonly nfloat DefaultSplitPosition = 320.0f;
		// default width of split-gutter in UISplitViewController.
		private static readonly nfloat DefaultSplitWidth = 1.0f;
		// default corner-radius of overlapping split-inner corners on the master and detail views.
		private static readonly nfloat DefaultCornerRadius = 5.0f;
		// default color of intruding inner corners (and divider background).
		private static readonly UIColor DefaultCornerColor = UIColor.Black;
		// corner-radius of split-inner corners for MGSplitViewDividerStylePaneSplitter style.
		private static readonly nfloat PanesplitterCornerRadius = 0.0f;
		// width of split-gutter for MGSplitViewDividerStylePaneSplitter style.
		private static readonly nfloat PanesplitterSplitWidth = 25.0f;
		// minimum width a view is allowed to become as a result of changing the splitPosition.
		private static readonly nfloat MinViewWidth = 200.0f;

		public MTSplitViewController()
		{
			Setup();
		}

		public MTSplitViewController(NSCoder coder) : base(coder)
		{
			Setup();
		}

		public MTSplitViewController(NSObjectFlag t) : base(t)
		{
			Setup();
		}

		public MTSplitViewController(IntPtr handle) : base(handle)
		{
			Setup();
		}

		public MTSplitViewController(string nibName, NSBundle bundle) : base(nibName, bundle)
		{
			Setup();
		}

		/// <summary>
		/// Will be triggered if the orientation of the panels changes.
		/// </summary>
		public event Action<MTSplitViewController, bool> WillChangeSplitOrientationToVertical;

		/// <summary>
		/// Will be triggered if the user wants to change the split position.
		/// </summary>
		public event Func<MTSplitViewController, nfloat, CGSize, nfloat> ConstrainSplitPosition;

		/// <summary>
		/// Will be triggered if the split position is moved.
		/// </summary>
		public event Action<MTSplitViewController, nfloat> WillMoveSplitToPosition;

		/// <summary>
		/// Will be triggered if the master view controller will be hidden.
		/// </summary>
		public event Action<MTSplitViewController, UIViewController, UIBarButtonItem, UIPopoverController> WillHideViewController;

		/// <summary>
		/// Will be called if the master view controller will be shown.
		/// </summary>
		public event Action<MTSplitViewController, UIViewController, UIBarButtonItem> WillShowViewController;

		/// <summary>
		/// Will be called if the master view controller will be shown in a popove.
		/// </summary>
		public event Action<MTSplitViewController, UIPopoverController, UIViewController> WillPresentViewController;

		// Indicates if the popover has to be reconfigured.
		private bool _reconfigurePopup;

		/// <summary>
		/// Gets or sets the hidden popover controller used to display the master view controller.
		/// </summary>
		public UIPopoverController HiddenPopoverController { get; set; }

		/// <summary>
		/// If TRUE, the controller provides a popover and a bar button item for the master controller if the master is hidden. Default is TRUE.
		/// </summary>
		public bool AutoProvidePopoverAndBarItem = true;

		/// <summary>
		/// Gets or sets the bar button item used to present the master view controller from.
		/// </summary>
		/// <value>
		/// The bar button item.
		/// </value>
		public UIBarButtonItem BarButtonItem { get; set; }

		/// <summary>
		/// Gets or sets a value indicating whether the master controller is shown in portrait mode.
		/// master in portrait.
		/// </summary>
		/// <value>
		/// <c>true</c> if shows master in portrait; otherwise, <c>false</c>.
		/// </value>
		public bool ShowsMasterInPortrait
		{
			get { return _showsMasterInPortrait; }
			set
			{
				if (value == ShowsMasterInPortrait) return;

				_showsMasterInPortrait = value;

				if (IsLandscape) return;

				// i.e. if this will cause a visual change.
				if (HiddenPopoverController != null && HiddenPopoverController.PopoverVisible)
				{
					HiddenPopoverController.Dismiss(false);
				}

				// Rearrange views.
				_reconfigurePopup = true;
				LayoutSubviews();
			}
		}
		private bool _showsMasterInPortrait;

		/// <summary>
		/// Gets or sets a value indicating whether the master controller is visible in landscape mode.
		/// master in landscape.
		/// </summary>
		/// <value>
		/// <c>true</c> if shows master in landscape; otherwise, <c>false</c>.
		/// </value>
		public bool ShowsMasterInLandscape
		{
			get { return _showsMasterInLandscape; }
			set
			{
				_showsMasterInLandscape = value;
				if (!IsLandscape) return;

				// i.e. if this will cause a visual change.
				if (HiddenPopoverController != null && HiddenPopoverController.PopoverVisible)
				{
					HiddenPopoverController.Dismiss(false);
				}

				// Rearrange views.
				_reconfigurePopup = true;
				LayoutSubviews();
			}
		}
		private bool _showsMasterInLandscape;

		/// <summary>
		/// If FALSE, split is horizontal, i.e. master above detail. If TRUE, split is vertical, i.e. master left of detail.
		/// </summary>
		/// <value>
		/// <c>true</c> if this instance is vertical; otherwise, <c>false</c>.
		/// </value>
		public bool IsVertical
		{
			get { return _isVertical; }
			set
			{
				if (value == IsVertical) return;

				if (HiddenPopoverController != null && HiddenPopoverController.PopoverVisible)
				{
					HiddenPopoverController.Dismiss(false);
				}

				_isVertical = value;

				// Inform delegate.
				if (WillChangeSplitOrientationToVertical != null)
				{
					WillChangeSplitOrientationToVertical(this, _isVertical);
				}

				LayoutSubviews();
			}
		}
		private bool _isVertical;

		/// <summary>
		/// If FALSE, master view is below/right of detail. Otherwise master view is above/left of detail.
		/// before detail.
		/// </summary>
		/// <value>
		/// <c>true</c> if master before detail; otherwise, <c>false</c>.
		/// </value>
		public bool MasterBeforeDetail
		{
			get { return _masterBeforeDetail; }
			set
			{
				if (value == MasterBeforeDetail) return;

				if (HiddenPopoverController != null && HiddenPopoverController.PopoverVisible)
				{
					HiddenPopoverController.Dismiss(false);
				}

				_masterBeforeDetail = value;

				if (IsShowingMaster)
				{
					LayoutSubviews();
				}
			}
		}
		private bool _masterBeforeDetail;

		/// <summary>
		/// Starting position of split in units, relative to top/left (depending on IsVertical setting) if MasterBeforeDetail is TRUE,
		/// else relative to bottom/right.
		/// </summary>
		/// <value>
		/// The split position.
		/// </value>
		public nfloat SplitPosition
		{
			get { return _splitPosition; }
			set
			{
				// Check to see if delegate wishes to constrain the position.
				bool constrained;
				var fullSize = SplitViewRectangleForOrientation(InterfaceOrientation).Size;
				var newPos = value;
				if (ConstrainSplitPosition != null)
				{
					newPos = ConstrainSplitPosition(this, value, fullSize);
					constrained = true; // implicitly trust delegate's response.
				}
				else
				{
					// Apply default constraints if delegate doesn't wish to participate.
					var minPos = MinViewWidth;
					var maxPos = (IsVertical ? fullSize.Width : fullSize.Height) - (MinViewWidth + SplitWidth);
					constrained = (newPos != SplitPosition && newPos >= minPos && newPos <= maxPos);
				}

				if (!constrained) return;

				if (HiddenPopoverController != null && HiddenPopoverController.PopoverVisible)
				{
					HiddenPopoverController.Dismiss(false);
				}

				_splitPosition = newPos;

				// Inform delegate.
				if (WillMoveSplitToPosition != null)
				{
					WillMoveSplitToPosition(this, SplitPosition);
				}

				if (IsShowingMaster)
				{
					LayoutSubviews();
				}
			}
		}

		/// <summary>
		/// Sets the split position animated.
		/// Note: split position is the width (in a left/right split, or height in a top/bottom split) of the master view.
		/// It is relative to the appropriate side of the splitView, which can be any of the four sides depending on the values
		/// in IsMasterBeforeDetail and IsVertical:
		/// IsVertical = YES, isMasterBeforeDetail = YES: splitPosition is relative to the LEFT edge. (Default)
		/// IsVertical = YES, isMasterBeforeDetail = FALSE: split position is relative to the RIGHT edge.
		/// IsVertical = NO, isMasterBeforeDetail = TRUE: split position is relative to the TOP edge.
		/// IsVertical = NO, isMasterBeforeDetail = FALSE: split position is relative to the BOTTOM edge.
		/// This implementation was chosen so you don't need to recalculate equivalent splitPositions if the user toggles masterBeforeDetail themselves.
		/// </summary>
		/// <param name='fNewPos'>the new position in units</param>
		public void SetSplitPositionAnimated(float fNewPos)
		{
			var animate = IsShowingMaster;
			if (animate)
			{
				UIView.BeginAnimations("SplitPosition");
			}
			SplitPosition = fNewPos;
			if (animate)
			{
				UIView.CommitAnimations();
			}
		}

		private nfloat _splitPosition;

		/// <summary>
		/// Gets or sets the width of the split.
		/// </summary>
		/// <value>
		/// The width of the split.
		/// </value>
		public nfloat SplitWidth
		{
			get { return _splitWidth; }
			set
			{
				if (value != SplitWidth && value >= 0)
				{
					_splitWidth = value;
					if (IsShowingMaster)
					{
						LayoutSubviews();
					}
				}
			}
		}

		private nfloat _splitWidth;

		/// <summary>
		/// Whether to let the user drag the divider to alter the split position.
		/// dragging divider.
		/// </summary>
		/// <value>
		/// <c>true</c> if allows dragging divider; otherwise, <c>false</c>.
		/// </value>
		public bool AllowsDraggingDivider
		{
			get { return DividerView != null && DividerView.AllowsDragging; }
			set
			{
				if (DividerView != null)
				{
					DividerView.AllowsDragging = value;
				}
			}
		}

		/// <summary>
		/// Array of UIViewControllers; master is at index 0, detail is at index 1.
		/// </summary>
		/// <value>
		/// The view controllers.
		/// </value>
		/// <exception cref='ArgumentException'>
		/// Is thrown when an argument passed to a method is invalid.
		/// </exception>
		public virtual UIViewController[] ViewControllers
		{
			get { return _viewControllers; }
			set
			{
				if (_viewControllers != null)
				{
					foreach (var oController in _viewControllers)
					{
						if (oController == null)
						{
							continue;
						}
						oController.View.RemoveFromSuperview();
					}
				}

				_viewControllers = new UIViewController[2];
				if (value != null && value.Length >= 2)
				{
					MasterViewController = value[0];
					DetailViewController = value[1];
				}
				else
				{
					throw new ArgumentException("Error: This component requires 2 view-controllers");
				}

				LayoutSubviews();
			}
		}

		private UIViewController[] _viewControllers;

		/// <summary>
		/// Gets or sets the master view controller.
		/// </summary>
		/// <value>
		/// The master view controller.
		/// </value>
		public virtual UIViewController MasterViewController
		{
			get
			{
				if (ViewControllers != null && ViewControllers.Length > 0)
				{
					return ViewControllers[0];
				}
				return null;
			}
			set
			{
				if (ViewControllers == null)
				{
					ViewControllers = new UIViewController[2];
					ViewControllers[1] = null;
				}

				if (ViewControllers[0] == value)
				{
					return;
				}

				// We need to remove the controller's view, otherwise it will float around as a zombie.
				if (ViewControllers[0] != null && ViewControllers[0].View != null)
				{
					ViewControllers[0].View.RemoveFromSuperview();
				}

				ViewControllers[0] = value;
				LayoutSubviews();
			}
		}

		/// <summary>
		/// Gets or sets the detail view controller.
		/// </summary>
		/// <value>
		/// The detail view controller.
		/// </value>
		public virtual UIViewController DetailViewController
		{
			get
			{
				if (ViewControllers != null && ViewControllers.Length >= 2)
				{
					return ViewControllers[1];
				}
				return null;
			}
			set
			{
				if (ViewControllers == null)
				{
					ViewControllers = new UIViewController[2];
					ViewControllers[0] = null;
				}

				if (ViewControllers[1] == value)
				{
					return;
				}

				// We need to remove the controller's view, otherwise it will float around as a zombie.
				if (ViewControllers[1] != null && ViewControllers[1].View != null)
				{
					ViewControllers[1].View.RemoveFromSuperview();
				}

				ViewControllers[1] = value;
				LayoutSubviews();
			}
		}

		/// <summary>
		/// Gets or sets the divider view.
		/// </summary>
		/// <value>
		/// The divider view.
		/// </value>
		public MTSplitDividerView DividerView
		{
			get { return _dividerView; }
			set
			{
				if (value == _dividerView)
				{
					return;
				}
				if (_dividerView != null)
				{
					_dividerView.Dispose();
				}
				_dividerView = value;
				_dividerView.SplitViewController = this;
				_dividerView.BackgroundColor = DefaultCornerColor;
				if (IsShowingMaster)
				{
					LayoutSubviews();
				}
			}
		}

		private MTSplitDividerView _dividerView;

		/// <summary>
		/// Gets or sets the divider style.
		/// </summary>
		/// <value>
		/// The divider style.
		/// </value>
		public MTSplitDividerView.DividerStyle DividerStyle
		{
			get { return _dividerStyle; }
			set
			{
				if (HiddenPopoverController != null && HiddenPopoverController.PopoverVisible)
				{
					HiddenPopoverController.Dismiss(false);
				}

				// We don't check to see if newStyle equals _dividerStyle, because it's a meta-setting.
				// Aspects could have been changed since it was set.
				_dividerStyle = value;

				// Reconfigure general appearance and behaviour.
				nfloat cornerRadius = 0f;
				if (_dividerStyle == MTSplitDividerView.DividerStyle.Thin)
				{
					cornerRadius = DefaultCornerRadius;
					SplitWidth = DefaultSplitWidth;
					AllowsDraggingDivider = false;
				}
				else if (_dividerStyle == MTSplitDividerView.DividerStyle.PaneSplitter)
				{
					cornerRadius = PanesplitterCornerRadius;
					SplitWidth = PanesplitterSplitWidth;
					AllowsDraggingDivider = true;
				}

				// Update divider and corners.
				if (DividerView != null)
				{
					DividerView.SetNeedsDisplay();
				}
				if (CornerViews != null)
				{
					foreach (var corner in CornerViews)
					{
						corner.CornerRadius = cornerRadius;
					}
				}
				// Layout all views.
				LayoutSubviews();
			}
		}

		private MTSplitDividerView.DividerStyle _dividerStyle;

		/// <summary>
		/// Sets the divider style animated.
		/// </summary>
		public void SetDividerStyleAnimated(MTSplitDividerView.DividerStyle eStyle)
		{
			var animate = IsShowingMaster;
			if (animate)
			{
				UIView.BeginAnimations("DividerStyle");
			}
			DividerStyle = eStyle;
			if (animate)
			{
				UIView.CommitAnimations();
			}
		}


		/// <summary>
		/// Returns TRUE if this view controller is in either of the two Landscape orientations, else FALSE.
		/// </summary>
		public bool IsLandscape
		{
			get { return InterfaceOrientation == UIInterfaceOrientation.LandscapeLeft || InterfaceOrientation == UIInterfaceOrientation.LandscapeRight; }
		}

		/// <summary>
		/// Gets a value indicating whether the master controller is currently shown.
		/// </summary>
		public bool IsShowingMaster
		{
			get
			{
				return ShouldShowMaster
				       && MasterViewController != null
				       && MasterViewController.View != null
				       && MasterViewController.View.Superview == View;
			}
		}

		/// <summary>
		/// Finds out if the master controller should be shown.
		/// </summary>
		/// <value>
		/// <c>true</c> if should show master; otherwise, <c>false</c>.
		/// </value>
		private bool ShouldShowMaster
		{
			get { return ShouldShowMasterForInterfaceOrientation(InterfaceOrientation); }
		}

		/// <summary>
		/// Gets or sets the corner views.
		/// Returns an array of two MGSplitCornersView objects, used to draw the inner corners.
		/// The first view is the "leading" corners (top edge of screen for left/right split, left edge of screen for top/bottom split).
		/// The second view is the "trailing" corners (bottom edge of screen for left/right split, right edge of screen for top/bottom split).
		/// Do NOT modify them, except to:
		/// 1. Change their background color
		/// 2. Change their corner radius
		/// </summary>
		/// <value>
		/// The corner views.
		/// </value>
		public MTSplitCornersView[] CornerViews { get; set; }

		/// <summary>
		/// Gets a human readable name of the interface orientation.
		/// </summary>
		/// <returns>
		/// The of interface orientation.
		/// </returns>
		/// <param name='theOrientation'>
		/// The orientation.
		/// </param>
		public string NameOfInterfaceOrientation(UIInterfaceOrientation theOrientation)
		{
			string sOrientationName = null;
			switch (theOrientation)
			{
				case UIInterfaceOrientation.Portrait:
					sOrientationName = "Portrait"; // Home button at bottom
					break;
				case UIInterfaceOrientation.PortraitUpsideDown:
					sOrientationName = "Portrait (Upside Down)"; // Home button at top
					break;
				case UIInterfaceOrientation.LandscapeLeft:
					sOrientationName = "Landscape (Left)"; // Home button on left
					break;
				case UIInterfaceOrientation.LandscapeRight:
					sOrientationName = "Landscape (Right)"; // Home button on right
					break;
			}

			return sOrientationName;
		}

		/// <summary>
		/// Returns TRUE if master view should be shown directly embedded in the splitview, instead of hidden in a popover.
		/// </summary>
		private bool ShouldShowMasterForInterfaceOrientation(UIInterfaceOrientation eOrientation)
		{
			if (eOrientation == UIInterfaceOrientation.LandscapeLeft || eOrientation == UIInterfaceOrientation.LandscapeRight)
			{
				return ShowsMasterInLandscape;
			}
			return ShowsMasterInPortrait;
		}

		/// <summary>
		/// Setup this instance's basic properties.
		/// </summary>
		private void Setup()
		{
			// Configure default behaviour.
			SplitWidth = DefaultSplitWidth;
			ShowsMasterInPortrait = false;
			ShowsMasterInLandscape = true;
			_reconfigurePopup = false;
			IsVertical = true;
			MasterBeforeDetail = true;
			SplitPosition = DefaultSplitPosition;
			var divRect = View.Bounds;
			if (IsVertical)
			{
				divRect.Y = SplitPosition;
				divRect.Height = SplitWidth;
			}
			else
			{
				divRect.X = SplitPosition;
				divRect.Width = SplitWidth;
			}
			DividerView = new MTSplitDividerView(divRect)
			{
				SplitViewController = this,
				BackgroundColor = DefaultCornerColor
			};
			DividerStyle = MTSplitDividerView.DividerStyle.Thin;
		}

		[Obsolete]
#pragma warning disable CS0809 // Obsolete member overrides non-obsolete member
		public override bool ShouldAutorotateToInterfaceOrientation(UIInterfaceOrientation toInterfaceOrientation)
#pragma warning restore CS0809 // Obsolete member overrides non-obsolete member
		{
			return true;
		}

		public override bool ShouldAutorotate()
		{
			return true;
		}

		public override void WillRotate(UIInterfaceOrientation toInterfaceOrientation, double duration)
		{
			if (MasterViewController != null)
			{
				MasterViewController.WillRotate(toInterfaceOrientation, duration);
			}
			if (DetailViewController != null)
			{
				DetailViewController.WillRotate(toInterfaceOrientation, duration);
			}
		}

		public override void DidRotate(UIInterfaceOrientation fromInterfaceOrientation)
		{
			if (MasterViewController != null)
			{
				MasterViewController.DidRotate(fromInterfaceOrientation);
			}
			if (DetailViewController != null)
			{
				DetailViewController.DidRotate(fromInterfaceOrientation);
			}
		}

		public override void WillAnimateRotation(UIInterfaceOrientation toInterfaceOrientation, double duration)
		{
			if (MasterViewController != null)
			{
				MasterViewController.WillAnimateRotation(toInterfaceOrientation, duration);
			}
			if (DetailViewController != null)
			{
				DetailViewController.WillAnimateRotation(toInterfaceOrientation, duration);
			}

			// Hide popover.
			if (HiddenPopoverController != null && HiddenPopoverController.PopoverVisible)
			{
				HiddenPopoverController.Dismiss(false);
			}

			// Re-tile views.
			_reconfigurePopup = true;
			LayoutSubviewsForInterfaceOrientation(toInterfaceOrientation, true);
		}

		public override void WillAnimateFirstHalfOfRotation(UIInterfaceOrientation toInterfaceOrientation, double duration)
		{
			if (MasterViewController != null)
			{
				MasterViewController.WillAnimateFirstHalfOfRotation(toInterfaceOrientation, duration);
			}
			if (DetailViewController != null)
			{
				DetailViewController.WillAnimateFirstHalfOfRotation(toInterfaceOrientation, duration);
			}
		}

		public override void DidAnimateFirstHalfOfRotation(UIInterfaceOrientation toInterfaceOrientation)
		{
			if (MasterViewController != null)
			{
				MasterViewController.DidAnimateFirstHalfOfRotation(toInterfaceOrientation);
			}

			if (DetailViewController != null)
			{
				DetailViewController.DidAnimateFirstHalfOfRotation(toInterfaceOrientation);
			}
		}

		public override void WillAnimateSecondHalfOfRotation(UIInterfaceOrientation fromInterfaceOrientation, double duration)
		{
			if (MasterViewController != null)
			{
				MasterViewController.WillAnimateSecondHalfOfRotation(fromInterfaceOrientation, duration);
			}
			if (DetailViewController != null)
			{
				DetailViewController.WillAnimateSecondHalfOfRotation(fromInterfaceOrientation, duration);
			}
		}

		protected virtual void LayoutSubviewsWithAnimation(bool animate)
		{
			LayoutSubviewsForInterfaceOrientation(InterfaceOrientation, animate);
		}

		protected virtual void LayoutSubviews()
		{
			LayoutSubviewsForInterfaceOrientation(InterfaceOrientation, true);
		}

		public override void ViewWillAppear(bool animated)
		{
			base.ViewWillAppear(animated);

			if (IsShowingMaster)
			{
				MasterViewController.ViewWillAppear(animated);
			}
			if (DetailViewController != null)
			{
				DetailViewController.ViewWillAppear(animated);
			}
		}

		public override void ViewDidAppear(bool animated)
		{
			base.ViewDidAppear(animated);

			if (IsShowingMaster)
			{
				MasterViewController.ViewDidAppear(animated);
			}
			if (DetailViewController != null)
			{
				DetailViewController.ViewDidAppear(animated);
			}
			_reconfigurePopup = true;
			LayoutSubviews();
		}

		public override void ViewWillDisappear(bool animated)
		{
			base.ViewWillDisappear(animated);

			if (IsShowingMaster)
			{
				MasterViewController.ViewWillDisappear(animated);
			}
			if (DetailViewController != null)
			{
				DetailViewController.ViewWillDisappear(animated);
			}
		}

		public override void ViewDidDisappear(bool animated)
		{
			base.ViewDidDisappear(animated);

			if (IsShowingMaster)
			{
				MasterViewController.ViewDidDisappear(animated);
			}
			if (DetailViewController != null)
			{
				DetailViewController.ViewDidDisappear(animated);
			}
		}

		/// <summary>
		/// Gets the rectangle that will be used to place the master and detail controller in for a sepcific orientation.
		/// You can override this if you don't want to use the full screen to be used for the split controller.
		/// </summary>
		/// <returns>
		/// The rectangle for the requested orientation.
		/// </returns>
		/// <param name='theOrientation'>
		/// The orientation.
		/// </param>
		protected virtual CGRect SplitViewRectangleForOrientation(UIInterfaceOrientation theOrientation)
		{
			return View.Bounds;
		}

		/// <summary>
		/// Layouts the subviews.
		/// </summary>
		protected virtual void LayoutSubviewsForInterfaceOrientation(UIInterfaceOrientation eOrientation, bool animate)
		{
			if (_reconfigurePopup)
			{
				ReconfigureForMasterInPopover(!ShouldShowMasterForInterfaceOrientation(eOrientation));
			}

			// Layout the master, detail and divider views appropriately, adding/removing subviews as needed.
			// First obtain relevant geometry.
			var mainRect = SplitViewRectangleForOrientation(eOrientation);
			var fullSize = mainRect.Size;
			var width = fullSize.Width;
			var height = fullSize.Height;

#if DEBUG
			//Console.WriteLine("Target orientation is " + this.NameOfInterfaceOrientation(eOrientation) + " dimensions will be " + width + " x " + height );
#endif

			// Layout the master, divider and detail views.
			var eNewFrame = mainRect; // new RectangleF (0, 0, width, height);
			UIViewController oController;
			UIView oView = null;
			var bShouldShowMaster = ShouldShowMasterForInterfaceOrientation(eOrientation);
			var bMasterFirst = MasterBeforeDetail;
			if (IsVertical)
			{
				// Master on left, detail on right (or vice versa).
				CGRect oMasterRect;
				CGRect oDividerRect;
				CGRect oDetailRect;
				if (bMasterFirst)
				{
					if (!bShouldShowMaster)
					{
						// Move off-screen.
						eNewFrame.X -= (SplitPosition + SplitWidth);
					}
					eNewFrame.Width = SplitPosition;
					oMasterRect = eNewFrame;

					eNewFrame.X += eNewFrame.Width;
					eNewFrame.Width = SplitWidth;
					oDividerRect = eNewFrame;

					eNewFrame.X += eNewFrame.Width;
					eNewFrame.Width = width - eNewFrame.X;
					oDetailRect = eNewFrame;
				}
				else
				{
					if (!bShouldShowMaster)
					{
						// Move off-screen.
						eNewFrame.Width += (SplitPosition + SplitWidth);
					}

					eNewFrame.Width -= (SplitPosition + SplitWidth);
					oDetailRect = eNewFrame;

					eNewFrame.X += eNewFrame.Width;
					eNewFrame.Width = SplitWidth;
					oDividerRect = eNewFrame;

					eNewFrame.X += eNewFrame.Width;
					eNewFrame.Width = SplitPosition;
					oMasterRect = eNewFrame;
				}

				// Position master.
				oController = MasterViewController;
				if (oController != null)
				{
					oView = oController.View;
					if (oView != null)
					{
						oView.Frame = oMasterRect;
						if (oView.Superview == null)
						{
							oController.ViewWillAppear(false);
							View.AddSubview(oView);
							oController.ViewDidAppear(false);
						}
					}
				}

				// Position divider.
				if (oView != null)
				{
					oView = DividerView;
					oView.Frame = oDividerRect;
					if (oView.Superview == null)
					{
						View.AddSubview(oView);
					}
				}

				// Position detail.
				oController = DetailViewController;
				if (oController != null)
				{
					oView = oController.View;
					if (oView != null)
					{
						oView.Frame = oDetailRect;
						if (oView.Superview == null)
						{
							oController.ViewWillAppear(false);
							View.InsertSubviewAbove(oView, MasterViewController.View);
							oController.ViewDidAppear(false);
						}
						else
						{
							View.BringSubviewToFront(oView);
						}
					}
				}
			}
			else
			{
				// Master above, detail below (or vice versa).
				CGRect oMasterRect, oDividerRect, oDetailRect;
				if (bMasterFirst)
				{
					if (!bShouldShowMaster)
					{
						// Move off-screen.
						eNewFrame.Y -= (SplitPosition + SplitWidth);
					}

					eNewFrame.Height = SplitPosition;
					oMasterRect = eNewFrame;

					eNewFrame.Y += eNewFrame.Height;
					eNewFrame.Height = SplitWidth;
					oDividerRect = eNewFrame;

					eNewFrame.Y += eNewFrame.Height;
					eNewFrame.Height = height - eNewFrame.Y;
					oDetailRect = eNewFrame;
				}
				else
				{
					if (!bShouldShowMaster)
					{
						// Move off-screen.
						eNewFrame.Height += (SplitPosition + SplitWidth);
					}

					eNewFrame.Height -= (SplitPosition + SplitWidth);
					oDetailRect = eNewFrame;

					eNewFrame.Y += eNewFrame.Height;
					eNewFrame.Height = SplitWidth;
					oDividerRect = eNewFrame;

					eNewFrame.Y += eNewFrame.Height;
					eNewFrame.Height = SplitPosition;
					oMasterRect = eNewFrame;
				}

				// Position master.
				oController = MasterViewController;
				if (oController != null)
				{
					oView = oController.View;
					if (oView != null)
					{
						oView.Frame = oMasterRect;
						if (oView.Superview == null)
						{
							oController.ViewWillAppear(false);
							View.AddSubview(oView);
							oController.ViewDidAppear(false);
						}
					}
				}

				// Position divider.
				if (oView != null && DividerView != null)
				{
					oView = DividerView;
					oView.Frame = oDividerRect;
					if (oView.Superview == null)
					{
						View.AddSubview(oView);
					}
				}

				// Position detail.
				oController = DetailViewController;
				if (oController != null)
				{
					oView = oController.View;
					if (oView != null)
					{
						oView.Frame = oDetailRect;
						if (oView.Superview == null)
						{
							oController.ViewWillAppear(false);
							View.InsertSubviewAbove(oView, MasterViewController.View);
							oController.ViewDidAppear(false);
						}
						else
						{
							View.BringSubviewToFront(oView);
						}
					}
				}
			}

			// Create corner views if necessary.
			MTSplitCornersView leadingCorners; // top/left of screen in vertical/horizontal split.
			MTSplitCornersView trailingCorners; // bottom/right of screen in vertical/horizontal split.
			if (CornerViews == null)
			{
				var cornerRect = new CGRect(0, 0, 10, 10); // arbitrary, will be resized below.
				leadingCorners = new MTSplitCornersView(cornerRect)
				{
					SplitViewController = this,
					CornerBackgroundColor = DefaultCornerColor,
					CornerRadius = DefaultCornerRadius
				};
				trailingCorners = new MTSplitCornersView(cornerRect)
				{
					SplitViewController = this,
					CornerBackgroundColor = DefaultCornerColor,
					CornerRadius = DefaultCornerRadius
				};
				CornerViews = new[] {leadingCorners, trailingCorners};
			}
			else
			{
				leadingCorners = CornerViews[0];
				trailingCorners = CornerViews[1];
			}

			// Configure and layout the corner-views.
			leadingCorners.Position = IsVertical ? MTSplitCornersView.CornersPosition.LeadingVertical : MTSplitCornersView.CornersPosition.LeadingHorizontal;
			trailingCorners.Position = IsVertical ? MTSplitCornersView.CornersPosition.TrailingVertical : MTSplitCornersView.CornersPosition.TrailingHorizontal;
			leadingCorners.AutoresizingMask = IsVertical ? UIViewAutoresizing.FlexibleBottomMargin : UIViewAutoresizing.FlexibleRightMargin;
			trailingCorners.AutoresizingMask = IsVertical ? UIViewAutoresizing.FlexibleTopMargin : UIViewAutoresizing.FlexibleLeftMargin;

			nfloat x;
			nfloat y;
			nfloat cornersWidth;
			nfloat cornersHeight;

			CGRect leadingRect;
			CGRect trailingRect;

			var radius = leadingCorners.CornerRadius;
			if (IsVertical)
			{
				// left/right split
				cornersWidth = (radius*2.0f) + SplitWidth;
				cornersHeight = radius;
				if (bShouldShowMaster)
				{
					if (bMasterFirst)
					{
						x = SplitPosition;
					}
					else
					{
						x = width - (SplitPosition + SplitWidth);
					}
				}
				else
				{
					x = 0 - SplitWidth;
				}
				x -= radius;
				y = mainRect.Top;
				leadingRect = new CGRect(x, y, cornersWidth, cornersHeight); // top corners
				trailingRect = new CGRect(x, (height - cornersHeight) + mainRect.Top, cornersWidth, cornersHeight); // bottom corners

			}
			else
			{
				// top/bottom split
				x = 0;
				if (bShouldShowMaster)
				{
					if (bMasterFirst)
					{
						y = SplitPosition;
					}
					else
					{
						y = height - (SplitPosition + SplitWidth);
					}
				}
				else
				{
					y = 0 - SplitWidth;
				}
				y -= radius;
				y += mainRect.Top;
				cornersWidth = radius;
				cornersHeight = (radius*2.0f) + SplitWidth;
				leadingRect = new CGRect(x, y, cornersWidth, cornersHeight); // left corners
				trailingRect = new CGRect((width - cornersWidth), y, cornersWidth, cornersHeight); // right corners
			}

			leadingCorners.Frame = leadingRect;
			trailingCorners.Frame = trailingRect;

			// Ensure corners are visible and frontmost.
			if (DetailViewController != null)
			{
				if (leadingCorners.Superview == null)
				{
					View.InsertSubviewAbove(leadingCorners, DetailViewController.View);
					View.InsertSubviewAbove(trailingCorners, DetailViewController.View);
				}
				else
				{
					View.BringSubviewToFront(leadingCorners);
					View.BringSubviewToFront(trailingCorners);
				}
			}

			if (DividerView != null && DividerView.Superview != null)
			{
				DividerView.Superview.BringSubviewToFront(DividerView);
			}
		}

		/// <summary>
		/// Reconfigures the controller to show master controller in a popover or as a real pane.
		/// </summary>
		/// <param name='bInPopover'>
		/// B in popover.
		/// </param>
		private void ReconfigureForMasterInPopover(bool bInPopover)
		{
			_reconfigurePopup = false;

			// Check if the user actually wants the master to be put into a popover.
			if (!AutoProvidePopoverAndBarItem)
			{
				return;
			}

			if (bInPopover && HiddenPopoverController != null || !bInPopover && HiddenPopoverController == null || MasterViewController == null)
			{
				// Nothing to do.
				return;
			}

			if (bInPopover && HiddenPopoverController == null && BarButtonItem == null)
			{
				// Create and configure popover for our masterViewController.
				if (HiddenPopoverController != null)
				{
					HiddenPopoverController.Dispose();
				}
				HiddenPopoverController = null;

				MasterViewController.ViewWillDisappear(false);
				HiddenPopoverController = new UIPopoverController(MasterViewController);
				MasterViewController.ViewDidDisappear(false);

				// Create and configure UIBarButtonItem.
				BarButtonItem = new UIBarButtonItem("Master", UIBarButtonItemStyle.Bordered, ShowMasterPopover);

				// Inform delegate of this state of affairs.
				if (WillHideViewController != null)
				{
					WillHideViewController(this, MasterViewController, BarButtonItem, HiddenPopoverController);
				}
			}
			else if (!bInPopover && HiddenPopoverController != null && BarButtonItem != null)
			{
				// I know this looks strange, but it fixes a bizarre issue with UIPopoverController leaving masterViewController's views in disarray.
				HiddenPopoverController.PresentFromRect(CGRect.Empty, View, UIPopoverArrowDirection.Any, false);

				// Remove master from popover and destroy popover, if it exists.
				HiddenPopoverController.Dismiss(false);
				HiddenPopoverController.Dispose();
				HiddenPopoverController = null;

				// Inform delegate that the _barButtonItem will become invalid.
				if (WillShowViewController != null)
				{
					WillShowViewController(this, MasterViewController, BarButtonItem);
				}

				// Destroy _barButtonItem.
				if (BarButtonItem != null)
				{
					BarButtonItem.Dispose();
				}
				BarButtonItem = null;

				// Move master view.
				var masterView = MasterViewController.View;
				if (masterView != null && masterView.Superview != View)
				{
					masterView.RemoveFromSuperview();
				}
			}
		}

		private void PopoverControllerDidDismissPopover(UIPopoverController popoverController)
		{
			ReconfigureForMasterInPopover(false);
		}

		/// <summary>
		/// Toggles the split orientation.
		/// </summary>
		public virtual void ToggleSplitOrientation()
		{
			if (!IsShowingMaster) return;

			if (CornerViews != null)
			{
				foreach (var corner in CornerViews)
				{
					corner.Hidden = true;
				}
				DividerView.Hidden = true;
			}
			UIView.Animate(0.1f, delegate
			{
				IsVertical = !IsVertical;
			},
				delegate
				{
					foreach (var oCorner in CornerViews)
					{
						oCorner.Hidden = false;
					}
					DividerView.Hidden = false;
				});
		}

		/// <summary>
		/// Toggles the master before detail.
		/// </summary>
		public void ToggleMasterBeforeDetail()
		{
			if (!IsShowingMaster) return;

			if (CornerViews != null)
			{
				foreach (var oCorner in CornerViews)
				{
					oCorner.Hidden = true;
				}
				DividerView.Hidden = true;
			}
			UIView.Animate(0.1f, delegate
			{
				MasterBeforeDetail = !MasterBeforeDetail;
			},
				delegate
				{
					foreach (var oCorner in CornerViews)
					{
						oCorner.Hidden = false;
					}
					DividerView.Hidden = false;
				});
		}

		/// <summary>
		/// Toggles the visibility of the master view.
		/// </summary>
		public virtual void ToggleMasterView()
		{
			if (HiddenPopoverController != null && HiddenPopoverController.PopoverVisible)
			{
				HiddenPopoverController.Dismiss(false);
			}

			if (!IsShowingMaster)
			{
				// We're about to show the master view. Ensure it's in place off-screen to be animated in.
				_reconfigurePopup = true;
				ReconfigureForMasterInPopover(false);
				LayoutSubviews();
			}

			// This action functions on the current primary orientation; it is independent of the other primary orientation.
			UIView.BeginAnimations("toggleMaster");
			if (IsLandscape)
			{
				ShowsMasterInLandscape = !ShowsMasterInLandscape;
			}
			else
			{
				ShowsMasterInPortrait = !ShowsMasterInPortrait;
			}
			UIView.CommitAnimations();
		}

		/// <summary>
		/// Called from the bar button item to show the master controller in a popover.
		/// </summary>
		public void ShowMasterPopover(object sender, EventArgs args)
		{
			if (HiddenPopoverController == null || HiddenPopoverController.PopoverVisible) return;

			// Inform delegate.
			if (WillPresentViewController != null)
			{
				WillPresentViewController(this, HiddenPopoverController, MasterViewController);
			}
			// Show popover.
			HiddenPopoverController.PresentFromBarButtonItem(BarButtonItem, UIPopoverArrowDirection.Any, true);
		}
	}
}
