import { Card, CardContent, Typography, useTheme } from '@mui/material';
import { Link } from 'react-router-dom';
import { PlayerResult } from 'src/models/PlayerResult';

export interface PlayerChipProps {
    player: PlayerResult;
}

export const PlayerChip = (props: PlayerChipProps) => {

    const theme = useTheme();

    return (
        <Card>
            <CardContent>
                <Link to={`/Ranking/${props.player.playerID}`} style={{textDecoration: "none"}}>
                    <Typography variant="h2" color={theme.palette.primary.main}>
                        {props.player.nickname}
                    </Typography>
                </Link>
                <Typography variant="body1">
                    {props.player.eloNumber} Punkte
                </Typography>
                <Typography variant="body1">
                    Letztes Spiel am{' '}
                    {props.player.lastUpdated.toLocaleDateString()}
                </Typography>
            </CardContent>
        </Card>
    );
};
