export const brandSurveyTheme = {
  themeName: "brandTailwind",
  colorPalette: "light",
  isPanelless: false,
  cssVariables: {
    /* Base */
    "--sjs-font-family": "Inter, system-ui, sans-serif",
    "--sjs-corner-radius": "12px",

    /* Background + surfaces */
    "--sjs-general-backcolor": "#ffffff",      // bg.white
    "--sjs-general-backcolor-dim": "#F5F5F5",  // bg.gray
    "--sjs-general-backcolor-dim-light": "#EBEBEB", // bg.hover

    /* Text */
    "--sjs-general-forecolor": "#000c2e",      // brand.navy
    "--sjs-general-forecolor-light": "#283361",// brand.navyLight

    /* Primary (buttons, progress, active states) */
    "--sjs-primary-backcolor": "#e94f2d",      // brand.orange
    "--sjs-primary-backcolor-light": "#FF6B3D",// brand.orangeLight
    "--sjs-primary-backcolor-dark": "#b32d13", // brand.orangeDark
    "--sjs-primary-forecolor": "#ffffff",

    /* Borders / inputs */
    "--sjs-border-default": "#283361",         // brand.navyLight
    "--sjs-border-light": "#EBEBEB",           // bg.hover

    /* Focus ring-ish color */
    "--sjs-secondary-backcolor": "#000c2e",    // brand.navy
    "--sjs-secondary-forecolor": "#ffffff",

    /* Error / danger (approx using orangeDark so it stays on-brand) */
    "--sjs-special-red": "#b32d13",            // brand.orangeDark

    /* Shadows (subtle modern look) */
    "--sjs-shadow-small": "0px 2px 10px rgba(0, 12, 46, 0.08)",
    "--sjs-shadow-medium": "0px 8px 20px rgba(0, 12, 46, 0.12)",
  },
};