import { Button, Dialog, DialogActions, DialogContent, DialogTitle, Stack } from "@mui/material"
import { useEnterGame } from "src/service/backend-service";

export interface EnterGameDialogProps {
    open: boolean;
    onClose: () => void;
}

export const EnterGameDialog = (props: EnterGameDialogProps) => {

    const gameEnterer = useEnterGame();

    return (
        <Dialog open={props.open} onClose={props.onClose}>
            <DialogTitle>Spiel erfassen</DialogTitle>
            <DialogContent>
                <Stack>
                    
                </Stack>
            </DialogContent>
            <DialogActions>
                <Button variant="contained">Speichern</Button>
                <Button variant="outlined">Abbrechen</Button>
            </DialogActions>
        </Dialog>
    )
}