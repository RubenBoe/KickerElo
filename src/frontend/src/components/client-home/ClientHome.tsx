import { Button, Stack, Typography } from '@mui/material';
import { useContext } from 'react';
import { ClientContext } from '../client-context/ClientContextProvider';
import { PlayerChip } from '../ui-elements/PlayerChip';

export const ClientHome = () => {
    const { client } = useContext(ClientContext);

    if (!client) throw new Error('Something went wrong.');
    return (
        <Stack gap={1}>
            <Stack>
                <Typography variant="h1">{client.clientName}</Typography>
                <Typography variant="body1">
                    Erstellt am {client.creationDate.toLocaleDateString()}
                </Typography>
                <Typography variant="body1">
                    Anzahl der Spieler*innen: {client.numberOfPlayers}
                </Typography>
                <Typography variant="body1">
                    Aktuelle Saison:{' '}
                    {
                        client.seasons.find((s) => s.endDate === null)
                            ?.seasonNumber
                    }{' '}
                    (seit{' '}
                    {client.seasons
                        .find((s) => s.endDate === null)
                        ?.startDate.toLocaleDateString()}
                    )
                </Typography>
            </Stack>
            <Typography variant="body1">Erste*r der Rangliste:</Typography>
            <PlayerChip player={client.currentLeader} />
            <Button variant='contained' >Spiel erfassen</Button>
        </Stack>
    );
};
