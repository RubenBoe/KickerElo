import {
    Button,
    CircularProgress,
    Dialog,
    DialogActions,
    DialogContent,
    DialogTitle,
    Stack,
} from '@mui/material';
import { useContext, useState } from 'react';
import { TeamResultCommand } from 'src/models/TeamResultCommand';
import { useEnterGame } from 'src/service/backend-service';
import { EnterTeamControl } from './EnterTeamControl';
import { ClientContext } from '../client-context/ClientContextProvider';

export interface EnterGameDialogProps {
    open: boolean;
    onClose: () => void;
}

export const EnterGameDialog = (props: EnterGameDialogProps) => {
    const { clientID } = useContext(ClientContext);

    const gameEnterer = useEnterGame();

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

    const isValid =
        team1.PlayerIDs.length === team2.PlayerIDs.length && //Teams of different lengths aren't balanced yet
        team1.PlayerIDs.length > 0 &&
        team2.PlayerIDs.length > 0 && // Have at least one player per team
        team1.Points !== team2.Points; // Ties aren't supported yet

    return (
        <Dialog open={props.open} onClose={props.onClose} fullWidth>
            <DialogTitle>Spiel erfassen</DialogTitle>
            <DialogContent>
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
                                gameEnterer.mutateAsync({
                                    ClientToken: clientID,
                                    Teams: [team1, team2],
                                })
                                .then(
                                    props.onClose
                                )
                            }
                        }}
                    >
                        Speichern
                    </Button>
                )}
                <Button variant="outlined" onClick={props.onClose}>
                    Abbrechen
                </Button>
            </DialogActions>
        </Dialog>
    );
};
