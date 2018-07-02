Steps to Follow
====
- Fire up the Back End Solution.
- Visit http://localhost:9000/PZL/swagger in order to have access to the API through a Swagger UI

FAQ
====

### What are the main **Architectural Patterns** followed?
- The Back End is composed of a Service which encapsulates the Data Access as well as the Business Logic
- Such service is actually a Microservice
- The Microservice is implemented using an **Onion Architecture**
![Onion Architecture](OnionArchitecture.png)
- More information on Onion Architectures can be found at:
  - http://jeffreypalermo.com/blog/the-onion-architecture-part-1/

### Why mark Domain Entity Fields as **virtual**?
- This allows for Interceptors to be used on the entities
- e.g.: Dirty Field Trackers

### What are the Projects within the **01 Solution Shared Libraries**?
- This are excerpts from a bigger *Commons Library* I built and use to develop services
- This commons library greatly facilitate my development lifecycle
- Each project within this sample library is actually published to its own Nuget. Any referencing project later on imports the Nugets it needs (and each one calls its own dependencies of course)
- In order to avoid seting up a Private Local Nuget Repo, or anything of that sort, the files have been copied here
- Each project has been trimmed, and only what was needed for this demo has been imported. This by no means represents the entirity of the library

Changes
====
- There were 2 entries for chyper #3 with Order Id = 8; 12 was missing. I changed the last ocurrance to 12.
- There were 2 entries for chyper #4 with Order Id = 12; 8 was missing. I changed the first ocurrance to 8.
