import {
    CircularProgress,
    Drawer,
    IconButton,
    Stack,
    Typography,
} from '@mui/material';
import { usePlayers } from 'src/service/backend-service';
import { PlayerChip } from '../ui-elements/PlayerChip';
import { Outlet, useNavigate, useParams } from 'react-router-dom';
import AddIcon from '@mui/icons-material/Add';
import { useState } from 'react';
import { AddPlayerDialog } from '../add-player/AddPlayerDialog';

export interface RankingProps {
    clientID: string;
}

export const Ranking = (props: RankingProps) => {
    const players = usePlayers(props.clientID);

    const [addDialogOpen, setAddDialogOpen] = useState(false);

    const nav = useNavigate();

    const param = useParams();
    const playerID = param['PlayerID'];

    if (players.isError) return 'Something went wrong';
    return (
        <Stack gap={1} overflow={'hidden'}>
            <Stack alignItems={'center'} gap={1} direction={'row'}>
                <Typography variant="h1">Rangliste</Typography>
                <IconButton onClick={() => setAddDialogOpen(true)}>
                    <AddIcon />
                </IconButton>
            </Stack>
            {players.isLoading ? (
                <CircularProgress />
            ) : (
                <Stack overflow={'auto'}>
                    {players.data!.map((player) => (
                        <PlayerChip player={player} key={player.playerID} />
                    ))}
                    <Drawer
                        open={playerID !== undefined}
                        anchor="bottom"
                        onClose={() => nav('/Ranking')}
                        PaperProps={{
                            sx: {
                                padding: '0.5rem',
                                overflow: 'hidden',
                            },
                        }}
                    >
                        <Outlet context={[players.data]} />
                    </Drawer>
                </Stack>
            )}
            <AddPlayerDialog
                open={addDialogOpen}
                onClose={() => setAddDialogOpen(false)}
            />
        </Stack>
    );
};
