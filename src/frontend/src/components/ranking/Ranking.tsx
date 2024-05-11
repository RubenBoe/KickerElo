import { Stack, Typography } from "@mui/material"
import { ClientDetails } from "src/models/ClientDetails"
import { usePlayers } from "src/service/backend-service"

export interface RankingProps {
    clientID: string;
}

export const Ranking = (props: RankingProps) => {

    const {data: players} = usePlayers(props.clientID)

    return (
        <Stack gap={1}>
            <Typography variant="h1">Rangliste</Typography>
            <Stack>

            </Stack>
        </Stack>
    )
}