import {
    FormControl,
    InputLabel,
    MenuItem,
    Select,
    Stack,
} from '@mui/material';
import { useContext, useMemo } from 'react';
import { TeamResultCommand } from 'src/models/TeamResultCommand';
import { ClientContext } from '../client-context/ClientContextProvider';
import { EnterPointsControl } from './EnterPointsControl';

export interface EnterTeamControlProps {
    team: TeamResultCommand;
    onChange: (newTeamResult: TeamResultCommand) => void;
    opponentTeamPlayerIDs: string[];
    isWinning: boolean;
}

export const EnterTeamControl = ({ team, onChange, opponentTeamPlayerIDs, isWinning }: EnterTeamControlProps) => {
    const { players } = useContext(ClientContext);
    
    const orderedPlayers = useMemo(
        () => {
            return [...players].sort((a, b) => a.nickname.localeCompare(b.nickname))
        },
        [players]
    )

    const addNewPlayer = (playerID: string) => {
        onChange({ ...team, PlayerIDs: [...team.PlayerIDs, playerID] });
    };

    const changePlayer = (previousPlayerID: string, newPlayerID: string) => {
        if (newPlayerID === '') {
            // Remove case
            onChange({ ...team, PlayerIDs: team.PlayerIDs.filter(p => p !== previousPlayerID) });
        } else {
            onChange({...team, PlayerIDs: team.PlayerIDs.map(p => p === previousPlayerID ? newPlayerID : p)})
        }
    };

    return (
        <Stack padding={'0.5rem'} flexGrow={1} gap={1}>
            {team.PlayerIDs.map((selectedPlayer) => {
                return (
                    <FormControl key={selectedPlayer} fullWidth>
                        <InputLabel>Spieler</InputLabel>
                        <Select label="Spieler" value={selectedPlayer} onChange={(e) => changePlayer(selectedPlayer, e.target.value as string)}>
                            <MenuItem value={''} sx={{ fontStyle: 'italic' }}>
                                Entfernen
                            </MenuItem>
                            {orderedPlayers
                                .filter(
                                    // Only show those that are not already selected for the team
                                    (p) =>
                                        (!team.PlayerIDs.includes(p.playerID)  && !opponentTeamPlayerIDs.includes(p.playerID)) ||
                                        p.playerID === selectedPlayer
                                )
                                .map((player) => (
                                    <MenuItem
                                        key={player.playerID}
                                        value={player.playerID}
                                    >
                                        {player.nickname}
                                    </MenuItem>
                                ))}
                        </Select>
                    </FormControl>
                );
            })}
            <FormControl fullWidth>
                <InputLabel>Spieler</InputLabel>
                <Select
                    label="Spieler"
                    onChange={(e) => addNewPlayer(e.target.value as string)}
                    value={""}
                >
                    {orderedPlayers
                        .filter(
                            // Only show those that are not already selected for the team
                            (p) => !team.PlayerIDs.includes(p.playerID) && !opponentTeamPlayerIDs.includes(p.playerID)
                        )
                        .map((player) => (
                            <MenuItem
                                key={player.playerID}
                                value={player.playerID}
                            >
                                {player.nickname}
                            </MenuItem>
                        ))}
                </Select>
            </FormControl>
            <EnterPointsControl
                points={team.Points}
                onChange={points => onChange({...team, PlayerIDs: [...team.PlayerIDs], Points: points})}
                isWinning={isWinning}
            />
        </Stack>
    );
};
