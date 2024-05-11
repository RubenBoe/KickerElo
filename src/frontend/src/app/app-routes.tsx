import { useContext } from 'react';
import { BrowserRouter, Navigate, Route, Routes } from 'react-router-dom';
import { ClientContext } from 'src/components/client-context/ClientContextProvider';
import { ClientHome } from 'src/components/client-home/ClientHome';
import { ClientSelection } from 'src/components/client-selection/ClientSelection';
import { PlayerDetailsContainer } from 'src/components/player-details/PlayerDetails';
import { Ranking } from 'src/components/ranking/Ranking';

export const AppRoutes = () => {
    const { clientID } = useContext(ClientContext);

    return (
        <BrowserRouter>
            <Routes>
                <Route path="/" element={<ClientSelection />}>
                    <Route index element={<ClientHome />} />
                    {clientID && (
                        <Route path="Ranking" element={<Ranking clientID={clientID} />}>
                            <Route
                                path=":PlayerID"
                                element={<PlayerDetailsContainer />}
                            />
                        </Route>
                    )}
                    <Route path="Games" element={'Games'} />
                    <Route path="*" element={<Navigate to={'/'} />} />
                </Route>
            </Routes>
        </BrowserRouter>
    );
};
