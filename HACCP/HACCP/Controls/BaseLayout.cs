using System;
using Xamarin.Forms;

namespace HACCP
{
    public class BaseLayout : ContentView
    {
        /// <summary>
        ///     Popup location options when relative to another view
        /// </summary>
        public enum PopupLocation
        {
            /// <summary>
            ///     Will show popup on top of the specified view
            /// </summary>
            Top,

            /// <summary>
            ///     Will show popup below of the specified view
            /// </summary>
            Bottom
            
        }

        #region Member Variable

        /// <summary>
        /// _layout
        /// </summary>
        private readonly RelativeLayout _layout;

        /// <summary>
        ///     The content
        /// </summary>
        private View _content;

        /// <summary>
        ///     The popup
        /// </summary>
        private View _popup;

        #endregion

        /// <summary>
        /// BaseLayout Constructor
        /// </summary>
        public BaseLayout()
        {
            base.Content = _layout = new RelativeLayout();
        }


        #region Properties

        /// <summary>
        ///     Gets or sets the content.
        /// </summary>
        /// <value>The content.</value>
        public new View Content
        {
            get { return _content; }
            set
            {
                if (_content != null)
                {
                    _layout.Children.Remove(_content);
                }

                _content = value;
                _layout.Children.Add(_content, () => Bounds);
            }
        }

        /// <summary>
        ///     Gets a value indicating whether this instance is popup active.
        /// </summary>
        /// <value><c>true</c> if this instance is popup active; otherwise, <c>false</c>.</value>
        public bool IsPopupActive
        {
            get { return _popup != null; }
        }


        #endregion


        #region Methods

        /// <summary>
        ///     Shows the popup centered to the parent view.
        /// </summary>
        /// <param name="popupView">The popup view.</param>
        public void ShowPopup(View popupView)
        {
            ShowPopup(
                popupView,
                Constraint.RelativeToParent(p => (Width - _popup.WidthRequest)/2),
                Constraint.RelativeToParent(p => (Height - _popup.HeightRequest)/2)
                );
        }

        /// <summary>
        ///     Shows the popup with constraints.
        /// </summary>
        /// <param name="popupView">The popup view.</param>
        /// <param name="xConstraint">X constraint.</param>
        /// <param name="yConstraint">Y constraint.</param>
        /// <param name="widthConstraint">Optional width constraint.</param>
        /// <param name="heightConstraint">Optional height constraint.</param>
        public void ShowPopup(View popupView, Constraint xConstraint, Constraint yConstraint,
            Constraint widthConstraint = null, Constraint heightConstraint = null)
        {
            DismissPopup();
            _popup = popupView;

            //this.layout.InputTransparent = true;
            //this.content.InputTransparent = true;
            _layout.Children.Add(_popup, xConstraint, yConstraint, widthConstraint, heightConstraint);

            _layout.ForceLayout();
        }


        /// <summary>
        ///     Shows the popup.
        /// </summary>
        /// <param name="popupView">The popup view.</param>
        /// <param name="presenter">The presenter.</param>
        /// <param name="location">The location.</param>
        /// <param name="paddingX">The padding x.</param>
        /// <param name="paddingY">The padding y.</param>
        public void ShowPopup(View popupView, View presenter, PopupLocation location, float paddingX = 0,
            float paddingY = 0)
        {
            DismissPopup();
            _popup = popupView;

            Constraint constraintX = null, constraintY = null;

            switch (location)
            {
                case PopupLocation.Bottom:
                    constraintX =
                        Constraint.RelativeToParent(parent => presenter.X + (presenter.Width - _popup.WidthRequest)/2);
                    constraintY =
                        Constraint.RelativeToParent(parent => parent.Y + presenter.Y + presenter.Height + paddingY);
                    break;
                case PopupLocation.Top:
                    constraintX =
                        Constraint.RelativeToParent(parent => presenter.X + (presenter.Width - _popup.WidthRequest)/2);
                    constraintY = Constraint.RelativeToParent(parent =>
                        parent.Y + presenter.Y - _popup.HeightRequest/2 - paddingY);
                    break;
                //case PopupLocation.Left:
                //    constraintX = Constraint.RelativeToView(presenter, (parent, view) => ((view.X + view.Height / 2) - parent.X) + this.popup.HeightRequest / 2);
                //    constraintY = Constraint.RelativeToView(presenter, (parent, view) => parent.Y + view.Y + view.Width + paddingY);
                //    break;
                //case PopupLocation.Right:
                //    constraintX = Constraint.RelativeToView(presenter, (parent, view) => ((view.X + view.Height / 2) - parent.X) + this.popup.HeightRequest / 2);
                //    constraintY = Constraint.RelativeToView(presenter, (parent, view) => parent.Y + view.Y - this.popup.WidthRequest - paddingY);
                //    break;
            }

            ShowPopup(popupView, constraintX, constraintY);
        }


        /// <summary>
        /// HideAlerts
        /// </summary>
        public void HideAlerts()
        {
            _layout.Children.Clear();
        }

        /// <summary>
        ///     Dismisses the popup.
        /// </summary>
        public void DismissPopup()
        {
            if (_popup != null)
            {
                _layout.Children.Remove(_popup);
                _popup = null;
            }

            _layout.InputTransparent = false;

            if (_content != null)
            {
                _content.InputTransparent = false;
            }
        }

        /// <summary>
        /// DismissPopup
        /// </summary>
        /// <param name="popuptype"></param>
        public void DismissPopup(Type popuptype)
        {
            if (_popup != null && _popup.GetType() == popuptype)
            {
                _layout.Children.Remove(_popup);
                _popup = null;
            }
            _layout.InputTransparent = false;

            if (_content != null)
            {
                _content.InputTransparent = false;
            }
        }

        /// <summary>
        /// OnSizeAllocated
        /// </summary>
        /// <param name="width"></param>
        /// <param name="height"></param>
        protected override void OnSizeAllocated(double width, double height)
        {
            base.OnSizeAllocated(width, height);

            if (_popup == null) return;
            _popup.HeightRequest = height;
            _popup.WidthRequest = width;
        }

        #endregion
    }
}