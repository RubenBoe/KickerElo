import {
    Button,
    CircularProgress,
    Dialog,
    DialogActions,
    DialogContent,
    DialogTitle,
    Stack,
    Typography,
} from '@mui/material';
import { useContext, useEffect, useState } from 'react';
import { TeamResultCommand } from 'src/models/TeamResultCommand';
import {
    useCalculateExpectedOutcome,
    useEnterGame,
} from 'src/service/backend-service';
import { EnterTeamControl } from './EnterTeamControl';
import { ClientContext } from '../client-context/ClientContextProvider';
import { Add } from '@mui/icons-material';
import { AddPlayerDialog } from '../add-player/AddPlayerDialog';
import { ContextComponentContext } from '../context-components/ContextComponentsProvider';

export interface EnterGameDialogProps {
    open: boolean;
    onClose: () => void;
}

export const EnterGameDialog = (props: EnterGameDialogProps) => {
    const { clientID, players } = useContext(ClientContext);
    const { showAlert } = useContext(ContextComponentContext);
    const gameEnterer = useEnterGame();
    const { mutateAsync: expectedOutcomeCalculatorExecutor, isPending } =
        useCalculateExpectedOutcome(clientID!);

    const [team1, setTeam1] = useState<TeamResultCommand>({
        PlayerIDs: [],
        Points: 0,
        TeamNumber: 1,
    });
    const [team2, setTeam2] = useState<TeamResultCommand>({
        PlayerIDs: [],
        Points: 0,
        TeamNumber: 2,
    });
    const [team1Expected, setTeam1Expected] = useState<number>();
    const [team2Expected, setTeam2Expected] = useState<number>();

    const [newPlayerDialogOpen, setNewPlayerDialogOpen] = useState(false);

    const handleClose = () => {
        // Reset data
        setTeam1({
            PlayerIDs: [],
            Points: 0,
            TeamNumber: 1,
        });
        setTeam2({
            PlayerIDs: [],
            Points: 0,
            TeamNumber: 2,
        });

        props.onClose();
    };

    useEffect(() => {
        if (
            team1.PlayerIDs.length === team2.PlayerIDs.length &&
            team1.PlayerIDs.length > 0
        ) {
            expectedOutcomeCalculatorExecutor([
                { TeamNumber: 1, PlayerIDs: team1.PlayerIDs },
                { TeamNumber: 2, PlayerIDs: team2.PlayerIDs },
            ]).then((res) => {
                res.teams.forEach((t) => {
                    if (t.teamNumber === 1) setTeam1Expected(t.points);
                    if (t.teamNumber === 2) setTeam2Expected(t.points);
                });
            });
        }
    }, [expectedOutcomeCalculatorExecutor, team1.PlayerIDs, team2.PlayerIDs]);

    const teamsValid =
        team1.PlayerIDs.length === team2.PlayerIDs.length && //Teams of different lengths aren't balanced yet
        team1.PlayerIDs.length > 0 &&
        team2.PlayerIDs.length > 0; // Have at least one player per team

    const isValid = teamsValid && team1.Points !== team2.Points; // Ties aren't supported yet

    return (
        <Dialog open={props.open} onClose={handleClose} fullWidth>
            <DialogTitle>
                <Stack
                    direction={'row'}
                    justifyContent={'space-between'}
                    alignItems={'center'}
                >
                    Spiel erfassen
                    <Button onClick={() => setNewPlayerDialogOpen(true)}>
                        <Add />
                        Neuer Spieler
                    </Button>
                </Stack>
            </DialogTitle>
            <DialogContent>
                <Stack gap={2}>
                    <Stack
                        direction={'row'}
                        justifyContent={'space-between'}
                        alignItems={'center'}
                    >
                        <EnterTeamControl
                            team={team1}
                            onChange={(team) => setTeam1(team)}
                            opponentTeamPlayerIDs={team2.PlayerIDs}
                            isWinning={team1.Points > team2.Points}
                        />
                        :
                        <EnterTeamControl
                            team={team2}
                            onChange={(team) => setTeam2(team)}
                            opponentTeamPlayerIDs={team1.PlayerIDs}
                            isWinning={team2.Points > team1.Points}
                        />
                    </Stack>
                    {teamsValid && (
                        <Stack gap={1} alignItems={'center'}>
                            <Typography color={theme => theme.palette.grey[500]}>Erwartetes Ergebnis</Typography>
                            {isPending ? (
                                <CircularProgress />
                            ) : (
                                <Typography color={theme => theme.palette.grey[500]}>
                                    <Stack
                                        direction={'row'}
                                        justifyContent={'space-between'}
                                        alignItems={'center'}
                                    >
                                        {team1Expected}:{team2Expected}
                                    </Stack>
                                </Typography>
                            )}
                        </Stack>
                    )}
                </Stack>
                <AddPlayerDialog
                    open={newPlayerDialogOpen}
                    onClose={() => setNewPlayerDialogOpen(false)}
                />
            </DialogContent>
            <DialogActions>
                {gameEnterer.isPending ? (
                    <CircularProgress />
                ) : (
                    <Button
                        variant="contained"
                        disabled={!isValid}
                        onClick={() => {
                            if (clientID) {
                                gameEnterer
                                    .mutateAsync({
                                        ClientToken: clientID,
                                        Teams: [team1, team2],
                                    })
                                    .then((res) => {
                                        const winnerTeam = res.teamResults.sort(
                                            (a, b) => b.points - a.points
                                        )[0];
                                        const teamNamesInMessage =
                                            winnerTeam.playerResults
                                                .slice(
                                                    0,
                                                    winnerTeam.playerResults
                                                        .length - 1
                                                )
                                                .map(
                                                    (player) =>
                                                        players.find(
                                                            (p) =>
                                                                p.playerID ===
                                                                player.playerID
                                                        )?.nickname ?? ''
                                                )
                                                .join(', ') +
                                                (winnerTeam.playerResults
                                                    .length > 1
                                                    ? ' und '
                                                    : '') +
                                                players.find(
                                                    (p) =>
                                                        winnerTeam
                                                            .playerResults[
                                                            winnerTeam
                                                                .playerResults
                                                                .length - 1
                                                        ].playerID ===
                                                        p.playerID
                                                )?.nickname ?? '';
                                        const message = `${teamNamesInMessage} ${
                                            winnerTeam.playerResults.length ===
                                            1
                                                ? 'hat'
                                                : 'haben'
                                        } ${
                                            winnerTeam.playerResults[0].eloGain
                                        } Punkte gewonnen!`;
                                        showAlert(message, 'info');
                                        handleClose();
                                    });
                            }
                        }}
                    >
                        Speichern
                    </Button>
                )}
                <Button variant="outlined" onClick={handleClose}>
                    Abbrechen
                </Button>
            </DialogActions>
        </Dialog>
    );
};
