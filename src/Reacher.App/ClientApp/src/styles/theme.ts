export const colors = {
  primaryText: "white",
  black: "#000e1a",
  gray: "rgba(235,235,245,0.6)",
  gray18: "rgba(235,235,245,0.18)",
  white: "#fff",
  blue: "#007ce0",
  navy: "#004175",
  api: "#CCFF00",
  apiLight: "rgb(219, 255, 77)",
  primary: "#fdaa26",
  primaryLight: "#de8a02",
  secondary: "#46e0b5",
  // primary: "#64dfdf",
  // primaryLight: "#72efdd",
};

const fontStyles = {
  fontFamily: "body",
  lineHeight: "body",
  fontWeight: "body",
  color: "primaryText",
  fontSize: "body",
};

const inputStyles = {
  ...fontStyles,
  mt: 1,
  fontWeight: "normal",
  height: "56px",
  fontSize: "19px",
  lineHeight: "1.32",
  borderWidth: 1,
  borderRadius: 10,
  width: "100%",
  borderColor: "transparent",
  bg: "#1c1c1e",
  cursor: "text",
  outline: "none",
  position: "relative",

  "::-webkit-search-decoration:hover, ::-webkit-search-cancel-button:hover": {
    cursor: "pointer",
  },
  "::placeholder": {
    color: "gray18",
  },
  "&:disabled": {
    bg: `rgba(235,235,245,0.3)`,
    opacity: 0.6,
  },
  ":not(select)&:read-only": {
    opacity: 0.6,
  },
  "&:focus": {
    borderColor: "white",
  },
};

const buttonFontStyle = {
  letterSpacing: "-0.19px",
  fontSize: 3,
  fontWeight: "bold",
  height: "56px",
  cursor: "pointer",
  borderRadius: "30px",
  px: 4,
  py: 3,
  pt: "14px",
};

const theme = {
  colors,
  fonts: {
    body: "Montserrat, Futura, system-ui, sans-serif",
  },

  buttons: {
    primary: {
      ...buttonFontStyle,
      color: "black",
      bg: "black",
      backgroundColor: "#FCA311",
      fontWeight: "bold",

      ":hover": {
        bg: "#de8a02",
        // boxShadow: "-8px 8px 0px 0px white",
      },
    },
    secondary: {
      ...buttonFontStyle,
      cursor: "pointer",
      color: "black",
      backgroundColor: "white",
    },
    outline: {
      ...buttonFontStyle,
      bg: "transparent",
      border: "1px solid white",
      ":hover": {
        backgroundColor: "white",
        color: "black",
        borderColor: "white",
      },
    },
    link: {
      ...buttonFontStyle,
      bg: "transparent",
      border: "1px solid white",
    },
  },
  forms: {
    input: {
      normal: {
        ...inputStyles,
        fontWeight: "bold",

        "::placeholder": {
          color: "gray18",
        },
      },
    },

    textarea: {
      ...inputStyles,
      height: "100px",
      p: 3,
    },

    label: {
      ...fontStyles,
      width: "auto",
      fontWeight: "bold",
      color: "gray",
    },
    },
    initialColorMode: 'dark',
    useSystemColorMode: false,
};

const tempFontSizes = [13, 16, 19, 20, 40, 60, 64, 80];
var fontSizes:Record<string,any> = {};
fontSizes["xs"] = tempFontSizes[0];
fontSizes["s"] = tempFontSizes[1];
fontSizes["m"] = tempFontSizes[2];
fontSizes["l"] = tempFontSizes[3];
fontSizes["xl"] = tempFontSizes[4];
fontSizes["xxl"] = tempFontSizes[5];
fontSizes["xxxl"] = tempFontSizes[6];
fontSizes["xxxxl"] = tempFontSizes[7];
fontSizes["heading"] = tempFontSizes[3];
fontSizes["body"] = tempFontSizes[3];

export default { ...theme, fontSizes };
