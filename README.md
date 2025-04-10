## Story Time
One of my favorite pastime activities has been gaming, 
but with life's various responsibilities, 
this pastime activity almost went extinct. 
I had this huge game library and no time to actually play any of it, 
and then it hit me: I could write a simple tool that automates getting the info I need for a good game (high rating) with a short playing time.
A couple of days later, I had the GameInfoFetcher web app,
which I hope will help others manage their pastime activities as well.

## Usage
- Press the upload button.
- Choose your library file (text file with each game name on a new line).
- Choose where to download the .csv.
- Open the .csv file in your favorite sheets application, view, and sort the info as you like.

### Usage Restrictions
- Each line should not exceed 100 characters.
- Each file should never exceed 10 MB.

## Technology Stack

### Frontend
- React TypeScript

### Backend
- .NET 8

### Local Database
- LiteDB

### Cloud Caching
- Memcached

### External API
- [RAWG.io](https://rawg.io/apidocs)

### Containerization
- Docker

### CI
- GitHub Workflows

### Secrets
- GitHub Secrets

## Working On
- Implement repository pattern for data access.
- Track nginx and config file while keeping secrets safe.
- Publish docker-compose built images to docker registery.
- Consume image from registery and orchestrate deployment on hosting service using docker swarm.

## [Hosted Website (Broken since april 9th 2025 need to renew subscription to google cloud)](http://35.246.219.119:3000/)
