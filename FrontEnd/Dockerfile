FROM node:22.12-alpine AS build

WORKDIR /app

# might not be needed here
ENV NODE_ENV=production DOCKER_BUILDKIT=1

COPY package.json yarn.lock ./

# yarn install dependencies
RUN --mount=type=cache,target=C:\\Users\\mohab\\AppData\\Local\\Yarn\\Cache\\v6 \
npm set cache C:\\Users\\mohab\\AppData\\Local\\Yarn\\Cache\\v6 && \
yarn install --production

# Copied files will be own by the create non admin user "node"
COPY . .

#build app
RUN yarn build

## Run Stage
FROM node:22.12-alpine

USER node

WORKDIR /usr/src/app/

COPY --chown=user:node --from=build /app/build ./build

RUN yarn add serve

EXPOSE 3000

CMD ["npx", "serve", "-s", "build"]
