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
                }
            ]
        }
    },
    palette: {
      primary: indigo,
      secondary: cyan,
    },
})