// eslint-disable-next-line @typescript-eslint/no-unused-vars
import { ClientContextProvider } from 'src/components/client-context/ClientContextProvider';
import { AppRoutes } from './app-routes';
import { QueryClient, QueryClientProvider } from '@tanstack/react-query';

export function App() {
    const queryClient = new QueryClient({
        defaultOptions: {
            queries: {
                retry: 1,
                refetchOnWindowFocus: false,
            }
        }
    });

    return (
        <QueryClientProvider client={queryClient}>
            <ClientContextProvider>
                <AppRoutes />
            </ClientContextProvider>
        </QueryClientProvider>
    );
}

export default App;
