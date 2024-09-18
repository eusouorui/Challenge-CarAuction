# Challenge - Car Auction

This application consists of a .NET MVC project for a Car Auction company.

# Installing and running

## 1. Database
The project __requires__ a database in order to run properly.
It's connection string is located under:
> "..\Challenge-CarAuction\Challenge-CarAuction\appsettings.json"

And it's named 
> DbConnectionString

Once it's using Entity Framework you should be able to create the database with the follwoing commands in the __NuGet Package Manager Console__:
```
add-migration (and then being prompted to add the migration name)
update-database
```

Or via [Microsoft's Entity Framework Documentation](https://learn.microsoft.com/en-us/ef/core/managing-schemas/migrations/?tabs=dotnet-core-cli#create-your-first-migration):
```
dotnet ef migrations add "Migration name"
dotnet ef database update
```

And the database should be created and ready to go.

## 2. Logic

As it's suposed to be a simple application there is no separate front and back office.
As so, the user should be in both the role of administrator and client.

### The user journey should be:
1. As an administrator create a (car) Manufacturer (example: Audi)
    
    ---
2. As an administrator create a (car) Model (example: A4)

    In a real case scenario these entities would be loaded with all the information about every car that the company could have available for Auctions.
    
    ---
3. Here, assuming the clients uploaded their car's info, as a client, create a car:
    As all three asked parameters could be characteristics of any car, I included them in every vehicle, always being available:
    - number of doors
    - numer of seats
    - load capacity
    
    However, as asked, sedans and hatchbacks require number of doors, SUVs require number of seats and Trucks require load capacity, for the ones that did not require their respective fields, they are optional.

    The Starting bid and year values are set here.

    Identifiers are hidden but are increasing integers. it was a bit ambiguous if it should have been user input or not. 
    A guid of some sort could have been a nice implementation, as in this case there are no number plates nor VIN numbers.
    
    ---

4. Create an auction under the autions tab.

Here you can choose a car model for auction. The auction is active as soon as it's created.

----

5. Bidding is access on the details page of a given auction and it's only available when the auction is active. The minimum bidding value is always the starting bid or the highest bid, if there's one. 

---

6. Bids can be closed in the same page.

## Disclaimer

Until the second to last day before delievery I thought there were no requierements for UI nor database as in I could implement the ones I prefer. After raising the doubt to hannah she has told me that there was no actual need for a database nor UI. As I has already created it I kept it as is. It still follows MVC principles and it's using interfaces as an extra layer.

## Personal Notes:

1. Nice to haves:

Cache so data access is quicker, however it's close to irrelevant in such a small application

2. Assumptions:

In this case license plates and VIN numbers are not being considered. However, I developed the logic assuming that in the case they would, the same car could be auctioned today and maybe in the future, as so, the application can handle two auctions of the same car. However, only one can be active at a time.

Also, for any specific reason, an auction can be abandoned, disabled and a new one created for the same car. However, the bids are lost.

Considering this logic it would make more sense for the starting bid to be defined in the auction and not the car entity. However, following the given instructions it was defined in the car entity.

If something that I have or have not explained in here does not feel right please do question it and I will try to explain.

3. Simplifications:

For the given task of a "simple" car auction application some corners were cut. Specifically on the frontend, as I used datatables.net for the frontend sorting, filtering, etc. 
Also, bids being under the "details" option in the Auction is not optimal as well design-wise.

Soft deletes and timestamps were not implemented, but could have been.

4. Testing:

This being a simple application no integration tests were performed.

Every interface has tests for it's respective CRUD functions.

    Thanks for reading,
    Rui