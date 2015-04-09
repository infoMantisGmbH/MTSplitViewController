//  MTSplitViewController
//  https://github.com/Krumelur/MTSplitViewController
// 
//  Ported to Monotouch by René Ruppert, October 2011
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
		public enum CORNERS_POSITION
		{
			LeadingVertical	= 0, // top of screen for a left/right split.
			TrailingVertical	= 1, // bottom of screen for a left/right split.
			LeadingHorizontal	= 2, // left of screen for a top/bottom split.
			TrailingHorizontal	= 3  // right of screen for a top/bottom split.
		} 
		
		public MTSplitCornersView () : base()
		{
		}
		
		public MTSplitCornersView (NSCoder coder) : base(coder)
		{
		}
		
		public MTSplitCornersView (NSObjectFlag t) : base(t)
		{
		}
		
		public MTSplitCornersView (IntPtr handle) : base(handle)
		{
		}
		
		public MTSplitCornersView (CGRect frame) : base(frame)
		{
			this.ContentMode = UIViewContentMode.Redraw;
			this.UserInteractionEnabled = false;
			this.Opaque = false;
			this.BackgroundColor = UIColor.Clear;
			this.CornerRadius = 0.0f; // actual value is set by the splitViewController.
			this.CornersPosition = MTSplitCornersView.CORNERS_POSITION.LeadingVertical;
		}
		
		private nfloat Deg2Rad(nfloat fDegrees)
		{
			// Converts degrees to radians.
			return fDegrees * ((nfloat)Math.PI / 180.0f);
		}
		

		private nfloat Rad2Deg(nfloat fRadians)
		{
			// Converts radians to degrees.
			return fRadians * (180f / (float)Math.PI);
		}
		
		public override void Draw (CGRect rect)
		{
			// Draw two appropriate corners, with cornerBackgroundColor behind them.
			if (this.CornerRadius < 0)
			{
				return;
			}
			
			var fMaxX = this.Bounds.GetMaxX();
			var fMaxY =this.Bounds.GetMaxY();
			UIBezierPath oPath = new UIBezierPath();
			CGPoint oPt = CGPoint.Empty;
			switch (this.CornersPosition)
			{
				case CORNERS_POSITION.LeadingVertical: // top of screen for a left/right split
					oPath.MoveTo(oPt);
					oPt.Y += this.CornerRadius;
					oPath.AppendPath(UIBezierPath.FromArc(oPt, this.CornerRadius, this.Deg2Rad(90), 0f, true));
					oPt.X += this.CornerRadius;
					oPt.Y -= this.CornerRadius;
					oPath.AddLineTo(oPt);
					oPath.AddLineTo(CGPoint.Empty);
					oPath.ClosePath();;
			
					oPt.X = fMaxX - this.CornerRadius;
					oPt.Y = 0;
					oPath.MoveTo(oPt);
					oPt.Y = fMaxY;
					oPath.AddLineTo(oPt);
					oPt.X += this.CornerRadius;
					oPath.AppendPath(UIBezierPath.FromArc(oPt, this.CornerRadius, this.Deg2Rad(180f), this.Deg2Rad(90), true));
					oPt.Y -= this.CornerRadius;
					oPath.AddLineTo(oPt);
					oPt.X -= this.CornerRadius;
					oPath.AddLineTo(oPt);
					oPath.ClosePath();
					break;
			
				case CORNERS_POSITION.TrailingVertical: // bottom of screen for a left/right split
					oPt.Y = fMaxY;
					oPath.MoveTo(oPt);
					oPt.Y -= this.CornerRadius;
					oPath.AppendPath(UIBezierPath.FromArc(oPt, this.CornerRadius, this.Deg2Rad(270f), this.Deg2Rad(360), false));
					oPt.X +=this.CornerRadius;
					oPt.Y +=this.CornerRadius;
					oPath.AddLineTo(oPt);
					oPt.X -=this.CornerRadius;
					oPath.AddLineTo(oPt);
					oPath.ClosePath();
			
					oPt.X = fMaxX -this.CornerRadius;
					oPt.Y = fMaxY;
					oPath.MoveTo(oPt);
					oPt.Y -=this.CornerRadius;
					oPath.AddLineTo(oPt);
					oPt.X +=this.CornerRadius;
					oPath.AppendPath(UIBezierPath.FromArc(oPt, this.CornerRadius, this.Deg2Rad(180f), this.Deg2Rad(270f), false));
					oPt.Y +=this.CornerRadius;
					oPath.AddLineTo(oPt);
					oPt.X -=this.CornerRadius;
					oPath.AddLineTo(oPt);
					oPath.ClosePath();
				break;
			
				case CORNERS_POSITION.LeadingHorizontal: // left of screen for a top/bottom split
					oPt.X = 0;
					oPt.Y =this.CornerRadius;
					oPath.MoveTo(oPt);
					oPt.Y -=this.CornerRadius;
					oPath.AddLineTo(oPt);
					oPt.X +=this.CornerRadius;
					oPath.AppendPath(UIBezierPath.FromArc(oPt, this.CornerRadius, this.Deg2Rad(180), this.Deg2Rad(270), false));
					oPt.Y +=this.CornerRadius;
					oPath.AddLineTo(oPt);
					oPt.X -=this.CornerRadius;
					oPath.AddLineTo(oPt);
					oPath.ClosePath();
			
					oPt.X = 0;
					oPt.Y = fMaxY -this.CornerRadius;
					oPath.MoveTo(oPt);
					oPt.Y = fMaxY;
					oPath.AddLineTo(oPt);
					oPt.X +=this.CornerRadius;
					oPath.AppendPath(UIBezierPath.FromArc(oPt, this.CornerRadius, this.Deg2Rad(180f), this.Deg2Rad(90f), true));
					oPt.Y -=this.CornerRadius;
					oPath.AddLineTo(oPt);
					oPt.X -=this.CornerRadius;
					oPath.AddLineTo(oPt);
					oPath.ClosePath();
					break;
			
				case CORNERS_POSITION.TrailingHorizontal: // right of screen for a top/bottom split
					oPt.Y =this.CornerRadius;
					oPath.MoveTo(oPt);
					oPt.Y -=this.CornerRadius;
					oPath.AppendPath(UIBezierPath.FromArc(oPt, this.CornerRadius, this.Deg2Rad(270f), this.Deg2Rad(360f), false));
					oPt.X +=this.CornerRadius;
					oPt.Y +=this.CornerRadius;
					oPath.AddLineTo(oPt);
					oPt.X -=this.CornerRadius;
					oPath.AddLineTo(oPt);
					oPath.ClosePath();
			
					oPt.Y = fMaxY -this.CornerRadius;
					oPath.MoveTo(oPt);
					oPt.Y +=this.CornerRadius;
					oPath.AppendPath(UIBezierPath.FromArc(oPt, this.CornerRadius, this.Deg2Rad(90), 0f, true));
					oPt.X +=this.CornerRadius;
					oPt.Y -=this.CornerRadius;
					oPath.AddLineTo(oPt);
					oPt.X -=this.CornerRadius;
					oPath.AddLineTo(oPt);
					oPath.ClosePath();
				break;
			
			default:
				break;
		}
		
		this.CornerBackgroundColor.SetFill();
		this.CornerBackgroundColor.SetStroke();
		oPath.Fill();
	}
		
		public nfloat CornerRadius
		{
			get
			{
				return this.fCornerRadius;
			}
			set
			{
				if(value != this.fCornerRadius)
				{
					this.fCornerRadius = value;
					this.SetNeedsDisplay();
				}
			}
		}
		private nfloat fCornerRadius;
		
		public MTSplitViewController SplitViewController
		{
			get
			{
				return this.oSplitViewController;
			}
			set
			{
				if(this.oSplitViewController != value)
				{
					this.oSplitViewController = value;
					this.SetNeedsDisplay();
				}
			}
		}
		private MTSplitViewController oSplitViewController;
		
		public CORNERS_POSITION CornersPosition
		{
			get
			{
				return this.eCornerPosition;
			}
			set
			{
				if(this.eCornerPosition != value)
				{
					this.eCornerPosition = value;
					this.SetNeedsDisplay();
				}
			}
		}
		private CORNERS_POSITION eCornerPosition;
		
		public UIColor CornerBackgroundColor
		{
			get
			{
				return this.oCornerBackgroundColor;
			}
			set
			{
				if(this.oCornerBackgroundColor != value)
				{
					this.oCornerBackgroundColor = value;
					this.SetNeedsDisplay();
				}
			}
		}
		private UIColor oCornerBackgroundColor;
	}
}

