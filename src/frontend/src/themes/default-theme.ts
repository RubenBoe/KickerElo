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
        }
    },
    palette: {
      primary: indigo,
      secondary: cyan,
    },
})