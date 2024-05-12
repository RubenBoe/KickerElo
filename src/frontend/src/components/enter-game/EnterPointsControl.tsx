import { Add, Remove } from '@mui/icons-material';
import { IconButton, Stack, Typography } from '@mui/material';

export interface EnterPointsControlProps {
    points: number;
    onChange: (newPoints: number) => void;
    isWinning: boolean;
}

export const EnterPointsControl = (props: EnterPointsControlProps) => {
    return (
        <Stack
            direction={'row'}
            alignItems={'center'}
            justifyContent={'space-around'}
        >
            <IconButton
                onClick={() => {
                    props.onChange(props.points === 0 ? 10 : props.points - 1);
                }}
            >
                <Remove />
            </IconButton>
            <Typography variant="h3" color={props.isWinning ? 'green' : 'red'}>
                {props.points}
            </Typography>
            <IconButton
                onClick={() => {
                    props.onChange(props.points === 10 ? 0 : props.points + 1);
                }}
            >
                <Add />
            </IconButton>
        </Stack>
    );
};
