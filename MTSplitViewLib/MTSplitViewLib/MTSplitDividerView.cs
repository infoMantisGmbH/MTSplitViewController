//  MTSplitViewController
//  https://github.com/Krumelur/MTSplitViewController
// 
//  Ported to Monotouch by René Ruppert, October 2011
//
//  Based on code by Matt Gemmell on 26/07/2010.
//  Copyright 2010 Instinctive Code.
//  https://github.com/mattgemmell/MGSplitViewController

using System;
using UIKit;
using CoreGraphics;
using CoreGraphics;
using Foundation;

namespace MTSplitViewLib
{
	public class MTSplitDividerView : UIView
	{
		public enum DIVIDER_STYLE
		{
			Thin			= 0, // Thin divider, like UISplitViewController (default).
			PaneSplitter	= 1  // Thick divider, drawn with a grey gradient and a grab-strip.
		}
		
		public MTSplitDividerView () : base()
		{
		}
		
		public MTSplitDividerView (NSCoder coder) : base(coder)
		{
		}
		
		public MTSplitDividerView (NSObjectFlag t) : base(t)
		{
		}
		
		public MTSplitDividerView (IntPtr handle) : base(handle)
		{
		}
		
		public MTSplitDividerView (CGRect frame) : base(frame)
		{
			this.UserInteractionEnabled = false;
			this.AllowsDragging = false;
			this.ContentMode = UIViewContentMode.Redraw;
		}
		
		public override void Draw (CGRect rect)
		{
			if (this.SplitViewController.DividerStyle == DIVIDER_STYLE.Thin)
			{
				base.Draw(rect);
			}
			else if (this.SplitViewController.DividerStyle == DIVIDER_STYLE.PaneSplitter)
			{
				// Draw gradient background.
				CGRect oBounds = this.Bounds;
				CGColorSpace oRGB = CGColorSpace.CreateDeviceRGB();
				var aLocations = new nfloat[] {0, 1};
				var aComponents = new nfloat[]
				{
					// light
					0.988f, 0.988f, 0.988f, 1.0f, 
					// dark
					0.875f, 0.875f, 0.875f, 1.0f
				};
				CGGradient oGradient = new CGGradient(oRGB, aComponents, aLocations);
				CGContext oContext = UIGraphics.GetCurrentContext();
				CGPoint oStart;
				CGPoint oEnd;
				if (this.SplitViewController.IsVertical)
				{
					// Light left to dark right.
					oStart = new CGPoint(oBounds.GetMinX(), oBounds.GetMidY());
					oEnd = new CGPoint(oBounds.GetMaxX(), oBounds.GetMidY());
				}
				else
				{
					// Light top to dark bottom.
					oStart = new CGPoint(oBounds.GetMidX(), oBounds.GetMinY());
					oEnd = new CGPoint(oBounds.GetMidX(), oBounds.GetMaxY());
				}
				oContext.DrawLinearGradient(oGradient, oStart, oEnd, CGGradientDrawingOptions.DrawsAfterEndLocation);
				oRGB.Dispose();
				oGradient.Dispose();
		
				// Draw borders.
				float fBorderThickness = 1.0f;
				UIColor.FromWhiteAlpha(0.7f, 1.0f).SetFill();
				UIColor.FromWhiteAlpha(0.7f, 1.0f).SetStroke();
				CGRect oBorderRect = oBounds;
				if (this.SplitViewController.IsVertical)
				{
					oBorderRect.Width = fBorderThickness;
					UIGraphics.RectFill(oBorderRect);
					oBorderRect.X = oBounds.GetMaxX() - fBorderThickness;
					UIGraphics.RectFill(oBorderRect);
				}
				else
				{
					oBorderRect.Height = fBorderThickness;
					UIGraphics.RectFill(oBorderRect);
					oBorderRect.Y = oBounds.GetMaxY() - fBorderThickness;
					UIGraphics.RectFill(oBorderRect);
				}
		
				// Draw grip.
				this.DrawGripThumbInRect(oBounds);
			}
		}
		
		private void DrawGripThumbInRect(CGRect rect)
		{
			nfloat fWidth = 9.0f;
			nfloat fHeight;
			if (this.SplitViewController.IsVertical)
			{
				fHeight = 30.0f;
			}
			else
			{
				fHeight = fWidth;
				fWidth = 30.0f;
			}
	
			// Draw grip in centred in rect.
			CGRect oGripRect = new CGRect(0, 0, fWidth, fHeight);
			oGripRect.X = ((rect.Width - oGripRect.Width) / 2.0f);
			oGripRect.Y = ((rect.Height - oGripRect.Height) / 2.0f);
	
			nfloat stripThickness = 1.0f;
			UIColor oStripColor = UIColor.FromWhiteAlpha(0.35f, 1.0f);
			UIColor oLightColor = UIColor.FromWhiteAlpha(1.0f, 1.0f);
			nfloat fSpace = 3.0f;
			if (this.SplitViewController.IsVertical)
			{
				oGripRect.Width = stripThickness;
				oStripColor.SetFill();
				oStripColor.SetStroke();
				UIGraphics.RectFill(oGripRect);
		
				oGripRect.X += stripThickness;
				oGripRect.Y += 1f;
				oLightColor.SetFill();
				oLightColor.SetStroke();
				UIGraphics.RectFill(oGripRect);
				oGripRect.X -= stripThickness;
				oGripRect.Y -= 1f;
		
				oGripRect.X += fSpace + stripThickness;
				oStripColor.SetFill();
				oStripColor.SetStroke();
				UIGraphics.RectFill(oGripRect);
		
				oGripRect.X += stripThickness;
				oGripRect.Y += 1f;
				oLightColor.SetFill();
				oLightColor.SetStroke();
				UIGraphics.RectFill(oGripRect);
				oGripRect.X -= stripThickness;
				oGripRect.Y -= 1f;
		
				oGripRect.X += fSpace + stripThickness;
				oStripColor.SetFill();
				oStripColor.SetStroke();
				UIGraphics.RectFill(oGripRect);
		
				oGripRect.X += stripThickness;
				oGripRect.Y += 1f;
				oLightColor.SetFill();
				oLightColor.SetStroke();
				UIGraphics.RectFill(oGripRect);
			}
			else
			{
				oGripRect.Height = stripThickness;
				oStripColor.SetFill();
				oStripColor.SetStroke();
				UIGraphics.RectFill(oGripRect);
		
				oGripRect.Y += stripThickness;
				oGripRect.X -= 1f;
				oLightColor.SetFill();
				oLightColor.SetStroke();
				UIGraphics.RectFill(oGripRect);
				oGripRect.Y -= stripThickness;
				oGripRect.X += 1f;
				
				oGripRect.Y += fSpace + stripThickness;
				oStripColor.SetFill();
				oStripColor.SetStroke();
				UIGraphics.RectFill(oGripRect);
				
				oGripRect.Y += stripThickness;
				oGripRect.X -= 1f;
				oLightColor.SetFill();
				oLightColor.SetStroke();
				UIGraphics.RectFill(oGripRect);
				oGripRect.Y -= stripThickness;
				oGripRect.X += 1f;
				
				oGripRect.Y += fSpace + stripThickness;
				oStripColor.SetFill();
				oStripColor.SetStroke();
				UIGraphics.RectFill(oGripRect);
				
				oGripRect.Y += stripThickness;
				oGripRect.X -= 1f;
				oLightColor.SetFill();
				oLightColor.SetStroke();
				UIGraphics.RectFill(oGripRect);
			}
		}
		
		public override void TouchesMoved (NSSet touches, UIEvent evt)
		{
			UITouch oTouch = touches.AnyObject as UITouch;
			if (oTouch != null)
			{
				CGPoint oLastPt = oTouch.PreviousLocationInView(this);
				CGPoint oPt = oTouch.LocationInView(this);
				var fOffset = (this.SplitViewController.IsVertical) ? oPt.X - oLastPt.X : oPt.Y - oLastPt.Y;
				if (!this.SplitViewController.MasterBeforeDetail)
				{
					fOffset = -fOffset;
				}
				this.SplitViewController.SplitPosition = this.SplitViewController.SplitPosition + fOffset;
			}
		}
		
		public virtual MTSplitViewController SplitViewController
		{
			get;
			set;
		}
		
		public virtual bool AllowsDragging
		{
			get
			{
				return this.bAlllowsDragging;
			}
			set
			{
				this.bAlllowsDragging = value;
				this.UserInteractionEnabled = value;
			}
		}
		private bool bAlllowsDragging;
	}
	
}

