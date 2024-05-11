// eslint-disable-next-line @typescript-eslint/no-unused-vars
import { ClientContextProvider } from 'src/components/client-context/ClientContextProvider';
import { AppRoutes } from './app-routes';
import styles from './app.module.less';

export function App() {
    return (
        <ClientContextProvider>
            <AppRoutes />
        </ClientContextProvider>
    );
}

export default App;
