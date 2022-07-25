# TLDR; Solution
Jump straight to running Pavel's solution [here](./docs/README.md).

# Recruitment assignment for Site Reliability Engineer

This task is intended for candidates applying for an SRE position at the Visma 
e-conomic operations team. This assignment is built around some of the 
technologies used in our production environment.

We're super happy that you're considering to join us here at e-conomic, the 
challenge below should hope to bring a small view into the life of an SRE here, 
and serve as a entrypoint for a good discussion at the technical interview.

## Introduction

e-conomic runs a broad palette of services to provide the functionality our 
application serves our customers, a big part of this functionality is provided 
by a layer of microservices.
These microservices are hosted in Kubernetes and have different requirements in 
terms of availability and resilience, both from the requests they serve but also
the data that some of them hold.
To verify how well a given candidate fit our needs, the test is built with the 
intention for you to show of your skills in some of the technologies we use on a
daily basis.  
Should you feel like expanding above and beyond the scope of the test, feel free
to do so. We will enjoy discussing your reasoning for doing so.

## The Challenge

Develop a microservice that:

* Takes HTTP GET requests with a random ID (/1, /529852, etc.), requests a 
  document from the microservice we have provided in the `dummy-pdf-or-png` 
  subdirectory of this repository, and then returns the document with correct 
  ime-type.
* Provides an endpoint for health monitoring.
* Has a Kubernetes Manifest or Helm Charts
* Has Readiness and LivenessChecks.
* Has a Dockerfile.
* Has tests, so regressions can be identified.
* Failing Safe is a priority.
* The service should log relevant information

Overall the service must be considered production-ready.

Bonus points if  

* Prometheus metrics are provided from the service.
* Resources are provisioned via IaC
* There is CI/CD (GitLab, GitHub Actions or similar) to build, test and deploy
* There is a good developer experience

## Delivery

Fork this repository into a public repository on your own Github profile, and 
deliver your solution there.

## Questions?

If you have questions about the task or would like us to further specify some of
the tings written above, you can contact the person who gave you the assignment.
