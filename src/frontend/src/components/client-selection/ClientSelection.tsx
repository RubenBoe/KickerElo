import {
    AppBar,
    Button,
    CircularProgress,
    Stack,
    TextField,
    Toolbar,
    Typography,
} from '@mui/material';
import { useContext, useState } from 'react';
import { NavLink, Outlet } from 'react-router-dom';
import { ClientContext } from '../client-context/ClientContextProvider';

const links = [
    {
        to: '/',
        label: 'Start',
    },
    {
        to: '/Ranking',
        label: 'Rangliste',
    },
    {
        to: '/Games',
        label: 'Spiele',
    },
];

export const ClientSelection = () => {
    const { isLoading, setClientID, client, clientID } =
        useContext(ClientContext);

    return (
        <Stack>
            {isLoading ? (
                <CircularProgress />
            ) : clientID === undefined || client === undefined ? (
                // Select a client
                <ClientSelector setClientID={setClientID} />
            ) : (
                <Stack>
                    <AppBar position="static">
                        <Toolbar>
                            <Stack
                                direction={'row'}
                                justifyContent={'space-between'}
                                alignItems={'center'}
                                width={'100%'}
                            >
                                <Stack direction={'row'} alignItems={'center'}>
                                    {links.map((link) => (
                                        <NavLink
                                            to={link.to}
                                            style={({ isActive }) => ({
                                                textDecoration: 'none',
                                                color: 'inherit',
                                            })}
                                        >
                                            {({ isActive }) => (
                                                <Button color="inherit" variant={isActive ? 'outlined' : "text"}>
                                                    {link.label}
                                                </Button>
                                            )}
                                        </NavLink>
                                    ))}
                                </Stack>
                                <Button
                                    variant="text"
                                    color="inherit"
                                    onClick={() => setClientID(undefined)}
                                >
                                    Client ändern
                                </Button>
                            </Stack>
                        </Toolbar>
                    </AppBar>
                    <Outlet />
                </Stack>
            )}
        </Stack>
    );
};

interface ClientSelectorProps {
    setClientID: (clientID: string) => void;
}

const ClientSelector = (props: ClientSelectorProps) => {
    const [value, setValue] = useState('');

    return (
        <Stack gap={1}>
            <TextField
                value={value}
                onChange={(e) => setValue(e.target.value)}
                label="Client Token"
            />
            <Button
                variant="contained"
                onClick={() => {
                    if (value) props.setClientID(value);
                }}
            >
                <Typography>Suchen</Typography>
            </Button>
        </Stack>
    );
};
