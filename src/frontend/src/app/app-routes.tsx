import { Button, Stack, Typography } from "@mui/material"
import { useState } from "react"
import { BrowserRouter, Outlet, Route, Routes } from "react-router-dom"


export const AppRoutes = () => {
    return (
        <BrowserRouter>
            <Routes>
                <Route path="/" element={<ClientSelection />}>
                    <Route index element={<ClientHome />} />
                </Route>
            </Routes>
        </BrowserRouter>
    )
}

const ClientSelection = () => {

    const [clientSelected, setClientSelected] = useState(false)

    return (
        clientSelected ? 
        <Outlet /> 
        : <Stack>

            <Typography>ClientsSelection</Typography>
            <Button onClick={() => setClientSelected(true)}>Select Client</Button>
        </Stack>
    )
}

const ClientHome = () => {
    return (
        <Typography>Client Home</Typography>
    )
}