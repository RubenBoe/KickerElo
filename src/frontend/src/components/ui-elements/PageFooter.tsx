import { Divider, Link, Stack, Typography } from "@mui/material"

export const PageFooter = () => {
    
    return(
        <Stack marginTop={"0.5rem"}>
            <Divider />
            <Typography variant="subtitle2">Danke für's Spielen!</Typography>
            <Typography variant="subtitle2">Ich freue mich über Feedback am <Link href="https://github.com/RubenBoe/KickerElo">Github-Projekt</Link>!</Typography>
        </Stack>
    )
}