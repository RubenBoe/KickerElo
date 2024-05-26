export interface TeamResultCommand extends TeamBase {
    Points: number
}

export interface TeamBase {
    PlayerIDs: string[],
    TeamNumber: number,
}