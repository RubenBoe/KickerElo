import {
    PropsWithChildren,
    createContext,
    useCallback,
    useEffect,
    useMemo,
    useState,
} from 'react';
import { ClientDetails } from 'src/models/ClientDetails';
import { getClientDetails } from 'src/service/backend-service';

export interface ClientContextType {
    clientID?: string;
    client?: ClientDetails;
    setClientID: (clientID: string | undefined) => void;
    isLoading: boolean;
}

export const ClientContext = createContext<ClientContextType>({
    setClientID: () => undefined,
    isLoading: true,
});

export interface ClientContextProviderProps {}

const clientIdStorageKey = 'clientIdStorageKey';

export const ClientContextProvider = (
    props: PropsWithChildren<ClientContextProviderProps>
) => {
    const [clientID, setClientID] = useState<string>();
    const [client, setClient] = useState<ClientDetails>();
    const [isLoading, setIsLoading] = useState(true);

    const handleSetClientID: (clientID: string | undefined) => void =
        useCallback((clientID: string | undefined) => {
            setIsLoading(true);
            if (clientID !== undefined) {
                window.localStorage.setItem(clientIdStorageKey, clientID);
                getClientDetails(clientID)
                    .then((res) => {
                        setClient(res);
                        setClientID(clientID);
                        setIsLoading(false);
                    })
                    .catch(() => {
                        setClient(undefined);
                        setClientID(clientID);
                        setIsLoading(false);
                    });
            } else {
                window.localStorage.removeItem(clientIdStorageKey);
                setClient(undefined);
                setClientID(clientID);
                setIsLoading(false);
            }
        }, []);

    useEffect(() => {
        const clientIdFromStorage =
            window.localStorage.getItem(clientIdStorageKey);
        if (clientIdFromStorage) {
            getClientDetails(clientIdFromStorage)
            .then((res) => {
                setClient(res);
                setClientID(clientIdFromStorage);
                setIsLoading(false);
            })
            .catch(
                () => {
                    setClient(undefined);
                    setClientID(undefined);
                    setIsLoading(false);
                }
            )
        } else {
            setClient(undefined);
            setClientID(undefined);
            setIsLoading(false);
        }
    }, []);

    const contextValue: ClientContextType = useMemo(() => {
        return {
            client,
            clientID,
            setClientID: handleSetClientID,
            isLoading,
        };
    }, [client, clientID, handleSetClientID, isLoading]);

    return (
        <ClientContext.Provider value={contextValue}>
            {props.children}
        </ClientContext.Provider>
    );
};
