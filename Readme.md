https://github.com/GFTRecruitmentCR/programming_exercise/

Steps to Follow
====
- Fire up the Back End Solution.
  - Go to the **Puzzle.API** project
    - Find it at **02 Service/03 Shell/02 API** folder inside the solution
    - Open up the **Properties** of the project
    - In the **Debug** tab, under **Start Options** add the following to the **Command Line Arguments**:
      - "**--console**"
      - This is in order to run the service on the **console**, for testing purposes
      - The service is built to run in the **console**, as a **Windows Service**, or with a couple more files as an **Azure Worker** too
- Visit http://localhost:9000/PZL/swagger in order to have access to the API through a Swagger UI
- These are the **API Endpoints** available, as well as the most important ones to explore
![API Endpoints](API.png)
- The most important parts of the business logic can be found in the following files within the *Solution Explorer*: </br>
├── **00 Solution Info**</br>
├── **01 Solution Shared Libraries**</br>
├── **02 Service**</br>
├──── **01 Core**</br>
├────── **01 Domain Entities**</br>
├────── **02 Contracts**</br>
├────── **03 Business Services**</br>
├──────── **Puzzle.BL**</br>
├────────── **Markov**</br>
├──────────── Cyphers.cs »» *The Markov Algorithm Logic is found here* </br>
├────────── **Puzzle**</br>
├──────────── Puzzles.cs »» *The Puzzle Resolution Logic is found here* </br>
├──── **02 Infrastructure**</br>
├──── **03 Shell**</br>


FAQ
====

### What are the main **Architectural Patterns** followed?
- The Back End is composed of a Service which encapsulates the Data Access as well as the Business Logic
- Such service is actually a **Microservice**
- The Microservice is implemented using an **Onion Architecture**
![Onion Architecture](OnionArchitecture.png)
- More information on Onion Architectures can be found at:
  - http://jeffreypalermo.com/blog/the-onion-architecture-part-1/
- Other patterns used include:
  - **Repositories**
  - **Units of Work**
  - **Business Aggregates**

### What's the overall Directory Layout?
. </br>
├── **Front End** »» *Placeholder for Front End Project(s)* </br>
├── **Puzzle Services** »» *Back End Service* </br>
├── **AssemblyInfo** »» *Used to unify the whole service Versioning (not hooked up yet)* </br>
├── **Docs** »» *Where relevant documentation about the service is placed* </br>
├── **Sources** »» *Source Files* </br>
├──── **Commons** »» *Commons Library is present here in order to avoid Private Nugets* </br>
├──── **Databases** »» *.json and .xml files containing the data to be used for this project* </br>
├──── **Service** »» *Code relevant to this solution* </br>

### What if one wanted to change where the data is coming from?
- As customary in Onion Architectures, the solution provides a set of Data Contracts (Interfaces) in order to access the data from the Business Logic
- If we wanted to handle data from a different source, we need only implement the Data Contracts and access the new data sources from there
- The current solution is plugged to the JSON files in the **Databases** folder, but just as easily it could plug to the XML files instead

### What's the deal with the InMemory database found in the **Infrastructure** folder?
- It's just **an** implementation of the common's data contracts which happens to work with objects in Memory
- This InMemory database was created to aid in testing

### Could we have several Data Sources at once?
- Absolutely. The data contracts have been implemented by the InMemory database and EF 6; but any technology can be used, as long as the contracts are implemented
- As a matter of fact, one could even choose live, per request or user, the data source one would use

### Why is the IoC project mostly empty and doesn't it include any DI library?
- It used to include all the logic to assemble contracts with their corresponding implementations though **StructureMap**
- With the creation of **Business Aggregates** however, the use of a library became unnecessary
- Still, the project is in itself a hinge project which affords much flexibility to the architecture
  - For small projects like this, it goes hugely underused (though it still fulfills a purpuse)
  - For bigger projects however, demanding more complex scenarios, the project is the perfect place to keep configuration off of the layers under it while still offering a clean and easy way to access the core from the outermost layers, e.g.: the API project

### Why mark Domain Entity Fields as **virtual**?
- This allows for Interceptors to be used on the entities
- e.g.: Dirty Field Trackers

### What are the Projects within the **01 Solution Shared Libraries**?
- This are excerpts from a bigger *Commons Library* I built and use to develop services
- This commons library greatly facilitate my development lifecycle
- Each project within this sample library is actually published to its own Nuget. Any referencing project later on imports the Nugets it needs (and each one calls its own dependencies of course)
- In order to avoid seting up a Private Local Nuget Repo, or anything of that sort, the files have been copied here
- Each project has been trimmed, and only what was needed for this demo has been imported. This by no means represents the entirity of the library

### When compiled, there are a bunch of warnings stating "... This package may not be fully compatible with your project." What are they about?
- The Commons Library has been moved to .Net Standard 2.0. However, there are 3 Nuget packages which have not been made available in .Net Standard, thus prompting these warnings
- In the future, suitable alternatives are to be pursuit, perhaps favoring .Net Core equivalents

Changes
====
- There were 2 entries for chyper #3 with Order Id = 8; 12 was missing. I changed the last ocurrance to 12.
- There were 2 entries for chyper #4 with Order Id = 12; 8 was missing. I changed the first ocurrance to 8.
