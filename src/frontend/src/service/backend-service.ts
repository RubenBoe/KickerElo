import {
    UseQueryResult,
    useMutation,
    useQuery,
    useQueryClient,
} from '@tanstack/react-query';
import axios from 'axios';
import { ClientDetails } from 'src/models/ClientDetails';
import { GameResult } from 'src/models/GameResult';
import { PlayerDetailsResult } from 'src/models/PlayerDetailsResult';
import { PlayerResult } from 'src/models/PlayerResult';
import { TeamResultCommand } from 'src/models/TeamResultCommand';

// const apiUrl = process.env["NX_API_URL"];
const apiUrl = 'http://localhost:7123/api/';

axios.interceptors.response.use((originalResponse) => {
    handleDates(originalResponse.data);
    return originalResponse;
});

const isoDateFormat =
    /^(\d{4})-(\d{2})-(\d{2})T(\d{2}):(\d{2}):(\d{2}(?:\.\d*)?)((-(\d{2}):(\d{2})|Z)?)$/;

function isIsoDateString(value: any): boolean {
    return value && typeof value === 'string' && isoDateFormat.test(value);
}

export function handleDates(body: any) {
    if (body === null || body === undefined || typeof body !== 'object')
        return body;

    for (const key of Object.keys(body)) {
        const value = body[key];
        if (isIsoDateString(value)) body[key] = new Date(value);
        else if (typeof value === 'object') handleDates(value);
    }
}

const getClientDetails = async (
    clientToken: string
): Promise<ClientDetails | undefined> => {
    return axios
        .get<ClientDetails>(`${apiUrl}Client/${clientToken}`)
        .then((res) => res.data);
};

const getPlayers = (clientToken: string) => {
    return axios
        .get<PlayerResult[]>(`${apiUrl}Players/${clientToken}`)
        .then((res) => res.data);
};

// Queries -----------------------------------------------------------------------------

export const useClientDetails = (clientToken: string) => {
    return useQuery({
        queryKey: ['client', clientToken],
        queryFn: () => getClientDetails(clientToken),
    });
};

export const usePlayers = (clientToken: string) => {
    const queryResult = useQuery({
        queryKey: ['players'],
        queryFn: () => getPlayers(clientToken),
    });
    return queryResult;
};

export const usePlayerDetails = (playerID: string) => {
    const queryResult = useQuery({
        queryKey: ['players', playerID],
        queryFn: () =>
            axios
                .get<PlayerDetailsResult>(`${apiUrl}Player/${playerID}`)
                .then((res) => res.data),
    });
    return queryResult;
};

export const useGetGames = (seasonID: string) => {
    return useQuery({
        queryKey: ['games'],
        queryFn: () =>
            axios
                .get<GameResult[]>(`${apiUrl}Games/${seasonID}`)
                .then((res) => res.data),
    });
};

// Mutations ------------------------------------------------------------------

export const useAddPlayer = () => {
    const queryClient = useQueryClient();

    return useMutation({
        mutationFn: (data: {
            clientToken: string;
            nickname: string;
            fullName: string;
        }) =>
            axios
                .post(`${apiUrl}AddPlayer`, {
                    ClientToken: data.clientToken,
                    Nickname: data.nickname,
                    FullName: data.fullName,
                })
                .then((res) => res.data),
        onSuccess: async () => {
            await queryClient.invalidateQueries({ queryKey: ['players'] });
        },
    });
};

export const useEnterGame = () => {
    const queryClient = useQueryClient();

    return useMutation({
        mutationFn: (data: {
            ClientToken: string;
            Teams: TeamResultCommand[];
        }) => {
            return axios
                .post<GameResult>(`${apiUrl}EnterGame`, {
                    ClientToken: data.ClientToken,
                    Teams: data.Teams,
                })
                .then((res) => res.data);
        },
        onSuccess: async () => {
            await Promise.all([
                queryClient.invalidateQueries({ queryKey: ['players'] }),
                queryClient.invalidateQueries({ queryKey: ['client'] }),
                queryClient.invalidateQueries({ queryKey: ['games'] }),
            ]);
        },
    });
};
