import { Alert, AlertColor, AlertPropsColorOverrides } from '@mui/material';
import { OverridableStringUnion } from '@mui/types';
import Snackbar from '@mui/material/Snackbar';

export interface ContextAlertProps {
    message: string;
    severity: OverridableStringUnion<AlertColor, AlertPropsColorOverrides>;
    open: boolean;
    onClose: () => void;
}

export const ContextAlert = (props: ContextAlertProps) => {
    return (
        <Snackbar open={props.open} autoHideDuration={10000} onClose={props.onClose}>
            <Alert severity={props.severity} variant='filled'>{props.message}</Alert>
        </Snackbar>
    );
};
