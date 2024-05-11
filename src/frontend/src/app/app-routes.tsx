import { BrowserRouter, Route, Routes } from "react-router-dom"
import { ClientHome } from "src/components/client-home/ClientHome"
import { ClientSelection } from "src/components/client-selection/ClientSelection"


export const AppRoutes = () => {
    return (
        <BrowserRouter>
            <Routes>
                <Route path="/" element={<ClientSelection />}>
                    <Route index element={<ClientHome />} />
                    <Route path="Ranking">
                        <Route index element={"Ranking"} />
                        <Route path=":PlayerID" element={"Player Details"} />
                    </Route>
                    <Route path="Games" element={"Games"} />
                </Route>
            </Routes>
        </BrowserRouter>
    )
}
