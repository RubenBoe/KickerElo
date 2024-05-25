import { CircularProgress, IconButton, Stack, Typography } from '@mui/material';
import { useContext, useState } from 'react';
import { useGetGames } from 'src/service/backend-service';
import { ClientContext } from '../client-context/ClientContextProvider';
import { ClientDetails } from 'src/models/ClientDetails';
import { GameChip } from '../ui-elements/GameChip';
import { EnterGameDialog } from '../enter-game/EnterGameDialog';
import AddIcon from '@mui/icons-material/Add';

export interface GamesProps {
    client: ClientDetails;
}

export const Games = ({ client }: GamesProps) => {
    const { players } = useContext(ClientContext);
    const currentSeason = client.seasons.find(s => s.endDate === null)!;
    const { data: games } = useGetGames(currentSeason.seasonId);

    const [enterGameDialogOpen, setEnterGameDialogOpen] = useState(false);

    return (
        <Stack gap={1} overflow={'hidden'}>
            <Stack alignItems={'center'} gap={1} direction={'row'}>
                <Typography variant="h1">Spiele</Typography>
                <IconButton onClick={() => setEnterGameDialogOpen(true)}>
                    <AddIcon />
                </IconButton>
            </Stack>
            {games ? (
                <Stack overflow={'auto'} gap={1}>
                    {games.map((game) => (
                        <GameChip key={game.gameID} game={game} players={players} />
                    ))}
                </Stack>
            ) : (
                <CircularProgress />
            )}
            <EnterGameDialog
                open={enterGameDialogOpen}
                onClose={() => setEnterGameDialogOpen(false)}
            />
        </Stack>
    );
};
