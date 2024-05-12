import { AppBar, Button, Stack, Toolbar } from '@mui/material';
import { NavLink, Outlet } from 'react-router-dom';

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

export interface ClientContentContainerProps {
    setClientID: (clientID?: string) => void;
}

export const ClientContentContainer = (props: ClientContentContainerProps) => {
    return (
        <Stack gap={1} height={'100%'} overflow={'hidden'}>
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
                                    key={link.to}
                                >
                                    {({ isActive }) => (
                                        <Button
                                            color="inherit"
                                            variant={
                                                isActive ? 'outlined' : 'text'
                                            }
                                        >
                                            {link.label}
                                        </Button>
                                    )}
                                </NavLink>
                            ))}
                        </Stack>
                        <Button
                            variant="text"
                            color="inherit"
                            onClick={() => props.setClientID()}
                        >
                            Client ändern
                        </Button>
                    </Stack>
                </Toolbar>
            </AppBar>
            <Stack padding={"0.5rem"} overflow={"hidden"}>
                <Outlet />
            </Stack>
        </Stack>
    );
};
