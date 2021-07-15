# Timeseries Data Store

A very simple timeseries data storage application which performs some simple aggregation on the raw timeseries data using a set of microservices. Idea is not to create a full fledge system with all bells and whistles, but a very simple solution to handle various technical challenges presented while working with microservices.

## Data Flow

![Data Flow](https://github.com/the-code-man/timeseries-data-store/blob/main/assets/data_flow.png)

## Running the microservices

###  Before running them!

I am using rabbitmq as the message broker. Simplest way to use it, is to run it as a docker container.

1. Create docker network. Although, you can use the default bridge network, I like to keep a dedicated network for the application for seamless communication between the services.

```bash
docker network create timeseries-datastore-net
```

2. Next spin up rabbitmq container. I have used the image that comes with management plugin. Notice that this container runs on the same network that was created above.

```bash
docker run -d --name rabbitmq -p 15672:15672 -p 5672:5672 --network timeseries-datastore-net rabbitmq:3-management
```

3. [OPTIONAL] Once rabbitmq is up, create a dedicated virtual host, create new user and give permissions to the user on the newly created virtual host. You can either use the management UI (address --> http://localhost:15672, username --> guest, password --> guest) or use rabbitmq apis. More information here --> https://www.rabbitmq.com/vhosts.html.

4. The microservices, by default, uses SQLlite as their databases, which is persisted on the file system. The databases are created by the services on startup.

5. By default, TimeSeries.API runs on SSL. You will have to create a self-signed certificate with "TimeSeries.Api.pfx" as the name. For running api with docker support, this certificate has to be placed in a folder that would be volume mounted into the container (See CERT_ROOT and CERT_PASSWORD environment variables).

### With Docker support

1. Create a .env and add it to the root of the solution. A sample of the same is available in the root.

```bash
DATABASE_TYPE=SqlLite
BUS_HOST=rabbitmq://rabbitmq/timeseries-datastore-host
BUS_USERNAME=timeseries
BUS_PASSWORD=timeseries
DATA_AGGREGATOR_DBS_ROOT=~/timeseries-dbs
CERT_ROOT=~/.aspnet/https
CERT_PASSWORD=HighlySecuredPassword@123
```

2. Use docker compose the start the microservices

```bash
docker-compose up
```

### Without Docker support

You can also run the application without docker support. To do so, follow below given instructions:

1. All projects in src folder are ASP.NET Core Web Apis with gRPC support. Each one of them has appsettings.json file. Settings in these files are self-explanatory. Modify them according to your environment setup.

> You can also choose to use other configuration providers like secrets.json, azure key vault etc. More information here --> https://docs.microsoft.com/en-us/aspnet/core/fundamentals/configuration/?view=aspnetcore-5.0

2. Start the projects one by one. In Visual Studio, you can also choose to set multiple startup projects for the solution and run all of them at once.

### Testing the Api

I have included some basic API tests in the form of postman collection (timeseries-data-store\api.tests). This can be used to validate if the api is running as expected. The collection uses a bunch of environment variables. Given below is an example of configured environment variables.

![Postman environment settings](https://github.com/the-code-man/timeseries-data-store/blob/main/assets/postman_env_example.png)

## Data Ingestor Client

The solution also has a data ingestor client application which can be used as a simulator. This is a .net core console application and is available in simulator solution folder.

After building the project, you can run the simulator from its bin folder by executing following command:

```BASH
TimeSeries.DataIngestor.Client.exe -a "https://localhost:5000" -d "FQREW" -b 5 -i 5000
```
To get more details about the parameter, use

```BASH
TimeSeries.DataIngestor.Client.exe --help
```
