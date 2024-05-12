import { Card, CardContent, Stack, Typography, useTheme } from '@mui/material';
import { GameResult } from 'src/models/GameResult';
import { PlayerDetailsResult } from 'src/models/PlayerDetailsResult';
import { PlayerResult } from 'src/models/PlayerResult';
import { TeamGameResult } from 'src/models/TeamGameResult';

export interface GameChipProps {
    game: GameResult;
    players: PlayerResult[];
    currentPlayer?: PlayerDetailsResult;
}

export const GameChip = ({ game, players, currentPlayer }: GameChipProps) => {
    const team1 = game.teamResults.find((t) => t.teamNumber === 1)!;
    const team2 = game.teamResults.find((t) => t.teamNumber === 2)!;

    return (
        <Card>
            <CardContent>
                <Stack direction="row" justifyContent={'space-between'} alignItems={"center"}>
                    <Team
                        team={team1}
                        players={players}
                        currentPlayer={currentPlayer}
                    />
                    :
                    <Team
                        team={team2}
                        players={players}
                        currentPlayer={currentPlayer}
                    />
                </Stack>
            </CardContent>
        </Card>
    );
};

interface TeamProps {
    team: TeamGameResult;
    players: PlayerResult[];
    currentPlayer?: PlayerDetailsResult;
}

const Team = ({ team, players, currentPlayer }: TeamProps) => {

    const isWinner = team.playerResults[0].eloGain > 0;

    return (
        <Stack alignItems={"center"} flexGrow={1}>
            <Stack>
                {team.playerResults.map((player) => (
                    <Typography
                        key={player.playerID}
                        variant={
                            currentPlayer?.playerID === player.playerID
                                ? 'body2'
                                : 'body1'
                        }
                    >
                        {
                            players.find((p) => p.playerID === player.playerID)!
                                .nickname
                        }
                        &nbsp;
                        ({player.eloGain > 0 && "+"}{player.eloGain})
                    </Typography>
                ))}
            </Stack>
            <Typography variant='h3' color={isWinner ? "green" : "red"}>{team.points}</Typography>
        </Stack>
    );
};
