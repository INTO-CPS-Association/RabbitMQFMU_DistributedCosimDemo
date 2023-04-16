1. Install Docker Desktop
2. Create custom network: `docker network create --subnet=172.20.0.0/16 examplenetwork`
4. Setup and test rabbitmq:
   1. CD to [rabbitmqserver](./rabbitmqserver)
   2. `docker build -t rabbitmq:latest .`
   3. `docker container run --rm --name rabbitmq-server -p 5672:5672 -p 15672:15672 -p 1883:1883 --network=examplenetwork --ip=172.20.0.2 rabbitmq:latest`
   4. This will launch it at localhost `5672` for TCP communication and http://localhost:15672 will serve the management interface. The default login is username: `guest` and password: `guest`
   5. `python .\consume.py`
   6. `python .\publish.py`
5. Export FMUs for linux machines.
   1. `docker build -t om:latest .`
   2. `$env:DISPLAY=":0"`
   3. `docker container run --rm --name om-container -v C:\Data\Activities\Presentations\2023\DLTE_DistributedCosimDemo\models:/models -w /models -it om:latest /bin/bash`
   4. `docker container run -it --rm --name om-container -v C:\Data\Activities\Presentations\2023\DLTE_DistributedCosimDemo\models:/models -w /models om:latest OMEdit`
6. Run standalone cosim
   1. CD to [maestro_stand_alone](./maestro_stand_alone)
   2. `java -jar maestro.jar sigver generate-algorithm scenario.conf -output results`
   3. `java -jar maestro.jar sigver execute-algorithm -mm multiModel.json -ep executionParameters.json -al results/masterModel.conf -output results -di -vim FMI2`
7. Start controller python running in local machine.
   1. CD to [distributed_ctrl_python](./distributed_ctrl_python)
   2. `python .\control.py`
8. Start distributed_oneway
   1. CD to [distributed_oneway](./distributed_oneway)
   2. `docker build -t maestro:latest .`
   3. `cd ..`
   4. `docker container run --rm --name maestro-container -v ${pwd}\distributed_oneway:/distributed_oneway -v ${pwd}\fmus:/fmus -w /distributed_oneway --network=examplenetwork  -it maestro:latest /bin/bash`
      1. `java -jar maestro.jar sigver generate-algorithm scenario.conf -output results`
      2. `java -jar maestro.jar sigver execute-algorithm -mm multiModel.json -ep executionParameters.json -al results/masterModel.conf -output results -di -vim FMI2`


Troubleshooting:
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
