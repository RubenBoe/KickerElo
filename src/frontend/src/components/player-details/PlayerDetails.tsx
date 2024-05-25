import { CircularProgress, Divider, Stack, Typography } from '@mui/material';
import { Navigate, useOutletContext, useParams } from 'react-router-dom';
import { PlayerResult } from 'src/models/PlayerResult';
import { usePlayerDetails } from 'src/service/backend-service';
import { PlayerChip } from '../ui-elements/PlayerChip';
import { GameChip } from '../ui-elements/GameChip';
import { useContext } from 'react';
import { ClientContext } from '../client-context/ClientContextProvider';

export const PlayerDetailsContainer = () => {
    const {players} = useContext(ClientContext);
    const params = useParams();
    const playerID = params['PlayerID'];

    const rank = players.findIndex(p => p.playerID === playerID) + 1;

    if (!playerID) {
        return <Navigate to="/Ranking" />;
    }
    return <PlayerDetails rank={rank} playerID={playerID} />;
};

interface PlayerDetailsProps {
    playerID: string;
    rank: number;
}

const PlayerDetails = (props: PlayerDetailsProps) => {
    const playerDetails = usePlayerDetails(props.playerID);
    const [players]: [PlayerResult[]] = useOutletContext();

    const client = useContext(ClientContext);
    const currentSeason = client.client!.seasons.find(s => s.endDate === null)!;

    if (playerDetails.isError) return 'Something went wrong';
    return (
        <Stack gap={1} sx={{maxHeight: "50vh"}} overflow={"hidden"}>
            <Typography variant="h2">Details:</Typography>
            {playerDetails.isLoading ? (
                <CircularProgress />
            ) : (
                <Stack gap={1} overflow={"auto"}>
                    <PlayerChip
                        player={{
                            eloNumber: playerDetails.data!.eloNumber,
                            lastUpdated: playerDetails.data!.lastUpdated,
                            nickname: `${playerDetails.data!.nickname} (${
                                playerDetails.data!.fullName
                            })`,
                            playerID: playerDetails.data!.playerID,
                        }}
                        ranking={props.rank}
                    />
                    <Divider />
                    <Stack>
                        <Typography variant="h3">Letzte Spiele:</Typography>
                        {playerDetails.data!.gamesPlayed.filter(g => g.seasonID === currentSeason.seasonId).map((game) => (
                            <Stack key={game.gameID} >
                                <GameChip game={game} players={players} currentPlayer={playerDetails.data!} />
                            </Stack>
                        ))}
                    </Stack>
                </Stack>
            )}
        </Stack>
    );
};
