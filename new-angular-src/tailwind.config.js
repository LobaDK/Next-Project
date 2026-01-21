/** @type {import('tailwindcss').Config} */
module.exports = {
  content: ["./src/**/*.{html,ts}"],
  theme: {
    extend: {
      colors: {
        brand: {
          orange: "#e94f2d",
          orangeLight: "#FF6B3D",
          orangeDark: "#b32d13",
          orangeDarker: "#260700",
          navy: "#000c2e",
          navyLight: "#283361",
          navyDark: "#00061a",
        },
        bg: {
          white: "#ffffff",
          gray: "#F5F5F5",
          hover: "#EBEBEB",
        },
      },
    },
  },
  plugins: [],
};
