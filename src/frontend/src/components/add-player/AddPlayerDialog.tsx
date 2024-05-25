import {
    Button,
    Dialog,
    DialogActions,
    DialogContent,
    DialogTitle,
    Stack,
    TextField,
} from '@mui/material';
import { useContext, useState } from 'react';
import { useAddPlayer } from 'src/service/backend-service';
import { ClientContext } from '../client-context/ClientContextProvider';

export interface AddPlayerDialogProps {
    open: boolean;
    onClose: () => void;
}

export const AddPlayerDialog = (props: AddPlayerDialogProps) => {
    const [nickname, setNickname] = useState('');
    const [fullName, setFullName] = useState('');

    const playerAdder = useAddPlayer();

    const { clientID } = useContext(ClientContext);

    const handleClose = () => {
        setNickname("");
        setFullName("");

        props.onClose();
    }

    if (!clientID) return 'Something went wrong';
    return (
        <Dialog open={props.open} onClose={props.onClose}>
            <DialogTitle>Spieler hinzufügen</DialogTitle>
            <DialogContent>
                <Stack gap={1} paddingTop={'1rem'}>
                    <TextField
                        value={nickname}
                        onChange={(e) => setNickname(e.target.value)}
                        label="Nickname"
                    />
                    <TextField
                        value={fullName}
                        onChange={(e) => setFullName(e.target.value)}
                        label="Vollständiger Name"
                    />
                </Stack>
            </DialogContent>
            <DialogActions>
                <Button
                    disabled={playerAdder.isPending}
                    onClick={() =>
                        playerAdder
                            .mutateAsync({
                                clientToken: clientID,
                                fullName: fullName,
                                nickname: nickname,
                            })
                            .then(handleClose)
                    }
                    variant="contained"
                >
                    Hinzufügen
                </Button>
                <Button variant="outlined" onClick={handleClose}>
                    Abbrechen
                </Button>
            </DialogActions>
        </Dialog>
    );
};
