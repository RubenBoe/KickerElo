import { createTheme } from "@mui/material";
import { cyan, indigo } from "@mui/material/colors";

export const theme = createTheme({
    components: {
        MuiButton: {
            styleOverrides: {
                root: {
                    textTransform: "none"
                }
            }
        },
        MuiTypography: {
            variants: [
                {
                    props: {
                        variant: "h1"
                    },
                    style: {
                        fontSize: "3rem"
                    }
                },
                {
                    props: {
                        variant: "h2"
                    },
                    style: {
                        fontSize: "2rem"
                    }
                },
                {
                    props: {
                        variant: "h3"
                    },
                    style: {
                        fontSize: "1.5rem"
                    }
                },
                {
                    props: {
                        variant: "body1"
                    },
                    style: {
                        fontSize: "1rem"
                    }
                },
                {
                    props: {
                        variant: "body2"
                    },
                    style: {
                        fontSize: "1rem",
                        fontWeight: 600
                    }
                },
                {
                    props: {
                        variant: "subtitle2",
                        component: "p",
                    },
                    style: {
                        fontSize: "0.75rem",
                    }
                }
            ]
        },
        MuiSvgIcon: {
            styleOverrides: {

            }
        }
    },
    palette: {
      primary: indigo,
      secondary: cyan,
      gold: {
          main: "#FFD700"
      },
      silver: {
          main: "#C0C0C0"
      },
      bronze: {
          main: "#CD7F32"
      }
    },
})

declare module '@mui/material/styles' {
    interface Palette {
      gold: Palette['primary'];
      silver: Palette['primary'];
      bronze: Palette['primary'];
    }
  
    interface PaletteOptions {
        gold?: PaletteOptions['primary'];
        silver?: PaletteOptions['primary'];
        bronze?: PaletteOptions['primary'];
    }
  }

declare module "@mui/material/SvgIcon" {
    interface SvgIconPropsColorOverrides {
      "gold": true;
      "silver": true;
      "bronze": true;
    }
  }