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
using CoreGraphics;
using Foundation;
using UIKit;

namespace MTSplitViewLib
{
	public class MTSplitCornersView : UIView
	{
		public enum CornersPosition
		{
			LeadingVertical = 0, // top of screen for a left/right split.
			TrailingVertical = 1, // bottom of screen for a left/right split.
			LeadingHorizontal = 2, // left of screen for a top/bottom split.
			TrailingHorizontal = 3 // right of screen for a top/bottom split.
		}

		public MTSplitCornersView() : base()
		{
		}

		public MTSplitCornersView(NSCoder coder) : base(coder)
		{
		}

		public MTSplitCornersView(NSObjectFlag t) : base(t)
		{
		}

		public MTSplitCornersView(IntPtr handle) : base(handle)
		{
		}

		public MTSplitCornersView(CGRect frame) : base(frame)
		{
			ContentMode = UIViewContentMode.Redraw;
			UserInteractionEnabled = false;
			Opaque = false;
			BackgroundColor = UIColor.Clear;
			CornerRadius = 0.0f; // actual value is set by the splitViewController.
			Position = CornersPosition.LeadingVertical;
		}

		private nfloat Deg2Rad(nfloat fDegrees)
		{
			// Converts degrees to radians.
			return fDegrees*((nfloat) Math.PI/180.0f);
		}


		private nfloat Rad2Deg(nfloat fRadians)
		{
			// Converts radians to degrees.
			return fRadians*(180f/(float) Math.PI);
		}

		public override void Draw(CGRect rect)
		{
			// Draw two appropriate corners, with cornerBackgroundColor behind them.
			if (CornerRadius < 0)
			{
				return;
			}

			var maxX = Bounds.GetMaxX();
			var maxY = Bounds.GetMaxY();
			var path = new UIBezierPath();
			var pt = CGPoint.Empty;
			switch (Position)
			{
				case CornersPosition.LeadingVertical: // top of screen for a left/right split
					path.MoveTo(pt);
					pt.Y += CornerRadius;
					path.AppendPath(UIBezierPath.FromArc(pt, CornerRadius, Deg2Rad(90), 0f, true));
					pt.X += CornerRadius;
					pt.Y -= CornerRadius;
					path.AddLineTo(pt);
					path.AddLineTo(CGPoint.Empty);
					path.ClosePath();

					pt.X = maxX - CornerRadius;
					pt.Y = 0;
					path.MoveTo(pt);
					pt.Y = maxY;
					path.AddLineTo(pt);
					pt.X += CornerRadius;
					path.AppendPath(UIBezierPath.FromArc(pt, CornerRadius, Deg2Rad(180f), Deg2Rad(90), true));
					pt.Y -= CornerRadius;
					path.AddLineTo(pt);
					pt.X -= CornerRadius;
					path.AddLineTo(pt);
					path.ClosePath();
					break;

				case CornersPosition.TrailingVertical: // bottom of screen for a left/right split
					pt.Y = maxY;
					path.MoveTo(pt);
					pt.Y -= CornerRadius;
					path.AppendPath(UIBezierPath.FromArc(pt, CornerRadius, Deg2Rad(270f), Deg2Rad(360), false));
					pt.X += CornerRadius;
					pt.Y += CornerRadius;
					path.AddLineTo(pt);
					pt.X -= CornerRadius;
					path.AddLineTo(pt);
					path.ClosePath();

					pt.X = maxX - CornerRadius;
					pt.Y = maxY;
					path.MoveTo(pt);
					pt.Y -= CornerRadius;
					path.AddLineTo(pt);
					pt.X += CornerRadius;
					path.AppendPath(UIBezierPath.FromArc(pt, CornerRadius, Deg2Rad(180f), Deg2Rad(270f), false));
					pt.Y += CornerRadius;
					path.AddLineTo(pt);
					pt.X -= CornerRadius;
					path.AddLineTo(pt);
					path.ClosePath();
					break;

				case CornersPosition.LeadingHorizontal: // left of screen for a top/bottom split
					pt.X = 0;
					pt.Y = CornerRadius;
					path.MoveTo(pt);
					pt.Y -= CornerRadius;
					path.AddLineTo(pt);
					pt.X += CornerRadius;
					path.AppendPath(UIBezierPath.FromArc(pt, CornerRadius, Deg2Rad(180), Deg2Rad(270), false));
					pt.Y += CornerRadius;
					path.AddLineTo(pt);
					pt.X -= CornerRadius;
					path.AddLineTo(pt);
					path.ClosePath();

					pt.X = 0;
					pt.Y = maxY - CornerRadius;
					path.MoveTo(pt);
					pt.Y = maxY;
					path.AddLineTo(pt);
					pt.X += CornerRadius;
					path.AppendPath(UIBezierPath.FromArc(pt, CornerRadius, Deg2Rad(180f), Deg2Rad(90f), true));
					pt.Y -= CornerRadius;
					path.AddLineTo(pt);
					pt.X -= CornerRadius;
					path.AddLineTo(pt);
					path.ClosePath();
					break;

				case CornersPosition.TrailingHorizontal: // right of screen for a top/bottom split
					pt.Y = CornerRadius;
					path.MoveTo(pt);
					pt.Y -= CornerRadius;
					path.AppendPath(UIBezierPath.FromArc(pt, CornerRadius, Deg2Rad(270f), Deg2Rad(360f), false));
					pt.X += CornerRadius;
					pt.Y += CornerRadius;
					path.AddLineTo(pt);
					pt.X -= CornerRadius;
					path.AddLineTo(pt);
					path.ClosePath();

					pt.Y = maxY - CornerRadius;
					path.MoveTo(pt);
					pt.Y += CornerRadius;
					path.AppendPath(UIBezierPath.FromArc(pt, CornerRadius, Deg2Rad(90), 0f, true));
					pt.X += CornerRadius;
					pt.Y -= CornerRadius;
					path.AddLineTo(pt);
					pt.X -= CornerRadius;
					path.AddLineTo(pt);
					path.ClosePath();
					break;
			}

			CornerBackgroundColor.SetFill();
			CornerBackgroundColor.SetStroke();
			path.Fill();
		}

		public nfloat CornerRadius
		{
			get { return _cornerRadius; }
			set
			{
				if (value != _cornerRadius)
				{
					_cornerRadius = value;
					SetNeedsDisplay();
				}
			}
		}
		private nfloat _cornerRadius;

		public MTSplitViewController SplitViewController
		{
			get { return _splitViewController; }
			set
			{
				if (_splitViewController != value)
				{
					_splitViewController = value;
					SetNeedsDisplay();
				}
			}
		}
		private MTSplitViewController _splitViewController;

		public CornersPosition Position
		{
			get { return _position; }
			set
			{
				if (_position != value)
				{
					_position = value;
					SetNeedsDisplay();
				}
			}
		}
		private CornersPosition _position;

		public UIColor CornerBackgroundColor
		{
			get { return _cornerBackgroundColor; }
			set
			{
				if (_cornerBackgroundColor != value)
				{
					_cornerBackgroundColor = value;
					SetNeedsDisplay();
				}
			}
		}
		private UIColor _cornerBackgroundColor;
	}
}
