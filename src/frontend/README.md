# Kicker Elo Frontend App

## Development and Build

Serve the project using ``npx nx serve frontend``. Build the project using ``npx nx build frontend``.

## Deployment

The frontend is deployed on an Azure Static Web App resource.
Deploy from the command line using ``swa deploy -a ./dist/frontend -d {token} --env production`` where ``{token}`` is the deployment token of the resource.
Remember to change the backend api endpoint before building for production. This is not yet automated.