import {
    Drawer,
    IconButton,
    Stack,
    Typography,
} from '@mui/material';
import { PlayerChip } from '../ui-elements/PlayerChip';
import { Outlet, useNavigate, useParams } from 'react-router-dom';
import AddIcon from '@mui/icons-material/Add';
import { useContext, useState } from 'react';
import { AddPlayerDialog } from '../add-player/AddPlayerDialog';
import { ClientContext } from '../client-context/ClientContextProvider';

export interface RankingProps {
    clientID: string;
}

export const Ranking = (props: RankingProps) => {
    const { players } = useContext(ClientContext);

    const [addDialogOpen, setAddDialogOpen] = useState(false);

    const nav = useNavigate();

    const param = useParams();
    const playerID = param['PlayerID'];

    return (
        <Stack gap={1} overflow={'hidden'}>
            <Stack alignItems={'center'} gap={1} direction={'row'}>
                <Typography variant="h1">Rangliste</Typography>
                <IconButton onClick={() => setAddDialogOpen(true)}>
                    <AddIcon />
                </IconButton>
            </Stack>
            {players && (
                <Stack overflow={'auto'}>
                    {players.map((player) => (
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
                        <Outlet context={[players]} />
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
