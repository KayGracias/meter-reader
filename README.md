# Meter Reading Uploader
As an Energy Company Account Manager, I want to be able to load a CSV file of Customer Meter Readings So that we can monitor their energy consumption and charge them accordingly

## Description

This solution is created with .NET Core and the database is created using EntityFramework code first approach. It also seeds Account data to the database on create.

There are 2 projects in the solution

* TestProject.MeterReader.WebAPI (API Project)
* TestProject.MeterReading.APP (App Project)

### API Project

The API project has an API that allows you to upload a csv file with meter readings, reads the values, validates and stores it into the database.

you can see the swagger definition when you run the WEB API at https://localhost:port/swagger/index.html

#### Validations

Before data is stored into the database, we make sure that the data in the file 
* No two entries are same
* A meter reading must be associated with an Account 
  ID to be deemed valid
* Reading values should be in the format NNNNN

#### Configuration

* Edit the appsettings.json file and pass your local db connection string to the key SQLConnectionString
* The database is created if not exists and the seed data is entered.


### APP Project

This APP consumes the API. When you run the APP you will be presented a page that will enable you to upload a csv file which in turn will call the web api to process it

#### Configuration

* Modify the file Index.cshtml.cs to ensure the property APIBaseUrl is set to the right endpoint of the Web API

### Executing program

* Build the solution.
* Run all tests and make sure all pass.
* Set multiple projects(API and APP) as startup projects 
* Run

*You can upload a file and process it using the try now in the swagger definition page or postman
*You can upload a file and process it via the APP


