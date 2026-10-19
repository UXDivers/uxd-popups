using CoreGraphics;
using Foundation;
using UIKit;

namespace UXDivers.Popups.Maui
{
    internal class PopupBackgroundView : UIView
    {
        public PopupPage? PopupPage { get; set; }
        public UIView? PopupContentView { get; set; }
        public Func<Task>? BackgroundTappedAction { get; set; }
        
        public PopupBackgroundView()
        {
            InsetsLayoutMarginsFromSafeArea = false;
        }

        public override void SafeAreaInsetsDidChange()
        {
            base.SafeAreaInsetsDidChange();
            SetNeedsLayout();
        }

        public override void LayoutSubviews()
        {
            base.LayoutSubviews();

            if (PopupPage is null)
            {
                return;
            }

            // Re-apply on every layout so rotation, landscape side flips and window resizes are covered
            var insets = SafeAreaInsets;
            PopupPage.ApplySafeAreaInsets(new Thickness(insets.Left, insets.Top, insets.Right, insets.Bottom));
            PopupPage.Arrange(new Rect(0, 0, Bounds.Width, Bounds.Height));
        }

        public override UIView? HitTest(CGPoint point, UIEvent? uievent)
        {
            if (uievent == null)
            {
                return null;
            }

            if (PopupContentView != null)
            {
                // Convert the touch point to the coordinate system of the popup content.
                CGPoint pointInPopup = ConvertPointToView(point, PopupContentView);

                // If the point is inside the popup content, let that view (or its subviews) handle the touch.
                if (PopupContentView.PointInside(pointInPopup, uievent))
                {
                    return PopupContentView.HitTest(pointInPopup, uievent);
                }
            }

            // if the background is transparent don't handle the touch. 
            if (PopupPage?.BackgroundInputTransparent == true)
            {
                return null;
            }

            // Otherwise, return self so that touchesEnded is called.
            return base.HitTest(point, uievent);
        }

        /// <summary>
        /// Called when touches end on this view. If the user taps outside the popup content,
        /// trigger the background tap action.
        /// </summary>
        public override void TouchesEnded(NSSet touches, UIEvent? evt)
        {
            // Ensure the touch is not inside the popup content.
            if (PopupContentView != null)
            {
                foreach (UITouch touch in touches)
                {
                    CGPoint point = touch.LocationInView(this);
                    CGPoint pointInPopup = ConvertPointToView(point, PopupContentView);
                    if (PopupContentView.PointInside(pointInPopup, evt))
                    {
                        // Touch was on the popup; do not trigger dismissal.
                        base.TouchesEnded(touches, evt);
                        return;
                    }
                }
            }

            // The touch is considered to be on the background.
            BackgroundTappedAction?.Invoke();
            base.TouchesEnded(touches, evt);
        }
    }
}