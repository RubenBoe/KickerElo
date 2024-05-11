import { CircularProgress, Drawer, Stack, Typography } from '@mui/material';
import { usePlayers } from 'src/service/backend-service';
import { PlayerChip } from '../ui-elements/PlayerChip';
import { Outlet, useNavigate, useParams } from 'react-router-dom';

export interface RankingProps {
    clientID: string;
}

export const Ranking = (props: RankingProps) => {
    const players = usePlayers(props.clientID);

    const nav = useNavigate();

    const param = useParams();
    const playerID = param['PlayerID'];

    if (players.isError) return 'Something went wrong';
    return (
        <Stack gap={1}>
            <Typography variant="h1">Rangliste</Typography>
            {players.isLoading ? (
                <CircularProgress />
            ) : (
                <Stack>
                    {players.data!.map((player) => (
                        <PlayerChip player={player} key={player.playerID} />
                    ))}
                    <Drawer
                        open={playerID !== undefined}
                        anchor="bottom"
                        onClose={() => nav('/Ranking')}
                        PaperProps={{
                            sx: {
                                padding: "0.5rem",
                                overflow: "hidden"
                            }
                        }}
                    >
                        <Outlet context={[players.data]} />
                    </Drawer>
                </Stack>
            )}
        </Stack>
    );
};
