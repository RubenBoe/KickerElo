import { Button, Stack, TextField, Typography } from "@mui/material";
import { useState } from "react";


export interface ClientSelectorProps {
    setClientID: (clientID: string) => void;
}

export const ClientSelector = (props: ClientSelectorProps) => {
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