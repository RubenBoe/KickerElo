import {
    Dispatch,
    PropsWithChildren,
    createContext,
    useCallback,
    useEffect,
    useMemo,
    useState,
} from 'react';
import { ClientDetails } from 'src/models/ClientDetails';
import { PlayerResult } from 'src/models/PlayerResult';
import {
    useClientDetails,
    usePlayers,
} from 'src/service/backend-service';

export interface ClientContextType {
    clientID?: string;
    client?: ClientDetails;
    setClientID: (clientID: string | undefined) => void;
    isLoading: boolean;
    players: PlayerResult[];
}

export const ClientContext = createContext<ClientContextType>({
    setClientID: () => undefined,
    isLoading: true,
    players: []
});

export interface ClientContextProviderProps {}

const clientIdStorageKey = 'clientIdStorageKey';

export const ClientContextProvider = (
    props: PropsWithChildren<ClientContextProviderProps>
) => {
    const [clientID, setClientID] = useState<string>();

    useEffect(() => {
        const clientIdFromStorage =
            window.localStorage.getItem(clientIdStorageKey);
        if (clientIdFromStorage) {
            setClientID(clientIdFromStorage);
        } else {
            setClientID(undefined);
        }
    }, []);

    const handleSetClientID: (clientID: string | undefined) => void =
        useCallback((clientID: string | undefined) => {
            if (clientID !== undefined) {
                window.localStorage.setItem(clientIdStorageKey, clientID);
                setClientID(clientID);
            } else {
                window.localStorage.removeItem(clientIdStorageKey);
                setClientID(clientID);
            }
        }, []);

    return clientID ? (
        <InnerClientContextProvider
            clientID={clientID}
            handleSetClientID={handleSetClientID} 
        >
            {props.children}
        </InnerClientContextProvider>
    ) : (
        <ClientContext.Provider value={{isLoading: false, players: [], setClientID: handleSetClientID, client: undefined, clientID: clientID}}>
            {props.children}
        </ClientContext.Provider>
    )
};

interface InnerClientContextProviderProps {
    clientID: string;
    handleSetClientID: (clientID: string | undefined) => void;
}

const InnerClientContextProvider = ({
    clientID,
    handleSetClientID,
    children
}: PropsWithChildren<InnerClientContextProviderProps>) => {
    const { data: client, isLoading, isError } = useClientDetails(clientID);
    const {data: players, isLoading: playersLoading} = usePlayers(clientID);

    const contextValue: ClientContextType = useMemo(() => {
        return {
            client,
            clientID,
            setClientID: handleSetClientID,
            isLoading: isLoading || playersLoading,
            players: players ?? []
        };
    }, [client, clientID, handleSetClientID, isLoading, players, playersLoading]);

    if (isError) handleSetClientID(undefined);
    return (
        <ClientContext.Provider value={contextValue}>
            {children}
        </ClientContext.Provider>
    );
}