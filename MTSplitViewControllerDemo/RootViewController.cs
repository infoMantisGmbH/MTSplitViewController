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
using Foundation;
using System;
using UIKit;

namespace MTSplitViewControllerDemo
{
	public class RootViewController : UITableViewController
	{
		public DetailViewController DetailViewController { get; set; }

		public override void ViewDidLoad()
		{
			base.ViewDidLoad();
			ClearsSelectionOnViewWillAppear = false;
			ContentSizeForViewInPopover = new CGSize(320f, 600f);
			TableView.Source = new RowTableViewSource(this);
		}

		public override bool ShouldAutorotateToInterfaceOrientation(UIInterfaceOrientation toInterfaceOrientation)
		{
			return true;
		}

		public void SelectFirstRow()
		{
			if (TableView.NumberOfSections() > 0 && TableView.NumberOfRowsInSection(0) > 0)
			{
				var oIndexPath = NSIndexPath.FromRowSection(0, 0);
				TableView.SelectRow(oIndexPath, true, UITableViewScrollPosition.Top);
				TableView.Source.RowSelected(TableView, oIndexPath);
			}
		}

		private class RowTableViewSource : UITableViewSource
		{
			private const string CellId = "CellID";

			public RowTableViewSource(RootViewController controller)
			{
				_controller = controller;
			}

			private readonly RootViewController _controller;

			public override nint NumberOfSections(UITableView tableView)
			{
				return 1;
			}

			public override nint RowsInSection(UITableView tableview, nint section)
			{
				return 10;
			}

			public override UITableViewCell GetCell(UITableView tableView, NSIndexPath indexPath)
			{
				var cell = tableView.DequeueReusableCell(CellId);
				if (cell == null)
				{
					cell = new UITableViewCell(UITableViewCellStyle.Default, CellId)
					{
						Accessory = UITableViewCellAccessory.None
					};
				}
				cell.TextLabel.Text = "Row " + indexPath.Row;
				return cell;
			}

			public override void RowSelected(UITableView tableView, NSIndexPath indexPath)
			{
				_controller.DetailViewController.DetailItem = "Row " + indexPath.Row;
			}
		}
	}
}
