import { Stack } from '@mui/material';
import { AgChartsReact } from 'ag-charts-react';
import { useContext, useMemo } from 'react';
import { PlayerDetailsResult } from 'src/models/PlayerDetailsResult';
import { ClientContext } from '../client-context/ClientContextProvider';
import { AgChartOptions } from "ag-charts-community";

export interface PlayerEloChartProps {
    playerResults: PlayerDetailsResult;
}

export const PlayerEloChart = ({ playerResults }: PlayerEloChartProps) => {
    const { players, client } = useContext(ClientContext);
    const currentSeason = client!.seasons.find(s => s.endDate === null)!;

    const options: AgChartOptions = useMemo(() => {
        const currentPlayerElo = players.find(
            (p) => p.playerID === playerResults.playerID
        )?.eloNumber;
        if (!currentPlayerElo) throw new Error("Couldn't find player");

        const eloGains = playerResults.gamesPlayed.filter(game => game.seasonID === currentSeason.seasonId).map((game) => {
            const playerResult = game.teamResults
                .find((team) =>
                    team.playerResults.some(
                        (p) => p.playerID === playerResults.playerID
                    )
                )
                ?.playerResults.find(
                    (p) => p.playerID === playerResults.playerID
                );
            if (!playerResult)
                throw new Error(
                    'Something is wrong with the data as the player did not play in one of their games.'
                );
            return { eloGain: playerResult.eloGain, date: game.date };
        });

        const accumulatedElo: { elo: number; date: Date }[] = [];
        for (let i = 0; i < eloGains.length; i++) {
            const step = eloGains[i];
            accumulatedElo.push({
                date: step.date,
                elo:
                    i === 0 ? currentPlayerElo : ( accumulatedElo[i - 1].elo - eloGains[i - 1].eloGain ),
            });
        }
        accumulatedElo.push({
            date: currentSeason.startDate,
            elo: accumulatedElo.length > 0 ? accumulatedElo[accumulatedElo.length - 1].elo - eloGains[eloGains.length - 1].eloGain : currentPlayerElo
        });

        accumulatedElo.reverse();

        const transformedEloList = accumulatedElo.map(
            step => ({
                elo: step.elo,
                date: step.date.toLocaleDateString("de", {hour: "numeric", minute: "2-digit", second: "2-digit"})
            })
        )

        return {
            data: transformedEloList,
            series: [{ type: "line", xKey: 'date', yKey: 'elo' }],
            axes: [
                {
                    type: "number",
                    
                },
                {
                    type: "category",
                    label: {
                        enabled: false,
                    }
                }
            ],
            theme: "ag-material"
        };
    }, [currentSeason, playerResults, players]);

    return (
        <Stack flexShrink={0} flexGrow={1} height={'300px'}>
            <AgChartsReact options={options} />
        </Stack>
    );
};
