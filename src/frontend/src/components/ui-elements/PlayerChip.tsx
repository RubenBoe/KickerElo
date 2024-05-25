import { Card, CardContent, Typography, useTheme } from '@mui/material';
import { useMemo } from 'react';
import { Link } from 'react-router-dom';
import { PlayerResult } from 'src/models/PlayerResult';
import EmojiEventsIcon from '@mui/icons-material/EmojiEvents';

export interface PlayerChipProps {
    player: PlayerResult;
    ranking: number;
}

export const PlayerChip = (props: PlayerChipProps) => {
    const theme = useTheme();

    const rank = useMemo(() => {
        if (props.ranking > 3) {
            return <Typography variant='h3' color={theme.palette.primary.main}>{props.ranking}.</Typography>
        }
        return <EmojiEventsIcon
            color={props.ranking === 1 ? 'gold': props.ranking === 2 ? "silver" : "bronze"}
        />
    }, [props.ranking, theme.palette.primary.main]);

    return (
        <Card style={{ flexShrink: 0 }}>
            <CardContent>
                <Link
                    to={`/Ranking/${props.player.playerID}`}
                    style={{ textDecoration: 'none', display: "flex", alignItems: "center", gap: "8px" }}
                >
                    {rank}
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
