using HACCP.Core;
using Xamarin.Forms;

namespace HACCP
{
    /// <summary>
    ///     Styles.
    /// </summary>
    public class Styles
    {
        #region Properties

        public static bool IsTablet
        {
            get { return Device.Idiom == TargetIdiom.Tablet; }
        }

        #endregion

        #region Methods

        /// <summary>
        ///     LoadStyles
        /// </summary>
        public static void LoadStyles()
        {
            Application.Current.Resources = new ResourceDictionary();
            //LoadDarkButtonStyle ();
            LoadBorderLessButtonStyle();
            LoadTransparentBorderLessButtonStyle();
            LoadButtonStyle();
            LoadEntryStyle();
            EntryStackStyle();
            LoadItemLabelStyle();
            LoadTempScaleLabelStyle();
            LoadLanguageButtonStyle();
            LoadAlertTitleStyle();
            LoadAlertMessageStyle();
            LoadAlertButtonStyle();
            LoadListBoxCellStyle();
            LoadLineSeperatorStyle();
            LoadPassWordEntryStyle();
            LoadItemIconStyle();
            LoadConnectionStatusLabelStyle();
            LoadDeviceNameStyle();
            LoadBlue24LabelStyle();
            LoadWhite32LabelStyle();
            LoadGolden16LabelStyle();
            LoadPopupOptionTextStyle();
            LoadYellowlabelStyle();
            LoadImageButtonStyle();
            LoadTemperatureEntryStyle();
            LoadWhite14LabelStyle();
            LoadWhite16LabelStyle();
            LoadYellowBorderButtonStyle();
            LoadWrapLabelStyle();
            LoadConverters();
            LoadWhite48LabelStyle();
            LoadBlue14LabelStyle();
            LoadPopupOptionSubTextStyle();
            LoadDeviceIdStyle();
            LoadDeviceIdLabelStyle();
            LoadToastTextStyle();
            LoadAppNameLabelStyle();
            LoadVersionLabelStyle();
            LoadThermoHeaderStyle();
            LoadSmallYellowLabel();
            LoadWhiteNormalStyle();
            LoadGrayHeaderLabel();
            LoadYellowBigLabelStyle();
            LoadNaButtonStyle();
            LoadWhiteXxStyle();
            LoadWhiteXxxStyle();
            LoadButton17Style();
            LoadBlue2ButtonNotActiveStyle();
            LoadBlue2ButtonActiveStyle();
            LoadItemLabel105Style();
            LoadItemLabel115Style();
            LoadItemLabel125Style();
            LoadItemLabel95Style();
            LoadThermomterTempTextStyle();
            LoadThermomterGridStyle();
            LoadRecordNoLabelStyle();
            LoadChecklistRecordTitleStyle();
            LoadWindowsScrollHelpGridStyle();
            LoadWindowsListScrollHelpGridStyle();
            LoadManualTemperatureStyle();
            LoadViewStatusStackStyle();
            LoadHomeIconsStyle();
            LoadUserNameStyle();
            LoadUserNameGridStyle();
            LoadBluetoothIconStyle();
            LoadBattertyconStyle();
            LoadCooperLogoStyle();
            LoadUploadRecordLabelStyle();
            LoadWindowsTitleStyle();
            LoadWindowsTitleStackStyle();
            LoadBottomGridStyle();
            LoadBottomLineStyle();
            LoadWhiteXxWrapStyle();
            LoadBottomPaneluttonStyle();
            LoadBlue2PlusGridStyle();
        }

        /// <summary>
        ///     LoadHomeIconsStyle
        /// </summary>
        public static void LoadHomeIconsStyle()
        {
            var homeIconStyle = new Style(typeof(Image))
            {
                Setters = {
					new Setter { Property = View.VerticalOptionsProperty, Value = LayoutOptions.Center },
                    new Setter { Property = View.HorizontalOptionsProperty, Value = LayoutOptions.Center },
					new Setter { Property = VisualElement.HeightRequestProperty, Value = IsTablet ? 50 : 40 }
				}
            };

            Application.Current.Resources.Add("HomeIconStyle", homeIconStyle);
        }

        /// <summary>
        ///     LoadTransparentBorderLessButtonStyle
        /// </summary>
        public static void LoadTransparentBorderLessButtonStyle()
        {
            if (HaccpAppSettings.SharedInstance.IsWindows)
            {
                Application.Current.Resources.Add("TransparentBorderLessButtonStyle", new Style(typeof(Button)));
                return;
            }

            var transparentBorderLessButtonStyle = new Style(typeof(Button))
            {
                Setters = {
					new Setter { Property = Button.BorderWidthProperty, Value = 0 },
					new Setter { Property = Button.BorderColorProperty, Value = Color.Transparent },
					new Setter { Property = VisualElement.BackgroundColorProperty, Value = Color.Transparent }
					//new Setter { Property = Button.HorizontalOptionsProperty , Value=LayoutOptions.Center},
					//new Setter { Property = Button.VerticalOptionsProperty , Value =LayoutOptions.Center }
				},
                Triggers = {
					new Trigger (typeof(Button)) {
						Property = VisualElement.IsEnabledProperty,
						Value = false,
						Setters = {
							new Setter { Property = Button.BorderColorProperty, Value = Color.Transparent },
							new Setter { Property = Button.BorderWidthProperty, Value = 0.01 }
						}
					}
				}
            };

            Application.Current.Resources.Add("TransparentBorderLessButtonStyle", transparentBorderLessButtonStyle);
        }

        /// <summary>
        ///     LoadBorderLessButtonStyle
        /// </summary>
        public static void LoadBorderLessButtonStyle()
        {
            if (HaccpAppSettings.SharedInstance.IsWindows)
            {
                Application.Current.Resources.Add("BorderLessButtonStyle", new Style(typeof(Button)));
                return;
            }

            var borderLessButtonStyle = new Style(typeof(Button))
            {
                Setters = {
					new Setter { Property = Button.BorderWidthProperty, Value = 0 },
					new Setter { Property = Button.BorderColorProperty, Value = Color.Transparent },
					new Setter { Property = VisualElement.BackgroundColorProperty, Value = Color.Transparent }
					//new Setter { Property = Button.HorizontalOptionsProperty , Value=LayoutOptions.Center},
					//new Setter { Property = Button.VerticalOptionsProperty , Value =LayoutOptions.Center }
				},
                Triggers = {
					new Trigger (typeof(Button)) {
						Property = VisualElement.IsEnabledProperty,
						Value = false,
						Setters = {
							new Setter { Property = Button.BorderColorProperty, Value = Color.Transparent },
							new Setter { Property = Button.BorderWidthProperty, Value = 0.01 }
						}
					}
				}
            };

            //if (Device.Idiom == TargetIdiom.Tablet)
            //	borderLessButtonStyle.Setters.Add (new Setter { Property = Button.WidthRequestProperty, Value = 100 });

            Application.Current.Resources.Add("BorderLessButtonStyle", borderLessButtonStyle);
        }

        /// <summary>
        ///     Loads the dark button style.
        /// </summary>
        public static void LoadDarkButtonStyle()
        {
            var darkButtonStyle = new Style(typeof(Button))
            {
                Setters = {
					new Setter { Property = VisualElement.BackgroundColorProperty, Value = Color.FromRgb (77, 115, 138) },
					new Setter { Property = Button.BorderRadiusProperty, Value = 0 },
					new Setter { Property = VisualElement.HeightRequestProperty, Value = IsTablet ? 80 : 40 },
					new Setter { Property = Button.TextColorProperty, Value = Color.White }
				}
            };

            Application.Current.Resources.Add("DarkButtonStyle", darkButtonStyle);
        }

        /// <summary>
        ///     LoadViewStatusStackStyle
        /// </summary>
        public static void LoadViewStatusStackStyle()
        {
            if (HaccpAppSettings.SharedInstance.IsWindows)
            {
                var windowsStyle = new Style(typeof(StackLayout))
                {
                    Setters = {
						new Setter { Property = Layout.PaddingProperty, Value = new Thickness (11, 0, 0, 7) },
						new Setter { Property = View.HorizontalOptionsProperty, Value = LayoutOptions.Fill },
						new Setter { Property = View.VerticalOptionsProperty, Value = LayoutOptions.Center }
					}
                };
                Application.Current.Resources.Add("ViewStatusStackStyle", windowsStyle);
            }
            else
            {
                var windowsStyle = new Style(typeof(StackLayout))
                {
                    Setters = {
						new Setter { Property = Layout.PaddingProperty, Value = new Thickness (15, 6, 15, 10) },
						new Setter { Property = View.HorizontalOptionsProperty, Value = LayoutOptions.Fill },
						new Setter { Property = View.VerticalOptionsProperty, Value = LayoutOptions.Center }
					}
                };
                Application.Current.Resources.Add("ViewStatusStackStyle", windowsStyle);
            }
        }

        /// <summary>
        ///     Loads the light button style.
        /// </summary>
        public static void LoadButtonStyle()
        {
            var buttonStyle = new Style(typeof(Button))
            {
                Setters = {
					new Setter { Property = VisualElement.HeightRequestProperty, Value = IsTablet ? 55 : 40 },
					new Setter { Property = VisualElement.WidthRequestProperty, Value = IsTablet ? 100 : 40 },
					new Setter { Property = Button.TextColorProperty, Value = Color.White }
				}
            };

            Application.Current.Resources.Add("Button", buttonStyle);
        }

        /// <summary>
        ///     LoadLanguageButtonStyle
        /// </summary>
        public static void LoadLanguageButtonStyle()
        {
            var buttonStyle = new Style(typeof(Button))
            {
                Setters = {
					new Setter { Property = VisualElement.HeightRequestProperty, Value = IsTablet ? 55 : 40 },
					new Setter { Property = Button.TextColorProperty, Value = Color.White },
					new Setter { Property = VisualElement.BackgroundColorProperty, Value = Color.FromHex ("#FDDB00") },
					new Setter { Property = VisualElement.BackgroundColorProperty, Value = Color.FromHex ("#2D5E70") }
				}
            };

            Application.Current.Resources.Add("LanguageButton", buttonStyle);
        }

        /// <summary>
        ///     Loads the dark button style.
        /// </summary>
        public static void LoadImageButtonStyle()
        {
            var imgButtonStyle = new Style(typeof(Button))
            {
                Setters = {
					new Setter { Property = VisualElement.BackgroundColorProperty, Value = Color.Transparent },
					new Setter { Property = VisualElement.HeightRequestProperty, Value = IsTablet ? 80 : 60 },
					new Setter { Property = VisualElement.WidthRequestProperty, Value = IsTablet ? 80 : 60 }
				}
            };

            Application.Current.Resources.Add("ImageButtonStyle", imgButtonStyle);




        }

        /// <summary>
        ///     LoadEntryStyle
        /// </summary>
        public static void LoadEntryStyle()
        {
            var textboxStyle = new Style(typeof(Entry))
            {
                Setters = {
					new Setter { Property = VisualElement.HeightRequestProperty, Value = IsTablet ? 55 : 40 },                    
					new Setter { Property = Entry.TextColorProperty, Value = Color.White } //,
				}
            };

            Application.Current.Resources.Add("Entry", textboxStyle);
        }

        /// <summary>
        ///     LoadPassWordEntryStyle
        /// </summary>
        public static void LoadPassWordEntryStyle()
        {
            var textboxStyle = new Style(typeof(Entry))
            {
                Setters = {
					new Setter { Property = VisualElement.HeightRequestProperty, Value = IsTablet ? 60 : 40 },
					new Setter { Property = Entry.TextColorProperty, Value = Color.Navy },
					new Setter { Property = Entry.IsPasswordProperty, Value = true },
					new Setter { Property = InputView.KeyboardProperty, Value = Keyboard.Numeric },
					new Setter { Property = VisualElement.BackgroundColorProperty, Value = Color.White }
				}
            };

            Application.Current.Resources.Add("PasswordEntry", textboxStyle);
        }

        /// <summary>
        ///     EntryStackStyle
        /// </summary>
        public static void EntryStackStyle()
        {
            var textboxStyle = new Style(typeof(StackLayout))
            {
                Setters = {
					new Setter { Property = Layout.PaddingProperty, Value = "5,5,5,5" },
					new Setter { Property = StackLayout.OrientationProperty, Value = StackOrientation.Vertical }
				}
            };

            Application.Current.Resources.Add("EntryStack", textboxStyle);


        }

        /// <summary>
        ///     LoadWindowsTitleStackStyle
        /// </summary>
        public static void LoadWindowsTitleStackStyle()
        {
            var stackStyle = new Style(typeof(StackLayout))
            {
                Setters = {
					new Setter { Property = Layout.PaddingProperty, Value = new Thickness (15, 0, 0, 10) },
					new Setter { Property = View.VerticalOptionsProperty, Value = LayoutOptions.Center },
					new Setter { Property = VisualElement.HeightRequestProperty, Value = IsTablet ? 45 : 35 },
					new Setter { Property = View.HorizontalOptionsProperty, Value = LayoutOptions.Fill },
					new Setter { Property = StackLayout.OrientationProperty, Value = StackOrientation.Vertical },
					new Setter { Property = VisualElement.BackgroundColorProperty, Value = Color.FromHex ("#14222b") }
				}
            };

            Application.Current.Resources.Add("WindowsTitleStack", stackStyle);
        }

        /// <summary>
        ///     LoadWindowsTitleStyle
        /// </summary>
        public static void LoadWindowsTitleStyle()
        {
            var labelStyle = new Style(typeof(Label))
            {
                Setters = {
					new Setter { Property = Label.FontSizeProperty, Value = IsTablet ? 25 : 27 },
					new Setter { Property = View.VerticalOptionsProperty, Value = TextAlignment.Center },
					new Setter { Property = Label.TextColorProperty, Value = "#E1E1E1" },
					new Setter { Property = Label.HorizontalTextAlignmentProperty, Value = TextAlignment.Center },
					new Setter { Property = Label.LineBreakModeProperty, Value = LineBreakMode.TailTruncation },
					new Setter { Property = Label.FontAttributesProperty, Value = "Bold" }
				}
            };

            Application.Current.Resources.Add("WindowsTitleStyle", labelStyle);
        }

        /// <summary>
        ///     LoadItemLabelStyle
        /// </summary>
        public static void LoadItemLabelStyle()
        {
            var itemLabelStyle = new Style(typeof(Label))
            {
                Setters = {
					new Setter { Property = Label.FontSizeProperty, Value = IsTablet ? 19 : 16 },
					//	new Setter { Property = Label.XAlignProperty, Value = TextAlignment.Center },
					new Setter { Property = Label.TextColorProperty, Value = Color.FromRgb (225, 225, 225) },
					new Setter { Property = View.VerticalOptionsProperty, Value = LayoutOptions.Center }
				},
                Triggers = {
					new Trigger (typeof(Label)) {
						Property = VisualElement.IsEnabledProperty,
						Value = false,
						Setters = {
							new Setter { Property = Label.TextColorProperty, Value = Color.FromRgba (225, 225, 225, 80) }
						}
					}
				}
            };

            Application.Current.Resources.Add("ItemLabelStyle", itemLabelStyle);
        }

        /// <summary>
        ///     LoadUploadRecordLabelStyle
        /// </summary>
        public static void LoadUploadRecordLabelStyle()
        {
            var itemLabelStyle = new Style(typeof(Label))
            {
                Setters = {
					new Setter { Property = Label.FontSizeProperty, Value = IsTablet ? 19 : 16 },
					new Setter { Property = Label.HorizontalTextAlignmentProperty, Value = TextAlignment.Center },
					new Setter { Property = Label.TextColorProperty, Value = Color.FromRgb (225, 225, 225) },
					new Setter { Property = View.VerticalOptionsProperty, Value = LayoutOptions.Center },
					new Setter { Property = VisualElement.WidthRequestProperty, Value = IsTablet ? 300 : 115 }
				},
                Triggers = {
					new Trigger (typeof(Label)) {
						Property = VisualElement.IsEnabledProperty,
						Value = false,
						Setters = {
							new Setter { Property = Label.TextColorProperty, Value = Color.FromRgba (225, 225, 225, 80) }
						}
					}
				}
            };



            Application.Current.Resources.Add("UploadRecordLabelStyle", itemLabelStyle);
        }

        /// <summary>
        ///     LoadTempScaleLabelStyle
        /// </summary>
        public static void LoadTempScaleLabelStyle()
        {
            var tempScaleLabelStyle = new Style(typeof(Label))
            {
                Setters = {
					new Setter { Property = Label.FontSizeProperty, Value = IsTablet ? 19 : 16 },
					new Setter { Property = Label.HorizontalTextAlignmentProperty, Value = TextAlignment.Center },
					new Setter { Property = Label.TextColorProperty, Value = Color.FromRgb (225, 225, 225) },
					new Setter { Property = View.VerticalOptionsProperty, Value = LayoutOptions.Center },
					new Setter { Property = Label.LineBreakModeProperty, Value = LineBreakMode.TailTruncation }
				},
                Triggers = {
					new Trigger (typeof(Label)) {
						Property = VisualElement.IsEnabledProperty,
						Value = false,
						Setters = {
							new Setter { Property = Label.TextColorProperty, Value = Color.FromRgba (225, 225, 225, 80) }
						}
					}
				}
            };

            Application.Current.Resources.Add("TempScaleLabelStyle", tempScaleLabelStyle);
        }

        /// <summary>
        ///     LoadItemIconStyle
        /// </summary>
        public static void LoadItemIconStyle()
        {
            var itemLabelStyle = new Style(typeof(Image))
            {
                Setters = {
					new Setter { Property = View.VerticalOptionsProperty, Value = LayoutOptions.Center }
				},
                Triggers = {
					new Trigger (typeof(Image)) {
						Property = VisualElement.IsEnabledProperty,
						Value = false,
						Setters = {
							new Setter { Property = VisualElement.OpacityProperty, Value = "0.4" }
						}
					}
				}
            };

            Application.Current.Resources.Add("ItemIconStyle", itemLabelStyle);
        }

        /// <summary>
        ///     LoadAlertTitleStyle
        /// </summary>
        public static void LoadAlertTitleStyle()
        {
            var alertTitleStyle = new Style(typeof(Label))
            {
                Setters = {
					new Setter { Property = Label.FontAttributesProperty, Value = FontAttributes.Bold },
					new Setter { Property = Label.FontSizeProperty, Value = IsTablet ? 23 : 20 },
					new Setter { Property = Label.HorizontalTextAlignmentProperty, Value = TextAlignment.Center },
					new Setter { Property = Label.TextColorProperty, Value = Color.FromRgb (77, 115, 138) }
				}
            };

            Application.Current.Resources.Add("AlertTitleStyle", alertTitleStyle);
        }

        /// <summary>
        ///     LoadNaButtonStyle
        /// </summary>
        public static void LoadNaButtonStyle()
        {
            var nAButtonStyle = new Style(typeof(Button))
            {
                Setters = {
					new Setter { Property = Button.FontSizeProperty, Value = IsTablet ? 23 : 20 },
					new Setter { Property = View.HorizontalOptionsProperty, Value = LayoutOptions.Center },
					new Setter { Property = VisualElement.BackgroundColorProperty, Value = Color.Transparent },
					new Setter { Property = Button.TextColorProperty, Value = Color.Yellow },
					new Setter { Property = Button.BorderColorProperty, Value = Color.Transparent },
					new Setter { Property = Button.BorderWidthProperty, Value = 0 }
				},
                Triggers = {
					new Trigger (typeof(Button)) {
						Property = VisualElement.IsEnabledProperty,
						Value = false,
						Setters = {
							new Setter { Property = Button.BorderColorProperty, Value = Color.Transparent },
							new Setter { Property = Button.BorderWidthProperty, Value = 0.01 }
						}
					}
				}
            };

            Application.Current.Resources.Add("NAButton", nAButtonStyle);
        }

        /// <summary>
        ///     LoadBottomPaneluttonStyle
        /// </summary>
        public static void LoadBottomPaneluttonStyle()
        {
            var bottomPaneluttonStyle = new Style(typeof(Button))
            {
                Setters = {
					new Setter { Property = Button.FontSizeProperty, Value = IsTablet ? 23 : 20 },
					new Setter { Property = View.HorizontalOptionsProperty, Value = LayoutOptions.Center },
					new Setter { Property = VisualElement.BackgroundColorProperty, Value = Color.Transparent },
					new Setter { Property = Button.TextColorProperty, Value = Color.Yellow },
					new Setter { Property = Button.BorderColorProperty, Value = Color.Transparent },
					new Setter { Property = VisualElement.WidthRequestProperty, Value = IsTablet ? 150 : 90 },
					new Setter { Property = Button.BorderWidthProperty, Value = 0 }
				},
                Triggers = {
					new Trigger (typeof(Button)) {
						Property = VisualElement.IsEnabledProperty,
						Value = false,
						Setters = {
							new Setter { Property = Button.BorderColorProperty, Value = Color.Transparent },
							new Setter { Property = Button.BorderWidthProperty, Value = 0.01 }
						}
					}
				}
            };

            Application.Current.Resources.Add("BottomPaneluttonStyle", bottomPaneluttonStyle);
        }

        /// <summary>
        ///     LoadAlertMessageStyle
        /// </summary>
        public static void LoadAlertMessageStyle()
        {
            var alertMessageStyle = new Style(typeof(HACCPLineSpacingLabel))
            {
                Setters = {
					new Setter { Property = Label.FontAttributesProperty, Value = FontAttributes.None },
					new Setter { Property = Label.FontSizeProperty, Value = IsTablet ? 19 : 16 },
					new Setter { Property = Label.HorizontalTextAlignmentProperty, Value = TextAlignment.Center },
					new Setter { Property = Label.TextColorProperty, Value = Color.FromRgb (77, 115, 138) }
				}
            };

            Application.Current.Resources.Add("AlertMessageStyle", alertMessageStyle);

        }

        /// <summary>
        ///     LoadAlertButtonStyle
        /// </summary>
        public static void LoadAlertButtonStyle()
        {
            var alertButtonStyle = new Style(typeof(Button))
            {
                Setters = {
					new Setter { Property = Button.FontAttributesProperty, Value = FontAttributes.None },
					new Setter { Property = Button.FontSizeProperty, Value = IsTablet ? 18 : 16 },
				
					new Setter { Property = VisualElement.BackgroundColorProperty, Value = Color.FromRgb (77, 115, 138) },
					new Setter { Property = Button.TextColorProperty, Value = Color.White },
					new Setter { Property = Button.BorderColorProperty, Value = Color.FromRgb (67, 108, 132) },
					new Setter { Property = Button.BorderRadiusProperty, Value = 5 }
				},
                Triggers = {
					new Trigger (typeof(Button)) {
						Property = VisualElement.IsEnabledProperty,
						Value = false,
						Setters = {
							new Setter { Property = Button.TextColorProperty, Value = Color.White },
							new Setter {
								Property = VisualElement.BackgroundColorProperty,
								Value = Color.FromRgba (77, 115, 138, 100)
							}
						}
					}
				}
            };

            Application.Current.Resources.Add("AlertButtonStyle", alertButtonStyle);
        }

        /// <summary>
        ///     LoadListBoxCellStyle
        /// </summary>
        public static void LoadListBoxCellStyle()
        {
            var listBoxCellStyle = new Style(typeof(TextCell))
            {
                Setters = {
					new Setter { Property = TextCell.TextColorProperty, Value = Color.White }
				}
            };

            Application.Current.Resources.Add("ListBoxCellStyle", listBoxCellStyle);
        }

        /// <summary>
        ///     LoadLineSeperatorStyle
        /// </summary>
        public static void LoadLineSeperatorStyle()
        {
            var lineSeperatorStyle = new Style(typeof(BoxView))
            {
                Setters = {
					new Setter { Property = BoxView.ColorProperty, Value = Color.FromRgb (45, 94, 112) }
				}
            };

            Application.Current.Resources.Add("LineSeperatorStyle", lineSeperatorStyle);
        }

        /// <summary>
        ///     LoadConnectionStatusLabelStyle
        /// </summary>
        public static void LoadConnectionStatusLabelStyle()
        {
            var connectionStatusStyle = new Style(typeof(Label))
            {
                Setters = {
					new Setter { Property = Label.TextColorProperty, Value = Color.White },
					new Setter { Property = Label.FontSizeProperty, Value = IsTablet ? 19 : 16 },
					new Setter { Property = Label.LineBreakModeProperty, Value = LineBreakMode.TailTruncation }
				}
            };

            Application.Current.Resources.Add("ConnectionStatusStyle", connectionStatusStyle);
        }

        /// <summary>
        ///     LoadWhiteNormalStyle
        /// </summary>
        public static void LoadWhiteNormalStyle()
        {
            var connectionStatusStyle = new Style(typeof(Label))
            {
                Setters = {
					new Setter { Property = Label.TextColorProperty, Value = Color.White },
					new Setter { Property = Label.FontSizeProperty, Value = IsTablet ? 19 : 16 },
					new Setter { Property = Label.FontAttributesProperty, Value = FontAttributes.None },
					new Setter { Property = View.HorizontalOptionsProperty, Value = LayoutOptions.StartAndExpand },
					new Setter { Property = View.VerticalOptionsProperty, Value = LayoutOptions.Center }
				}
            };

            if (Device.OS == TargetPlatform.Windows)
            {
                connectionStatusStyle.Setters.Add(new Setter
                {
                    Property = VisualElement.WidthRequestProperty,
                    Value = 330
                });
            }

            Application.Current.Resources.Add("WhiteNormal", connectionStatusStyle);
        }

        /// <summary>
        ///     LoadDeviceIdStyle
        /// </summary>
        public static void LoadDeviceIdStyle()
        {
            var deviceIdStyle = new Style(typeof(Label))
            {
                Setters = {
					new Setter { Property = Label.TextColorProperty, Value = Color.FromHex ("#c7f8ff") },
					new Setter { Property = Label.FontSizeProperty, Value = IsTablet ? 22 : 19 },
					new Setter { Property = Label.FontAttributesProperty, Value = FontAttributes.Bold },
					new Setter { Property = Label.LineBreakModeProperty, Value = LineBreakMode.TailTruncation }
				}
            };

            Application.Current.Resources.Add("DeviceIdStyle", deviceIdStyle);
        }

        /// <summary>
        ///     LoadDeviceIdLabelStyle
        /// </summary>
        public static void LoadDeviceIdLabelStyle()
        {
            var deviceIdLabelStyle = new Style(typeof(Label))
            {
                Setters = {
					new Setter { Property = Label.TextColorProperty, Value = Color.FromHex ("#012634") },
					new Setter { Property = Label.FontSizeProperty, Value = IsTablet ? 22 : 19 },
					new Setter { Property = Label.FontAttributesProperty, Value = FontAttributes.Bold },
					new Setter { Property = Label.LineBreakModeProperty, Value = LineBreakMode.TailTruncation }
				}
            };

            Application.Current.Resources.Add("DeviceIdLabelStyle", deviceIdLabelStyle);
        }

        /// <summary>
        ///     LoadDeviceNameStyle
        /// </summary>
        public static void LoadDeviceNameStyle()
        {
            var deviceNameStyle = new Style(typeof(Label))
            {
                Setters = {
					new Setter { Property = Label.TextColorProperty, Value = Color.White },
					new Setter { Property = Label.FontSizeProperty, Value = IsTablet ? 24 : 21 }
				}
            };

            Application.Current.Resources.Add("DeviceNameStyle", deviceNameStyle);
        }

        /// <summary>
        ///     LoadBlue24LabelStyle
        /// </summary>
        public static void LoadBlue24LabelStyle()
        {
            var blue24LabelStyle = new Style(typeof(Label))
            {
                Setters = {
					new Setter { Property = Label.TextColorProperty, Value = Color.FromHex ("#bde4fa") },
					new Setter { Property = Label.FontSizeProperty, Value = IsTablet ? 27 : 24 }
				}
            };

            Application.Current.Resources.Add("Blue24LabelStyle", blue24LabelStyle);
        }

        /// <summary>
        ///     LoadManualTemperatureStyle
        /// </summary>
        public static void LoadManualTemperatureStyle()
        {
            var blue24LabelStyle = new Style(typeof(Label))
            {
                Setters = {
					new Setter { Property = Label.TextColorProperty, Value = Color.White },
					new Setter { Property = Label.FontSizeProperty, Value = IsTablet ? 22 : 20 }
				}
            };


            Application.Current.Resources.Add("ManualTemperatureStyle", blue24LabelStyle);
        }

        /// <summary>
        ///     LoadWhite32LabelStyle
        /// </summary>
        public static void LoadWhite32LabelStyle()
        {
            var white32LabelStyle = new Style(typeof(Label))
            {
                Setters = {
					new Setter { Property = Label.TextColorProperty, Value = Color.White },
					new Setter { Property = Label.FontSizeProperty, Value = IsTablet ? 37 : 34 }
				}
            };

            Application.Current.Resources.Add("White32LabelStyle", white32LabelStyle);
        }

        /// <summary>
        ///     LoadConverters
        /// </summary>
        public static void LoadConverters()
        {
            Application.Current.Resources.Add("RecordStatusImageConverter", new RecordStatusToImageNameConverter());
            Application.Current.Resources.Add("AddButtonEnableToImageConverter", new AddButtonEnableToImageConverter());
            Application.Current.Resources.Add("MinusButtonEnableToImageConverter",
                new MinusButtonEnableToImageConverter());
            Application.Current.Resources.Add("NotConverter", new NotConverter());
            Application.Current.Resources.Add("UserStatusConverter", new UserStatusConverter());
            Application.Current.Resources.Add("strConverter", new EmptyStringConverter());
            Application.Current.Resources.Add("guidConverter", new GuidConverter());
            Application.Current.Resources.Add("BatteryLevelConverter", new BatteryLevelConverter());
            Application.Current.Resources.Add("FontSizeconverter", new FontSizeconverter());
        }

        /// <summary>
        ///     LoadGolden16LabelStyle
        /// </summary>
        public static void LoadGolden16LabelStyle()
        {
            var golden16LabelStyle = new Style(typeof(Label))
            {
                Setters = {
					new Setter { Property = Label.TextColorProperty, Value = Color.Yellow },
					new Setter { Property = Label.FontSizeProperty, Value = IsTablet ? 22 : 19 }
				}
            };

            Application.Current.Resources.Add("Golden16LabelStyle", golden16LabelStyle);
        }

        /// <summary>
        ///     LoadPopupOptionTextStyle
        /// </summary>
        public static void LoadPopupOptionTextStyle()
        {
            var popupOptionTextStyle = new Style(typeof(Label))
            {
                Setters = {
					new Setter { Property = Label.FontSizeProperty, Value = IsTablet ? 19 : 16 },
					new Setter { Property = Label.TextColorProperty, Value = Color.FromHex ("#4d738a") },
					new Setter { Property = View.HorizontalOptionsProperty, Value = LayoutOptions.Start }
				}
            };

            if (Device.OS == TargetPlatform.Windows)
                popupOptionTextStyle.Setters.Add(new Setter
                {
                    Property = VisualElement.WidthRequestProperty,
                    Value = 300
                });

            Application.Current.Resources.Add("PopupOptionTextStyle", popupOptionTextStyle);
        }

        /// <summary>
        ///     LoadYellowlabelStyle
        /// </summary>
        public static void LoadYellowlabelStyle()
        {
            var yellowLabelStyle = new Style(typeof(Label))
            {
                Setters = {
					new Setter { Property = Label.FontSizeProperty, Value = IsTablet ? 19 : 16 },
					new Setter { Property = Label.TextColorProperty, Value = Color.Yellow }
				}
            };

            Application.Current.Resources.Add("YellowLabelStyle", yellowLabelStyle);
        }

        /// <summary>
        ///     LoadTemperatureEntryStyle
        /// </summary>
        public static void LoadTemperatureEntryStyle()
        {
            var textboxStyle = new Style(typeof(Entry))
            {
                Setters = {
					//new Setter { Property = Entry.HeightRequestProperty, Value = 70 },
					new Setter { Property = Entry.TextColorProperty, Value = Color.Transparent }
				}
            };

            Application.Current.Resources.Add("TemperatureEntry", textboxStyle);
        }

        /// <summary>
        ///     LoadWhite14LabelStyle
        /// </summary>
        public static void LoadWhite14LabelStyle()
        {
            var white14LabelStyle = new Style(typeof(Label))
            {
                Setters = {
					new Setter { Property = Label.FontSizeProperty, Value = IsTablet ? 15 : 14 },
					new Setter { Property = Label.TextColorProperty, Value = Color.White },
					new Setter { Property = View.VerticalOptionsProperty, Value = LayoutOptions.Center },
					new Setter { Property = Label.LineBreakModeProperty, Value = LineBreakMode.TailTruncation }
				}
            };

            Application.Current.Resources.Add("White14LabelStyle", white14LabelStyle);
        }

        /// <summary>
        ///     LoadWhite16LabelStyle
        /// </summary>
        public static void LoadWhite16LabelStyle()
        {
            var white16LabelStyle = new Style(typeof(Label))
            {
                Setters = {
					new Setter { Property = Label.FontSizeProperty, Value = IsTablet ? 31 : 28 },
					new Setter { Property = Label.TextColorProperty, Value = Color.White },
					new Setter { Property = View.VerticalOptionsProperty, Value = LayoutOptions.Center },
					new Setter { Property = Label.LineBreakModeProperty, Value = LineBreakMode.TailTruncation }
				}
            };

            Application.Current.Resources.Add("White16LabelStyle", white16LabelStyle);
        }

        /// <summary>
        ///     LoadYellowBorderButtonStyle
        /// </summary>
        public static void LoadYellowBorderButtonStyle()
        {
            var yellowBorderButtonStyle = new Style(typeof(Button))
            {
                Setters = {
					new Setter { Property = Button.FontSizeProperty, Value = IsTablet ? 20 : 18 },
					new Setter { Property = Button.BorderColorProperty, Value = Color.FromHex ("#FDDB00") },
					new Setter { Property = Button.BorderWidthProperty, Value = IsTablet ? 4 : 2 }
				}
            };

            Application.Current.Resources.Add("YellowBorderButtonStyle", yellowBorderButtonStyle);
        }

        /// <summary>
        ///     LoadChecklistRecordTitleStyle
        /// </summary>
        public static void LoadRecordNoLabelStyle()
        {
            var checklistRecordTitleStyle = new Style(typeof(Label))
            {
                Setters = {new Setter { Property = Label.FontSizeProperty, Value = IsTablet ? 19 : 16 },
					new Setter { Property = Label.TextColorProperty, Value = Color.FromHex ("#C3EFFF") },
					new Setter { Property = Label.LineBreakModeProperty, Value = LineBreakMode.TailTruncation }  ,
				}
            };

            Application.Current.Resources.Add("RecordNoLabelStyle", checklistRecordTitleStyle);
        }


        /// <summary>
        ///     LoadChecklistRecordTitleStyle
        /// </summary>
        public static void LoadChecklistRecordTitleStyle()
        {
            var checklistRecordTitleStyle = new Style(typeof(Label))
            {
                Setters = {
					new Setter { Property = Label.FontSizeProperty, Value = IsTablet ? 19 : 16 },
					new Setter { Property = Label.HorizontalTextAlignmentProperty, Value = TextAlignment.Center },
					new Setter { Property = Label.TextColorProperty, Value = Color.White },
					new Setter { Property = View.VerticalOptionsProperty, Value = LayoutOptions.Center },
					new Setter { Property = View.HorizontalOptionsProperty, Value = LayoutOptions.Center },
					new Setter { Property = VisualElement.WidthRequestProperty, Value = IsTablet ? 190 : 150 }
				}
            };

            Application.Current.Resources.Add("ChecklistRecordTitleStyle", checklistRecordTitleStyle);
        }

        /// <summary>
        ///     LoadWrapLabelStyle
        /// </summary>
        public static void LoadWrapLabelStyle()
        {
            var wrapLabelStyle = new Style(typeof(Label))
            {
                Setters = {
					new Setter { Property = Label.FontSizeProperty, Value = IsTablet ? 19 : 16 },
					new Setter { Property = Label.HorizontalTextAlignmentProperty, Value = TextAlignment.Center },
					new Setter { Property = Label.TextColorProperty, Value = Color.White },
					new Setter { Property = View.VerticalOptionsProperty, Value = LayoutOptions.Center },
					new Setter { Property = View.HorizontalOptionsProperty, Value = LayoutOptions.Center },
					new Setter { Property = Label.LineBreakModeProperty, Value = LineBreakMode.WordWrap }
				}
            };

            Application.Current.Resources.Add("WrapLabelStyle", wrapLabelStyle);
        }

        /// <summary>
        ///     LoadWhite48LabelStyle
        /// </summary>
        public static void LoadWhite48LabelStyle()
        {
            var white36LabelStyle = new Style(typeof(Label))
            {
                Setters = {
					new Setter { Property = Label.FontSizeProperty, Value = IsTablet ? 51 : 48 },
					new Setter { Property = Label.TextColorProperty, Value = Color.White }
				}
            };

            Application.Current.Resources.Add("White48LabelStyle", white36LabelStyle);
        }

        /// <summary>
        ///     LoadBlue14LabelStyle
        /// </summary>
        public static void LoadBlue14LabelStyle()
        {
            var blue14LabelStyle = new Style(typeof(Label))
            {
                Setters = {
					new Setter { Property = Label.FontSizeProperty, Value = IsTablet ? 17 : 14 },
					new Setter { Property = Label.TextColorProperty, Value = Color.FromHex ("#C3EFFF") },
					new Setter { Property = Label.LineBreakModeProperty, Value = LineBreakMode.TailTruncation }
				}
            };
            Application.Current.Resources.Add("Blue14LabelStyle", blue14LabelStyle);
        }

        /// <summary>
        ///     LoadPopupOptionSubTextStyle
        /// </summary>
        public static void LoadPopupOptionSubTextStyle()
        {
            var popupOptionSubTextStyle = new Style(typeof(Label))
            {
                Setters = {
					new Setter { Property = Label.FontSizeProperty, Value = IsTablet ? 14 : 11 },
					new Setter { Property = Label.TextColorProperty, Value = Color.FromHex ("#4d738a") }
				}
            };

            Application.Current.Resources.Add("PopupOptionSubTextStyle", popupOptionSubTextStyle);
        }

        /// <summary>
        ///     LoadToastTextStyle
        /// </summary>
        public static void LoadToastTextStyle()
        {
            var toastTextStyle = new Style(typeof(Label))
            {
                Setters = {
					new Setter { Property = Label.FontSizeProperty, Value = IsTablet ? 17 : 14 },
					new Setter { Property = Label.TextColorProperty, Value = Color.White },
					new Setter { Property = View.VerticalOptionsProperty, Value = LayoutOptions.Center },
					new Setter { Property = View.HorizontalOptionsProperty, Value = LayoutOptions.Center }
				}
            };

            Application.Current.Resources.Add("ToastTextStyle", toastTextStyle);
        }

        /// <summary>
        ///     LoadAppNameLabelStyle
        /// </summary>
        public static void LoadAppNameLabelStyle()
        {
            var appNameLabelStyle = new Style(typeof(Label))
            {
                Setters = {
					new Setter { Property = Label.FontSizeProperty, Value = IsTablet ? 20 : 15 },
					new Setter { Property = Label.TextColorProperty, Value = Color.White }
				}
            };

            Application.Current.Resources.Add("HACCPManager", appNameLabelStyle);
        }

        /// <summary>
        ///     LoadVersionLabelStyle
        /// </summary>
        public static void LoadVersionLabelStyle()
        {
            var appNameLabelStyle = new Style(typeof(Label))
            {
                Setters = {
					new Setter { Property = Label.FontSizeProperty, Value = IsTablet ? 15 : 12 },
					new Setter { Property = Label.TextColorProperty, Value = Color.White }
				}
            };

            Application.Current.Resources.Add("VersionLabel", appNameLabelStyle);
        }

        /// <summary>
        ///     LoadThermoHeaderStyle
        /// </summary>
        public static void LoadThermoHeaderStyle()
        {
            var thermoHeaderStyle = new Style(typeof(Grid))
            {
                Setters = {
					new Setter { Property = VisualElement.BackgroundColorProperty, Value = Color.FromHex ("#254f5e") },
					new Setter { Property = VisualElement.HeightRequestProperty, Value = IsTablet ? 100 : 70 },
					new Setter { Property = View.HorizontalOptionsProperty, Value = LayoutOptions.Fill }
				}
            };

            Application.Current.Resources.Add("ThermoHeader", thermoHeaderStyle);
        }

        /// <summary>
        ///     LoadSmallYellowLabel
        /// </summary>
        public static void LoadSmallYellowLabel()
        {
            var yellowLabelStyle = new Style(typeof(Label))
            {
                Setters = {
					new Setter { Property = Label.TextColorProperty, Value = Color.Yellow },
					new Setter { Property = Label.FontSizeProperty, Value = IsTablet ? 16 : 13 },
					new Setter { Property = View.VerticalOptionsProperty, Value = LayoutOptions.Center }
				}
            };

            Application.Current.Resources.Add("SmallYellowLabel", yellowLabelStyle);
        }

        /// <summary>
        ///     LoadYellowBigLabelStyle
        /// </summary>
        public static void LoadYellowBigLabelStyle()
        {
            var yellowLabelStyle = new Style(typeof(Label))
            {
                Setters = {
					new Setter { Property = Label.TextColorProperty, Value = Color.Yellow },
					new Setter { Property = Label.FontSizeProperty, Value = IsTablet ? 23 : 17 },
					new Setter { Property = VisualElement.BackgroundColorProperty, Value = Color.Transparent },
					new Setter { Property = View.VerticalOptionsProperty, Value = LayoutOptions.Center },
					new Setter { Property = Label.LineBreakModeProperty, Value = LineBreakMode.TailTruncation }
				}
            };

            //			if (IsTablet) {
            //				yellowLabelStyle.Setters.Add (new Setter{ Property = Label.WidthRequestProperty, Value = 100 });
            //			}

            Application.Current.Resources.Add("YellowBigLabel", yellowLabelStyle);
        }

        /// <summary>
        ///     LoadWhiteXxStyle
        /// </summary>
        public static void LoadWhiteXxStyle()
        {
            var appNameLabelStyle = new Style(typeof(Label))
            {
                Setters = {
					new Setter { Property = Label.FontSizeProperty, Value = IsTablet ? 19 : 17 },
					new Setter { Property = View.VerticalOptionsProperty, Value = LayoutOptions.Center },
					new Setter { Property = Label.TextColorProperty, Value = Color.White },
					new Setter { Property = Label.LineBreakModeProperty, Value = LineBreakMode.TailTruncation }
				}
            };

            if (Device.OS == TargetPlatform.Windows)
            {
                appNameLabelStyle.Setters.Add(new Setter
                {
                    Property = VisualElement.WidthRequestProperty,
                    Value = IsTablet ? 400 : 335
                });
            }

            Application.Current.Resources.Add("WhiteXX", appNameLabelStyle);
        }

        /// <summary>
        ///     LoadWhiteXxWrapStyle
        /// </summary>
        public static void LoadWhiteXxWrapStyle()
        {
            var appNameLabelStyle = new Style(typeof(Label))
            {
                Setters = {
					new Setter { Property = Label.FontSizeProperty, Value = IsTablet ? 20 : 17 },
					new Setter { Property = View.VerticalOptionsProperty, Value = LayoutOptions.Center },
					new Setter { Property = Label.TextColorProperty, Value = Color.White },
					new Setter { Property = Label.LineBreakModeProperty, Value = LineBreakMode.WordWrap }
				}
            };

            if (Device.OS == TargetPlatform.Windows)
            {
                appNameLabelStyle.Setters.Add(new Setter
                {
                    Property = VisualElement.WidthRequestProperty,
                    Value = IsTablet ? 400 : 335
                });
            }

            Application.Current.Resources.Add("WhiteXXWrap", appNameLabelStyle);
        }

        /// <summary>
        ///     LoadGrayHeaderLabel
        /// </summary>
        public static void LoadGrayHeaderLabel()
        {
            var yellowLabelStyle = new Style(typeof(Label))
            {
                Setters = {
					new Setter { Property = Label.TextColorProperty, Value = Color.Gray },
					new Setter { Property = Label.FontSizeProperty, Value = IsTablet ? 20 : 17 },
					new Setter { Property = Label.LineBreakModeProperty, Value = LineBreakMode.TailTruncation }
				}
            };

            if (Device.OS == TargetPlatform.Windows)
            {
                yellowLabelStyle.Setters.Add(new Setter
                {
                    Property = VisualElement.WidthRequestProperty,
                    Value = IsTablet ? 400 : 335
                });
            }

            Application.Current.Resources.Add("GrayHeader", yellowLabelStyle);
        }

        /// <summary>
        ///     LoadWhiteXxxStyle
        /// </summary>
        public static void LoadWhiteXxxStyle()
        {
            var appNameLabelStyle = new Style(typeof(Label))
            {
                Setters = {
					new Setter { Property = Label.FontSizeProperty, Value = IsTablet ? 31 : 28 },
					new Setter { Property = View.VerticalOptionsProperty, Value = LayoutOptions.Center },
					new Setter { Property = View.HorizontalOptionsProperty, Value = LayoutOptions.Center },
					new Setter { Property = Label.TextColorProperty, Value = Color.White }
				}
            };

            Application.Current.Resources.Add("WhiteXXX", appNameLabelStyle);
        }

        /// <summary>
        ///     LoadButton17Style
        /// </summary>
        public static void LoadButton17Style()
        {
            var appNameLabelStyle = new Style(typeof(Button))
            {
                Setters = {
					new Setter { Property = Button.FontSizeProperty, Value = IsTablet ? 20 : 17 },
					new Setter { Property = View.HorizontalOptionsProperty, Value = LayoutOptions.Fill }
				}
            };

            Application.Current.Resources.Add("Button17", appNameLabelStyle);
        }

        /// <summary>
        ///     LoadBlue2ButtonActiveStyle
        /// </summary>
        public static void LoadBlue2ButtonActiveStyle()
        {
            var blue2ButtonActiveStyle = new Style(typeof(Button))
            {
                Setters = {
					new Setter { Property = Button.TextColorProperty, Value = Color.White },
					new Setter { Property = VisualElement.BackgroundColorProperty, Value = Color.FromHex ("#254f5e") },
					new Setter { Property = Button.BorderColorProperty, Value = Color.White },
					new Setter { Property = Button.BorderWidthProperty, Value = 1 },
					new Setter { Property = View.HorizontalOptionsProperty, Value = LayoutOptions.Center }
				}
            };

            Application.Current.Resources.Add("Blue2ButtonActiveStyle", blue2ButtonActiveStyle);
        }

        /// <summary>
        ///     LoadBlue2ButtonNotActiveStyle
        /// </summary>
        public static void LoadBlue2ButtonNotActiveStyle()
        {
            var blue2ButtonNotActiveStyle = new Style(typeof(Button))
            {
                Setters = {
					new Setter { Property = Button.TextColorProperty, Value = Color.White },
					//new Setter { Property = Button.l, Value = Color.White } ,
					new Setter { Property = VisualElement.BackgroundColorProperty, Value = Color.FromHex ("#3e7589") },
					new Setter { Property = Button.BorderColorProperty, Value = Color.White },
					new Setter { Property = Button.BorderWidthProperty, Value = 1 },
					new Setter { Property = View.HorizontalOptionsProperty, Value = LayoutOptions.Center }
				}
            };

            Application.Current.Resources.Add("Blue2ButtonNotActiveStyle", blue2ButtonNotActiveStyle);
        }

        /// <summary>
        ///     LoadUserNameStyle
        /// </summary>
        public static void LoadUserNameStyle()
        {
            var userNameStyle = new Style(typeof(Label))
            {
                Setters = {
					new Setter { Property = Label.FontSizeProperty, Value = IsTablet ? 18 : 15 },
					new Setter { Property = VisualElement.HeightRequestProperty, Value = IsTablet ? 26 : 23 },
					new Setter { Property = Label.HorizontalTextAlignmentProperty, Value = TextAlignment.Center },
					new Setter { Property = Label.TextColorProperty, Value = Color.Yellow },
					new Setter { Property = View.VerticalOptionsProperty, Value = LayoutOptions.Center },
					new Setter { Property = View.HorizontalOptionsProperty, Value = LayoutOptions.Center },
					new Setter { Property = Label.LineBreakModeProperty, Value = LineBreakMode.TailTruncation }
				}
            };

            Application.Current.Resources.Add("UserNameStyle", userNameStyle);
        }

        /// <summary>
        ///     LoadUserNameGridStyle
        /// </summary>
        public static void LoadUserNameGridStyle()
        {
            var userNameGridStyle = new Style(typeof(Grid))
            {
                Setters = {
					new Setter {
						Property = Layout.PaddingProperty,
						Value = IsTablet ? new Thickness (0, 0, 0, 0) : new Thickness (0, 4, 0, 0)
					},
					new Setter { Property = View.VerticalOptionsProperty, Value = LayoutOptions.End },
					new Setter { Property = VisualElement.HeightRequestProperty, Value = IsTablet ? 30 : 20 }
				}
            };

            Application.Current.Resources.Add("UserNameGridStyle", userNameGridStyle);
        }

        /// <summary>
        ///     LoadItemLabel95Style
        /// </summary>
        public static void LoadItemLabel95Style()
        {
            var itemLabelStyle = new Style(typeof(Label))
            {
                Setters = {
					new Setter { Property = Label.FontSizeProperty, Value = IsTablet ? 19 : 15 },
					new Setter { Property = Label.HorizontalTextAlignmentProperty, Value = TextAlignment.Center },
					new Setter { Property = Label.TextColorProperty, Value = Color.White },
					new Setter { Property = View.VerticalOptionsProperty, Value = LayoutOptions.Center },
					new Setter { Property = View.HorizontalOptionsProperty, Value = LayoutOptions.Center },
					new Setter { Property = VisualElement.WidthRequestProperty, Value = IsTablet ? 300 : 95 }
				},
                Triggers = {
					new Trigger (typeof(Label)) {
						Property = VisualElement.IsEnabledProperty,
						Value = false,
						Setters = {
							new Setter {
								Property = Label.TextColorProperty,
								Value = Color.FromRgba (225, 225, 225, 80)
							}
						}
					}
				}
            };

            Application.Current.Resources.Add("ItemLabel95", itemLabelStyle);
        }

        /// <summary>
        ///     LoadItemLabel115Style
        /// </summary>
        public static void LoadItemLabel115Style()
        {
            var itemLabelStyle = new Style(typeof(Label))
            {
                Setters = {
					new Setter { Property = Label.FontSizeProperty, Value = IsTablet ? 19 : 15 },
					new Setter { Property = Label.HorizontalTextAlignmentProperty, Value = TextAlignment.Center },
					new Setter { Property = Label.TextColorProperty, Value = Color.White },
					new Setter { Property = View.VerticalOptionsProperty, Value = LayoutOptions.Center },
					new Setter { Property = View.HorizontalOptionsProperty, Value = LayoutOptions.Center },
					new Setter { Property = VisualElement.WidthRequestProperty, Value = IsTablet ? 300 : 115 }
				},
                Triggers = {
					new Trigger (typeof(Label)) {
						Property = VisualElement.IsEnabledProperty,
						Value = false,
						Setters = {
							new Setter {
								Property = Label.TextColorProperty,
								Value = Color.FromRgba (225, 225, 225, 80)
							}
						}
					}
				}
            };

            Application.Current.Resources.Add("ItemLabel115", itemLabelStyle);
        }

        /// <summary>
        ///     LoadItemLabel125Style
        /// </summary>
        public static void LoadItemLabel125Style()
        {
            var itemLabelStyle = new Style(typeof(Label))
            {
                Setters = {
					new Setter { Property = Label.FontSizeProperty, Value = IsTablet ? 19 : 15 },
					new Setter { Property = Label.HorizontalTextAlignmentProperty, Value = TextAlignment.Center },
					new Setter { Property = Label.TextColorProperty, Value = Color.White },
					new Setter { Property = View.VerticalOptionsProperty, Value = LayoutOptions.Center },
					new Setter { Property = View.HorizontalOptionsProperty, Value = LayoutOptions.Center },
					new Setter { Property = VisualElement.WidthRequestProperty, Value = IsTablet ? 300 : 125 }
				},
                Triggers = {
					new Trigger (typeof(Label)) {
						Property = VisualElement.IsEnabledProperty,
						Value = false,
						Setters = {
							new Setter {
								Property = Label.TextColorProperty,
								Value = Color.FromRgba (225, 225, 225, 80)
							}
						}
					}
				}
            };

            Application.Current.Resources.Add("ItemLabel125", itemLabelStyle);
        }

        /// <summary>
        ///     LoadItemLabel105Style
        /// </summary>
        public static void LoadItemLabel105Style()
        {
            var itemLabelStyle = new Style(typeof(Label))
            {
                Setters = {
					new Setter { Property = Label.FontSizeProperty, Value = IsTablet ? 19 : 15 },
					new Setter { Property = Label.HorizontalTextAlignmentProperty, Value = TextAlignment.Center },
					new Setter { Property = Label.TextColorProperty, Value = Color.White },
					new Setter { Property = View.VerticalOptionsProperty, Value = LayoutOptions.Center },
					new Setter { Property = View.HorizontalOptionsProperty, Value = LayoutOptions.Center },
					new Setter { Property = Label.LineBreakModeProperty, Value = LineBreakMode.WordWrap },
					new Setter { Property = VisualElement.WidthRequestProperty, Value = IsTablet ? 300 : 105 }
				},
                Triggers = {
					new Trigger (typeof(Label)) {
						Property = VisualElement.IsEnabledProperty,
						Value = false,
						Setters = {
							new Setter {
								Property = Label.TextColorProperty,
								Value = Color.FromRgba (225, 225, 225, 80)
							}
						}
					}
				}
            };

            Application.Current.Resources.Add("ItemLabel105", itemLabelStyle);
        }

        /// <summary>
        ///     LoadThermomterTempTextStyle
        /// </summary>
        public static void LoadThermomterTempTextStyle()
        {
            var thermomterTempTextStyle = new Style(typeof(Label))
            {
                Setters = {
					new Setter {
						Property = View.HorizontalOptionsProperty,
						Value = IsTablet ? LayoutOptions.Center : LayoutOptions.Start
					},
					new Setter { Property = Label.TextColorProperty, Value = Color.White }
				}
            };

            Application.Current.Resources.Add("ThermomterTempTextStyle", thermomterTempTextStyle);
        }

        /// <summary>
        ///     LoadThermomterGridStyle
        /// </summary>
        public static void LoadThermomterGridStyle()
        {
            var thermomterGridStyle = new Style(typeof(Grid))
            {
                Setters = {
					new Setter { Property = View.VerticalOptionsProperty, Value = LayoutOptions.Center },
					new Setter { Property = View.HorizontalOptionsProperty, Value = LayoutOptions.Center },
					new Setter { Property = VisualElement.HeightRequestProperty, Value = IsTablet ? 100 : 80 },
					new Setter { Property = VisualElement.WidthRequestProperty, Value = IsTablet ? 100 : 80 }
				}
            };

            Application.Current.Resources.Add("ThermomterGridStyle", thermomterGridStyle);
        }

        /// <summary>
        ///     LoadWindowsScrollHelpGridStyle
        /// </summary>
        public static void LoadWindowsScrollHelpGridStyle()
        {
            var windowsScrollHelpGridStyle = new Style(typeof(Grid))
            {
                Setters = {
					new Setter {
						Property = VisualElement.HeightRequestProperty,
						Value = HaccpAppSettings.SharedInstance.IsWindows ? 0 : 0
					},
                    //new Setter {
                    //    Property = VisualElement.IsVisibleProperty,
                    //    Value = HaccpAppSettings.SharedInstance.IsWindows
                    //}
				}
            };

            Application.Current.Resources.Add("WindowsScrollHelpGridStyle", windowsScrollHelpGridStyle);
        }

        /// <summary>
        ///     LoadWindowsListScrollHelpGridStyle
        /// </summary>
        public static void LoadWindowsListScrollHelpGridStyle()
        {
            var windowsListScrollHelpGridStyle = new Style(typeof(Grid))
            {
                Setters = {
					new Setter {
						Property = VisualElement.HeightRequestProperty,
						Value = HaccpAppSettings.SharedInstance.IsWindows ? 0 : 0
					},
                    //new Setter {
                    //    Property = VisualElement.IsVisibleProperty,
                    //    Value = HaccpAppSettings.SharedInstance.IsWindows
                    //}
				}
            };

            Application.Current.Resources.Add("WindowsListScrollHelpGridStyle", windowsListScrollHelpGridStyle);
        }

        /// <summary>
        ///     LoadBluetoothIconStyle
        /// </summary>
        public static void LoadBluetoothIconStyle()
        {
            var bluetoothIconStyle = new Style(typeof(Image));
            if (Device.OS == TargetPlatform.Android && Device.Idiom == TargetIdiom.Tablet)
                bluetoothIconStyle.Setters.Add(new Setter { Property = VisualElement.HeightRequestProperty, Value = 28 });


            Application.Current.Resources.Add("BluetoothIconStyle", bluetoothIconStyle);
        }

        /// <summary>
        ///     LoadBattertyconStyle
        /// </summary>
        public static void LoadBattertyconStyle()
        {
            var batteryIconStyle = new Style(typeof(Image));
            if (Device.OS == TargetPlatform.Android && Device.Idiom == TargetIdiom.Tablet)
                batteryIconStyle.Setters.Add(new Setter { Property = VisualElement.HeightRequestProperty, Value = 15 });


            Application.Current.Resources.Add("BatteryIconStyle", batteryIconStyle);
        }

        /// <summary>
        ///     LoadCooperLogoStyle
        /// </summary>
        public static void LoadCooperLogoStyle()
        {
            var cooperLogoStyle = new Style(typeof(Image));
            cooperLogoStyle.Setters.Add(Device.Idiom == TargetIdiom.Tablet
                ? new Setter { Property = VisualElement.WidthRequestProperty, Value = 115 }
                : new Setter { Property = VisualElement.WidthRequestProperty, Value = 90 });
            Application.Current.Resources.Add("CooperLogoStyle", cooperLogoStyle);
        }

        /// <summary>
        ///     LoadBottomGridStyle
        /// </summary>
        public static void LoadBottomGridStyle()
        {
            var bottomGridStyle = new Style(typeof(Grid));
            bottomGridStyle.Setters.Add(Device.OS == TargetPlatform.Windows
                ? new Setter { Property = VisualElement.HeightRequestProperty, Value = 50 }
                : new Setter { Property = VisualElement.HeightRequestProperty, Value = 50 });
            Application.Current.Resources.Add("BottomGridStyle", bottomGridStyle);
        }

        /// <summary>
        ///     LoadBottomLineStyle
        /// </summary>
        public static void LoadBottomLineStyle()
        {
            var bottomLineStyle = new Style(typeof(BoxView));
            bottomLineStyle.Setters.Add(Device.OS == TargetPlatform.Windows
                ? new Setter { Property = VisualElement.HeightRequestProperty, Value = 50 }
                : new Setter { Property = VisualElement.HeightRequestProperty, Value = 50 });
            Application.Current.Resources.Add("BottomLineStyle", bottomLineStyle);
        }

        public static void LoadBlue2PlusGridStyle()
        {
            var blue2PlusGridStyle = new Style(typeof(Grid));
            blue2PlusGridStyle.Setters.Add(Device.OS == TargetPlatform.Windows
                ? new Setter { Property = VisualElement.HeightRequestProperty, Value = 35 }
                : new Setter { Property = VisualElement.HeightRequestProperty, Value = 30 });
            Application.Current.Resources.Add("Blue2PlusGridStyle", blue2PlusGridStyle);
        }

        #endregion
    }
}