import {
    CircularProgress,
    Stack,
} from '@mui/material';
import { useContext } from 'react';
import { ClientContext } from '../client-context/ClientContextProvider';
import { ClientSelector } from './ClientSelector';
import { ClientContentContainer } from './ClientContentContainer';

export const ClientSelection = () => {
    const { isLoading, setClientID, clientID: client, clientID } =
        useContext(ClientContext);

    return (
        <Stack overflow={"hidden"} height={"100%"}>
            {isLoading ? (
                <CircularProgress />
            ) : clientID === undefined || client === undefined ? (
                // Select a client
                <ClientSelector setClientID={setClientID} />
            ) : (
                <ClientContentContainer setClientID={setClientID} />
            )}
        </Stack>
    );
};
