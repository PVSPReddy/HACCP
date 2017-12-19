using System;
using Xamarin.Forms;

namespace HACCP
{

	public class HACCPShapeView : BoxView
	{
        /// <summary>
        ///  BindableProperty ShapeTypeProperty
        /// </summary>
		public static readonly BindableProperty ShapeTypeProperty = BindableProperty.Create<HACCPShapeView, ShapeType> (s => s.ShapeType, ShapeType.Box);

        /// <summary>
        /// BindableProperty StrokeColorProperty
        /// </summary>
		public static readonly BindableProperty StrokeColorProperty = BindableProperty.Create<HACCPShapeView, Color> (s => s.StrokeColor, Color.Default);

        /// <summary>
        /// BindableProperty StrokeWidthProperty
        /// </summary>
		public static readonly BindableProperty StrokeWidthProperty = BindableProperty.Create<HACCPShapeView, float> (s => s.StrokeWidth, 1f);

        /// <summary>
        /// BindableProperty IndicatorPercentageProperty
        /// </summary>
		public static readonly BindableProperty IndicatorPercentageProperty = BindableProperty.Create<HACCPShapeView, float> (s => s.IndicatorPercentage, 0f);

        /// <summary>
        /// BindableProperty CornerRadiusProperty
        /// </summary>
		public static readonly BindableProperty CornerRadiusProperty = BindableProperty.Create<HACCPShapeView, float> (s => s.CornerRadius, 0f);

        /// <summary>
        /// BindableProperty PaddingProperty
        /// </summary>
		public static readonly BindableProperty PaddingProperty = BindableProperty.Create<HACCPShapeView, Thickness> (s => s.Padding, default(Thickness));

        /// <summary>
        /// ShapeType
        /// </summary>
		public ShapeType ShapeType {
			get{ return (ShapeType)GetValue (ShapeTypeProperty); }
			set{ SetValue (ShapeTypeProperty, value); }
		}

        /// <summary>
        /// StrokeColor
        /// </summary>
		public Color StrokeColor {
			get{ return (Color)GetValue (StrokeColorProperty); }
			set{ SetValue (StrokeColorProperty, value); }
		}

        /// <summary>
        /// StrokeWidth
        /// </summary>
		public float StrokeWidth {
			get{ return (float)GetValue (StrokeWidthProperty); }
			set{ SetValue (StrokeWidthProperty, value); }
		}

        /// <summary>
        /// IndicatorPercentage
        /// </summary>
		public float IndicatorPercentage {
			get{ return (float)GetValue (IndicatorPercentageProperty); }
			set {
				if (ShapeType != ShapeType.CircleIndicator)
					throw new ArgumentException ("Can only specify this property with CircleIndicator");
				SetValue (IndicatorPercentageProperty, value);
			}
		}


        /// <summary>
        /// CornerRadius
        /// </summary>
		public float CornerRadius {
			get{ return (float)GetValue (CornerRadiusProperty); }
			set {
				if (ShapeType != ShapeType.Box)
					throw new ArgumentException ("Can only specify this property with Box");
				SetValue (CornerRadiusProperty, value);
			}
		}

        /// <summary>
        /// Padding
        /// </summary>
		public Thickness Padding {
			get{ return (Thickness)GetValue (PaddingProperty); }
			set{ SetValue (PaddingProperty, value); }
		}
	}

    /// <summary>
    /// ShapeType Enum
    /// </summary>
	public enum ShapeType
	{
		Box,
		Circle,
		CircleIndicator
	}
}

