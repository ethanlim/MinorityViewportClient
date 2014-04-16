MultipleKinectsPlatformClient
=============================

## Description ##

This client manages numerous depth sensors and forward the skeleton data to the [Minority Viewport](https://github.com/ethanlim/MinorityViewport "Minority Viewport") application.

## Development Environment ##

### Minimum Prerequisite ###

1. Kinect SDK v1.7
2. Kinect Depth Sensors < 3

### Set Up ###

1. Git Clone to the Folder Directory
2. Open Visual Studio 
3. File -> New -> Project From Existing Code
4. Select the Folder 
5. Select Windows application
6. Select all code file and include into project
7. Ensure the following reference are inserted

	![Alt references](https://minority-viewport.s3.amazonaws.com/client/img/references.bmp)

8. Select the App.xaml and change the following to Build Action to Application Domain

	![Alt build action](https://minority-viewport.s3.amazonaws.com/client/img/application_settings.bmp) 
 
