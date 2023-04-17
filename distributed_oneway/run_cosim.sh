java -jar maestro.jar sigver generate-algorithm scenario.conf -output results
java -jar maestro.jar sigver execute-algorithm -mm multiModel.json -ep executionParameters.json -al results/masterModel.conf -output results -di -vim FMI2
