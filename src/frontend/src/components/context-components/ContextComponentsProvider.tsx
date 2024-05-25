import React, { useCallback, useState } from 'react';
import { PropsWithChildren } from 'react';
import { ContextAlert } from './ContextAlert';
import { AlertColor, AlertPropsColorOverrides } from '@mui/material';
import { OverridableStringUnion } from '@mui/types';

export interface ContextComponentContextType {
    showAlert: (
        message: string,
        severity: OverridableStringUnion<AlertColor, AlertPropsColorOverrides>
    ) => void;
}

export const ContextComponentContext =
    React.createContext<ContextComponentContextType>({
        showAlert: () => undefined,
    });

export const ContextComponentProvider = (props: PropsWithChildren) => {

    const [alertOpen, setAlertOpen] = useState(false);
    const [alertMessage, setAlertMessage] = useState("");
    const [alertSeverity, setAlertSeverity] = useState<OverridableStringUnion<AlertColor, AlertPropsColorOverrides>>("info")

    const showAlert = useCallback(
        (
            message: string,
            severity: OverridableStringUnion<AlertColor, AlertPropsColorOverrides>
        ) => {
            setAlertMessage(message);
            setAlertSeverity(severity);
            setAlertOpen(true);
        },
        []
    )

    return (
        <ContextComponentContext.Provider value={{showAlert}}>
            {props.children}
            <ContextAlert
                open={alertOpen}
                onClose={() => setAlertOpen(false)}
                message={alertMessage}
                severity={alertSeverity}
            />
        </ContextComponentContext.Provider>
    );
};
