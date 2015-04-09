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
	public class MTSplitDividerView : UIView
	{
		public enum DividerStyle
		{
			Thin = 0, // Thin divider, like UISplitViewController (default).
			PaneSplitter = 1 // Thick divider, drawn with a grey gradient and a grab-strip.
		}

		public MTSplitDividerView() : base()
		{
		}

		public MTSplitDividerView(NSCoder coder) : base(coder)
		{
		}

		public MTSplitDividerView(NSObjectFlag t) : base(t)
		{
		}

		public MTSplitDividerView(IntPtr handle) : base(handle)
		{
		}

		public MTSplitDividerView(CGRect frame) : base(frame)
		{
			UserInteractionEnabled = false;
			AllowsDragging = false;
			ContentMode = UIViewContentMode.Redraw;
		}

		public override void Draw(CGRect rect)
		{
			if (SplitViewController.DividerStyle == DividerStyle.Thin)
			{
				base.Draw(rect);
			}
			else if (SplitViewController.DividerStyle == DividerStyle.PaneSplitter)
			{
				// Draw gradient background.
				var bounds = Bounds;
				var rgb = CGColorSpace.CreateDeviceRGB();
				var locations = new nfloat[] {0, 1};
				var components = new nfloat[]
				{
					// light
					0.988f, 0.988f, 0.988f, 1.0f, 
					// dark
					0.875f, 0.875f, 0.875f, 1.0f
				};
				var gradient = new CGGradient(rgb, components, locations);
				var context = UIGraphics.GetCurrentContext();
				CGPoint start;
				CGPoint end;
				if (SplitViewController.IsVertical)
				{
					// Light left to dark right.
					start = new CGPoint(bounds.GetMinX(), bounds.GetMidY());
					end = new CGPoint(bounds.GetMaxX(), bounds.GetMidY());
				}
				else
				{
					// Light top to dark bottom.
					start = new CGPoint(bounds.GetMidX(), bounds.GetMinY());
					end = new CGPoint(bounds.GetMidX(), bounds.GetMaxY());
				}
				context.DrawLinearGradient(gradient, start, end, CGGradientDrawingOptions.DrawsAfterEndLocation);
				rgb.Dispose();
				gradient.Dispose();

				// Draw borders.
				var borderThickness = 1.0f;
				UIColor.FromWhiteAlpha(0.7f, 1.0f).SetFill();
				UIColor.FromWhiteAlpha(0.7f, 1.0f).SetStroke();
				var borderRect = bounds;
				if (SplitViewController.IsVertical)
				{
					borderRect.Width = borderThickness;
					UIGraphics.RectFill(borderRect);
					borderRect.X = bounds.GetMaxX() - borderThickness;
					UIGraphics.RectFill(borderRect);
				}
				else
				{
					borderRect.Height = borderThickness;
					UIGraphics.RectFill(borderRect);
					borderRect.Y = bounds.GetMaxY() - borderThickness;
					UIGraphics.RectFill(borderRect);
				}

				// Draw grip.
				DrawGripThumbInRect(bounds);
			}
		}

		private void DrawGripThumbInRect(CGRect rect)
		{
			nfloat width = 9.0f;
			nfloat height;
			if (SplitViewController.IsVertical)
			{
				height = 30.0f;
			}
			else
			{
				height = width;
				width = 30.0f;
			}

			// Draw grip in centred in rect.
			var gripRect = new CGRect(0, 0, width, height);
			gripRect.X = (rect.Width - gripRect.Width)/2.0f;
			gripRect.Y = (rect.Height - gripRect.Height)/2.0f;

			nfloat stripThickness = 1.0f;
			var stripColor = UIColor.FromWhiteAlpha(0.35f, 1.0f);
			var lightColor = UIColor.FromWhiteAlpha(1.0f, 1.0f);
			nfloat space = 3.0f;
			if (SplitViewController.IsVertical)
			{
				gripRect.Width = stripThickness;
				stripColor.SetFill();
				stripColor.SetStroke();
				UIGraphics.RectFill(gripRect);

				gripRect.X += stripThickness;
				gripRect.Y += 1f;
				lightColor.SetFill();
				lightColor.SetStroke();
				UIGraphics.RectFill(gripRect);
				gripRect.X -= stripThickness;
				gripRect.Y -= 1f;

				gripRect.X += space + stripThickness;
				stripColor.SetFill();
				stripColor.SetStroke();
				UIGraphics.RectFill(gripRect);

				gripRect.X += stripThickness;
				gripRect.Y += 1f;
				lightColor.SetFill();
				lightColor.SetStroke();
				UIGraphics.RectFill(gripRect);
				gripRect.X -= stripThickness;
				gripRect.Y -= 1f;

				gripRect.X += space + stripThickness;
				stripColor.SetFill();
				stripColor.SetStroke();
				UIGraphics.RectFill(gripRect);

				gripRect.X += stripThickness;
				gripRect.Y += 1f;
				lightColor.SetFill();
				lightColor.SetStroke();
				UIGraphics.RectFill(gripRect);
			}
			else
			{
				gripRect.Height = stripThickness;
				stripColor.SetFill();
				stripColor.SetStroke();
				UIGraphics.RectFill(gripRect);

				gripRect.Y += stripThickness;
				gripRect.X -= 1f;
				lightColor.SetFill();
				lightColor.SetStroke();
				UIGraphics.RectFill(gripRect);
				gripRect.Y -= stripThickness;
				gripRect.X += 1f;

				gripRect.Y += space + stripThickness;
				stripColor.SetFill();
				stripColor.SetStroke();
				UIGraphics.RectFill(gripRect);

				gripRect.Y += stripThickness;
				gripRect.X -= 1f;
				lightColor.SetFill();
				lightColor.SetStroke();
				UIGraphics.RectFill(gripRect);
				gripRect.Y -= stripThickness;
				gripRect.X += 1f;

				gripRect.Y += space + stripThickness;
				stripColor.SetFill();
				stripColor.SetStroke();
				UIGraphics.RectFill(gripRect);

				gripRect.Y += stripThickness;
				gripRect.X -= 1f;
				lightColor.SetFill();
				lightColor.SetStroke();
				UIGraphics.RectFill(gripRect);
			}
		}

		public override void TouchesMoved(NSSet touches, UIEvent evt)
		{
			var touch = touches.AnyObject as UITouch;
			if (touch == null) return;

			var lastPt = touch.PreviousLocationInView(this);
			var pt = touch.LocationInView(this);
			var offset = (SplitViewController.IsVertical) ? pt.X - lastPt.X : pt.Y - lastPt.Y;
			if (!SplitViewController.MasterBeforeDetail)
			{
				offset = -offset;
			}
			SplitViewController.SplitPosition = SplitViewController.SplitPosition + offset;
		}

		public virtual MTSplitViewLib.MTSplitViewController SplitViewController { get; set; }

		public virtual bool AllowsDragging
		{
			get { return _allowsDragging; }
			set
			{
				_allowsDragging = value;
				UserInteractionEnabled = value;
			}
		}
		private bool _allowsDragging;
	}

}
