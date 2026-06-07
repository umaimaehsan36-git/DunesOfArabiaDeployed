namespace DunesOfArabia.Theme
{
    /// <summary>
    /// Global theme constants for the Dunes of Arabia application.
    /// Use these values throughout the project to maintain consistent branding.
    /// </summary>
    public static class ThemeConstants
    {
        /// <summary>
        /// Primary color palette for the Saudi Heritage theme
        /// </summary>
        public static class Colors
        {
            // Primary Browns (Heritage theme)
            public const string PrimaryBrown = "#8B5A3C";      // Main brand color
            public const string DarkBrown = "#5C3D2E";         // Darker variant for hover/active states
            public const string LightBrown = "#D4AF8E";        // Gold accent
            public const string BeigeLight = "#E8DCC8";        // Light beige for borders/backgrounds

            // Neutral colors
            public const string White = "#FFFFFF";
            public const string TextDark = "#333333";
            public const string TextGray = "#666666";
            public const string TextLight = "#8a7060";

            // Status colors
            public const string ErrorRed = "#dc3545";
            public const string ErrorBackground = "#f8d7da";
            public const string ErrorBorder = "#f5c6cb";
            public const string ErrorText = "#721c24";

            public const string SuccessGreen = "#28a745";
            public const string InfoBlue = "#17a2b8";
            public const string WarningOrange = "#ffc107";

            // Gradient
            public const string GradientStart = "#8B5A3C";
            public const string GradientEnd = "#5C3D2E";
        }

        /// <summary>
        /// Typography settings
        /// </summary>
        public static class Typography
        {
            public const string PrimaryFont = "'Segoe UI', Tahoma, Geneva, Verdana, sans-serif";
            public const string HeadingFont = "Georgia, serif";

            // Font sizes
            public const string FontSizeXS = "0.75rem";
            public const string FontSizeSM = "0.8rem";
            public const string FontSizeBase = "0.95rem";
            public const string FontSizeLG = "1rem";
            public const string FontSizeXL = "1.1rem";
            public const string FontSize2XL = "1.3rem";
            public const string FontSize3XL = "1.8rem";
            public const string FontSize4XL = "2rem";
            public const string FontSize5XL = "2.5rem";

            // Font weights
            public const int FontWeightLight = 300;
            public const int FontWeightNormal = 400;
            public const int FontWeightMedium = 500;
            public const int FontWeightSemiBold = 600;
            public const int FontWeightBold = 700;
        }

        /// <summary>
        /// Spacing values (use multiples of 0.5rem = 8px base)
        /// </summary>
        public static class Spacing
        {
            public const string XS = "0.25rem";   // 4px
            public const string SM = "0.5rem";    // 8px
            public const string MD = "1rem";      // 16px
            public const string LG = "1.5rem";    // 24px
            public const string XL = "2rem";      // 32px
            public const string XXL = "3rem";     // 48px
        }

        /// <summary>
        /// Border radius values
        /// </summary>
        public static class BorderRadius
        {
            public const string None = "0";
            public const string SM = "4px";
            public const string MD = "6px";
            public const string LG = "12px";
            public const string Full = "9999px";
        }

        /// <summary>
        /// Box shadow values
        /// </summary>
        public static class Shadows
        {
            public const string None = "none";
            public const string SM = "0 2px 4px rgba(0, 0, 0, 0.1)";
            public const string MD = "0 4px 12px rgba(0, 0, 0, 0.15)";
            public const string LG = "0 8px 20px rgba(0, 0, 0, 0.25)";
            public const string XL = "0 20px 60px rgba(0, 0, 0, 0.3)";
            public const string BrownHover = "0 8px 20px rgba(139, 90, 60, 0.4)";
        }

        /// <summary>
        /// Animation/transition values
        /// </summary>
        public static class Animation
        {
            public const string TransitionFast = "0.15s ease";
            public const string TransitionBase = "0.3s ease";
            public const string TransitionSlow = "0.6s ease-out";
        }

        /// <summary>
        /// Breakpoints for responsive design
        /// </summary>
        public static class Breakpoints
        {
            public const string Mobile = "max-width: 600px";
            public const string Tablet = "max-width: 900px";
            public const string Desktop = "min-width: 901px";
        }

        /// <summary>
        /// Z-index scale for layering
        /// </summary>
        public static class ZIndex
        {
            public const int Base = 0;
            public const int Dropdown = 10;
            public const int Sticky = 20;
            public const int Fixed = 30;
            public const int Modal = 100;
            public const int Tooltip = 110;
        }

        /// <summary>
        /// Component-specific color schemes
        /// </summary>
        public static class Components
        {
            public static class Button
            {
                public const string PrimaryBg = Colors.PrimaryBrown;
                public const string PrimaryHover = Colors.DarkBrown;
                public const string PrimaryText = Colors.White;
                public const string PrimaryBorder = "none";

                public const string SecondaryBg = "transparent";
                public const string SecondaryBorder = Colors.PrimaryBrown;
                public const string SecondaryText = Colors.PrimaryBrown;

                public const string DisabledBg = "#cccccc";
                public const string DisabledText = "#666666";
            }

            public static class Input
            {
                public const string Border = Colors.BeigeLight;
                public const string BorderFocus = Colors.PrimaryBrown;
                public const string Background = Colors.White;
                public const string Text = Colors.TextDark;
                public const string Placeholder = "#999999";
                public const string BorderRadiusValue = "6px";
                public const string FocusShadow = "0 0 0 4px rgba(139, 90, 60, 0.1)";
            }

            public static class Card
            {
                public const string Background = Colors.White;
                public const string Border = "none";
                public const string BorderRadiusValue = "12px";
                public const string Shadow = Shadows.XL;
            }

            public static class Navbar
            {
                public const string Background = Colors.White;
                public const string TextColor = Colors.TextDark;
                public const string BrandColor = Colors.PrimaryBrown;
                public const string LinkHover = Colors.PrimaryBrown;
            }

            public static class Validation
            {
                public const string ErrorBg = Colors.ErrorBackground;
                public const string ErrorText = Colors.ErrorText;
                public const string ErrorBorder = Colors.ErrorBorder;
                public const string SuccessBg = "#d4edda";
                public const string SuccessText = "#155724";
                public const string SuccessBorder = "#c3e6cb";
            }
        }
    }
}