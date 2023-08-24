Instructions Using Docker-Compose:
1. Install Docker Desktop or equivalent for your platform.
2. Contact Claudio <claudio.gomes@ece.au.dk> for access to rabbitqm on AWS. You will get a password.
3. `docker-compose up` -> Starts all services
   1. Wait for all services to be up and running.
   2. Open terminal for controller-container: 
      1. `docker exec -it controller-container /bin/bash`
      2. `python3 control.py`
   3. Open terminal for rabbitmqfmu-container
      1. `docker exec -it rabbitmqfmu-container /bin/bash`
      2. `./run_cosim.sh`
   4. Exit all the above terminals.
4. `docker-compose down` -> Removes all containers


Instructions that do not use Docker-Compose (useful for troubleshooting the virtual machines individually):
1. Install Docker Desktop or equivalent for your platform
2. Create custom network: `docker network create --subnet=172.20.0.0/16 examplenetwork`
3. Setup and test rabbitmq:
   1. CD to [rabbitmqserver](./rabbitmqserver)
   2. `docker build -t rabbitmq:latest .`
   3. `docker container run --rm --name rabbitmq-server -p 5672:5672 -p 15672:15672 -p 1883:1883 --network=examplenetwork --ip=172.20.0.2 rabbitmq:latest`
   4. This will launch it at localhost `5672` for TCP communication and http://localhost:15672 will serve the management interface. The default login is username: `guest` and password: `guest`. If rabbitmq is running but the webpage does not load, and you're on windows, check that the hosts file contains entries `127.0.0.1    localhost` and `::1           localhost`.
   5. `python .\consume.py`
   6. `python .\publish.py`
4. Export FMUs for linux machines.
   1. Use [OpenModelica](https://openmodelica.org/download/download-linux/) from a linux machine, or use [Linux Subsystem for Windows](https://learn.microsoft.com/en-us/windows/wsl/tutorials/gui-apps) 
   2. Open models and export them as FMUs.
5. Run standalone cosim
   1. CD to [maestro_stand_alone](./maestro_stand_alone)
   2. `docker build -t maestro:latest .`
   3. `cd ..`
   4. Start container terminal: `docker container run --rm --name maestro-container -v ${pwd}\maestro_stand_alone:/maestro_stand_alone -v ${pwd}\fmus:/fmus -w /maestro_stand_alone -it maestro:latest /bin/bash`
   5. Then inside container terminal: `java -jar maestro.jar import Sg1 -output=results -v --interpret scenario.json`
6. Run distributed scenario:
   1. Make sure rabbitmq-server container is running (see previous step)
   2. Start controller python running in local machine.
      1. CD to [distributed_ctrl_python](./distributed_ctrl_python)
      2. `python .\control.py`
   3. Start distributed_oneway
      1. CD to [distributed_oneway](./distributed_oneway)
      2. `docker build -t rabbitmqfmu:latest .`
      3. `cd ..`
      4. `docker container run --rm --name rabbitmqfmu-container -v ${pwd}\distributed_oneway:/distributed_oneway -v ${pwd}\fmus:/fmus -w /distributed_oneway --network=examplenetwork  -it rabbitmqfmu:latest /bin/bash`
         1. `java -jar maestro.jar sigver generate-algorithm scenario.conf -output results`
         2. `java -jar maestro.jar sigver execute-algorithm -mm multiModel.json -ep executionParameters.json -al results/masterModel.conf -output results -di -vim FMI2`


Troubleshooting rabbitmqfmu missing dependencies:
- `wget http://archive.ubuntu.com/ubuntu/pool/main/i/icu/libicu66_66.1-2ubuntu2_amd64.deb`
- `dpkg -i libicu66_66.1-2ubuntu2_amd64.deb`
- https://stackoverflow.com/questions/72133316/libssl-so-1-1-cannot-open-shared-object-file-no-such-file-or-directory
- Add missing parameters in modeldescription
      """
      <ScalarVariable name="config.ssl" valueReference="16" variability="fixed" causality="parameter" initial="exact">
         <Boolean start="true"/>
      </ScalarVariable>
      <ScalarVariable name="config.queueupperbound" valueReference="17" variability="fixed" causality="parameter" initial="exact">
         <Integer start="100"/>
      </ScalarVariable>
      """
- Why do we need two model description files?

Other commands for testing containers
- `docker network ls`
- `docker container run --rm --name ping --network=examplenetwork -it myubuntu /bin/bash`
- `docker container run --rm --name consume --network=examplenetwork -v $pwd\aux_scripts:/aux_scripts -w /aux_scripts -it myubuntu /bin/bash`
  - `apt-get install python3 python-pip3`
- `docker inspect rabbitmq-server`
