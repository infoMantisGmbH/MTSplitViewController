//  MTSplitViewController
//  https://github.com/Krumelur/MTSplitViewController
// 
//  Ported to Monotouch by René Ruppert, October 2011
//  Ported to Xamarin.iOS by infoMantis GmbH, April 2015
//
//  Based on code by Matt Gemmell on 26/07/2010.
//  Copyright 2010 Instinctive Code.
//  https://github.com/mattgemmell/MGSplitViewController

using CoreGraphics;
using MTSplitViewLib;
using System;
using System.Collections.Generic;
using UIKit;

namespace MTSplitViewControllerDemo
{
	public class DetailViewController : UIViewController
	{
		private readonly MTSplitViewController _splitViewController;
		private string _detailitem;
		private UIBarButtonItem _toggleItem;
		private UIBarButtonItem _verticalItem;
		private UIBarButtonItem _dividerStyleItem;
		private UIBarButtonItem _masterBeforeDetailItem;
		private UIPopoverController _popoverController;
		private UILabel _detailDescriptionLabel;
		private UIToolbar _toolbar;

		public DetailViewController(MTSplitViewController splitViewController)
		{
			_splitViewController = splitViewController;
		}

		public override void LoadView()
		{
			base.LoadView();
			View.AutoresizingMask = UIViewAutoresizing.FlexibleWidth | UIViewAutoresizing.FlexibleHeight;
			View.BackgroundColor = UIColor.White;
			_splitViewController.WillHideViewController += HandleWillHideViewController;
			_splitViewController.WillShowViewController += HandleWillShowViewController;
			_splitViewController.WillPresentViewController += HandleWillPresentViewController;
			_splitViewController.WillChangeSplitOrientationToVertical += HandleWillChangeSplitOrientationToVertical;
			_splitViewController.WillMoveSplitToPosition += HandleWillMoveSplitToPosition;
			_splitViewController.ConstrainSplitPosition += HandleConstrainSplitPosition;
		}

		public override void ViewWillAppear(bool animated)
		{
			base.ViewWillAppear(animated);
			_toolbar = new UIToolbar(new CGRect(0, 0, View.Bounds.Width, 44))
			{
				AutoresizingMask = UIViewAutoresizing.FlexibleWidth
			};
			View.AddSubview(_toolbar);

			_detailDescriptionLabel = new UILabel(new CGRect(0, 0, View.Bounds.Width, 30))
			{
				TextAlignment = UITextAlignment.Center,
				Center = new CGPoint(View.Bounds.Width/2, View.Bounds.Height/2)
			};
			Console.WriteLine(View.Bounds + "; " + _detailDescriptionLabel.Center);
			_detailDescriptionLabel.AutoresizingMask = UIViewAutoresizing.FlexibleWidth | UIViewAutoresizing.FlexibleRightMargin | UIViewAutoresizing.FlexibleLeftMargin;
			View.AddSubview(_detailDescriptionLabel);

			_toggleItem = new UIBarButtonItem("Show", UIBarButtonItemStyle.Bordered, delegate
			{
				_splitViewController.ToggleMasterView();
				ConfigureView();
			});

			_verticalItem = new UIBarButtonItem("Horizontal", UIBarButtonItemStyle.Bordered, delegate
			{
				_splitViewController.ToggleSplitOrientation();
				ConfigureView();
			});

			_dividerStyleItem = new UIBarButtonItem("Dragging", UIBarButtonItemStyle.Bordered, delegate
			{
				var eNewStyle = _splitViewController.DividerStyle == MTSplitDividerView.DividerStyle.Thin
					? MTSplitDividerView.DividerStyle.PaneSplitter
					: MTSplitDividerView.DividerStyle.Thin;
				_splitViewController.SetDividerStyleAnimated(eNewStyle);
				ConfigureView();
			});

			_masterBeforeDetailItem = new UIBarButtonItem("Detail First", UIBarButtonItemStyle.Bordered, delegate
			{
				_splitViewController.ToggleMasterBeforeDetail();
				ConfigureView();
			});

			_toolbar.SetItems(new[] {_toggleItem, _verticalItem, _dividerStyleItem, _masterBeforeDetailItem}, false);
		}

		private void HandleWillHideViewController(
			MTSplitViewController oSplitController,
			UIViewController oMasterControler,
			UIBarButtonItem oBarBtnItm,
			UIPopoverController oPopover)
		{
			Console.WriteLine("WillHideViewController()");
			if (oBarBtnItm != null)
			{
				oBarBtnItm.Title = "Popover";
				var aItems = new List<UIBarButtonItem>(_toolbar.Items);
				aItems.Insert(0, oBarBtnItm);
				_toolbar.SetItems(aItems.ToArray(), true);
			}
			_popoverController = oPopover;
		}

		private nfloat HandleConstrainSplitPosition(MTSplitViewController oSplitController, nfloat fProposedPosition, CGSize oViewSize)
		{
			Console.WriteLine("ConstrainSplitPosition(): " + fProposedPosition);
			return fProposedPosition;
		}

		private void HandleWillMoveSplitToPosition(MTSplitViewController oSplitControler, nfloat fSplitPos)
		{
			Console.WriteLine("WillMoveSplitToPosition(): " + fSplitPos);
		}

		private void HandleWillChangeSplitOrientationToVertical(MTSplitViewController oSplitController, bool bIsVertical)
		{
			Console.WriteLine("WillChangeSplitOrientationToVertical(): " + bIsVertical);
		}

		private void HandleWillPresentViewController(
			MTSplitViewController oSplitController,
			UIPopoverController oPopoverController,
			UIViewController oMasterController)
		{
			Console.WriteLine("WillPresentViewController()");
		}

		private void HandleWillShowViewController(MTSplitViewController oSplitController, UIViewController oMasterController, UIBarButtonItem oBarBtnItm)
		{
			Console.WriteLine("WillShowViewController()");
			if (oBarBtnItm != null)
			{
				var aItems = new List<UIBarButtonItem>(_toolbar.Items);
				aItems.Remove(oBarBtnItm);
				_toolbar.SetItems(aItems.ToArray(), true);
			}
			_popoverController = null;
		}

		public string DetailItem
		{
			get { return _detailitem; }
			set
			{
				_detailitem = value;
				ConfigureView();
				if (_popoverController != null)
				{
					_popoverController.Dismiss(true);
				}
			}
		}

		public void ConfigureView()
		{
			// Update the user interface for the detail item.
			_detailDescriptionLabel.Text = _detailitem;
			_toggleItem.Title = _splitViewController.IsShowingMaster ? "Hide Master" : "Show Master";
			_verticalItem.Title = _splitViewController.IsVertical ? "Horizontal Split" : "Vertical Split";
			_dividerStyleItem.Title = _splitViewController.DividerStyle.ToString();
			_masterBeforeDetailItem.Title = _splitViewController.MasterBeforeDetail ? "Detail First" : "Master First";
		}
	}
}
