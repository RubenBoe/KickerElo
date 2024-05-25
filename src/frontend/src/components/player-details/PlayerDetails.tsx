import {
    CircularProgress,
    Divider,
    Stack,
    Tab,
    Tabs,
    Typography,
} from '@mui/material';
import { Navigate, useOutletContext, useParams } from 'react-router-dom';
import { PlayerResult } from 'src/models/PlayerResult';
import { usePlayerDetails } from 'src/service/backend-service';
import { PlayerChip } from '../ui-elements/PlayerChip';
import { GameChip } from '../ui-elements/GameChip';
import { useContext, useState } from 'react';
import { ClientContext } from '../client-context/ClientContextProvider';
import { PlayerEloChart } from './PlayerEloChart';

export const PlayerDetailsContainer = () => {
    const { players } = useContext(ClientContext);
    const params = useParams();
    const playerID = params['PlayerID'];

    const rank = players.findIndex((p) => p.playerID === playerID) + 1;

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
    const currentSeason = client.client!.seasons.find(
        (s) => s.endDate === null
    )!;

    const [tab, setTab] = useState('lastGames');

    if (playerDetails.isError) return 'Something went wrong';
    return (
        <Stack gap={1} sx={{ maxHeight: '50vh' }} overflow={'hidden'}>
            <Typography variant="h2">Details:</Typography>
            {playerDetails.isLoading ? (
                <CircularProgress />
            ) : (
                <Stack gap={1} overflow={'auto'} position={'relative'}>
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
                    <Tabs
                        value={tab}
                        onChange={(e, newVal) => setTab(newVal.toString())}
                        style={{
                            position: 'sticky',
                            top: -8,
                            backgroundColor: 'white',
                            zIndex: 1
                        }}
                    >
                        <Tab value={'lastGames'} label={'Letzte Spiele'} />
                        <Tab value={'chart'} label="Verlauf" />
                    </Tabs>
                    {tab === 'lastGames' && (
                        <Stack gap={1}>
                            {playerDetails
                                .data!.gamesPlayed.filter(
                                    (g) => g.seasonID === currentSeason.seasonId
                                )
                                .map((game) => (
                                    <GameChip
                                        key={game.gameID}
                                        game={game}
                                        players={players}
                                        currentPlayer={playerDetails.data!}
                                    />
                                ))}
                        </Stack>
                    )}
                    {tab === "chart" && playerDetails.data && (
                        <PlayerEloChart 
                            playerResults={playerDetails.data}
                        />
                    )}
                </Stack>
            )}
        </Stack>
    );
};
