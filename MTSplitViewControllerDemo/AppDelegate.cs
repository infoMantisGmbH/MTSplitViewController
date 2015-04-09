//  MTSplitViewController
//  https://github.com/Krumelur/MTSplitViewController
//  
//  Ported to Monotouch by René Ruppert, October 2011
//  Ported to Xamarin.iOS by infoMantis GmbH, April 2015
//
//  Based on code by Matt Gemmell on 26/07/2010.
//  Copyright 2010 Instinctive Code.
//  https://github.com/mattgemmell/MGSplitViewController

using Foundation;
using MTSplitViewLib;
using UIKit;

namespace MTSplitViewControllerDemo
{
	[Register("AppDelegate")]
	public class AppDelegate : UIApplicationDelegate
	{
		private UIWindow _window;
		private MTSplitViewController _splitViewController;
		private RootViewController _masterController;
		private DetailViewController _detailController;
		private UINavigationController _navController;

		public override bool FinishedLaunching(UIApplication app, NSDictionary options)
		{
			// create a new window instance based on the screen size
			_window = new UIWindow(UIScreen.MainScreen.Bounds);

			_splitViewController = new MTSplitViewController();

			_masterController = new RootViewController();
			_detailController = new DetailViewController(_splitViewController);
			_masterController.DetailViewController = _detailController;
			_navController = new UINavigationController(_masterController);


			_splitViewController.ViewControllers = new UIViewController[] {_navController, _detailController};
			_splitViewController.ShowsMasterInLandscape = true;
			_splitViewController.ShowsMasterInPortrait = true;
			_window.AddSubview(_splitViewController.View);

			_window.MakeKeyAndVisible();

			_masterController.SelectFirstRow();
			_detailController.ConfigureView();

			return true;
		}
	}
}
